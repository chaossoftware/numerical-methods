using MathLib.DrawEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MathLib.MathMethods.Lyapunov {
    public class KantzMethod : LyapunovMethod {

        private int DimMin { get; set; }
        private int DimMax { get; set; }
        private int Tau { get; set; }
        private int MaxIterations { get; set; }
        private int Window { get; set; }        //'theiler window' (0)
        private int Epscount { get; set; }      //number of length scales to use (5)
        private double Epsmin = 1e-3;//{ get; set; }     
        private double Epsmax = 1e-2;//{ get; set; }     

        private const int BOX = 128;
        private const ushort ibox = BOX - 1;
        
        bool eps0set = false, eps1set = false;

        double max, min;
        long reference = long.MaxValue;
        private int Blength;

        double[,] lyap;
        int[,] box = new int[BOX, BOX];
        int[] liste, found;
        int[,] lfound, count;

        public Dictionary<string, DataSeries> SlopesList;

        public KantzMethod(double[] timeSeries, int dimMin, int dimMax, int tau, int maxIterations, int window, double scaleMin, double scaleMax, int epscount)
            :base(timeSeries) {

            DimMin = dimMin;
            if (DimMin < 2)
                throw new ArgumentException("DimMin < 2");

            DimMax = dimMax;
            if (DimMax < 2)
                throw new ArgumentException("DimMax < 2");

            if (DimMin > DimMax)
                throw new ArgumentException("DimMin > DimMax");

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
            int i,j,l;

            rescaleData();

            if (eps0set)
                Epsmin /= max;
  
            if (eps1set)
                Epsmax /= max;


  
            reference = Math.Min(reference, timeSeries.Length - MaxIterations - (DimMax - 1) * Tau);
            if ((MaxIterations + (DimMax - 1) * Tau) >= timeSeries.Length) {
                throw new ArgumentException("Too few points to handle these parameters!");
            }
  
            liste = new int[timeSeries.Length];
            found = new int[DimMax - 1];
            lfound = new int[DimMax - 1, timeSeries.Length];
            count = new int[DimMax - 1, MaxIterations + 1];
            lyap = new double[DimMax - 1, MaxIterations + 1];

            if (Epscount == 1)
                eps_fak = 1d;
            else
                eps_fak = Math.Pow(Epsmax / Epsmin, 1d / (Epscount - 1));

            for (l = 0; l < Epscount; l++) {
                epsilon = Epsmin * Math.Pow(eps_fak, l);

                Array.Clear(count, 0, count.Length);
                Array.Clear(lyap, 0, lyap.Length);

                PutInBoxes(epsilon);

                for (i = 0; i < reference; i++) {
                    LfindNeighbors(i, epsilon);
                    Iterate(i);
                }

                Log.AppendFormat(CultureInfo.InvariantCulture, "epsilon= {0:F5}\n", epsilon * max);

                for (i = DimMin - 2; i < DimMax - 1; i++) {
                    Log.AppendFormat(CultureInfo.InvariantCulture, "#epsilon= {0:F5}  dim= {1}\n", epsilon * max, i + 2);
                    DataSeries dict = new DataSeries();

                    for (j = 0; j <= MaxIterations; j++)
                        if (count[i, j] != 0) {
                            Log.AppendFormat(CultureInfo.InvariantCulture, "{0}\t{1:F5}\t{2}\n", j, lyap[i, j] / count[i, j], count[i, j]);
                            dict.AddDataPoint(j, lyap[i, j] / count[i, j]);
                        }
                    Log.Append("\n");
                    if (dict.Length > 1)
                        SlopesList.Add(string.Format("d={0} eps={1:F5}", (i + 2), epsilon * max), dict);
                }
            }
        }


        private void PutInBoxes(double eps) {
            int i, j, k;

            for (i = 0; i < BOX; i++)
                for (j = 0; j < BOX; j++)
                    box[i, j]= -1;

            for (i = 0; i < Blength; i++) {
                j = (int)(timeSeries[i] / eps) & ibox;
                k = (int)(timeSeries[i + Tau] / eps) & ibox;
                liste[i] = box[j, k];
                box[j, k] = i;
            }
        }


        private void LfindNeighbors(long act, double eps) {
            int k, k1;
            int i, j, i1, i2, j1, element;
            double dx, eps2 = Math.Sqrt(eps);

            Array.Clear(found, 0, found.Length);

            i = (int)(timeSeries[act] / eps) & ibox;
            j = (int)(timeSeries[act + Tau] / eps) & ibox;
  
            for (i1 = i - 1; i1 <= i + 1; i1++) {
                i2 = i1 & ibox;
    
                for (j1 = j - 1; j1 <= j + 1; j1++) {
                    element = box[i2, j1 & ibox];
      
                    while (element != -1) {
	
                        if ((element < (act- Window)) || (element > (act+ Window))) {
	                        dx = Math.Sqrt(timeSeries[act] - timeSeries[element]);
	                        
                            if (dx <= eps2) {
	    
                                for (k = 1; k < DimMax; k++) {
	                                k1 = k * Tau;
	                                dx += Math.Sqrt(timeSeries[act + k1] - timeSeries[element + k1]);
	      
                                    if (dx <= eps2) {
		                                k1 = k - 1;
		                                lfound[k1, found[k1]] = element;
		                                found[k1]++;
	                                }
	                                else
		                                break;
	                            }
	                        }
	                    }
	
                        element=liste[element];
                    }
                }
            }
        }


        private void Iterate(long act) {
            double[,] lfactor = new double[DimMax - 1, MaxIterations + 1];
            double[] dx = new double[MaxIterations + 1];
            int i, j ,l, l1;
            long k, element;
            long[,] lcount = new long[DimMax - 1, MaxIterations + 1];
              
            for (j = DimMin - 2; j < DimMax - 1; j++) {
                for (k = 0; k < found[j]; k++) {
                    element = lfound[j, k];
            
                    for (i = 0; i <= MaxIterations; i++)
                        dx[i] = Math.Sqrt(timeSeries[act + i] - timeSeries[element + i]);
            
                    for (l = 1; l < j + 2; l++) {
                        l1 = l * Tau;
            
                        for (i = 0; i <= MaxIterations; i++)
                            dx[i] += Math.Sqrt(timeSeries[act + i + l1] - timeSeries[element + l1 + i]);
                    }
            
                    for (i = 0; i <= MaxIterations; i++)
                        if (dx[i] > 0.0) {
                            lcount[j, i]++;
                            lfactor[j, i] += dx[i];
                        }
                }
            }
  
            for (i = DimMin - 2; i < DimMax - 1; i++)
                for (j = 0; j <= MaxIterations; j++)
                    if (lcount[i, j] != 0) {
	                    count[i, j]++;
	                    lyap[i, j] += Math.Log(lfactor[i, j] / lcount[i, j]) / 2.0;
                    }
  
        }


        private void rescaleData() {
            max = Ext.countMax(timeSeries);
            min = Ext.countMin(timeSeries);

            max -= min;

            if (max != 0.0) {
                for (int i = 0; i < timeSeries.Length; i++)
                    timeSeries[i] = (timeSeries[i] - min) / max;
            }
            else {
                throw new Exception(string.Format(
                    "rescale data: data ranges from {0} to {1}. It makes\n\t\tno sense to continue. Exiting",
                    min, min + (max)));
            }
        }


        public override string GetInfoShort() {
            return "Done";
        }

        
        public override string GetInfoFull() {
            StringBuilder fullInfo = new StringBuilder();

            fullInfo.AppendFormat("Min Embedding dimension: {0}\n", DimMin)
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
