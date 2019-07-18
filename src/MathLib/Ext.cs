using System;

namespace MathLib
{
    public static class Ext
    {
        /// <summary>
        /// get maximum absolute value from signal
        /// </summary>
        /// <returns></returns>
        public static double CountMaxAbs(double[] timeSeries)
        {
            double maxVal = double.MinValue;

            foreach (double val in timeSeries)
            {
                maxVal = Math.Max(maxVal, Math.Abs(val));
            }

            return maxVal;
        }

        /// <summary>
        /// get maximum value from signal
        /// </summary>
        /// <returns></returns>
        public static double CountMax(double[] timeSeries)
        {
            double maxVal = double.MinValue;

            foreach (double val in timeSeries)
            {
                maxVal = Math.Max(maxVal, val);
            }

            return maxVal;
        }

        /// <summary>
        /// get minimum value from signal
        /// </summary>
        /// <returns></returns>
        public static double CountMin(double[] timeSeries)
        {
            double minVal = double.MaxValue;

            foreach (double val in timeSeries)
            {
                minVal = Math.Min(minVal, val);
            }

            return minVal;
        }

        public static double RescaleData(double[] timeSeries)
        {
            var max = CountMax(timeSeries);
            var min = CountMin(timeSeries);
            var interval = max - min;

            if (interval == 0d)
            {
                throw new ArgumentException($"Data amplitude is zero, it makes no sense to continue.");
            }

            for (int i = 0; i < timeSeries.Length; i++)
            {
                timeSeries[i] = (timeSeries[i] - min) / interval;
            }

            return interval;
        }
    }
}
