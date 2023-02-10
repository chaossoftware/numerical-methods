using ChaosSoft.Core.Data;
using System.Text;

namespace ChaosSoft.Core.NumericalMethods.Lyapunov
{
    /// <summary>
    /// Base class for lyapunov exponents calculation methods implementations.
    /// </summary>
    public interface ITimeSeriesLyapunov
    {
        DataSeries Slope { get; set; }

        StringBuilder Log { get; }

        void Calculate(double[] timeSeries);
    }
}
