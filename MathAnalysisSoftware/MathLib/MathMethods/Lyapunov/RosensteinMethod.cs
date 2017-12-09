using System;
using System.Globalization;
using System.Text;

namespace MathLib.MathMethods.Lyapunov
{
    public class RosensteinMethod : LyapunovMethod {

        private int Dim { get; set; }
        private int Tau { get; set; }
        private int Steps { get; set; }
        private int MinDist { get; set; }   //window around the reference point which should be omitted
        private double Eps0 { get; set; }   //minimal length scale for the neighborhood search

        private const int NMAX = 256;
        private int nmax = NMAX - 1;
        private double eps;
        private double min, max;

        private bool epsset = false;

        private double[] lyap;
        private long[] found;
        private int[,] box = new int[NMAX, NMAX];
        private int[] list;
        int Blength;

        public RosensteinMethod(double[] timeSeries, int dim, int tau, int steps, int minDist, double scaleMin)
            :base(timeSeries) {
            
            this.Dim = dim;
            this.Tau = tau;
            this.Steps = steps;
            this.MinDist = minDist;

            if (scaleMin != 0) {
                epsset = true;
                this.Eps0 = scaleMin;
            }
            else {
                this.Eps0 = 1e-3;
            }

            Blength = timeSeries.Length - (Dim - 1) * Tau - Steps;
        }


        public override void Calculate() {
            bool[] done;
            bool alldone;
            long n;
            long maxlength;

            LleHelper.RescaleData(timeSeries, out min, out max);

            if (epsset)
                Eps0 /= max;

            list = new int[timeSeries.Length];
            lyap = new double[Steps + 1];
            found = new long[Steps + 1];
            done = new bool[timeSeries.Length];

            for (int i = 0; i < timeSeries.Length; i++)
                done[i] = false;

            maxlength = timeSeries.Length - Tau * (Dim - 1) - Steps - 1 - MinDist;
            alldone = false;

            for (eps = Eps0; !alldone; eps *= 1.1) {
                
                LleHelper.PutInBoxes(timeSeries, box, list, eps, Blength, Tau * (Dim - 1));

                alldone = true;
                for (n = 0; n <= maxlength; n++) {
                    if (!done[n])
                        done[n] = iterate(n);
                    alldone &= done[n];
                }

                Log.AppendFormat("epsilon: {0:F5} already found: {1}\n", eps * max, found[0]);
            }
            for (int i = 0; i <= Steps; i++)
                if (found[i] != 0) {
                    double val = lyap[i] / found[i] / 2.0;
                    slope.AddDataPoint(i, val);
                    Log.AppendFormat("{0}\t{1:F15}\n", i, val);
                }
        }





        private bool iterate(long act) {
            bool ok = false;
            int x, y, i1, k, del1 = Dim * Tau;
            long element, minelement = -1;
            double dx, mindx = 1.0;

            x = (int)(timeSeries[act] / eps) & nmax;
            y = (int)(timeSeries[act + Tau * (Dim - 1)] / eps) & nmax;
            
            for (int i = x - 1; i <= x + 1; i++) {
                i1 = i & nmax;
                
                for (int j = y - 1; j <= y + 1; j++) {
                    element = box[i1, j & nmax];
                    
                    while (element != -1) {

                        if (Math.Abs(act - element) > MinDist) {
                            dx = 0.0;
                            
                            for (k = 0; k < del1; k += Tau) {
                                dx += (timeSeries[act + k] - timeSeries[element + k]) *
                                  (timeSeries[act + k] - timeSeries[element + k]);
                                
                                if (dx > eps * eps) break;
                            }

                            if (k == del1) {
                                if (dx < mindx) {
                                    ok = true;
                                    
                                    if (dx > 0.0) {
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

            if ((minelement != -1)) {
                act--;
                minelement--;
                
                for (int i = 0; i <= Steps; i++) {
                    act++;
                    minelement++;
                    dx = 0.0;
                    
                    for (int j = 0; j < del1; j += Tau) {
                        dx += (timeSeries[act + j] - timeSeries[minelement + j]) * (timeSeries[act + j] - timeSeries[minelement + j]);
                    }

                    if (dx > 0.0) {
                        found[i]++;
                        lyap[i] += Math.Log(dx);
                    }
                }
            }
            return ok;
        }



        public override string GetInfoShort() {
            return "Done";
        }

        
        public override string GetInfoFull() {
            StringBuilder fullInfo = new StringBuilder();

            fullInfo.AppendFormat("Embedding dimension: {0}\n", Dim)
                .AppendFormat("Delay: {0}\n", Tau)
                .AppendFormat("Steps: {0}\n", Steps)
                .AppendFormat("Window around the reference point which should be omitted: {0}\n", MinDist)
                .AppendFormat(CultureInfo.InvariantCulture, "Min scale: {0:F5}\n\n", Eps0)
                .Append(Log.ToString());
            return fullInfo.ToString();
        }

    }
}
