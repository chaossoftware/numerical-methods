using System;
using System.Text;
using MathLib.Data;
using MathLib.MathMethods.EmbeddingDimension;

namespace MathLib.MathMethods.Lyapunov
{
    public class JakobianMethod : LleMethod
    {
        private const int OUT = 10;
        private const double EPSMAX = 1.0;
        private const int tau = 1;

        bool epsset = false;
        bool inverse;
        int length, iterations, exclude = 0;
        int eDim = 2/*, DIMENSION = 1,DELAY=1*/, minNeighbors = 30;
        double epsstep = 1.2;

        double averr;
        double avneig = 0.0, aveps = 0.0;
        double[][] mat;
        double[] vec, abstand;
        double epsmin;
        long count = 0;
        int[] list;
        int[] found;
        int[] indexes;

        private Random random;
        private readonly BoxAssistedFnn fnn;

        public JakobianMethod(double[] timeSeries, int eDim, int iterations, double scaleMin, double epsstep, int minNeigh, bool inverse)
            : base(timeSeries)
        {
            this.eDim = eDim;
            this.iterations = iterations;
            this.epsmin = scaleMin;
            this.epsset = true;
            this.epsstep = epsstep;
            this.minNeighbors = minNeigh;
            this.inverse = inverse;

            length = TimeSeries.Length;

            Slope = new Timeseries();
            Log = new StringBuilder();
            random = new Random();
            fnn = new BoxAssistedFnn(512);
        }

        public override void Calculate()
        {
            double[,] delta;
            double[] dynamics;
            double[] lfactor;
            double[] factor;
            double dim;
            double av = 0d;
            double var = 0d;
            double maxinterval = 0d;
            long start, i, j;

            double min = 0d;
            double interval = 0d;

            if (minNeighbors > (length - tau * (eDim - 1) - 1))
            {
                throw new Exception("Your time series is not long enough to find " + minNeighbors + " neighbors!Exiting.\n");
            }

            averr = 0.0;

            RescaleData(TimeSeries, out min, out interval);

            if (interval > maxinterval)
            {
                maxinterval = interval;
            }

            Variance(TimeSeries, length, out av, out var);

            if (inverse)
            {
                Array.Reverse(TimeSeries);
            }

            epsmin = epsset ? epsmin / maxinterval : 0.001d;

            list = new int[length];
            found = new int[length];

            dynamics = new double[eDim];
            factor = new double[eDim];
            lfactor = new double[eDim];
            delta = new double[eDim, eDim];
            vec = new double[eDim + 1];
            mat = new double[eDim + 1][];

            for (int ii = 0; ii < eDim + 1; ii++)
            {
                mat[ii] = new double[eDim + 1];
            }

            indexes = MakeIndex(eDim, tau);

            random = new Random(int.MaxValue);

            for (i = 0; i < 10000; i++)
            {
                random.Next();
            }

            for (i = 0; i < eDim; i++)
            {
                factor[i] = 0d;

                for (j = 0; j < eDim; j++)
                {
                    delta[i, j] = (double)random.Next() / int.MaxValue;
                }
            }

            GramSchmidt(delta, lfactor);

            start = Math.Min(iterations, length - tau);

            abstand = new double[length];

            var lastTime = Time(DateTime.Now);

            for (i = (eDim - 1) * tau; i < start; i++)
            {
                count++;
                make_dynamics(dynamics, i);
                MakeIteration(dynamics, delta);
                GramSchmidt(delta, lfactor);

                for (j = 0; j < eDim; j++)
                {
                    factor[j] += Math.Log(lfactor[j]) / tau;
                }

                if (((Time(DateTime.Now) - lastTime) > OUT) || (i == (start - 1)))
                {
                    //time(&lasttime);

                    Log.Append($"{count} ");

                    for (j = 0; j < eDim; j++)
                    {
                        Log.Append($"{factor[j] / count} ");
                    }

                    Log.AppendLine();
                }
            }

            dim = 0.0;

            for (i = 0; i < eDim; i++)
            {
                dim += factor[i];

                if (dim < 0.0)
                {
                    break;
                }
            }

            dim = i < eDim ? 
                i + (dim - factor[i]) / Math.Abs(factor[i]) : 
                eDim;

            Log.Append("#Average relative forecast error:= ");
            Log.Append($"{Math.Sqrt(averr / count) / var}");
            Log.AppendLine();

            Log.Append("#Average absolute forecast error:= ");
            Log.Append($"{Math.Sqrt(averr / count) * interval}");
            Log.AppendLine();

            Log.AppendLine($"#Average Neighborhood Size= {aveps * maxinterval / count}");
            Log.AppendLine($"#Average num. of neighbors= {avneig / count}");
            Log.AppendLine($"#estimated KY-Dimension= {dim}");
        }

        public override string GetInfoShort()
        {
            return Log.ToString();
        }

        public override string GetInfoFull()
        {
            StringBuilder fullInfo = new StringBuilder();

            //fullInfo.AppendFormat("Min Embedding dimension: {0}\n", DimMin)
            //    .AppendFormat("Max Embedding dimension: {0}\n", DimMax)
            //    .AppendFormat("Delay: {0}\n", Tau)
            //    .AppendFormat("Max Iterations: {0}\n", MaxIterations)
            //    .AppendFormat("Window around the reference point which should be omitted: {0}\n", Window)
            //    .AppendFormat(CultureInfo.InvariantCulture, "Min scale: {0:F5}\n", Epsmin)
            //    .AppendFormat(CultureInfo.InvariantCulture, "Max scale: {0:F5}\n\n", Epsmax)
            //    .Append(Log.ToString());
            return fullInfo.ToString();
        }

        public void SetSlope(string index)
        {
            //SlopesList.TryGetValue(index, out slope);
        }

        double Sort(long act, long nFound, out long nfound, out bool enough)
        {
            double maxeps = 0.0, dx, dswap, maxdx;
            int self = 0, i, j, del, hf, iswap;
            long imax = nFound;

            enough = false;

            for (i = 0; i < imax; i++)
            {
                hf = found[i];
                if (hf != act)
                {
                    maxdx = Math.Abs(TimeSeries[act] - TimeSeries[hf]);

                    for (j = 1; j < eDim; j++)
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
                found[self] = found[imax - 1];
            }

            for (i = 0; i < minNeighbors; i++)
            {
                for (j = i + 1; j < imax - 1; j++)
                {
                    if (abstand[j] < abstand[i])
                    {
                        dswap = abstand[i];
                        abstand[i] = abstand[j];
                        abstand[j] = dswap;
                        iswap = found[i];
                        found[i] = found[j];
                        found[j] = iswap;
                    }
                }
            }

            if (!epsset || (abstand[minNeighbors - 1] >= epsmin))
            {
                nfound = minNeighbors;
                enough = true;
                maxeps = abstand[minNeighbors - 1];

                return maxeps;
            }

            for (i = minNeighbors; i < imax - 2; i++)
            {
                for (j = i + 1; j < imax - 1; j++)
                {
                    if (abstand[j] < abstand[i])
                    {
                        dswap = abstand[i];
                        abstand[i] = abstand[j];
                        abstand[j] = dswap;
                        iswap = found[i];
                        found[i] = found[j];
                        found[j] = iswap;
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

        void make_dynamics(double[] dynamics, long act)
        {
            long i, hi, j, hj, k, t = act, d;
            long nfound = 0;
            double[,] imat;
            double foundeps = 0.0, epsilon, hv, hv1;
            double new_vec;
            bool got_enough;

            epsilon = epsmin / epsstep;

            do
            {
                epsilon *= epsstep;

                if (epsilon > EPSMAX)
                {
                    epsilon = EPSMAX;
                }

                fnn.PutInBoxes(TimeSeries, list, epsilon, (eDim - 1) * tau, length - tau, 0, 0);

                nfound = FindMultiNeighbors(TimeSeries, fnn.Boxes, list, length - tau, 512,
                            eDim, tau, epsilon, found, (int)act);

                if (nfound > minNeighbors)
                {
                    foundeps = Sort(act, nfound, out nfound, out got_enough);
                    if (got_enough)
                        break;
                }
            } while (epsilon < EPSMAX);

            avneig += nfound;
            aveps += foundeps;

            if (!epsset)
            {
                epsmin = aveps / count;
            }

            if (nfound < minNeighbors)
            {
                throw new Exception("#Not enough neighbors found. Exiting\n");
            }

            for (i = 0; i <= eDim; i++)
            {
                vec[i] = 0.0;

                for (j = 0; j <= eDim; j++)
                {
                    mat[i][j] = 0.0;
                }
            }

            for (i = 0; i < nfound; i++)
            {
                act = found[i];
                mat[0][0] += 1.0;

                for (j = 0; j < eDim; j++)
                {
                    mat[0][j + 1] += TimeSeries[act - indexes[j]];
                }

                for (j = 0; j < eDim; j++)
                {
                    hv1 = TimeSeries[act - indexes[j]];
                    hj = j + 1;

                    for (k = j; k < eDim; k++)
                    {
                        mat[hj][k + 1] += TimeSeries[act - indexes[k]] * hv1;
                    }
                }
            }

            for (i = 0; i <= eDim; i++)
            {
                for (j = i; j <= eDim; j++)
                {
                    mat[j][i] = (mat[i][j] /= (double)nfound);
                }
            }

            imat = InvertMatrix(mat, eDim + 1);

            for (i = 0; i <= eDim; i++)
            {
                vec[i] = 0.0;
            }

            for (i = 0; i < nfound; i++)
            {
                act = found[i];
                hv = TimeSeries[act + tau];
                vec[0] += hv;

                for (j = 0; j < eDim; j++)
                {
                    vec[j + 1] += hv * TimeSeries[act - indexes[j]];

                }
            }

            for (i = 0; i <= eDim; i++)
            {
                vec[i] /= (double)nfound;
            }

            new_vec = 0.0;

            for (i = 0; i <= eDim; i++)
            {
                new_vec += imat[0, i] * vec[i];
            }

            for (i = 1; i <= eDim; i++)
            {
                hi = i - 1;
                dynamics[hi] = 0.0;

                for (j = 0; j <= eDim; j++)
                {
                    dynamics[hi] += imat[i, j] * vec[j];
                }
            }

            for (i = 0; i < eDim; i++)
            {
                new_vec += dynamics[i] * TimeSeries[t - indexes[i]];
            }

            averr += (new_vec - TimeSeries[t + tau]) * (new_vec - TimeSeries[t + tau]);

        }

        void GramSchmidt(double[,] delta, double[] stretch)
        {
            double[,] dnew = new double[eDim, eDim];
            double norm;
            double[] diff = new double[eDim];
            long i, j, k;

            for (i = 0; i < eDim; i++)
            {
                for (j = 0; j < eDim; j++)
                {
                    diff[j] = 0.0;
                }

                for (j = 0; j < i; j++)
                {
                    norm = 0.0;

                    for (k = 0; k < eDim; k++)
                    {
                        norm += delta[i, k] * dnew[j, k];
                    }

                    for (k = 0; k < eDim; k++)
                    {
                        diff[k] -= norm * dnew[j, k];
                    }
                }

                norm = 0.0;

                for (j = 0; j < eDim; j++)
                {
                    norm += Math.Pow(delta[i, j] + diff[j], 2);
                }

                stretch[i] = (norm = Math.Sqrt(norm));

                for (j = 0; j < eDim; j++)
                {
                    dnew[i, j] = (delta[i, j] + diff[j]) / norm;
                }
            }

            for (i = 0; i < eDim; i++)
            {
                for (j = 0; j < eDim; j++)
                {
                    delta[i, j] = dnew[i, j];
                }
            }
        }

        void MakeIteration(double[] dynamics, double[,] delta)
        {
            double[,] dnew;
            long i, j, k;

            dnew = new double[eDim, eDim];

            for (i = 0; i < eDim; i++)
            {
                dnew[i, 0] = dynamics[0] * delta[i, 0];

                for (k = 1; k < eDim; k++)
                {
                    dnew[i, 0] += dynamics[k] * delta[i, k];
                }

                for (j = 1; j < eDim; j++)
                {
                    dnew[i, j] = delta[i, j - 1];
                }
            }

            for (i = 0; i < eDim; i++)
            {
                for (j = 0; j < eDim; j++)
                {
                    delta[i, j] = dnew[i, j];
                }
            }
        }

        double[,] InvertMatrix(double[][] mat, int size)
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
                        hmat[j][k] = mat[j][k];
                    }
                }

                SolveLe(hmat, vec, size);

                for (j = 0; j < size; j++)
                    imat[j, i] = vec[j];
            }

            return imat;
        }


        void SolveLe(double[][] mat, double[] vec, int n)
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
                    throw new Exception("Singular matrix! Exiting!\n");

                for (j = i + 1; j < n; j++)
                {
                    q = -mat[j][i] / pivot;
                    mat[j][i] = 0.0;
                    for (k = i + 1; k < n; k++)
                        mat[j][k] += q * hvec[k];
                    vec[j] += q * vec[i];
                }
            }
            vec[n - 1] /= mat[n - 1][n - 1];
            for (i = n - 2; i >= 0; i--)
            {
                hvec = mat[i];
                for (j = n - 1; j > i; j--)
                    vec[i] -= hvec[j] * vec[j];
                vec[i] /= hvec[i];
            }
        }

        int FindMultiNeighbors(double[] s, int[,] box, int[] list,
                         long l, int bs, int emb, int tau, double eps,
                         int[] flist, int act)
        {
            int nf = 0;
            int i, i1, i2, j, j1, k, k1;
            int ib = bs - 1;
            int element;
            double dx = 0.0;

            i = (int)(s[act] / eps) & ib;
            j = (int)(s[act] / eps) & ib;

            for (i1 = i - 1; i1 <= i + 1; i1++)
            {
                i2 = i1 & ib;

                for (j1 = j - 1; j1 <= j + 1; j1++)
                {
                    element = box[i2, j1 & ib];

                    while (element != -1)
                    {
                        for (k = 0; k < emb; k++)
                        {
                            k1 = -k * tau;

                            dx = Math.Abs(s[k1 + act] - s[element + k1]);
                            if (dx > eps)
                            {
                                break;
                            }

                            if (dx > eps)
                            {
                                break;
                            }
                        }

                        if (dx <= eps)
                        {
                            flist[nf++] = element;
                        }

                        element = list[element];
                    }
                }
            }

            return nf;
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

        private void Variance(double[] s, long l, out double av, out double var)
        {
            double h;

            av = var = 0.0;

            for (long i = 0; i < l; i++)
            {
                h = s[i];
                av += h;
                var += h * h;
            }

            av /= (double)l;
            var = Math.Sqrt(Math.Abs(var / (double)l - av * av));

            if (var == 0.0)
            {
                throw new Exception("Variance of the data is zero. Exiting!\n\n");
            }
        }

        private DateTime start = new DateTime(1970, 1, 1);

        private long Time(DateTime dt) =>
            (long)(dt - start).TotalSeconds;
    }
}
