using System;
using System.Text;
using ChaosSoft.Core.Data;
using ChaosSoft.Core.DataUtils;
using ChaosSoft.Core.IO;
using ChaosSoft.NumericalMethods.Algebra;
using ChaosSoft.NumericalMethods.PhaseSpace;

namespace ChaosSoft.NumericalMethods.Lyapunov
{
    /// <summary>
    /// LLE by rosenstein<br/>
    /// M. T. Rosenstein, J. J. Collins, C. J. De Luca, A practical method for calculating largest Lyapunov exponents from small data sets, Physica D 65, 117 (1993)
    /// </summary>
    public sealed class LleRosenstein : ITimeSeriesLyapunov, IDescribable
    {
        private const string Paper = "M. T. Rosenstein, J. J. Collins, C. J. De Luca, A practical method for calculating largest Lyapunov exponents from small data sets, Physica D 65, 117 (1993)";
        
        private readonly int _eDim;
        private readonly int _tau;
        private readonly int _iterations;
        private readonly int _window;
        private readonly double _epsMin;

        private double epsilon; //minimal length scale for the neighborhood search
        private double eps;

        private readonly double[] _lyap;
        private readonly int[] _found;

        private string result = "not calculated";

        /// <summary>
        /// Initializes a new instance of the<see cref="LleRosenstein"/> class for full set of parameters.
        /// </summary>
        /// <param name="eDim">embedding dimension</param>
        /// <param name="tau">reconstruction time delay</param>
        /// <param name="iterations">number of iterations</param>
        /// <param name="window">window around the reference point which should be omitted</param>
        /// <param name="epsMin">scales too small</param>
        public LleRosenstein(int eDim, int tau, int iterations, int window, double epsMin)
        {
            _eDim = eDim;
            _tau = tau;
            _iterations = iterations;
            _window = window;
            _epsMin = epsMin;

            _lyap = new double[iterations + 1];
            _found = new int[iterations + 1];

            Slope = new DataSeries();
            Log = new StringBuilder();
        }

        // last parameter (eps) is 0 to obtain further it's default value
        /// <summary>
        /// Initializes a new instance of the <see cref="LleRosenstein"/> class with default values for:<br/>
        /// time delay: 1, iterations: 50, theiler window:0, eps. min: 0<br/>
        /// (eps. min is 0 to obtain further its default value)
        /// </summary>
        /// <param name="eDim">embedding dimension</param>
        public LleRosenstein(int eDim) : this(eDim, 1, 50, 0, 0) 
        {
        }

        /// <summary>
        /// Gets lyapunov exponent slope.
        /// </summary>
        public DataSeries Slope { get; set; }

        /// <summary>
        /// Gets execution log.
        /// </summary>
        public StringBuilder Log { get; }

        /// <summary>
        /// Gets method setup info (parameters values)
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            new StringBuilder()
            .AppendLine("LLE by Rosenstein")
            .AppendLine($"m = {_eDim}")
            .AppendLine($"τ = {_tau}")
            .AppendLine($"iterations = {_iterations}")
            .AppendLine($"theiler window = {_window}")
            .AppendLine($"min ε = {Format.General(epsilon)}")
            .ToString();

        /// <summary>
        /// Gets help on the method and its params
        /// </summary>
        /// <returns></returns>
        public string GetHelp() =>
            new StringBuilder()
            .AppendLine($"LLE by Rosenstein [{Paper}]")
            .AppendLine("m - embedding dimension (default: 2)")
            .AppendLine("τ - reconstruction delay (default: 1)")
            .AppendLine("iterations (default: 50)")
            .AppendLine("theiler window - Window around the reference point which should be omitted (default: 0)")
            .AppendLine("min ε - Min scale (default: 1e-3)")
            .ToString();

        /// <summary>
        /// Gets string representation of result (calculated or not).
        /// </summary>
        /// <returns></returns>
        public string GetResultAsString() => result;

        /// <summary>
        /// Calculates largest lyapunov exponent from timeseries.
        /// The result is stored in <see cref="Slope"/> (lle corresponds to the most smooth part of slope).
        /// </summary>
        /// <param name="timeSeries">source series</param>
        public void Calculate(double[] timeSeries)
        {
            double[] series = new double[timeSeries.Length];
            Array.Copy(timeSeries, series, series.Length);

            if (_iterations + (_eDim - 1) * _tau >= series.Length)
            {
                throw new ArgumentException(
                    "Too few points to handle specified parameters, it makes no sense to continue.");
            }

            BoxAssistedFnn fnn = new BoxAssistedFnn(256, series.Length);

            int n;
            int bLength = series.Length - (_eDim - 1) * _tau - _iterations;
            int maxlength = bLength - 1 - _window;
            bool[] done = new bool[series.Length];
            bool alldone;

            var interval = Vector.Rescale(series);

            epsilon = _epsMin == 0 ? 1e-3 : _epsMin / interval;

            for (int i = 0; i < series.Length; i++)
            {
                done[i] = false;
            }

            alldone = false;

            Log.AppendLine("epsilon\t\tneighbors");

            for (eps = epsilon; !alldone; eps *= 1.1)
            {
                fnn.PutInBoxes(series, eps, 0, bLength, 0, _tau * (_eDim - 1));

                alldone = true;

                for (n = 0; n <= maxlength; n++)
                {
                    if (!done[n])
                    {
                        bool ok = fnn.FindNeighborsR(series, _eDim, _tau, eps, n, _window, out int minelement);
                        Iterate(series, n, minelement);

                        done[n] = ok;
                    }

                    alldone &= done[n];
                }

                Log.AppendFormat("{0:F5}\t\t{1}\n", eps * interval, _found[0]);
            }

            for (int i = 0; i <= _iterations; i++)
            {
                if (_found[i] != 0)
                {
                    double val = _lyap[i] / _found[i] / 2.0;
                    Slope.AddDataPoint(i, val);
                }
            }

            result = "slope calculated";
        }

        private void Iterate(double[] series, int act, int minelement)
        {
            double dx;
            int del1 = _eDim * _tau;

            if (minelement != -1)
            {
                act--;
                minelement--;
                
                for (int i = 0; i <= _iterations; i++)
                {
                    act++;
                    minelement++;
                    dx = 0.0;
                    
                    for (int j = 0; j < del1; j += _tau)
                    {
                        dx += Numbers.QuickPow2(series[act + j] - series[minelement + j]);
                    }

                    if (dx > 0.0)
                    {
                        _found[i]++;
                        _lyap[i] += Math.Log(dx);
                    }
                }
            }
        }
    }
}
