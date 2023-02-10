using ChaosSoft.Core.Data;

namespace ChaosSoft.Core.Transform
{
    public static class PseudoPoincareMap
    {
        public static DataSeries GetMapDataFrom(double[] timeSeries, int step)
        {
            var ppDataSeries = new DataSeries();

            for (int i = 0; i < timeSeries.Length - step; i++)
            {
                ppDataSeries.AddDataPoint(timeSeries[i], timeSeries[i + step]);
            }

            return ppDataSeries;
        }

        public static DataSeries GetMapDataFrom(double[] timeSeries) => GetMapDataFrom(timeSeries, 1);
    }
}
