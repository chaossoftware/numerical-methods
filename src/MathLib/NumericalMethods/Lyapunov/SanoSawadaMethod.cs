using MathLib.Data;
using MathLib.IO;
using MathLib.NumericalMethods.EmbeddingDimension;
using System;
using System.Diagnostics;
using System.Text;

namespace MathLib.NumericalMethods.Lyapunov
{
    /// <summary>
    /// M.Sano and Y.Sawada, Measurement of the Lyapunov spectrum from a chaotic time series, Phys. Rev.Lett. 55, 1082 (1985).
    /// </summary>
    public class SanoSawadaMethod : LyapunovMethod
    {
        private const int OutputInterval = 10000;
        private const double EpsMax = 1.0;

        private readonly BoxAssistedFnn fnn;

        private readonly int _eDim;
        private readonly bool _inverse;
        private readonly int _minNeighbors = 30;
        private readonly double _epsStep = 1.2;
        private readonly int _iterations;
        private readonly int _length;
        private readonly int _tau;

        private bool epsset = false;

        private double averr;
        private double avneig = 0.0, aveps = 0.0;
        private double[,] matrix;
        private double[] vector, abstand;
        private double epsmin;
        private long count = 0;
        private int[] indexes;
        private Random random;

        /// <summary>
        /// The method estimates the whole spectrum of Lyapunov exponents for a given, possibly multivariate, time series. 
        /// Whole spectrum means: If d components are given and the embedding dimension is m than m*d exponents will be determined. 
        /// The method is based on the work of Sano and Sawada.
        /// </summary>
        /// <param name="timeSeries"></param>
        /// <param name="eDim"></param>
        /// <param name="iterations"></param>
        /// <param name="scaleMin"></param>
        /// <param name="epsstep"></param>
        /// <param name="minNeigh"></param>
        /// <param name="inverse"></param>
        public SanoSawadaMethod(double[] timeSeries, int eDim, int tau, int iterations, double scaleMin, double epsstep, int minNeigh, bool inverse)
            : base(timeSeries)
        {
            _eDim = eDim;
            _tau = tau;
            _iterations = iterations;
            epsmin = scaleMin;
            epsset = scaleMin != 0;
            _epsStep = epsstep;
            _minNeighbors = minNeigh;
            _inverse = inverse;

            _length = TimeSeries.Length;

            if (_minNeighbors > (_length - _tau * (eDim - 1) - 1))
            {
                throw new ArgumentException($"Too few points to find {_minNeighbors} neighbors, it makes no sense to continue.");
            }

            Slope = new Timeseries();
            random = new Random();
            fnn = new BoxAssistedFnn(512, _length);
            Result = new LyapunovSpectrum(eDim);
        }

        public LyapunovSpectrum Result { get; }

        public override string ToString() =>
            new StringBuilder()
            .AppendLine("LE spectrum by Sano/Sawada")
            .AppendLine($"m = {_eDim}")
            .AppendLine($"τ = {_tau}")
            .AppendLine($"iterations = {_iterations}")
            .AppendLine($"min ε = {NumFormat.ToShort(epsmin)}")
            .AppendLine($"neighbour size increase factor = {NumFormat.ToShort(_epsStep)}")
            .AppendLine($"neighbors count = {_minNeighbors}")
            .AppendLine($"invert timeseries = {_inverse}")
            .ToString();

        public override string GetHelp()
        {
            throw new NotImplementedException();
        }

        public override string GetResult() => Result.ToString();

        public override void Calculate()
        {
            double[,] delta;
            double[] dynamics;
            double[] lfactor;
            double[] factor;
            double maxinterval = 0d;
            int start, i, j;

            averr = 0.0;

            var interval = Ext.RescaleData(TimeSeries);

            if (interval > maxinterval)
            {
                maxinterval = interval;
            }

            double variance = Ext.Variance(TimeSeries);

            if (variance == 0.0)
            {
                throw new CalculationException("Variance of the data is zero.");
            }

            if (_inverse)
            {
                Array.Reverse(TimeSeries);
            }

            epsmin = 
                epsset ? 
                epsmin / maxinterval : 
                interval / 1e-3;

            dynamics = new double[_eDim];
            factor = new double[_eDim];
            lfactor = new double[_eDim];
            delta = new double[_eDim, _eDim];
            vector = new double[_eDim + 1];
            matrix = new double[_eDim + 1, _eDim + 1];

            indexes = MakeIndex(_eDim, _tau);

            random = new Random(int.MaxValue);

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

            start = Math.Min(_iterations, _length - _tau);

            abstand = new double[_length];

            var timer = Stopwatch.StartNew();

            for (i = (_eDim - 1) * _tau; i < start; i++)
            {
                count++;
                MakeDynamics(dynamics, i);
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
                        Log.Append($"{NumFormat.ToShort(factor[j] / count)} ");
                        Result.Spectrum[j] = factor[j] / count;
                    }

                    Log.AppendLine();
                }
            }

            Log.AppendLine();
            Log.AppendLine($"Avg. rel. forecast error = {NumFormat.ToShort(Math.Sqrt(averr / count) / variance)}");
            Log.AppendLine($"Avg. abs. forecast error = {NumFormat.ToShort(Math.Sqrt(averr / count) * interval)}");
            Log.AppendLine($"Avg. neighborhood size   = {NumFormat.ToShort(aveps * maxinterval / count)}");
            Log.AppendLine($"Avg. number of neighbors = {NumFormat.ToShort(avneig / count)}");
        }

        private double Sort(long act, long nFound, out long nfound, out bool enough)
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
                    maxdx = Math.Abs(TimeSeries[act] - TimeSeries[hf]);

                    for (j = 1; j < _eDim; j++)
                    {
                        del = indexes[j];
                        dx = Math.Abs(TimeSeries[act - del] - TimeSeries[hf - del]);
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

            if (!epsset || (abstand[_minNeighbors - 1] >= epsmin))
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
                if (abstand[i] > epsmin)
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

        private void MakeDynamics(double[] dynamics, int act)
        {
            long i, hi, j, hj, k, t = act;
            long nfound;
            double[,] imat;
            double foundeps = 0.0, epsilon, hv, hv1;
            double new_vec;

            epsilon = epsmin / _epsStep;

            do
            {
                epsilon *= _epsStep;

                if (epsilon > EpsMax)
                {
                    epsilon = EpsMax;
                }

                fnn.PutInBoxes(TimeSeries, epsilon, (_eDim - 1) * _tau, _length - _tau, 0, 0);
                nfound = fnn.FindNeighborsJ(TimeSeries, _eDim, _tau, epsilon, act);

                if (nfound > _minNeighbors)
                {
                    bool got_enough;

                    foundeps = Sort(act, nfound, out nfound, out got_enough);

                    if (got_enough)
                    {
                        break;
                    }
                }
            } 
            while (epsilon < EpsMax);

            avneig += nfound;
            aveps += foundeps;

            if (!epsset)
            {
                epsmin = aveps / count;
            }

            if (nfound < _minNeighbors)
            {
                throw new CalculationException("Not enough neighbors found.");
            }

            Ext.FillVectorWith(vector, 0d);
            Ext.FillMatrixWith(matrix, 0d);

            for (i = 0; i < nfound; i++)
            {
                act = fnn.Found[i];
                matrix[0, 0] += 1.0;

                for (j = 0; j < _eDim; j++)
                {
                    matrix[0, j + 1] += TimeSeries[act - indexes[j]];
                }

                for (j = 0; j < _eDim; j++)
                {
                    hv1 = TimeSeries[act - indexes[j]];
                    hj = j + 1;

                    for (k = j; k < _eDim; k++)
                    {
                        matrix[hj, k + 1] += TimeSeries[act - indexes[k]] * hv1;
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

            imat = InvertMatrix(matrix, _eDim + 1);

            Ext.FillVectorWith(vector, 0d);

            for (i = 0; i < nfound; i++)
            {
                act = fnn.Found[i];
                hv = TimeSeries[act + _tau];
                vector[0] += hv;

                for (j = 0; j < _eDim; j++)
                {
                    vector[j + 1] += hv * TimeSeries[act - indexes[j]];
                }
            }

            for (i = 0; i <= _eDim; i++)
            {
                vector[i] /= (double)nfound;
            }

            new_vec = 0.0;

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
                new_vec += dynamics[i] * TimeSeries[t - indexes[i]];
            }

            averr += (new_vec - TimeSeries[t + _tau]) * (new_vec - TimeSeries[t + _tau]);

        }

        private void GramSchmidt(double[,] delta, double[] stretch)
        {
            double[,] dnew = new double[_eDim, _eDim];
            double norm;
            double[] diff = new double[_eDim];
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
                    norm += Math.Pow(delta[i, j] + diff[j], 2);
                }

                stretch[i] = (norm = Math.Sqrt(norm));

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
            double[,] dnew;
            long i, j, k;

            dnew = new double[_eDim, _eDim];

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

        private double[,] InvertMatrix(double[, ] mat, int size)
        {
            int i, j, k;
            double[,] imat;
            double[] vec = new double[size];
            double[][] hmat;

            hmat = new double[size][];

            for (int ii = 0; ii < size; ii++)
            {
                hmat[ii] = new double[size];
            }

            imat = new double[size, size];

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

        private int[] MakeIndex(int eDim, int delay)
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
