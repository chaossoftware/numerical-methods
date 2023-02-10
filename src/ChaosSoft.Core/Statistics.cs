using System;

namespace ChaosSoft.Core
{
    public static class Statistics
    {
        /// <summary>
        /// Expectation of the squared deviation of a random variable from its population mean or sample mean. 
        /// Variance is a measure of dispersion, meaning it is a measure of how far a set of numbers 
        /// is spread out from their average value.
        /// </summary>
        /// <param name="series">input series</param>
        /// <returns>variance value</returns>
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

        /// <summary>
        /// Mean is the sum of numbers divided by their count.
        /// </summary>
        /// <param name="series">input series</param>
        /// <returns>mean value</returns>
        public static double Mean(double[] series)
        {
            double sum = 0;

            for (int i = 0; i < series.Length; i++)
            {
                sum += series[i];
            }

            return sum / series.Length;
        }

        /// <summary>
        /// Autocorrelation is  the correlation of a signal with a delayed copy of itself as a function of delay. 
        /// Informally, it is the similarity between observations of a random variable as a function of the time lag between them.
        /// </summary>
        /// <param name="series">input series</param>
        /// <returns></returns>
        public static double[] Acf(double[] series)
        {
            int seriesLen = series.Length;
            double mean = Statistics.Mean(series);
            double[] autocorrelation = new double[seriesLen / 2];
            double n, d, xim;

            for (int t = 0; t < autocorrelation.Length; t++)
            {
                n = 0; // Numerator
                d = 0; // Denominator

                for (int i = 0; i < seriesLen; i++)
                {
                    xim = series[i] - mean;
                    n += xim * (series[(i + t) % seriesLen] - mean);
                    d += xim * xim;
                }

                autocorrelation[t] = n / d;
            }

            return autocorrelation;
        }
    }
}
