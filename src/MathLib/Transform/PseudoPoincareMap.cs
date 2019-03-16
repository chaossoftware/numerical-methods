using MathLib.Data;

namespace MathLib.Transform
{
    public static class PseudoPoincareMap
    {
        public static Timeseries GetMapDataFrom(double[] timeSeries, int step)
        {
            var ppDataSeries = new Timeseries();

            for (int i = 0; i < timeSeries.Length - step; i++)
            {
                ppDataSeries.AddDataPoint(timeSeries[i], timeSeries[i + step]);
            }

            return ppDataSeries;
        }

        public static Timeseries GetMapDataFrom(double[] timeSeries) => GetMapDataFrom(timeSeries, 1);
    }
}
