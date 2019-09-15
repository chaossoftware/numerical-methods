using System;
using System.Globalization;
using System.Text;
using MathLib.IO;
using MathLib.MathMethods.EmbeddingDimension;

namespace MathLib.MathMethods.Lyapunov
{
    public class RosensteinMethod : LyapunovMethod
    {
        private readonly BoxAssistedFnn fnn;
        private readonly int eDim;
        private readonly int tau;
        private readonly int iterations;
        private readonly int window;        //window around the reference point which should be omitted
        private readonly double scaleMin;
        private readonly int length;

        private double epsilon;             //minimal length scale for the neighborhood search
        private double eps;

        private double[] lyap;
        private int[] found;

        public RosensteinMethod(double[] timeSeries, int eDim, int tau, int iterations, int window, double scaleMin)
            : base(timeSeries)
        {
            this.eDim = eDim;
            this.tau = tau;
            this.iterations = iterations;
            this.window = window;
            this.scaleMin = scaleMin;
            this.length = timeSeries.Length;

            if (iterations + (eDim - 1) * tau >= length)
            {
                throw new ArgumentException("Too few points to handle specified parameters, it makes no sense to continue.");
            }

            this.fnn = new BoxAssistedFnn(256, length);
        }

        public override string ToString() =>
            new StringBuilder()
            .AppendLine($"m = {eDim}")
            .AppendLine($"τ = {tau}")
            .AppendLine($"iterations = {iterations}")
            .AppendLine($"theiler window = {window}")
            .AppendLine($"min ε = {NumFormat.ToShort(epsilon)}")
            .ToString();

        public override string GetInfoFull() =>
            new StringBuilder()
            .AppendLine($"Embedding dimension: {eDim}")
            .AppendLine($"Delay: {tau}")
            .AppendLine($"Iterations: {iterations}")
            .AppendLine($"Window around the reference point which should be omitted: {window}")
            .AppendLine($"Min scale: {NumFormat.ToShort(epsilon)}")
            .ToString();

        public override string GetResult() => "Successful";

        public override void Calculate()
        {
            bool[] done;
            bool alldone;
            int n;
            int bLength = length - (this.eDim - 1) * this.tau - this.iterations;
            int maxlength = bLength - 1 - window;

            lyap = new double[iterations + 1];
            found = new int[iterations + 1];
            done = new bool[length];

            var interval = Ext.RescaleData(TimeSeries);

            epsilon = scaleMin == 0 ? 1e-3 : scaleMin / interval;

            for (int i = 0; i < length; i++)
            {
                done[i] = false;
            }

            alldone = false;

            for (eps = epsilon; !alldone; eps *= 1.1)
            {
                fnn.PutInBoxes(TimeSeries, eps, 0, bLength, 0, tau * (eDim - 1));

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

            for (int i = 0; i <= iterations; i++)
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
            int del1 = eDim * tau;
            bool ok = fnn.FindNeighborsR(TimeSeries, eDim, tau, eps, act, window, out minelement);

            if (minelement != -1)
            {
                act--;
                minelement--;
                
                for (int i = 0; i <= iterations; i++)
                {
                    act++;
                    minelement++;
                    dx = 0.0;
                    
                    for (int j = 0; j < del1; j += tau)
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
