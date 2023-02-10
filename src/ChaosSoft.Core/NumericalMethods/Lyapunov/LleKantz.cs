using ChaosSoft.Core.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ChaosSoft.Core.IO;
using ChaosSoft.Core.NumericalMethods.PhaseSpace;
using ChaosSoft.Core.Extensions;

namespace ChaosSoft.Core.NumericalMethods.Lyapunov
{
    /// <summary>
    /// H. Kantz, A robust method to estimate the maximal Lyapunov exponent of a time series, Phys. Lett. A 185, 77 (1994)
    /// </summary>
    public sealed class LleKantz : ITimeSeriesLyapunov, IDescribable
    {
        private const string Paper = "H. Kantz, A robust method to estimate the maximal Lyapunov exponent of a time series, Phys. Lett. A 185, 77 (1994)";
        private readonly int _eDim;
        private readonly int _tau;
        private readonly int _iterations;
        private readonly int _window; // (0)
        private readonly double _epsMin;
        private readonly double _epsMax;

        private int epsCount;
        private double epsMin;
        private double epsMax;

        private readonly double[] _lyap;
        private readonly int[] _count;
        private int nf;

        /// <summary>
        /// The method estimates the largest Lyapunov exponent of a given scalar data set using the algorithm of Kantz.
        /// </summary>
        /// <param name="eDim">embedding dimension</param>
        /// <param name="tau"></param>
        /// <param name="iterations"></param>
        /// <param name="window">theiler window</param>
        /// <param name="epsMin"></param>
        /// <param name="epsMax"></param>
        /// <param name="epsCount">number of length scales to use</param>
        public LleKantz(int eDim, int tau, int iterations, int window, double epsMin, double epsMax, int epsCount)
        {
            _eDim = eDim;
            _tau = tau;
            _iterations = iterations;
            _window = window;
            _epsMin = epsMin;
            _epsMax = epsMax;
            this.epsCount = epsCount;

            _lyap = new double[_iterations + 1];
            _count = new int[_iterations + 1];

            SlopesList = new Dictionary<string, DataSeries>();

            Slope = new DataSeries();
            Log = new StringBuilder();
        }

        public LleKantz(int eDim) : this(eDim, 1, 50, 0, 0, 0, 5)
        {
        }

        public DataSeries Slope { get; set; }

        public StringBuilder Log { get; }

        public Dictionary<string, DataSeries> SlopesList { get; set; }

        /// <summary>
        /// Gets method setup info (parameters values)
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            new StringBuilder()
            .AppendLine("LLE by Kantz")
            .AppendLine($"m = {_eDim}")
            .AppendLine($"τ = {_tau}")
            .AppendLine($"iterations = {_iterations}")
            .AppendLine($"theiler window = {_window}")
            .AppendLine($"min ε = {NumFormatter.ToShort(epsMin)}")
            .AppendLine($"max ε = {NumFormatter.ToShort(epsMax)}")
            .ToString();

        /// <summary>
        /// Gets help on the method and its params
        /// </summary>
        /// <returns></returns>
        public string GetHelp() =>
            new StringBuilder()
            .AppendLine($"LLE by Kantz [{Paper}]")
            .AppendLine("m - embedding dimension (default: 2)")
            .AppendLine("τ - reconstruction delay (default: 1)")
            .AppendLine("iterations (default: 50)")
            .AppendLine("theiler window - Window around the reference point which should be omitted (default: 0)")
            .AppendLine("min ε - Min scale (default: 1e-3)")
            .AppendLine($"max ε - Max scale (default: 1e-2)")
            .ToString();

        public string GetResultAsString() => "Successful";

        public void Calculate(double[] timeSeries)
        {
            double[] series = new double[timeSeries.Length];
            Array.Copy(timeSeries, series, series.Length);

            if (_iterations + (_eDim - 1) * _tau >= series.Length)
            {
                throw new ArgumentException("Too few points to handle specified parameters, it makes no sense to continue.");
            }

            BoxAssistedFnn _fnn = new BoxAssistedFnn(128, series.Length);

            double epsFak;
            double epsilon;
            int j,l;
            var blength = series.Length - (_eDim - 1) * _tau - _iterations;

            double interval = Arrays.Rescale(series);

            epsMin = _epsMin == 0 ? 1e-3 : _epsMin / interval;

            epsMax = _epsMax == 0 ? 1e-2 : _epsMax / interval;

            if (epsMin >= epsMax)
            {
                throw new ArgumentException("EpsMin > EpsMax");
            }

            if (epsMin == epsMax)
            {
                epsCount = 1;
            }

            var reference = Math.Min(int.MaxValue, blength);

            nf = 0;

            epsFak = epsCount == 1 ? 1d : Math.Pow(epsMax / epsMin, 1d / (epsCount - 1));

            for (l = 0; l < epsCount; l++)
            {
                epsilon = epsMin * Math.Pow(epsFak, l);

                Array.Clear(_count, 0, _count.Length);
                Array.Clear(_lyap, 0, _lyap.Length);

                _fnn.PutInBoxes(series, epsilon, 0, blength, 0, _tau);

                for (int i = 0; i < reference; i++)
                {
                    nf = _fnn.FindNeighborsK(series, _eDim, _tau, epsilon, i, _window);
                    Iterate(series, i, _fnn.Found);
                }

                Log.AppendFormat(CultureInfo.InvariantCulture, "epsilon= {0:F5}\n", epsilon * interval);

                DataSeries dict = new DataSeries();

                for (j = 0; j <= _iterations; j++)
                {
                    if (_count[j] != 0)
                    {
                        Log.AppendFormat(CultureInfo.InvariantCulture, "{0}\t{1:F5}\t{2}\n", j, _lyap[j] / _count[j], _count[j]);
                        dict.AddDataPoint(j, _lyap[j] / _count[j]);
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

        private void Iterate(double[] series, long act, int[] found)
        {
            double[] lfactor = new double[_iterations + 1];
            double[] dx = new double[_iterations + 1];
            int i, j ,l, l1;
            long k, element;
            long[] lcount = new long[_iterations + 1];
              
            for (k = 0; k < nf; k++)
            {
                element = found[k];
            
                for (i = 0; i <= _iterations; i++)
                {
                    dx[i] = FastMath.Pow2(series[act + i] - series[element + i]);
                }

                for (l = 1; l < _eDim; l++)
                {
                    l1 = l * _tau;
            
                    for (i = 0; i <= _iterations; i++)
                    {
                        dx[i] += FastMath.Pow2(series[act + i + l1] - series[element + l1 + i]);
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
                    _count[j]++;
                    _lyap[j] += Math.Log(lfactor[j] / lcount[j]) / 2.0;
                }
            }
        }
    }
}
