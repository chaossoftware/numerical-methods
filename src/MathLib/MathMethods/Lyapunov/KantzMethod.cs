using MathLib.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using MathLib.IO;
using MathLib.MathMethods.EmbeddingDimension;

namespace MathLib.MathMethods.Lyapunov
{
    public class KantzMethod : LyapunovMethod
    {
        private readonly BoxAssistedFnn fnn;
        private readonly int eDim;
        private readonly int tau;
        private readonly int iterations;
        private readonly int window; //'theiler window' (0)
        private readonly double scaleMin;
        private readonly double scaleMax;
        private readonly int length;

        private int epscount;   //number of length scales to use (5)
        private double epsmin;  //1e-3  
        private double epsmax;  //1e-2

        private double[] lyap;
        private int[] count;
        private int nf;

        public KantzMethod(double[] timeSeries, int eDim, int tau, int iterations, int window, double scaleMin, double scaleMax, int epscount)
            : base(timeSeries)
        {
            this.eDim = eDim;

            this.tau = tau;
            this.iterations = iterations;
            this.window = window;
            this.scaleMin = scaleMin;
            this.scaleMax = scaleMax;
            this.length = timeSeries.Length;

            if (iterations + (eDim - 1) * tau >= length)
            {
                throw new ArgumentException("Too few points to handle specified parameters, it makes no sense to continue.");
            }

            this.fnn = new BoxAssistedFnn(128, length);
            SlopesList = new Dictionary<string, Timeseries>();
        }

        public Dictionary<string, Timeseries> SlopesList { get; set; }

        public override string ToString() =>
            new StringBuilder()
            .AppendLine($"m = {eDim}")
            .AppendLine($"τ = {tau}")
            .AppendLine($"iterations = {iterations}")
            .AppendLine($"theiler window = {window}")
            .AppendLine($"min ε = {epsmin.ToString(NumFormat.Short, CultureInfo.InvariantCulture)}")
            .AppendLine($"max ε = {epsmax.ToString(NumFormat.Short, CultureInfo.InvariantCulture)}")
            .ToString();

        public override string GetInfoFull() =>
            new StringBuilder()
            .AppendLine($"Embedding dimension: {eDim}")
            .AppendLine($"Delay: {tau}")
            .AppendLine($"Max Iterations: {iterations}")
            .AppendLine($"Window around the reference point which should be omitted: {window}")
            .AppendLine($"Min scale: {epsmin.ToString(NumFormat.Short, CultureInfo.InvariantCulture)}")
            .AppendLine($"Max scale: {epsmax.ToString(NumFormat.Short, CultureInfo.InvariantCulture)}")
            .ToString();

        public override string GetResult() => "Successful";

        public override void Calculate()
        {
            double eps_fak;
            double epsilon;
            int j,l;
            var blength = length - (this.eDim - 1) * this.tau - this.iterations;

            var interval = Ext.RescaleData(TimeSeries);

            epsmin = 
                scaleMin == 0 ? 
                1e-3 : 
                scaleMin / interval;

            epsmax = 
                scaleMax == 0 ? 
                1e-2 : 
                scaleMax / interval;

            if (epsmin >= epsmax)
            {
                throw new ArgumentException("EpsMin > EpsMax");
            }

            this.epscount = epsmin == epsmax ? 1 : epscount;

            var reference = Math.Min(int.MaxValue, blength);

            nf = 0;
            count = new int[iterations + 1];
            lyap = new double[iterations + 1];

            eps_fak = epscount == 1 ? 1d : Math.Pow(epsmax / epsmin, 1d / (epscount - 1));

            for (l = 0; l < epscount; l++)
            {
                epsilon = epsmin * Math.Pow(eps_fak, l);

                Array.Clear(count, 0, count.Length);
                Array.Clear(lyap, 0, lyap.Length);

                fnn.PutInBoxes(TimeSeries, epsilon, 0, blength, 0, tau);

                for (int i = 0; i < reference; i++)
                {
                    nf = fnn.FindNeighborsK(TimeSeries, eDim, tau, epsilon, i, window);
                    Iterate(i);
                }

                Log.AppendFormat(CultureInfo.InvariantCulture, "epsilon= {0:F5}\n", epsilon * interval);

                Timeseries dict = new Timeseries();

                for (j = 0; j <= iterations; j++)
                {
                    if (count[j] != 0)
                    {
                        Log.AppendFormat(CultureInfo.InvariantCulture, "{0}\t{1:F5}\t{2}\n", j, lyap[j] / count[j], count[j]);
                        dict.AddDataPoint(j, lyap[j] / count[j]);
                    }
                }
                    
                Log.AppendLine();

                if (dict.Length > 1)
                {
                    SlopesList.Add(string.Format("eps = {0:F5}", epsilon * interval), dict);
                }
            }
        }

        public void SetSlope(string index)
        {
            if (SlopesList.ContainsKey(index))
            {
                Slope = SlopesList[index];
            }
        }

        private void Iterate(long act)
        {
            double[] lfactor = new double[iterations + 1];
            double[] dx = new double[iterations + 1];
            int i, j ,l, l1;
            long k, element;
            long[] lcount = new long[iterations + 1];
              
            for (k = 0; k < nf; k++)
            {
                element = fnn.Found[k];
            
                for (i = 0; i <= iterations; i++)
                {
                    dx[i] = Math.Pow(TimeSeries[act + i] - TimeSeries[element + i], 2);
                }

                for (l = 1; l < eDim; l++)
                {
                    l1 = l * tau;
            
                    for (i = 0; i <= iterations; i++)
                    {
                        dx[i] += Math.Pow(TimeSeries[act + i + l1] - TimeSeries[element + l1 + i], 2);
                    }
                }
            
                for (i = 0; i <= iterations; i++)
                {
                    if (dx[i] > 0.0)
                    {
                        lcount[i]++;
                        lfactor[i] += dx[i];
                    }
                }
            }
  
            for (j = 0; j <= iterations; j++)
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
