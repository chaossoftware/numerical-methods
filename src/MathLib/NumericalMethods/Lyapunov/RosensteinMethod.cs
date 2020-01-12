using System;
using System.Text;
using MathLib.IO;
using MathLib.NumericalMethods.EmbeddingDimension;

namespace MathLib.NumericalMethods.Lyapunov
{
    /// <summary>
    /// 
    /// M. T. Rosenstein, J. J. Collins, C. J. De Luca, A practical method for calculating largest Lyapunov exponents from small data sets, Physica D 65, 117 (1993)
    /// </summary>
    public class RosensteinMethod : LyapunovMethod
    {
        private readonly BoxAssistedFnn _fnn;
        private readonly int _eDim;
        private readonly int _tau;
        private readonly int _iterations;
        private readonly int _window;
        private readonly double _epsMin;
        private readonly int _length;

        private double epsilon; //minimal length scale for the neighborhood search
        private double eps;

        private double[] lyap;
        private int[] found;

        /// <summary>
        /// The method estimates the largest Lyapunov exponent of a given scalar data set using the algorithm of Rosenstein et al.
        /// </summary>
        /// <param name="timeSeries">timeseries to analyze</param>
        /// <param name="eDim">embedding dimension</param>
        /// <param name="tau"></param>
        /// <param name="iterations"></param>
        /// <param name="window">window around the reference point which should be omitted</param>
        /// <param name="scaleMin"></param>
        public RosensteinMethod(double[] timeSeries, int eDim, int tau, int iterations, int window, double scaleMin)
            : base(timeSeries)
        {
            _eDim = eDim;
            _tau = tau;
            _iterations = iterations;
            _window = window;
            _epsMin = scaleMin;
            _length = timeSeries.Length;

            if (iterations + (eDim - 1) * tau >= _length)
            {
                throw new ArgumentException("Too few points to handle specified parameters, it makes no sense to continue.");
            }

            _fnn = new BoxAssistedFnn(256, _length);
        }

        public override string ToString() =>
            new StringBuilder()
            .AppendLine("Rosenstein method")
            .AppendLine($"m = {_eDim}")
            .AppendLine($"τ = {_tau}")
            .AppendLine($"iterations = {_iterations}")
            .AppendLine($"theiler window = {_window}")
            .AppendLine($"min ε = {NumFormat.ToShort(epsilon)}")
            .ToString();

        public override string GetInfoFull() =>
            new StringBuilder()
            .AppendLine("Rosenstein method")
            .AppendLine($"Embedding dimension: {_eDim}")
            .AppendLine($"Delay: {_tau}")
            .AppendLine($"Iterations: {_iterations}")
            .AppendLine($"Window around the reference point which should be omitted: {_window}")
            .AppendLine($"Min scale: {NumFormat.ToShort(epsilon)}")
            .ToString();

        public override string GetResult() => "Successful";

        public override void Calculate()
        {
            bool[] done;
            bool alldone;
            int n;
            int bLength = _length - (_eDim - 1) * _tau - _iterations;
            int maxlength = bLength - 1 - _window;

            lyap = new double[_iterations + 1];
            found = new int[_iterations + 1];
            done = new bool[_length];

            var interval = Ext.RescaleData(TimeSeries);

            epsilon = _epsMin == 0 ? 1e-3 : _epsMin / interval;

            for (int i = 0; i < _length; i++)
            {
                done[i] = false;
            }

            alldone = false;

            for (eps = epsilon; !alldone; eps *= 1.1)
            {
                _fnn.PutInBoxes(TimeSeries, eps, 0, bLength, 0, _tau * (_eDim - 1));

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

            for (int i = 0; i <= _iterations; i++)
            {
                if (found[i] != 0)
                {
                    double val = lyap[i] / found[i] / 2.0;
                    Slope.AddDataPoint(i, val);
                    Log.AppendFormat("{0}\t{1:F15}\n", i, val);
                }
            }
        }

        private bool Iterate(int act)
        {
            int minelement;
            double dx;
            int del1 = _eDim * _tau;
            bool ok = _fnn.FindNeighborsR(TimeSeries, _eDim, _tau, eps, act, _window, out minelement);

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
