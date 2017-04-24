using MathLib.DrawEngine;
using System;

namespace MathLib
{
    public static class Ext {

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
        public static double countMin(double[] timeSeries) {
            double minVal = double.MaxValue;

            foreach (double val in timeSeries)
                minVal = Math.Min(minVal, val);

            return minVal;
        }


        /// <summary>
        /// Returns the product of two normally (Gaussian) distributed random 
        /// deviates with meanof zero and standard deviation of 1.0
        /// </summary>
        /// <param name="random">insatance of Random</param>
        /// <returns></returns>
        public static double Gauss2(Random random) {
            double v1, v2, _arg;
            do {
                v1 = 2d * random.NextDouble() - 1d;
                v2 = 2d * random.NextDouble() - 1d;
                _arg = v1 * v1 + v2 * v2;
            }
            while (_arg >= 1d || _arg == 0d);

            return v1 * v2 * (-2d + Math.Log(_arg) / _arg);
        }


        public static DataSeries GeneratePseudoPoincareMapData(double[] timeSeries) {
            DataSeries ppDataSeries = new DataSeries();

            for (int i = 0; i < timeSeries.Length - 1; i++)
                ppDataSeries.AddDataPoint(timeSeries[i], timeSeries[i + 1]);

            return ppDataSeries;
        }
    }
}
