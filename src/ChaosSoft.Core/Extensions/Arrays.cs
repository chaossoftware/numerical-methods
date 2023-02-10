using System;

namespace ChaosSoft.Core.Extensions
{
    public static class Arrays
    {
        /// <summary>
        /// get maximum absolute value from signal
        /// </summary>
        /// <returns></returns>
        public static double MaxAbs(double[] series)
        {
            double maxVal = double.MinValue;

            foreach (double val in series)
            {
                maxVal = FastMath.Max(maxVal, Math.Abs(val));
            }

            return maxVal;
        }

        public static double[] GenerateUniformArray(int length, double start, double step)
        {
            double[] array = new double[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = start + step * i;
            }

            return array;
        }

        public static int[] GenerateUniformArray(int length, int start, int step)
        {
            int[] array = new int[length];

            for (int i = 0; i < length; i++)
            {
                array[i] = start + step * i;
            }

            return array;
        }

        public static void FillArrayWith(double[] vector, double value)
        {
            int i;
            int len = vector.Length;

            for (i = 0; i < len; i++)
            {
                vector[i] = value;
            }
        }

        public static double Rescale(double[] series)
        {
            var max = FastMath.Max(series);
            var min = FastMath.Min(series);
            var interval = max - min;

            if (interval == 0d)
            {
                throw new ArgumentException("Data amplitude is zero, it makes no sense to continue.");
            }

            for (int i = 0; i < series.Length; i++)
            {
                series[i] = (series[i] - min) / interval;
            }

            return interval;
        }
    }

}
