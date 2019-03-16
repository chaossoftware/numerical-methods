using System;

namespace MathLib
{
    public static class Ext
    {
        /// <summary>
        /// get maximum absolute value from signal
        /// </summary>
        /// <returns></returns>
        public static double countMaxAbs(double[] timeSeries) {
            double maxVal = double.MinValue;

            foreach (double val in timeSeries)
                maxVal = Math.Max(maxVal, Math.Abs(val));

            return maxVal;
        }

        /// <summary>
        /// get maximum value from signal
        /// </summary>
        /// <returns></returns>
        public static double countMax(double[] timeSeries) {
            double maxVal = double.MinValue;

            foreach (double val in timeSeries)
                maxVal = Math.Max(maxVal, val);

            return maxVal;
        }

        /// <summary>
        /// get minimum value from signal
        /// </summary>
        /// <returns></returns>
        public static double countMin(double[] timeSeries)
        {
            double minVal = double.MaxValue;

            foreach (double val in timeSeries)
            {
                minVal = Math.Min(minVal, val);
            }

            return minVal;
        }
    }
}
