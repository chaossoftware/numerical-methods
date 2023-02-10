using ChaosSoft.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChaosSoft.Core.Extensions
{
    public static class DataSeriesExt
    {
        public static int SlopeChangePointIndex(DataSeries timeseries, int groupingCoefficient, double cutOffValue)
        {
            var smoothed = new List<DataPoint>();     // reduced smoothed data
            var d1 = new List<DataPoint>();     // 1st derivative
            var d2 = new List<DataPoint>();     // 2nd derivative
            var m = new List<DataPoint>();      // reasonably large values from D2

            // smoothen the data
            for (int i = 1; i < timeseries.Length / groupingCoefficient; i++)
            {
                double ysum = 0d;

                for (int j = 0; j < groupingCoefficient; j++)
                {
                    ysum += timeseries.DataPoints[i * groupingCoefficient + j].Y;
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
