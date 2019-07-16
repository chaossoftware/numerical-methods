using System;
using System.Text;
using MathLib.Data;

namespace MathLib.MathMethods.Lyapunov
{
    public class JakobianMethod : LleMethod
    {
        private Random random;

        public JakobianMethod(double[] timeSeries, int embeddingDimension, int iterations, double scaleMin, double epsstep, int minNeigh, bool inverse)
            : base(timeSeries)
        {
            random = new Random();

            //DIMENSION = dimension;
            embed = embeddingDimension;
            dimset = true;

            ITERATIONS = iterations;

            epsmin = scaleMin;
            epsset = true;

            EPSSTEP = epsstep;
            MINNEIGHBORS = minNeigh;
            INVERSE = inverse;

            LENGTH = TimeSeries.Length;

            Slope = new Timeseries();
            Log = new StringBuilder();
        }


        public override void Calculate()
        {
            double[,] delta;
            double[] dynamics;
            double[] lfactor;
            double[] factor;
            double dim;
            double[] hseries;
            double interval, av, var;
            double maxinterval;
            long start, i, j;
            double min = 0;

            if (MINNEIGHBORS > (LENGTH - DELAY * (embed - 1) - 1))
                throw new Exception("Your time series is not long enough to find " + MINNEIGHBORS + " neighbors!Exiting.\n");

            interval = 0d;
            av = 0d;
            var = 0d;
            maxinterval = 0d;

            averr = 0.0;

            RescaleData(TimeSeries, out min, out interval);

            if (interval > maxinterval)
            {
                maxinterval = interval;
            }

            Variance(TimeSeries, LENGTH, out av, out var);

            if (INVERSE)
            {
                hseries = new double[LENGTH];

                for (i = 0; i < LENGTH; i++)
                {
                    hseries[LENGTH - 1 - i] = TimeSeries[i];
                }

                for (i = 0; i < LENGTH; i++)
                {
                    TimeSeries[i] = hseries[i];
                }
            }

            if (!epsset)
            {
                epsmin = 1d / 1000d;
            }
            else
            {
                epsmin /= maxinterval;
            }

            box = new int[BOX, BOX];
            list = new int[LENGTH];
            found = new long[LENGTH];

            dynamics = new double[embed];

            factor = new double[embed];
            lfactor = new double[embed];
            delta = new double[embed, embed];
            vec = new double[embed + 1];
            //mat = new double[alldim + 1, alldim + 1];
            mat = new double[embed + 1][];

            for (int ii = 0; ii < embed + 1; ii++)
            {
                mat[ii] = new double[embed + 1];
            }

            indexes = MakeMultiIndex(embed, DELAY);

            random = new Random(int.MaxValue);

            for (i = 0; i < 10000; i++)
            {
                random.Next();
            }

            for (i = 0; i < embed; i++)
            {
                factor[i] = 0.0;

                for (j = 0; j < embed; j++)
                {
                    delta[i, j] = (double)random.Next() / (double)long.MaxValue;
                }
            }

            GramSchmidt(delta, lfactor);

            start = ITERATIONS;

            if (start > (LENGTH - DELAY))
            {
                start = LENGTH - DELAY;
            }

            abstand = new double[LENGTH];

            var lastTime = Time(DateTime.Now);

            for (i = (embed - 1) * DELAY; i < start; i++)
            {
                count++;
                make_dynamics(dynamics, i);
                MakeIteration(dynamics, delta);
                GramSchmidt(delta, lfactor);

                for (j = 0; j < embed; j++)
                {
                    factor[j] += Math.Log(lfactor[j]) / (double)DELAY;
                }

                if (((Time(DateTime.Now) - lastTime) > OUT) || (i == (start - 1)))
                {
                    //time(&lasttime);

                    Log.Append($"{count} ");

                    for (j = 0; j < embed; j++)
                    {
                        Log.Append($"{factor[j] / count} ");
                    }

                    Log.AppendLine();
                }
            }

            dim = 0.0;
            for (i = 0; i < embed; i++)
            {
                dim += factor[i];
                if (dim < 0.0)
                    break;
            }
            if (i < embed)
                dim = i + (dim - factor[i]) / Math.Abs(factor[i]);
            else
                dim = embed;

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


        private const int OUT = 10;
        private const int BOX = 512;
        private const double EPSMAX = 1.0;
        private const int DELAY = 1;

        bool epsset = false;
        bool INVERSE;
        bool dimset = false;
        int LENGTH, ITERATIONS, exclude = 0;
        int embed = 2/*, DIMENSION = 1,DELAY=1*/, MINNEIGHBORS = 30;
        double EPSSTEP = 1.2;

        double averr;
        double avneig = 0.0, aveps = 0.0;
        double[][] mat;
        double[] vec, abstand;
        double epsmin;
        long imax = BOX - 1, count = 0;
        int[,] box;
        int[] list;
        long[] found;
        int[] indexes;

        double Sort(long act, long nFound, out long nfound, out bool enough)
        {
            double maxeps = 0.0, dx, dswap, maxdx;
            long self = 0, i, j, del, hf, iswap;
            long imax = nFound;

            enough = false;

            for (i = 0; i < imax; i++)
            {
                hf = found[i];
                if (hf != act)
                {
                    maxdx = Math.Abs(TimeSeries[act] - TimeSeries[hf]);

                    for (j = 1; j < embed; j++)
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

            for (i = 0; i < MINNEIGHBORS; i++)
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

            if (!epsset || (abstand[MINNEIGHBORS - 1] >= epsmin))
            {
                nfound = MINNEIGHBORS;
                enough = true;
                maxeps = abstand[MINNEIGHBORS - 1];

                return maxeps;
            }

            for (i = MINNEIGHBORS; i < imax - 2; i++)
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
            double[] hser;
            double[,] imat;
            double foundeps = 0.0, epsilon, hv, hv1;
            double new_vec;
            bool got_enough;

            hser = TimeSeries;

            epsilon = epsmin / EPSSTEP;
            do
            {
                epsilon *= EPSSTEP;
                if (epsilon > EPSMAX)
                    epsilon = EPSMAX;
                PutInBoxes(TimeSeries, box, list, epsilon, (embed - 1) * DELAY, LENGTH - DELAY, 0);
                nfound = FindMultiNeighbors(TimeSeries, box, list, hser, LENGTH - DELAY, BOX,
                            embed, DELAY, epsilon, found);
                if (nfound > MINNEIGHBORS)
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

            if (nfound < MINNEIGHBORS)
            {
                throw new Exception("#Not enough neighbors found. Exiting\n");
            }

            for (i = 0; i <= embed; i++)
            {
                vec[i] = 0.0;

                for (j = 0; j <= embed; j++)
                {
                    mat[i][j] = 0.0;
                }
            }

            for (i = 0; i < nfound; i++)
            {
                act = found[i];
                mat[0][0] += 1.0;

                for (j = 0; j < embed; j++)
                {
                    mat[0][j + 1] += TimeSeries[act - indexes[j]];
                }

                for (j = 0; j < embed; j++)
                {
                    hv1 = TimeSeries[act - indexes[j]];
                    hj = j + 1;

                    for (k = j; k < embed; k++)
                    {
                        mat[hj][k + 1] += TimeSeries[act - indexes[k]] * hv1;
                    }
                }
            }

            for (i = 0; i <= embed; i++)
            {
                for (j = i; j <= embed; j++)
                {
                    mat[j][i] = (mat[i][j] /= (double)nfound);
                }
            }

            imat = InvertMatrix(mat, embed + 1);

            for (i = 0; i <= embed; i++)
            {
                vec[i] = 0.0;
            }

            for (i = 0; i < nfound; i++)
            {
                act = found[i];
                hv = TimeSeries[act + DELAY];
                vec[0] += hv;

                for (j = 0; j < embed; j++)
                {
                    vec[j + 1] += hv * TimeSeries[act - indexes[j]];

                }
            }

            for (i = 0; i <= embed; i++)
            {
                vec[i] /= (double)nfound;
            }

            new_vec = 0.0;

            for (i = 0; i <= embed; i++)
            {
                new_vec += imat[0, i] * vec[i];
            }

            for (i = 1; i <= embed; i++)
            {
                hi = i - 1;
                dynamics[hi] = 0.0;

                for (j = 0; j <= embed; j++)
                {
                    dynamics[hi] += imat[i, j] * vec[j];
                }
            }

            for (i = 0; i < embed; i++)
            {
                new_vec += dynamics[i] * TimeSeries[t - indexes[i]];
            }

            averr += (new_vec - TimeSeries[t + DELAY]) * (new_vec - TimeSeries[t + DELAY]);

        }

        void GramSchmidt(double[,] delta, double[] stretch)
        {
            double[,] dnew = new double[embed, embed];
            double norm;
            double[] diff = new double[embed];
            long i, j, k;

            for (i = 0; i < embed; i++)
            {
                for (j = 0; j < embed; j++)
                {
                    diff[j] = 0.0;
                }

                for (j = 0; j < i; j++)
                {
                    norm = 0.0;

                    for (k = 0; k < embed; k++)
                    {
                        norm += delta[i, k] * dnew[j, k];
                    }

                    for (k = 0; k < embed; k++)
                    {
                        diff[k] -= norm * dnew[j, k];
                    }
                }

                norm = 0.0;

                for (j = 0; j < embed; j++)
                {
                    norm += Math.Pow(delta[i, j] + diff[j], 2);
                }

                stretch[i] = (norm = Math.Sqrt(norm));

                for (j = 0; j < embed; j++)
                {
                    dnew[i, j] = (delta[i, j] + diff[j]) / norm;
                }
            }

            for (i = 0; i < embed; i++)
            {
                for (j = 0; j < embed; j++)
                {
                    delta[i, j] = dnew[i, j];
                }
            }
        }

        void MakeIteration(double[] dynamics, double[,] delta)
        {
            double[,] dnew;
            long i, j, k;

            dnew = new double[embed, embed];

            for (i = 0; i < embed; i++)
            {
                dnew[i, 0] = dynamics[0] * delta[i, 0];

                for (k = 1; k < embed; k++)
                {
                    dnew[i, 0] += dynamics[k] * delta[i, k];
                }

                for (j = 1; j < embed; j++)
                {
                    dnew[i, j] = delta[i, j - 1];
                }
            }

            for (i = 0; i < embed; i++)
            {
                for (j = 0; j < embed; j++)
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

        long FindMultiNeighbors(double[] s, int[,] box, int[] list, double[] x,
                         long l, int bs, int emb, int del, double eps,
                         long[] flist)
        {
            long nf = 0;
            int i, i1, i2, j, j1, k, k1;
            int ib = bs - 1;
            long element;
            double dx = 0.0;

            i = (int)(x[0] / eps) & ib;
            j = (int)(x[0] / eps) & ib;

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
                            k1 = -k * (int)del;

                            dx = Math.Abs(x[k1] - s[element + k1]);
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

        private int[] MakeMultiIndex(int emb, int del)
        {
            int[] mmi = new int[emb];

            for (long i = 0; i < emb; i++)
            {
                mmi[i] = (int)((i) * del);
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
