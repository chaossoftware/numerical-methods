using System;
using System.Globalization;
using System.Text;
using MathLib.IO;

namespace MathLib.MathMethods.Lyapunov
{
    public class RosensteinMethod : LleMethod
    {
        private const int NMax = 256;

        private int dim;
        private int tau;
        private int steps;
        private int minDist; //window around the reference point which should be omitted
        private double eps0; //minimal length scale for the neighborhood search

        
        private int nmax = NMax - 1;
        private double eps;
        private double min, max;

        private bool epsset = false;

        private double[] lyap;
        private long[] found;
        private int[,] box = new int[NMax, NMax];
        private int[] list;
        private int bLength;

        public RosensteinMethod(double[] timeSeries, int dim, int tau, int steps, int minDist, double scaleMin)
            : base(timeSeries)
        {
            this.dim = dim;
            this.tau = tau;
            this.steps = steps;
            this.minDist = minDist;

            if (scaleMin != 0)
            {
                epsset = true;
                this.eps0 = scaleMin;
            }
            else
            {
                this.eps0 = 1e-3;
            }

            bLength = timeSeries.Length - (this.dim - 1) * this.tau - this.steps;
        }


        public override void Calculate()
        {
            bool[] done;
            bool alldone;
            long n;
            long maxlength;

            RescaleData(TimeSeries, out min, out max);

            if (epsset)
            {
                eps0 /= max;
            }

            list = new int[TimeSeries.Length];
            lyap = new double[steps + 1];
            found = new long[steps + 1];
            done = new bool[TimeSeries.Length];

            for (int i = 0; i < TimeSeries.Length; i++)
            {
                done[i] = false;
            }

            maxlength = TimeSeries.Length - tau * (dim - 1) - steps - 1 - minDist;
            alldone = false;

            for (eps = eps0; !alldone; eps *= 1.1)
            {
                PutInBoxes(TimeSeries, box, list, eps, 0, bLength, tau * (dim - 1));

                alldone = true;

                for (n = 0; n <= maxlength; n++)
                {
                    if (!done[n])
                    {
                        done[n] = Iterate(n);
                    }

                    alldone &= done[n];
                }

                Log.AppendFormat("epsilon: {0:F5} already found: {1}\n", eps * max, found[0]);
            }

            for (int i = 0; i <= steps; i++)
            {
                if (found[i] != 0)
                {
                    double val = lyap[i] / found[i] / 2.0;
                    Slope.AddDataPoint(i, val);
                    Log.AppendFormat("{0}\t{1:F15}\n", i, val);
                }
            }
        }

        public override string GetInfoShort() => "Done";

        public override string GetInfoFull() => new StringBuilder()
                .AppendFormat("Embedding dimension: {0}\n", dim)
                .AppendFormat("Delay: {0}\n", tau)
                .AppendFormat("Steps: {0}\n", steps)
                .AppendFormat("Window around the reference point which should be omitted: {0}\n", minDist)
                .Append("Min scale: ").AppendLine(eps0.ToString(NumFormat.Short, CultureInfo.InvariantCulture))
                .AppendLine().Append(Log.ToString())
                .ToString();

        private bool Iterate(long act)
        {
            bool ok = false;
            int x, y, i1, k, del1 = dim * tau;
            long element, minelement = -1;
            double dx, mindx = 1.0;

            x = (int)(TimeSeries[act] / eps) & nmax;
            y = (int)(TimeSeries[act + tau * (dim - 1)] / eps) & nmax;
            
            for (int i = x - 1; i <= x + 1; i++)
            {
                i1 = i & nmax;
                
                for (int j = y - 1; j <= y + 1; j++)
                {
                    element = box[i1, j & nmax];
                    
                    while (element != -1)
                    {
                        if (Math.Abs(act - element) > minDist)
                        {
                            dx = 0.0;
                            
                            for (k = 0; k < del1; k += tau)
                            {
                                dx += (TimeSeries[act + k] - TimeSeries[element + k]) *
                                  (TimeSeries[act + k] - TimeSeries[element + k]);

                                if (dx > eps * eps)
                                {
                                    break;
                                }
                            }

                            if (k == del1)
                            {
                                if (dx < mindx)
                                {
                                    ok = true;
                                    
                                    if (dx > 0.0)
                                    {
                                        mindx = dx;
                                        minelement = element;
                                    }
                                }
                            }
                        }

                        element = list[element];
                    }
                }
            }

            if ((minelement != -1))
            {
                act--;
                minelement--;
                
                for (int i = 0; i <= steps; i++)
                {
                    act++;
                    minelement++;
                    dx = 0.0;
                    
                    for (int j = 0; j < del1; j += tau)
                    {
                        dx += (TimeSeries[act + j] - TimeSeries[minelement + j]) * (TimeSeries[act + j] - TimeSeries[minelement + j]);
                    }

                    if (dx > 0.0)
                    {
                        found[i]++;
                        lyap[i] += Math.Log(dx);
                    }
                }
            }
            return ok;
        }



    }
}
