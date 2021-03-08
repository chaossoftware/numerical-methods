using ChaosSoft.Core.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ChaosSoft.Core.IO;
using ChaosSoft.Core.NumericalMethods.EmbeddingDimension;

namespace ChaosSoft.Core.NumericalMethods.Lyapunov
{
    /// <summary>
    /// H. Kantz, A robust method to estimate the maximal Lyapunov exponent of a time series, Phys. Lett. A 185, 77 (1994)
    /// </summary>
    public class LleKantz : LyapunovMethod
    {
        private const string Paper = "H. Kantz, A robust method to estimate the maximal Lyapunov exponent of a time series, Phys. Lett. A 185, 77 (1994)";
        private readonly BoxAssistedFnn _fnn;
        private readonly int _eDim;
        private readonly int _tau;
        private readonly int _iterations;
        private readonly int _window; // (0)
        private readonly double _epsMin;
        private readonly double _epsMax;
        private readonly int _length;

        private int epscount;   // (5)
        private double epsmin;  //1e-3  
        private double epsmax;  //1e-2

        private double[] lyap;
        private int[] count;
        private int nf;

        /// <summary>
        /// The method estimates the largest Lyapunov exponent of a given scalar data set using the algorithm of Kantz.
        /// </summary>
        /// <param name="timeSeries">timeseries to analyze</param>
        /// <param name="eDim">embedding dimension</param>
        /// <param name="tau"></param>
        /// <param name="iterations"></param>
        /// <param name="window">theiler window</param>
        /// <param name="scaleMin"></param>
        /// <param name="scaleMax"></param>
        /// <param name="epscount">number of length scales to use</param>
        public LleKantz(double[] timeSeries, int eDim, int tau, int iterations, int window, double scaleMin, double scaleMax, int epscount)
            : base(timeSeries)
        {
            _eDim = eDim;

            _tau = tau;
            _iterations = iterations;
            _window = window;
            _epsMin = scaleMin;
            _epsMax = scaleMax;
            _length = timeSeries.Length;
            this.epscount = epscount;

            if (iterations + (eDim - 1) * tau >= _length)
            {
                throw new ArgumentException("Too few points to handle specified parameters, it makes no sense to continue.");
            }

            _fnn = new BoxAssistedFnn(128, _length);
            SlopesList = new Dictionary<string, Timeseries>();
        }

        public Dictionary<string, Timeseries> SlopesList { get; set; }

        public override string ToString() =>
            new StringBuilder()
            .AppendLine("LLE by Kantz")
            .AppendLine($"m = {_eDim}")
            .AppendLine($"τ = {_tau}")
            .AppendLine($"iterations = {_iterations}")
            .AppendLine($"theiler window = {_window}")
            .AppendLine($"min ε = {NumFormat.ToShort(epsmin)}")
            .AppendLine($"max ε = {NumFormat.ToShort(epsmax)}")
            .ToString();

        public override string GetHelp() =>
            new StringBuilder()
            .AppendLine($"LLE by Kantz [{Paper}]")
            .AppendLine("m - embedding dimension (default: 2)")
            .AppendLine("τ - reconstruction delay (default: 1)")
            .AppendLine("iterations (default: 50)")
            .AppendLine("theiler window - Window around the reference point which should be omitted (default: 0)")
            .AppendLine("min ε - Min scale (default: 1e-3)")
            .AppendLine($"max ε - Max scale (default: 1e-2)")
            .ToString();

        public override string GetResult() => "Successful";

        public override void Calculate()
        {
            double eps_fak;
            double epsilon;
            int j,l;
            var blength = _length - (_eDim - 1) * _tau - _iterations;

            var interval = Ext.RescaleData(TimeSeries);

            epsmin = 
                _epsMin == 0 ? 
                1e-3 : 
                _epsMin / interval;

            epsmax = 
                _epsMax == 0 ? 
                1e-2 : 
                _epsMax / interval;

            if (epsmin >= epsmax)
            {
                throw new ArgumentException("EpsMin > EpsMax");
            }

            this.epscount = epsmin == epsmax ? 1 : epscount;

            var reference = Math.Min(int.MaxValue, blength);

            nf = 0;
            count = new int[_iterations + 1];
            lyap = new double[_iterations + 1];

            eps_fak = epscount == 1 ? 1d : Math.Pow(epsmax / epsmin, 1d / (epscount - 1));

            for (l = 0; l < epscount; l++)
            {
                epsilon = epsmin * Math.Pow(eps_fak, l);

                Array.Clear(count, 0, count.Length);
                Array.Clear(lyap, 0, lyap.Length);

                _fnn.PutInBoxes(TimeSeries, epsilon, 0, blength, 0, _tau);

                for (int i = 0; i < reference; i++)
                {
                    nf = _fnn.FindNeighborsK(TimeSeries, _eDim, _tau, epsilon, i, _window);
                    Iterate(i);
                }

                Log.AppendFormat(CultureInfo.InvariantCulture, "epsilon= {0:F5}\n", epsilon * interval);

                Timeseries dict = new Timeseries();

                for (j = 0; j <= _iterations; j++)
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
                    SlopesList.Add(string.Format("ε = {0:F5}", epsilon * interval), dict);
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
            double[] lfactor = new double[_iterations + 1];
            double[] dx = new double[_iterations + 1];
            int i, j ,l, l1;
            long k, element;
            long[] lcount = new long[_iterations + 1];
              
            for (k = 0; k < nf; k++)
            {
                element = _fnn.Found[k];
            
                for (i = 0; i <= _iterations; i++)
                {
                    dx[i] = Math.Pow(TimeSeries[act + i] - TimeSeries[element + i], 2);
                }

                for (l = 1; l < _eDim; l++)
                {
                    l1 = l * _tau;
            
                    for (i = 0; i <= _iterations; i++)
                    {
                        dx[i] += Math.Pow(TimeSeries[act + i + l1] - TimeSeries[element + l1 + i], 2);
                    }
                }
            
                for (i = 0; i <= _iterations; i++)
                {
                    if (dx[i] > 0.0)
                    {
                        lcount[i]++;
                        lfactor[i] += dx[i];
                    }
                }
            }
  
            for (j = 0; j <= _iterations; j++)
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
