namespace ChaosSoft.Core
{
    public static class FastMath
    {
        /// <summary>
        /// Gets maximum value from series (optimized by speed).
        /// </summary>
        /// <param name="series">array</param>
        /// <returns>array maximum value</returns>
        public static double Max(double[] series)
        {
            double maxVal = double.MinValue;

            foreach (double val in series)
            {
                if (val > maxVal)
                {
                    maxVal = val;
                }
            }

            return maxVal;
        }

        /// <summary>
        /// Gets maximum of two numbers (optimized by speed).
        /// </summary>
        /// <param name="number1">number 1</param>
        /// <param name="number2">number 2</param>
        /// <returns>maximum value</returns>
        public static double Max(double number1, double number2) =>
            number1 > number2 ? number1 : number2;

        /// <summary>
        /// Gets minimum value from series (optimized by speed).
        /// </summary>
        /// <param name="series">array</param>
        /// <returns>array minimum value</returns>
        public static double Min(double[] series)
        {
            double minVal = double.MaxValue;

            foreach (double val in series)
            {
                if (val < minVal)
                {
                    minVal = val;
                }
            }

            return minVal;
        }

        /// <summary>
        /// Gets minimum of two numbers (optimized by speed).
        /// </summary>
        /// <param name="number1">number 1</param>
        /// <param name="number2">number 2</param>
        /// <returns>minumum value</returns>
        public static double Min(double number1, double number2) =>
            number1 < number2 ? number1 : number2;

        /// <summary>
        /// Gets the 2nd power of a number (optimized by speed).
        /// </summary>
        /// <param name="number">number to calculate power</param>
        /// <returns>the 2nd power of number</returns>
        public static double Pow2(double number) =>
            number * number;
    }

}
