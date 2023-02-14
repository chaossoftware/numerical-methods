using ChaosSoft.Core.Data;

namespace ChaosSoft.NumericalMethods.Transform
{
    /// <summary>
    /// Methods for getting delayed coordinates data.
    /// </summary>
    public static class DelayedCoordinates
    {
        /// <summary>
        /// Gets delayed coordinates data from timeseries using specific delay.
        /// </summary>
        /// <param name="series">input series</param>
        /// <param name="delay">delay step</param>
        /// <returns>delayed coordinates data as <see cref="DataSeries"/></returns>
        public static DataSeries GetData(double[] series, int delay)
        {
            var ppDataSeries = new DataSeries();

            for (int i = 0; i < series.Length - delay; i++)
            {
                ppDataSeries.AddDataPoint(series[i], series[i + delay]);
            }

            return ppDataSeries;
        }

        /// <summary>
        /// Gets delayed coordinates data from timeseries using delay = 1.
        /// </summary>
        /// <param name="series">input series</param>
        /// <returns>delayed coordinates data as <see cref="DataSeries"/></returns>
        public static DataSeries GetData(double[] series) => 
            GetData(series, 1);
    }
}
