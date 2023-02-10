using ChaosSoft.Core.Data;
using ChaosSoft.Core.Extensions;
using ChaosSoft.Core.IO;
using ChaosSoft.Core.NumericalMethods.PhaseSpace;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ChaosSoft.Core.NumericalMethods.Lyapunov
{
    /// <summary>
    /// M.Sano and Y.Sawada, Measurement of the Lyapunov spectrum from a chaotic time series, Phys. Rev.Lett. 55, 1082 (1985).
    /// </summary>
    public sealed class LeSpecSanoSawada : ITimeSeriesLyapunov, IDescribable
    {
        private const string Paper = "M.Sano and Y.Sawada, Measurement of the Lyapunov spectrum from a chaotic time series, Phys. Rev.Lett. 55, 1082 (1985)";
        private const int OutputInterval = 10000;
        private const double EpsMax = 1.0;

        private readonly int _eDim;
        private readonly bool _inverse;
        private readonly int _minNeighbors;
        private readonly bool _epsSet;
        private readonly double _epsStep;
        private readonly int _tau;
        private int iterations;

        private double errorAvg;
        private double neighborsAvg = 0;
        private double epsAvg = 0;
        private double epsMin;
        private double[,] matrix;
        private double[] vector;
        private double[] abstand;
        private long count = 0;
        private int[] indexes;

        /// <summary>
        /// The method estimates the whole spectrum of Lyapunov exponents for a given, possibly multivariate, time series. 
        /// Whole spectrum means: If d components are given and the embedding dimension is m than m*d exponents will be determined. 
        /// The method is based on the work of Sano and Sawada.
        /// </summary>
        /// <param name="eDim"></param>
        /// <param name="tau"></param>
        /// <param name="iterations"></param>
        /// <param name="scaleMin"></param>
        /// <param name="epsstep"></param>
        /// <param name="minNeigh"></param>
        /// <param name="inverse"></param>
        public LeSpecSanoSawada(int eDim, int tau, int iterations, double scaleMin, double epsstep, int minNeigh, bool inverse)
        {
            _eDim = eDim;
            _tau = tau;
            this.iterations = iterations;
            epsMin = scaleMin;
            _epsSet = scaleMin != 0;
            _epsStep = epsstep;
            _minNeighbors = minNeigh;
            _inverse = inverse;
            
            Result = new double[eDim];
            Slope = new DataSeries();
            Log = new StringBuilder();
        }


        // 3rd from end parameter (eps) is 0 to obtain further it's default value
        public LeSpecSanoSawada(int eDim) : this(eDim, 1, 0, 0, 1.2, 30, false)
        {
        }

        public DataSeries Slope { get; set; }

        public StringBuilder Log { get; }

        public double[] Result { get; }

        /// <summary>
        /// Gets method setup info (parameters values)
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            new StringBuilder()
            .AppendLine("LES by Sano & Sawada")
            .AppendLine($"m = {_eDim}")
            .AppendLine($"τ = {_tau}")
            .AppendLine($"iterations = {iterations}")
            .AppendLine($"min ε = {NumFormatter.ToShort(epsMin)}")
            .AppendLine($"neighbour size increase factor = {NumFormatter.ToShort(_epsStep)}")
            .AppendLine($"neighbors count = {_minNeighbors}")
            .AppendLine($"invert timeseries = {_inverse}")
            .ToString();

        /// <summary>
        /// Gets help on the method and its params
        /// </summary>
        /// <returns></returns>
        public string GetHelp() =>
            new StringBuilder()
            .AppendLine($"LES by Sano & Sawada [{Paper}]")
            .AppendLine("m - embedding dimension (default: 2)")
            .AppendLine("τ - reconstruction delay (default: 1)")
            .AppendLine("iterations (default: number of points)")
            .AppendLine("min ε - Min scale (default: ??)")
            .AppendLine($"neighbour size increase factor (default: 1.2)")
            .AppendLine($"neighbors count (default: 30)")
            .AppendLine($"invert timeseries (default: false)")
            .ToString();

        public string GetResultAsString() =>
            new StringBuilder().Append("LE Spectrum: ")
                .AppendLine(string.Join("; ", Result.Select(le => NumFormatter.ToShort(le))))
                .AppendLine($"Dky = {NumFormatter.ToShort(StochasticProperties.KYDimension(Result))}")
                .AppendLine($"Eks = {NumFormatter.ToShort(StochasticProperties.KSEntropy(Result))}")
                .AppendLine($"PVC = {NumFormatter.ToShort(StochasticProperties.PhaseVolumeContractionSpeed(Result))}")
                .ToString();

        public void Calculate(double[] timeSeries)
        {
            double[] series = new double[timeSeries.Length];
            Array.Copy(timeSeries, series, series.Length);

            if (iterations == 0)
            {
                iterations = series.Length;
            }

            if (_minNeighbors > (series.Length - _tau * (_eDim - 1) - 1))
            {
                throw new ArgumentException($"Too few points to find {_minNeighbors} neighbors, it makes no sense to continue.");
            }

            BoxAssistedFnn fnn = new BoxAssistedFnn(512, series.Length);

            int start, i, j;

            errorAvg = 0.0;

            double interval = Arrays.Rescale(series);
            double variance = Statistics.Variance(series);

            if (variance == 0.0)
            {
                throw new CalculationException("Variance of the data is zero.");
            }

            if (_inverse)
            {
                Array.Reverse(series);
            }

            epsMin = _epsSet ? epsMin / interval : interval / 1e-3;

            double[] dynamics = new double[_eDim];
            double[] factor = new double[_eDim];
            double[] lfactor = new double[_eDim];
            double[,] delta = new double[_eDim, _eDim];
            vector = new double[_eDim + 1];
            matrix = new double[_eDim + 1, _eDim + 1];

            indexes = MakeIndex(_eDim, _tau);

            Random random = new Random(int.MaxValue);

            for (i = 0; i < 10000; i++)
            {
                random.Next();
            }

            for (i = 0; i < _eDim; i++)
            {
                factor[i] = 0d;

                for (j = 0; j < _eDim; j++)
                {
                    delta[i, j] = (double)random.Next() / int.MaxValue;
                }
            }

            GramSchmidt(delta, lfactor);

            start = Math.Min(iterations, series.Length - _tau);

            abstand = new double[series.Length];

            var timer = Stopwatch.StartNew();

            for (i = (_eDim - 1) * _tau; i < start; i++)
            {
                count++;
                MakeDynamics(series, fnn, dynamics, i);
                MakeIteration(dynamics, delta);
                GramSchmidt(delta, lfactor);

                for (j = 0; j < _eDim; j++)
                {
                    factor[j] += Math.Log(lfactor[j]) / _tau;
                }

                if (timer.ElapsedMilliseconds > OutputInterval || (i == (start - 1)))
                {
                    timer.Restart();

                    Log.Append($"{count} ");

                    for (j = 0; j < _eDim; j++)
                    {
                        Log.Append($"{NumFormatter.ToShort(factor[j] / count)} ");
                        Result[j] = factor[j] / count;
                    }

                    Log.AppendLine();
                }
            }

            Log.AppendLine();
            Log.AppendLine($"Avg. rel. forecast error = {NumFormatter.ToShort(Math.Sqrt(errorAvg / count) / variance)}");
            Log.AppendLine($"Avg. abs. forecast error = {NumFormatter.ToShort(Math.Sqrt(errorAvg / count) * interval)}");
            Log.AppendLine($"Avg. neighborhood size   = {NumFormatter.ToShort(epsAvg * interval / count)}");
            Log.AppendLine($"Avg. number of neighbors = {NumFormatter.ToShort(neighborsAvg / count)}");
        }

        private double Sort(double[] series, BoxAssistedFnn fnn, long act, long nFound, out long nfound, out bool enough)
        {
            double maxeps, dx, dswap, maxdx;
            int self = 0, i, j, del, hf, iswap;
            long imax = nFound;

            enough = false;

            for (i = 0; i < imax; i++)
            {
                hf = fnn.Found[i];

                if (hf != act)
                {
                    maxdx = Math.Abs(series[act] - series[hf]);

                    for (j = 1; j < _eDim; j++)
                    {
                        del = indexes[j];
                        dx = Math.Abs(series[act - del] - series[hf - del]);
                        if (dx > maxdx) maxdx = dx;
                    }

                    abstand[i] = maxdx;
                }
                else
                {
                    self = i;
                }
            }

            if (self != (imax - 1))
            {
                abstand[self] = abstand[imax - 1];
                fnn.Found[self] = fnn.Found[imax - 1];
            }

            for (i = 0; i < _minNeighbors; i++)
            {
                for (j = i + 1; j < imax - 1; j++)
                {
                    if (abstand[j] < abstand[i])
                    {
                        dswap = abstand[i];
                        abstand[i] = abstand[j];
                        abstand[j] = dswap;
                        iswap = fnn.Found[i];
                        fnn.Found[i] = fnn.Found[j];
                        fnn.Found[j] = iswap;
                    }
                }
            }

            if (!_epsSet || (abstand[_minNeighbors - 1] >= epsMin))
            {
                nfound = _minNeighbors;
                enough = true;
                maxeps = abstand[_minNeighbors - 1];

                return maxeps;
            }

            for (i = _minNeighbors; i < imax - 2; i++)
            {
                for (j = i + 1; j < imax - 1; j++)
                {
                    if (abstand[j] < abstand[i])
                    {
                        dswap = abstand[i];
                        abstand[i] = abstand[j];
                        abstand[j] = dswap;
                        iswap = fnn.Found[i];
                        fnn.Found[i] = fnn.Found[j];
                        fnn.Found[j] = iswap;
                    }
                }
                if (abstand[i] > epsMin)
                {
                    nfound = i + 1;
                    enough = true;
                    maxeps = abstand[i];

                    return maxeps;
                }
            }

            maxeps = abstand[imax - 2];
            nfound = nFound;
            return maxeps;
        }

        private void MakeDynamics(double[] series, BoxAssistedFnn fnn, double[] dynamics, int act)
        {
            long i, hi, j, hj, k, t = act;
            long nfound;
            double foundeps = 0.0, epsilon, hv, hv1;

            epsilon = epsMin / _epsStep;

            do
            {
                epsilon *= _epsStep;

                if (epsilon > EpsMax)
                {
                    epsilon = EpsMax;
                }

                fnn.PutInBoxes(series, epsilon, (_eDim - 1) * _tau, series.Length - _tau, 0, 0);
                nfound = fnn.FindNeighborsJ(series, _eDim, _tau, epsilon, act);

                if (nfound > _minNeighbors)
                {
                    bool got_enough;

                    foundeps = Sort(series, fnn, act, nfound, out nfound, out got_enough);

                    if (got_enough)
                    {
                        break;
                    }
                }
            } 
            while (epsilon < EpsMax);

            neighborsAvg += nfound;
            epsAvg += foundeps;

            if (!_epsSet)
            {
                epsMin = epsAvg / count;
            }

            if (nfound < _minNeighbors)
            {
                throw new CalculationException("Not enough neighbors found.");
            }

            Arrays.FillArrayWith(vector, 0d);
            Matrixes.FillWith(matrix, 0d);

            for (i = 0; i < nfound; i++)
            {
                act = fnn.Found[i];
                matrix[0, 0] += 1.0;

                for (j = 0; j < _eDim; j++)
                {
                    matrix[0, j + 1] += series[act - indexes[j]];
                }

                for (j = 0; j < _eDim; j++)
                {
                    hv1 = series[act - indexes[j]];
                    hj = j + 1;

                    for (k = j; k < _eDim; k++)
                    {
                        matrix[hj, k + 1] += series[act - indexes[k]] * hv1;
                    }
                }
            }

            for (i = 0; i <= _eDim; i++)
            {
                for (j = i; j <= _eDim; j++)
                {
                    matrix[j, i] = (matrix[i, j] /= (double)nfound);
                }
            }

            double[,] imat = InvertMatrix(matrix, _eDim + 1);

            Arrays.FillArrayWith(vector, 0d);

            for (i = 0; i < nfound; i++)
            {
                act = fnn.Found[i];
                hv = series[act + _tau];
                vector[0] += hv;

                for (j = 0; j < _eDim; j++)
                {
                    vector[j + 1] += hv * series[act - indexes[j]];
                }
            }

            for (i = 0; i <= _eDim; i++)
            {
                vector[i] /= (double)nfound;
            }

            double new_vec = 0.0;

            for (i = 0; i <= _eDim; i++)
            {
                new_vec += imat[0, i] * vector[i];
            }

            for (i = 1; i <= _eDim; i++)
            {
                hi = i - 1;
                dynamics[hi] = 0.0;

                for (j = 0; j <= _eDim; j++)
                {
                    dynamics[hi] += imat[i, j] * vector[j];
                }
            }

            for (i = 0; i < _eDim; i++)
            {
                new_vec += dynamics[i] * series[t - indexes[i]];
            }

            double tmp = new_vec - series[t + _tau];
            errorAvg += tmp * tmp;

        }

        private void GramSchmidt(double[,] delta, double[] stretch)
        {
            double[,] dnew = new double[_eDim, _eDim];
            double[] diff = new double[_eDim];
            double norm;
            long i, j, k;

            for (i = 0; i < _eDim; i++)
            {
                for (j = 0; j < _eDim; j++)
                {
                    diff[j] = 0.0;
                }

                for (j = 0; j < i; j++)
                {
                    norm = 0.0;

                    for (k = 0; k < _eDim; k++)
                    {
                        norm += delta[i, k] * dnew[j, k];
                    }

                    for (k = 0; k < _eDim; k++)
                    {
                        diff[k] -= norm * dnew[j, k];
                    }
                }

                norm = 0.0;

                for (j = 0; j < _eDim; j++)
                {
                    norm += FastMath.Pow2(delta[i, j] + diff[j]);
                }

                norm = Math.Sqrt(norm);
                stretch[i] = norm;

                for (j = 0; j < _eDim; j++)
                {
                    dnew[i, j] = (delta[i, j] + diff[j]) / norm;
                }
            }

            for (i = 0; i < _eDim; i++)
            {
                for (j = 0; j < _eDim; j++)
                {
                    delta[i, j] = dnew[i, j];
                }
            }
        }

        private void MakeIteration(double[] dynamics, double[,] delta)
        {
            long i, j, k;
            double[,] dnew = new double[_eDim, _eDim];

            for (i = 0; i < _eDim; i++)
            {
                dnew[i, 0] = dynamics[0] * delta[i, 0];

                for (k = 1; k < _eDim; k++)
                {
                    dnew[i, 0] += dynamics[k] * delta[i, k];
                }

                for (j = 1; j < _eDim; j++)
                {
                    dnew[i, j] = delta[i, j - 1];
                }
            }

            for (i = 0; i < _eDim; i++)
            {
                for (j = 0; j < _eDim; j++)
                {
                    delta[i, j] = dnew[i, j];
                }
            }
        }

        private double[,] InvertMatrix(double[,] mat, int size)
        {
            int i, j, k;
            double[] vec = new double[size];
            double[,] imat = new double[size, size];
            double[][] hmat = new double[size][];

            for (int ii = 0; ii < size; ii++)
            {
                hmat[ii] = new double[size];
            }

            for (i = 0; i < size; i++)
            {
                for (j = 0; j < size; j++)
                {
                    vec[j] = (i == j) ? 1d : 0d;

                    for (k = 0; k < size; k++)
                    {
                        hmat[j][k] = mat[j, k];
                    }
                }

                SolveLe(hmat, vec, size);

                for (j = 0; j < size; j++)
                {
                    imat[j, i] = vec[j];
                }
            }

            return imat;
        }


        private void SolveLe(double[][] mat, double[] vec, int n)
        {
            double vswap;
            double[] mswap, hvec;
            double max, h, pivot, q;
            int i, j, k, maxi;

            for (i = 0; i < n - 1; i++)
            {
                max = Math.Abs(mat[i][i]);
                maxi = i;

                for (j = i + 1; j < n; j++)
                {
                    if ((h = Math.Abs(mat[j][i])) > max)
                    {
                        max = h;
                        maxi = j;
                    }
                }

                if (maxi != i)
                {
                    mswap = mat[i];
                    mat[i] = mat[maxi];
                    mat[maxi] = mswap;
                    vswap = vec[i];
                    vec[i] = vec[maxi];
                    vec[maxi] = vswap;
                }

                hvec = mat[i];
                pivot = hvec[i];

                if (Math.Abs(pivot) == 0.0)
                {
                    throw new CalculationException("Singular matrix.");
                }

                for (j = i + 1; j < n; j++)
                {
                    q = -mat[j][i] / pivot;
                    mat[j][i] = 0.0;

                    for (k = i + 1; k < n; k++)
                    {
                        mat[j][k] += q * hvec[k];
                    }

                    vec[j] += q * vec[i];
                }
            }

            vec[n - 1] /= mat[n - 1][n - 1];

            for (i = n - 2; i >= 0; i--)
            {
                hvec = mat[i];

                for (j = n - 1; j > i; j--)
                {
                    vec[i] -= hvec[j] * vec[j];
                }

                vec[i] /= hvec[i];
            }
        }

        private static int[] MakeIndex(int eDim, int delay)
        {
            int[] mmi = new int[eDim];

            for (int i = 0; i < eDim; i++)
            {
                mmi[i] = i * delay;
            }

            return mmi;
        }
    }
}
