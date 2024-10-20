using ChaosSoft.Core;
using ChaosSoft.Core.Data;
using ChaosSoft.Core.Logging;
using ChaosSoft.NumericalMethods.Algebra;
using System;
using System.Text;

namespace ChaosSoft.NumericalMethods.Lyapunov;

/// <summary>
/// LLE by Wolf (fixed evolution time)
/// A Wolf, JB Swift, HL Swinney, JA Vastano, Determining Lyapunov exponents from a time series, Physica D: Nonlinear Phenomena, 1985
/// </summary>
public sealed class LleWolf : ITimeSeriesLyapunov, IHasDescription
{
    private const string Paper = "A Wolf, JB Swift, HL Swinney, JA Vastano, Determining Lyapunov exponents from a time series, Physica D: Nonlinear Phenomena, 1985";
    
    private readonly int _eDim;
    private readonly int _tau;
    private readonly int _evolv;
    private readonly double _dt;
    private readonly double _epsMin;
    private readonly double _epsMax;

    private readonly double[] _pt1;
    private readonly double[] _pt2;
    
    // Pre-calculated variables
    private readonly double _evMulStMulLog2;

    // ---------------------------------
    private int ind2;
    private double dii;
    private double zmult;
    private double angleMax;

    /// <summary>
    /// Initializes a new instance of the <see cref="LleWolf"/> class for full set of parameters.
    /// </summary>
    /// <param name="eDim">embedding dimension</param>
    /// <param name="tau">reconstruction time delay</param>
    /// <param name="dt">series time step</param>
    /// <param name="epsMin">scales too small</param>
    /// <param name="epsMax">scales too large</param>
    /// <param name="evolv">constant propagation time</param>
    public LleWolf(int eDim, int tau, double dt, double epsMin, double epsMax, int evolv) : base()
    {
        _eDim = eDim;
        _tau = tau;
        _dt = dt;
        _epsMin = epsMin;
        _epsMax = epsMax;
        _evolv = evolv;

        _pt1 = new double[_eDim];
        _pt2 = new double[_eDim];

        _evMulStMulLog2 = _evolv * _dt * Math.Log(2d);

        Slope = new DataSeries();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LleWolf"/> class with default values for:<br/>
    /// time delay: 1, eps. min: 1e-3, eps. max = 1e-2, evolution time: 1
    /// </summary>
    /// <param name="eDim">embedding dimension</param>
    /// <param name="stepSize">series time step</param>
    public LleWolf(int eDim, double stepSize) : this(eDim, 1, stepSize, 1e-3, 1e-2, 1)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LleWolf"/> class with default values for:<br/>
    /// time delay: 1, series time step: 1, eps. min: 1e-3, eps. max = 1e-2, evolution time: 1
    /// </summary>
    /// <param name="eDim">embedding dimension</param>
    public LleWolf(int eDim) : this(eDim, 1, 1d, 1e-3, 1e-2, 1)
    {
    }

    /// <summary>
    /// Gets LLE change in time.
    /// </summary>
    public DataSeries Slope { get; set; }

    /// <summary>
    /// Gets largest Lyapunov exponent.
    /// </summary>
    public double Result { get; private set; }

    /// <summary>
    /// Gets method setup info (parameters values)
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        new StringBuilder()
            .AppendLine("LLE by Wolf (fixed evolution time)")
            .AppendLine($" - m = {_eDim}")
            .AppendLine($" - τ = {_tau}")
            .AppendLine($" - Δt = {NumFormat.Format(_dt)}")
            .AppendLine($" - min ε = {NumFormat.Format(_epsMin)}")
            .AppendLine($" - max ε = {NumFormat.Format(_epsMax)}")
            .AppendLine($" - evolution steps: {_evolv}")
            .ToString();

    /// <summary>
    /// Gets help on the method and its params
    /// </summary>
    /// <returns></returns>
    public string Description =>
        new StringBuilder()
            .AppendLine($"Largest Lyapunov exponent by Wolf (fixed evolution time) [{Paper}]")
            .AppendLine("m - embedding dimension (default: 2)")
            .AppendLine("τ - reconstruction delay (default: 1)")
            .AppendLine("Δt - Step size (default: 1.0)")
            .AppendLine("min ε - Min scale (default: 1e-3)")
            .AppendLine("max ε - Max scale (default: 1e-2)")
            .AppendLine("evolution steps (default: 1)")
            .ToString();


    /// <summary>
    /// Calculates largest lyapunov exponent from timeseries.
    /// The result is stored in <see cref="Result"/>.
    /// </summary>
    /// <param name="timeSeries">source series</param>
    public void Calculate(double[] timeSeries)
    {
        double zlyap = 0;
        ////int ind = 0; // In fortran was 1
        double sum = 0d; // Holds running exponent estimate sans i/time.
        int its = 0; // Total number of propagation steps.
        int propagation;

        // Find nearest neighbor to first data point.
        double di = 1e38;

        // Calculate useful size of timeseries.
        int dataPointsCount = timeSeries.Length - _eDim * _tau - _evolv;

        dii = 0; // <initialization absent in fortran>
        ind2 = 0; // Points to second trajectory <in fortran was empty and defined later>.

        // Don't take point too close to fiducial point.
        for (int i = 10; i < dataPointsCount; i++)
        {
            // Compute separation between fiducial point and candidate.
            double d = 0d;

            for (int j = 0; j < _eDim; j++)
            {
                d += Numbers.FastPow2(GetAttractorPoint(timeSeries, 0, j) - GetAttractorPoint(timeSeries, i, j));
            }

            d = Math.Sqrt(d);

            // Store the best point so far but no closer than noise scale.
            if (d >= _epsMin && d <= di)
            {
                di = d;
                ind2 = i;
            }
        }

        Log.Debug("Propagation time\tLyapunov exponent\tNearest neighbor\tInformation dimension\n");

        // ind - points to fiducial trajectory
        for (int ind = _evolv; ind < dataPointsCount; ind += _evolv)
        { 
            // Get coordinates of evolved points.
            for (int j = 0; j < _eDim; j++)
            {
                _pt1[j] = GetAttractorPoint(timeSeries, ind, j);
                _pt2[j] = GetAttractorPoint(timeSeries, ind2 + _evolv, j);
            }

            // Compute final separation between pair, update exponent.
            var df = 0d;

            for (int j = 0; j < _eDim; j++)
            {
                df += Numbers.FastPow2(_pt1[j] - _pt2[j]);
            }

            df = Math.Sqrt(df);

            its++;
            sum += Math.Log(df / di) / _evMulStMulLog2;
            zlyap = sum / its;

            propagation = _evolv * its;
            Log.Debug("{0}\t{1:F4}\t\t{2:F4}\t\t{3:F4}", propagation, zlyap, di, df);
            Slope.AddDataPoint(propagation, zlyap);

            // Look for replacement point.
            int indold = ind2;
            zmult = 1d; // Multiplier of scalMax when go to longer distances.
            angleMax = 0.3;

            bool calculated;

            do
            {
                calculated = LookForReplacementPoint(timeSeries, dataPointsCount, ind, indold, df);
            }
            while (!calculated);

            di = dii;
        }

        Result = zlyap;
    }

    private bool LookForReplacementPoint(double[] series, int dataPointsCount, int ind, int indold, double df)
    {
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
                dnew += Numbers.FastPow2(_pt1[j] - GetAttractorPoint(series, i, j));
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
                dot += (_pt1[j] - GetAttractorPoint(series, i, j)) * (_pt1[j] - _pt2[j]);
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

        if (thmin > angleMax)
        {
            // Can't find a replacement - look at longer distances.
            zmult++;

            if (zmult <= 5d)
            {
                return false;
            }

            // No replacement at 5*scale, double search angle, reset distance.
            zmult = 1d;
            angleMax *= 2d;

            if (angleMax < Math.PI)
            {
                return false;
            }

            ind2 = indold + _evolv;
            dii = df;
        }

        return true;
    }

    ///<summary>Define delay coordinates with a statement function</summary>
    ///<returns>j-th component of i-th reconstructed attractor point</returns>
    private double GetAttractorPoint(double[] series, int i, int j)
    {
        int point = i + j * _tau;
        return point < series.Length ? series[point] : 0;
    }
}
