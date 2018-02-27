using MathLib.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MathLib.MathMethods.Lyapunov {
    public class KantzMethod : LyapunovMethod {

        private int DimMax { get; set; }
        private int Tau { get; set; }
        private int MaxIterations { get; set; }
        private int Window { get; set; }        //'theiler window' (0)
        private int Epscount { get; set; }      //number of length scales to use (5)
        private double Epsmin = 1e-3;    
        private double Epsmax = 1e-2;    

        private const int BOX = 128;
        private const ushort ibox = BOX - 1;
        
        bool eps0set = false, eps1set = false;

        double max, min;
        long reference = long.MaxValue;
        private int Blength;

        double[] lyap;
        int[,] box = new int[BOX, BOX];
        int[] liste;
        int[] lfound, count;
        int found;

        public Dictionary<string, DataSeries> SlopesList;

        public KantzMethod(double[] timeSeries, int dimMax, int tau, int maxIterations, int window, double scaleMin, double scaleMax, int epscount)
            :base(timeSeries) {

            DimMax = dimMax;
            if (DimMax < 2)
                throw new ArgumentException("DimMax < 2");

            Tau = tau;
            MaxIterations = maxIterations;
            Window = window;

            if (scaleMin != 0) {
                eps0set = true;
                Epsmin = scaleMin;
            }

            if (scaleMax != 0) {
                eps1set = true;
                Epsmax = scaleMax;
            }

            if (Epsmin >= Epsmax)
                throw new ArgumentException("EpsMin > EpsMax");

            if (Epsmin == Epsmax)
                Epscount = 1;
            else
                Epscount = epscount;

            SlopesList = new Dictionary<string, DataSeries>();
            Blength = timeSeries.Length - (DimMax - 1) * Tau - MaxIterations;
        }


        public override void Calculate() {
            double eps_fak;
            double epsilon;
            int j,l;

            LleHelper.RescaleData(timeSeries, out min, out max);

            if (eps0set)
                Epsmin /= max;
  
            if (eps1set)
                Epsmax /= max;
  
            reference = Math.Min(reference, Blength);
            if ((MaxIterations + (DimMax - 1) * Tau) >= timeSeries.Length) {
                throw new ArgumentException("Too few points to handle these parameters!");
            }
  
            liste = new int[timeSeries.Length];
            found = 0;
            lfound = new int[timeSeries.Length];
            count = new int[MaxIterations + 1];
            lyap = new double[MaxIterations + 1];

            if (Epscount == 1)
                eps_fak = 1d;
            else
                eps_fak = Math.Pow(Epsmax / Epsmin, 1d / (Epscount - 1));

            for (l = 0; l < Epscount; l++) {
                epsilon = Epsmin * Math.Pow(eps_fak, l);

                Array.Clear(count, 0, count.Length);
                Array.Clear(lyap, 0, lyap.Length);

                LleHelper.PutInBoxes(timeSeries, box, liste, epsilon, Blength, Tau);

                for (int i = 0; i < reference; i++) {
                    LfindNeighbors(i, epsilon);
                    Iterate(i);
                }

                Log.AppendFormat(CultureInfo.InvariantCulture, "epsilon= {0:F5}\n", epsilon * max);

                DataSeries dict = new DataSeries();

                for (j = 0; j <= MaxIterations; j++)
                    if (count[j] != 0) {
                        Log.AppendFormat(CultureInfo.InvariantCulture, "{0}\t{1:F5}\t{2}\n", j, lyap[j] / count[j], count[j]);
                        dict.AddDataPoint(j, lyap[j] / count[j]);
                    }
                Log.Append("\n");

                if (dict.Length > 1)
                    SlopesList.Add(string.Format("eps = {0:F5}", epsilon * max), dict);
            }
        }


        private void LfindNeighbors(long act, double eps) {
            int k, k1;
            int i, j, i1, i2, j1, element;
            double dx, eps2 = Math.Pow(eps, 2);

            found = 0;

            i = (int)(timeSeries[act] / eps) & ibox;
            j = (int)(timeSeries[act + Tau] / eps) & ibox;
  
            for (i1 = i - 1; i1 <= i + 1; i1++) {
                i2 = i1 & ibox;
    
                for (j1 = j - 1; j1 <= j + 1; j1++) {
                    element = box[i2, j1 & ibox];
      
                    while (element != -1) {
	
                        if ((element < (act- Window)) || (element > (act+ Window))) {
	                        dx = Math.Pow(timeSeries[act] - timeSeries[element], 2);
	                        
                            if (dx <= eps2) {
	    
                                k = DimMax - 1;
	                            k1 = k * Tau;
	                            dx += Math.Pow(timeSeries[act + k1] - timeSeries[element + k1], 2);
	      
                                if (dx <= eps2) {
		                            k1 = k - 1;
		                            lfound[found] = element;
		                            found++;
	                            }
	                            else
		                            break;
	                        }
	                    }
	
                        element=liste[element];
                    }
                }
            }
        }


        private void Iterate(long act) {
            double[] lfactor = new double[MaxIterations + 1];
            double[] dx = new double[MaxIterations + 1];
            int i, j ,l, l1;
            long k, element;
            long[] lcount = new long[MaxIterations + 1];
              
            for (k = 0; k < found; k++) {
                element = lfound[k];
            
                for (i = 0; i <= MaxIterations; i++)
                    dx[i] = Math.Pow(timeSeries[act + i] - timeSeries[element + i], 2);
            
                for (l = 1; l < DimMax; l++) {
                    l1 = l * Tau;
            
                    for (i = 0; i <= MaxIterations; i++)
                        dx[i] += Math.Pow(timeSeries[act + i + l1] - timeSeries[element + l1 + i], 2);
                }
            
                for (i = 0; i <= MaxIterations; i++)
                    if (dx[i] > 0.0) {
                        lcount[i]++;
                        lfactor[i] += dx[i];
                    }
            }
  
            for (j = 0; j <= MaxIterations; j++)
                if (lcount[j] != 0) {
	                count[j]++;
	                lyap[j] += Math.Log(lfactor[j] / lcount[j]) / 2.0;
                }
        }


        public override string GetInfoShort() {
            return "Done";
        }

        
        public override string GetInfoFull() {
            StringBuilder fullInfo = new StringBuilder();

            fullInfo
                .AppendFormat("Max Embedding dimension: {0}\n", DimMax)
                .AppendFormat("Delay: {0}\n", Tau)
                .AppendFormat("Max Iterations: {0}\n", MaxIterations)
                .AppendFormat("Window around the reference point which should be omitted: {0}\n", Window)
                .AppendFormat(CultureInfo.InvariantCulture, "Min scale: {0:F5}\n", Epsmin)
                .AppendFormat(CultureInfo.InvariantCulture, "Max scale: {0:F5}\n\n", Epsmax)
                .Append(Log.ToString());
            return fullInfo.ToString();
        }


        public void SetSlope(string index) {
            SlopesList.TryGetValue(index, out slope);
        }

    }
}
