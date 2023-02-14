using ChaosSoft.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChaosSoft.NumericalMethods.Extensions
{
    /// <summary>
    /// Helpers for <see cref="DataSeries"/>.
    /// </summary>
    public static class DataSeriesUtils
    {
        /// <summary>
        /// Gets index of point where slope changes.
        /// </summary>
        /// <param name="series">input series</param>
        /// <param name="groupingCoefficient">groupping coefficient</param>
        /// <param name="cutOffValue">cut-off value</param>
        /// <returns></returns>
        public static int SlopeChangePointIndex(DataSeries series, int groupingCoefficient, double cutOffValue)
        {
            var smoothed = new List<DataPoint>();     // reduced smoothed data
            var d1 = new List<DataPoint>();     // 1st derivative
            var d2 = new List<DataPoint>();     // 2nd derivative
            var m = new List<DataPoint>();      // reasonably large values from D2

            // smoothen the data
            for (int i = 1; i < series.Length / groupingCoefficient; i++)
            {
                double ysum = 0d;

                for (int j = 0; j < groupingCoefficient; j++)
                {
                    ysum += series.DataPoints[i * groupingCoefficient + j].Y;
                }

                smoothed.Add(new DataPoint(i, ysum / groupingCoefficient));
            }

            // 1st derivative
            for (int i = 1; i < smoothed.Count; i++)
            {
                d1.Add(new DataPoint(i, smoothed[i - 1].Y - smoothed[i].Y));
            }

            // 2nd derivative
            for (int i = 1; i < d1.Count; i++)
            {
                d2.Add(new DataPoint(i, d1[i - 1].Y - d1[i].Y));
            }

            // collect 'reasonably' large values from D2
            foreach (var p in d2.Where(p => Math.Abs(p.Y / cutOffValue) > 1))
            {
                m.Add(p);
            }

            return m.Any() ? (int)(m.Last().X * groupingCoefficient) : 0;
        }
    }

}
