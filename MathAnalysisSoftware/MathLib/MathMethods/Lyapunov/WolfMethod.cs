using System;
using System.Globalization;
using System.Text;

namespace MathLib.MathMethods.Lyapunov {
    public class WolfMethod : LyapunovMethod {
        private int Dim { get; set; }
        private int Tau { get; set; }
        private double StepSize { get; set; }
        private double ScaleMin { get; set; }
        private double ScaleMax { get; set; }
        private int Evolv { get; set; }

        private double[] pt1;
        private double[] pt2; 

        private double zlyap;
        public double rezult;

        private int step;


        //Pre-calculated variablees;
        double Ev_Mul_St_Mul_Log2;
        int tsLen;

        public WolfMethod(double[] timeSeries, int dim, int tau, double stepSize, double scaleMin, double scaleMax, int evolv)
            : base(timeSeries) {
            
            Dim = dim;
            Tau = tau;
            StepSize = stepSize;
            ScaleMin = scaleMin;
            ScaleMax = scaleMax;
            Evolv = evolv;

            pt1 = new double[Dim];
            pt2 = new double[Dim];

            Ev_Mul_St_Mul_Log2 = (double)Evolv * StepSize * Math.Log(2d);
            tsLen = timeSeries.Length;
        }


        public override void Calculate() {
            
            //ind points to fiducial trajectory
            //ind2 points to second trajectory
            //sum holds running exponent estimate sans i/time
            //its is total number of propagation steps

            //int ind = 0; //в фортране было 1
            int ind2 = 0;//в фортране без нуля и нет            
            double sum = 0d;
            int its = 0;

            double dii = 0;//в фортране нет

            //calculate useful size of datafile
            int dataPointsCount = timeSeries.Length - Dim * Tau - Evolv;

            //find nearest neighbor to first data point
            double di = 1e38;

            //dont take point too close to fiducial point
            for (int i = 10; i < dataPointsCount; i++) {
                //compute separation between fiducial point and candidate
                double d = 0d;

                for (int j = 0; j < Dim; j++) 
                    d += Math.Pow(GetAttractorPoint(0, j) - GetAttractorPoint(i, j), 2);

                d = Math.Sqrt(d);

                //store the best point so far but no closer than noise scale
                if (d >= ScaleMin && d <= di) {
                    di = d;
                    ind2 = i;
                }
            }

            //Log.Append("Lyapunov exponent\tTotal Propagation Time\tDI\tInformation dimention\n");

            for(int ind = Evolv; ind < dataPointsCount; ind += Evolv) { 
                //get coordinates of evolved points
                for (int j = 0; j < Dim; j++) {
                    pt1[j] = GetAttractorPoint(ind, j);
                    pt2[j] = GetAttractorPoint(ind2 + Evolv, j);
                }

                //compute final separation between pair, update exponent
                double df = 0d;

                for (int j = 0; j < Dim; j++)
                    df += Math.Pow(pt1[j] - pt2[j], 2);

                df = Math.Sqrt(df);

                its++;
                sum += Math.Log(df / di) / Ev_Mul_St_Mul_Log2;
                zlyap = sum / (double)its;

                //Log.AppendFormat("{0:F5}\t{1}\t{2:F5}\t{3:F5}\n", zlyap, Evolv * its, di, df);
                slope.AddDataPoint(step++, zlyap);

                //look for replacement point
                //zmult is multiplier of scalMax when go to longer distances
                int indold = ind2;
                double zmult = 1d;
                double anglmx = 0.3;

                metka:

                double thmin = Math.PI;//3.14;

                //search over all points
                for (int i = 0; i < dataPointsCount; i++) {
                    //dont take points too close in time to fiducial point
                    int iii = Math.Abs(i - ind);

                    if (iii < 9)
                        continue;

                    //compute distance between fiducial point and candidate
                    double dnew = 0d;
                    for (int j = 0; j < Dim; j++)
                        dnew += Math.Pow(pt1[j] - GetAttractorPoint(i, j), 2);

                    dnew = Math.Sqrt(dnew);

                    //look further away than noise scale, closer than zmult*scalMax
                    if (dnew > zmult * ScaleMax || dnew < ScaleMin)
                        continue;

                    //find angular change old to new vector
                    double dot = 0d;
                    for (int j = 0; j < Dim; j++)
                        dot += (pt1[j] - GetAttractorPoint(i, j)) * (pt1[j] - pt2[j]);

                    double cth = Math.Abs(dot / (dnew * df));

                    cth = Math.Min(cth, 1d);

                    double th = Math.Acos(cth);

                    //save point with smallest angular change so far
                    if (th < thmin) { 
                        thmin = th;
                        dii = dnew;
                        ind2 = i;
                    }
                }

                if (thmin > anglmx) {

                    //cant find a replacement - look at longer distances
                    zmult++;
                    if (zmult <= 5d) goto metka;

                    //no replacement at 5*scale, double search angle, reset distance
                    zmult = 1d;
                    anglmx *= 2d;
                    if (anglmx < Math.PI /*3.14*/) goto metka;

                    ind2 = indold + Evolv;
                    dii = df;
                }

                di = dii;
            }

            rezult = zlyap;
        }

        ///<summary>Define delay coordinates with a statement function</summary>
        ///<returns>j-th component of i-th reconstructed attractor point</returns>
        private double GetAttractorPoint(int i, int j) {
            int point = i + j * Tau;
            if (point < tsLen)
                return timeSeries[point];
            else
                return 0;
        }


        public override string GetInfoShort() {
            return string.Format(CultureInfo.InvariantCulture, "{0:F5}", rezult);
        }


        public override string GetInfoFull() {
            StringBuilder fullInfo = new StringBuilder();

            fullInfo.AppendFormat(CultureInfo.InvariantCulture, "Lyapunov exponent: {0:F5}\n", rezult)
                .AppendFormat("Embedding dimension: {0}\n", Dim)
                .AppendFormat("Reconstruction delay: {0}\n", Tau)
                .AppendFormat(CultureInfo.InvariantCulture, "Step size: {0:F5}\n", StepSize)
                .AppendFormat(CultureInfo.InvariantCulture, "Min scale: {0:F5}\n", ScaleMin)
                .AppendFormat(CultureInfo.InvariantCulture, "Max scale: {0:F5}\n", ScaleMax)
                .AppendFormat("Evolution steps: {0}\n\n", Evolv)
                .Append(Log.ToString());
            return fullInfo.ToString();
        }

    }
}
