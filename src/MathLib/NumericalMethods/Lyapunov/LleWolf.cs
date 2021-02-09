using MathLib.IO;
using System;
using System.Text;

namespace MathLib.NumericalMethods.Lyapunov
{
    /// <summary>
    /// LLE by Wolf (fixed evolution time)
    /// A Wolf, JB Swift, HL Swinney, JA Vastano, Determining Lyapunov exponents from a time series, Physica D: Nonlinear Phenomena, 1985
    /// </summary>
    public class LleWolf : LyapunovMethod
    {
        private const string Paper = "A Wolf, JB Swift, HL Swinney, JA Vastano, Determining Lyapunov exponents from a time series, Physica D: Nonlinear Phenomena, 1985";
        private readonly int _eDim;
        private readonly int _tau;
        private readonly int _evolv;
        private readonly double _dt;
        private readonly double _epsMin;
        private readonly double _epsMax;

        // Pre-calculated variables
        private readonly double evMulStMulLog2;
        private readonly int tsLen;

        private double[] pt1;
        private double[] pt2; 

        private double zlyap;

        private int step;

        public LleWolf(double[] timeSeries, int eDim, int tau, double stepSize, double scaleMin, double scaleMax, int evolv)
            : base(timeSeries)
        {
            _eDim = eDim;
            _tau = tau;
            _dt = stepSize;
            _epsMin = scaleMin;
            _epsMax = scaleMax;
            _evolv = evolv;

            pt1 = new double[_eDim];
            pt2 = new double[_eDim];

            evMulStMulLog2 = _evolv * _dt * Math.Log(2d);
            tsLen = timeSeries.Length;
        }

        public LleWolf(double[] timeSeries) : this(timeSeries, 2, 1, 1d, 1e-3, 1e-2, 1)
        {
        }

        public LleWolf(double[] timeSeries, int eDim, double stepSize) : this(timeSeries, eDim, 1, stepSize, 1e-3, 1e-2, 1)
        {
        }

        public double Result { get; protected set; }

        public override string ToString() =>
            new StringBuilder()
                .AppendLine("LLE by Wolf (fixed evolution time)")
                .AppendLine($"m = {_eDim}")
                .AppendLine($"τ = {_tau}")
                .AppendLine($"Δt = {NumFormat.ToShort(_dt)}")
                .AppendLine($"min ε = {NumFormat.ToShort(_epsMin)}")
                .AppendLine($"max ε = {NumFormat.ToShort(_epsMax)}")
                .AppendLine($"evolution steps: {_evolv}")
                .ToString();

        public override string GetHelp() =>
            new StringBuilder()
                .AppendLine($"LLE by Wolf (fixed evolution time) [{Paper}]")
                .AppendLine("m - embedding dimension (default: 2)")
                .AppendLine("τ - reconstruction delay (default: 1)")
                .AppendLine("Δt - Step size (default: 1.0)")
                .AppendLine("min ε - Min scale (default: 1e-3)")
                .AppendLine("max ε - Max scale (default: 1e-2)")
                .AppendLine("evolution steps (default: 1)")
                .ToString();

        public override string GetResult() => NumFormat.ToShort(Result);

        public override void Calculate()
        {
            //int ind = 0; // <In fortran was 1>.
            int ind2 = 0; // Points to second trajectory <in fortran was empty and defined later>.
            double sum = 0d; // Holds running exponent estimate sans i/time.
            int its = 0; // Total number of propagation steps.

            double dii = 0; // <initialization absent in fortran>

            // Calculate useful size of timeseries.
            int dataPointsCount = TimeSeries.Length - _eDim * _tau - _evolv;

            // Find nearest neighbor to first data point.
            double di = 1e38;

            // Don't take point too close to fiducial point.
            for (int i = 10; i < dataPointsCount; i++)
            {
                // Compute separation between fiducial point and candidate.
                double d = 0d;

                for (int j = 0; j < _eDim; j++)
                {
                    d += Math.Pow(GetAttractorPoint(0, j) - GetAttractorPoint(i, j), 2);
                }

                d = Math.Sqrt(d);

                // Store the best point so far but no closer than noise scale.
                if (d >= _epsMin && d <= di)
                {
                    di = d;
                    ind2 = i;
                }
            }

            Log.Append("Total Propagation Time\tLyapunov exponent\tDI\tInformation dimention\n");

            // ind - points to fiducial trajectory
            for (int ind = _evolv; ind < dataPointsCount; ind += _evolv)
            { 
                // Get coordinates of evolved points.
                for (int j = 0; j < _eDim; j++)
                {
                    pt1[j] = GetAttractorPoint(ind, j);
                    pt2[j] = GetAttractorPoint(ind2 + _evolv, j);
                }

                // Compute final separation between pair, update exponent.
                double df = 0d;

                for (int j = 0; j < _eDim; j++)
                    df += Math.Pow(pt1[j] - pt2[j], 2);

                df = Math.Sqrt(df);

                its++;
                sum += Math.Log(df / di) / evMulStMulLog2;
                zlyap = sum / its;

                Log.AppendFormat("{0}\t{1:F5}\t{2:F5}\t{3:F5}\n", _evolv * its, zlyap, di, df);
                Slope.AddDataPoint(step++, zlyap);

                // Look for replacement point.
                int indold = ind2;
                double zmult = 1d; // Multiplier of scalMax when go to longer distances.
                double anglmx = 0.3;

                metka:

                double thmin = Math.PI;

                // Search over all points.
                for (int i = 0; i < dataPointsCount; i++)
                {
                    // Don't take points too close in time to fiducial point.
                    int iii = Math.Abs(i - ind);

                    if (iii < 9)
                    {
                        continue;
                    }

                    // Compute distance between fiducial point and candidate.
                    double dnew = 0d;

                    for (int j = 0; j < _eDim; j++)
                    {
                        dnew += Math.Pow(pt1[j] - GetAttractorPoint(i, j), 2);
                    }

                    dnew = Math.Sqrt(dnew);

                    // Look further away than noise scale, closer than zmult*scalMax.
                    if (dnew > zmult * _epsMax || dnew < _epsMin)
                    {
                        continue;
                    }

                    // Find angular change old to new vector.
                    double dot = 0d;

                    for (int j = 0; j < _eDim; j++)
                    {
                        dot += (pt1[j] - GetAttractorPoint(i, j)) * (pt1[j] - pt2[j]);
                    }

                    double cth = Math.Abs(dot / (dnew * df));

                    cth = Math.Min(cth, 1d);

                    double th = Math.Acos(cth);

                    // Save point with smallest angular change so far.
                    if (th < thmin)
                    { 
                        thmin = th;
                        dii = dnew;
                        ind2 = i;
                    }
                }

                if (thmin > anglmx)
                {
                    // Can't find a replacement - look at longer distances.
                    zmult++;

                    if (zmult <= 5d)
                    {
                        goto metka;
                    }

                    // No replacement at 5*scale, double search angle, reset distance.
                    zmult = 1d;
                    anglmx *= 2d;

                    if (anglmx < Math.PI)
                    {
                        goto metka;
                    }

                    ind2 = indold + _evolv;
                    dii = df;
                }

                di = dii;
            }

            Result = zlyap;
        }

        ///<summary>Define delay coordinates with a statement function</summary>
        ///<returns>j-th component of i-th reconstructed attractor point</returns>
        private double GetAttractorPoint(int i, int j)
        {
            int point = i + j * _tau;
            return point < tsLen ? TimeSeries[point] : 0;
        }
    }
}
