using ChaosSoft.Core.Data;

namespace ChaosSoft.NumericalMethods.Lyapunov;

/// <summary>
/// Abstractions for Lyapunov exponents calculation from timeseries methods.
/// </summary>
public interface ITimeSeriesLyapunov
{
    /// <summary>
    /// Gets lyapunov exponent slope.
    /// </summary>
    DataSeries Slope { get; set; }

    /// <summary>
    /// Calculates lyapunov exponents from timeseries.
    /// </summary>
    /// <param name="timeSeries">source series</param>
    void Calculate(double[] timeSeries);
}
