using ChaosSoft.Core.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ChaosSoft.Core
{
    public static class Ext
    {
        /// <summary>
        /// get maximum absolute value from signal
        /// </summary>
        /// <returns></returns>
        public static double CountMaxAbs(double[] series)
        {
            double maxVal = double.MinValue;

            foreach (double val in series)
            {
                maxVal = FastMath.Max(maxVal, Math.Abs(val));
            }

            return maxVal;
        }

        public static double Max(double[,] matrix)
        {
            double maxVal = double.MinValue;

            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    maxVal = FastMath.Max(maxVal, matrix[x, y]);
                }
            }

            return maxVal;
        }

        public static double RescaleData(double[] series)
        {
            var max = FastMath.Max(series);
            var min = FastMath.Min(series);
            var interval = max - min;

            if (interval == 0d)
            {
                throw new ArgumentException($"Data amplitude is zero, it makes no sense to continue.");
            }

            for (int i = 0; i < series.Length; i++)
            {
                series[i] = (series[i] - min) / interval;
            }

            return interval;
        }

        public static double Variance(double[] series)
        {
            int length = series.Length;
            double h;
            double av = 0d;
            double variance = 0d;

            for (int i = 0; i < length; i++)
            {
                h = series[i];
                av += h;
                variance += h * h;
            }

            av /= length;
            variance = Math.Sqrt(Math.Abs(variance / length - av * av));          

            return variance;
        }

        public static void FillVectorWith(double[] vector, double value)
        {
            int i;
            int len = vector.Length;

            for (i = 0; i < len; i++)
            {
                vector[i] = value;
            }
        }

        public static void FillMatrixWith(double[,] matrix, double value)
        {
            int i, j;
            int xLen = matrix.GetLength(0); 
            int yLen = matrix.GetLength(1);

            for (i = 0; i < xLen; i++)
            {
                for (j = 0; j < yLen; j++)
                {
                    matrix[i, j] = value;
                }
            }
        }

        public static int SlopeChangePointIndex(Timeseries timeseries, int groupingCoefficient, double cutOffValue)
        {
            var smoothed = new List<PointF>();     // reduced smoothed data
            var d1 = new List<PointF>();     // 1st derivative
            var d2 = new List<PointF>();     // 2nd derivative
            var m = new List<PointF>();      // reasonably large values from D2

            // smoothen the data
            for (int i = 1; i < timeseries.Length / groupingCoefficient; i++)
            {
                double ysum = 0.0f;

                for (int j = 0; j < groupingCoefficient; j++)
                {
                    ysum += timeseries.DataPoints[i * groupingCoefficient + j].Y;
                }

                smoothed.Add(new PointF(i, (float)ysum / groupingCoefficient));
            }

            // 1st derivative
            for (int i = 1; i < smoothed.Count; i++)
            {
                d1.Add(new PointF(i, smoothed[i - 1].Y - smoothed[i].Y));
            }

            // 2nd derivative
            for (int i = 1; i < d1.Count; i++)
            {
                d2.Add(new PointF(i, d1[i - 1].Y - d1[i].Y));
            }

            // collect 'reasonably' large values from D2
            foreach (var p in d2.Where(p => Math.Abs(p.Y / cutOffValue) > 1))
            {
                m.Add(p);
            }

            return m.Any() ? (int)(m.Last().X * groupingCoefficient) /*+ groupingCoefficient*/ : 0;
        }
    }

}
