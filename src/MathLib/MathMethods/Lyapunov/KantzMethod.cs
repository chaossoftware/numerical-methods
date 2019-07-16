using MathLib.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MathLib.IO;

namespace MathLib.MathMethods.Lyapunov
{
    public class KantzMethod : LleMethod
    {
        private const int Box = 128;
        private const ushort IBox = Box - 1;

        private int dimMax;
        private int tau;
        private int maxIterations;
        private int window; //'theiler window' (0)
        private int epscount; //number of length scales to use (5)
        private double epsmin = 1e-3;    
        private double epsmax = 1e-2;

        private bool eps0set = false;
        private bool eps1set = false;

        double max, min;
        long reference = long.MaxValue;
        private int blength;

        double[] lyap;
        int[,] box = new int[Box, Box];
        int[] liste;
        int[] lfound, count;
        int found;

        public Dictionary<string, Timeseries> SlopesList;

        public KantzMethod(double[] timeSeries, int dimMax, int tau, int maxIterations, int window, double scaleMin, double scaleMax, int epscount)
            : base(timeSeries)
        {
            this.dimMax = dimMax;
            if (this.dimMax < 2)
            {
                throw new ArgumentException("DimMax < 2");
            }

            this.tau = tau;
            this.maxIterations = maxIterations;
            this.window = window;

            if (scaleMin != 0)
            {
                eps0set = true;
                epsmin = scaleMin;
            }

            if (scaleMax != 0)
            {
                eps1set = true;
                epsmax = scaleMax;
            }

            if (epsmin >= epsmax)
            {
                throw new ArgumentException("EpsMin > EpsMax");
            }

            this.epscount = epsmin == epsmax ? 1 : epscount;

            SlopesList = new Dictionary<string, Timeseries>();
            blength = timeSeries.Length - (this.dimMax - 1) * this.tau - this.maxIterations;
        }

        public override void Calculate()
        {
            double eps_fak;
            double epsilon;
            int j,l;

            RescaleData(TimeSeries, out min, out max);

            if (eps0set)
            {
                epsmin /= max;
            }

            if (eps1set)
            {
                epsmax /= max;
            }

            reference = Math.Min(reference, blength);

            if ((maxIterations + (dimMax - 1) * tau) >= TimeSeries.Length)
            {
                throw new ArgumentException("Too few points to handle these parameters!");
            }
  
            liste = new int[TimeSeries.Length];
            found = 0;
            lfound = new int[TimeSeries.Length];
            count = new int[maxIterations + 1];
            lyap = new double[maxIterations + 1];

            eps_fak = epscount == 1 ? 1d : Math.Pow(epsmax / epsmin, 1d / (epscount - 1));

            for (l = 0; l < epscount; l++)
            {
                epsilon = epsmin * Math.Pow(eps_fak, l);

                Array.Clear(count, 0, count.Length);
                Array.Clear(lyap, 0, lyap.Length);

                PutInBoxes(TimeSeries, box, liste, epsilon, 0, blength, tau);

                for (int i = 0; i < reference; i++)
                {
                    LfindNeighbors(i, epsilon);
                    Iterate(i);
                }

                Log.AppendFormat(CultureInfo.InvariantCulture, "epsilon= {0:F5}\n", epsilon * max);

                Timeseries dict = new Timeseries();

                for (j = 0; j <= maxIterations; j++)
                {
                    if (count[j] != 0)
                    {
                        Log.AppendFormat(CultureInfo.InvariantCulture, "{0}\t{1:F5}\t{2}\n", j, lyap[j] / count[j], count[j]);
                        dict.AddDataPoint(j, lyap[j] / count[j]);
                    }
                }
                    
                Log.Append("\n");

                if (dict.Length > 1)
                {
                    SlopesList.Add(string.Format("eps = {0:F5}", epsilon * max), dict);
                }
            }
        }

        public override string GetInfoShort() => "Done";

        public override string GetInfoFull() =>
            new StringBuilder()
                .AppendFormat("Max Embedding dimension: {0}\n", dimMax)
                .AppendFormat("Delay: {0}\n", tau)
                .AppendFormat("Max Iterations: {0}\n", maxIterations)
                .AppendFormat("Window around the reference point which should be omitted: {0}\n", window)
                .Append("Min scale: ").AppendLine(epsmin.ToString(NumFormat.Short, CultureInfo.InvariantCulture))
                .Append("Max scale: ").AppendLine(epsmax.ToString(NumFormat.Short, CultureInfo.InvariantCulture))
                .AppendLine().Append(Log.ToString())
                .ToString();

        public void SetSlope(string index)
        {
            if (SlopesList.ContainsKey(index))
            {
                Slope = SlopesList[index];
            }
        }

        private void LfindNeighbors(long act, double eps)
        {
            int k, k1;
            int i, j, i1, i2, j1, element;
            double dx, eps2 = Math.Pow(eps, 2);

            found = 0;

            i = (int)(TimeSeries[act] / eps) & IBox;
            j = (int)(TimeSeries[act + tau] / eps) & IBox;
  
            for (i1 = i - 1; i1 <= i + 1; i1++)
            {
                i2 = i1 & IBox;
    
                for (j1 = j - 1; j1 <= j + 1; j1++)
                {
                    element = box[i2, j1 & IBox];
      
                    while (element != -1)
                    {
	
                        if ((element < (act- window)) || (element > (act+ window)))
                        {
	                        dx = Math.Pow(TimeSeries[act] - TimeSeries[element], 2);
	                        
                            if (dx <= eps2)
                            {
	    
                                k = dimMax - 1;
	                            k1 = k * tau;
	                            dx += Math.Pow(TimeSeries[act + k1] - TimeSeries[element + k1], 2);
	      
                                if (dx <= eps2)
                                {
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


        private void Iterate(long act)
        {
            double[] lfactor = new double[maxIterations + 1];
            double[] dx = new double[maxIterations + 1];
            int i, j ,l, l1;
            long k, element;
            long[] lcount = new long[maxIterations + 1];
              
            for (k = 0; k < found; k++)
            {
                element = lfound[k];
            
                for (i = 0; i <= maxIterations; i++)
                {
                    dx[i] = Math.Pow(TimeSeries[act + i] - TimeSeries[element + i], 2);
                }

                for (l = 1; l < dimMax; l++)
                {
                    l1 = l * tau;
            
                    for (i = 0; i <= maxIterations; i++)
                    {
                        dx[i] += Math.Pow(TimeSeries[act + i + l1] - TimeSeries[element + l1 + i], 2);
                    }
                }
            
                for (i = 0; i <= maxIterations; i++)
                {
                    if (dx[i] > 0.0)
                    {
                        lcount[i]++;
                        lfactor[i] += dx[i];
                    }
                }
            }
  
            for (j = 0; j <= maxIterations; j++)
            {
                if (lcount[j] != 0)
                {
                    count[j]++;
                    lyap[j] += Math.Log(lfactor[j] / lcount[j]) / 2.0;
                }
            }
        }
    }
}
