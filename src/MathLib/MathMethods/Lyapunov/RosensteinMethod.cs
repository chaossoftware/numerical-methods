using System;
using System.Globalization;
using System.Text;
using MathLib.IO;
using MathLib.MathMethods.EmbeddingDimension;

namespace MathLib.MathMethods.Lyapunov
{
    public class RosensteinMethod : LyapunovMethod
    {
        private int eDim;
        private int tau;
        private int steps;
        private int window; //window around the reference point which should be omitted
        private double eps0; //minimal length scale for the neighborhood search

        private double eps;

        private bool epsset = false;

        private double[] lyap;
        private long[] found;
        private int bLength;

        private readonly BoxAssistedFnn fnn;

        public RosensteinMethod(double[] timeSeries, int eDim, int tau, int steps, int minDist, double scaleMin)
            : base(timeSeries)
        {
            this.eDim = eDim;
            this.tau = tau;
            this.steps = steps;
            this.window = minDist;

            if (scaleMin != 0)
            {
                epsset = true;
                this.eps0 = scaleMin;
            }
            else
            {
                this.eps0 = 1e-3;
            }

            bLength = timeSeries.Length - (this.eDim - 1) * this.tau - this.steps;

            fnn = new BoxAssistedFnn(256, timeSeries.Length);
        }


        public override void Calculate()
        {
            bool[] done;
            bool alldone;
            long n;
            long maxlength;

            var interval = Ext.RescaleData(TimeSeries);

            if (epsset)
            {
                eps0 /= interval;
            }

            lyap = new double[steps + 1];
            found = new long[steps + 1];
            done = new bool[TimeSeries.Length];

            for (int i = 0; i < TimeSeries.Length; i++)
            {
                done[i] = false;
            }

            maxlength = TimeSeries.Length - tau * (eDim - 1) - steps - 1 - window;
            alldone = false;

            for (eps = eps0; !alldone; eps *= 1.1)
            {
                fnn.PutInBoxes(TimeSeries, eps, 0, bLength, 0, tau * (eDim - 1));

                alldone = true;

                for (n = 0; n <= maxlength; n++)
                {
                    if (!done[n])
                    {
                        done[n] = Iterate(n);
                    }

                    alldone &= done[n];
                }

                Log.AppendFormat("epsilon: {0:F5} already found: {1}\n", eps * interval, found[0]);
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

        public override string GetInfoFull() => 
            new StringBuilder()
            .AppendLine($"Embedding dimension: {eDim}")
            .AppendLine($"Delay: {tau}")
            .AppendLine($"Steps: {steps}")
            .AppendLine($"Window around the reference point which should be omitted: {window}")
            .AppendLine($"Min scale: {eps0.ToString(NumFormat.Short, CultureInfo.InvariantCulture)}")
            .AppendLine().Append(Log.ToString())
            .ToString();

        private bool Iterate(long act)
        {
            bool ok = false;
            int x, y, i1, k, del1 = eDim * tau;
            long element, minelement = -1;
            double dx, eps2 = Math.Pow(eps, 2), mindx = 1.0;

            x = (int)(TimeSeries[act] / eps) & fnn.MaxBoxIndex;
            y = (int)(TimeSeries[act + tau * (eDim - 1)] / eps) & fnn.MaxBoxIndex;
            
            for (int i = x - 1; i <= x + 1; i++)
            {
                i1 = i & fnn.MaxBoxIndex;
                
                for (int j = y - 1; j <= y + 1; j++)
                {
                    element = fnn.Boxes[i1, j & fnn.MaxBoxIndex];
                    
                    while (element != -1)
                    {
                        if (Math.Abs(act - element) > window)
                        {
                            dx = 0.0;
                            
                            for (k = 0; k < del1; k += tau)
                            {
                                dx += Math.Pow(TimeSeries[act + k] - TimeSeries[element + k], 2);

                                if (dx > eps2)
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

                        element = fnn.List[element];
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
