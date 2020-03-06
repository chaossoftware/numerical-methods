namespace MathLib.NumericalMethods
{
    public class AutoCorrelationFunction
    {
        public double[] GetFromSeries(double[] timeSeries)
        {
            double mean = Mean(timeSeries);

            double[] autocorrelation = new double[timeSeries.Length / 2];
            for (int t = 0; t < autocorrelation.Length; t++)
            {
                double n = 0; // Numerator
                double d = 0; // Denominator

                for (int i = 0; i < timeSeries.Length; i++)
                {
                    double xim = timeSeries[i] - mean;
                    n += xim * (timeSeries[(i + t) % timeSeries.Length] - mean);
                    d += xim * xim;
                }

                autocorrelation[t] = n / d;
            }

            return autocorrelation;
        }

        private double Mean(double[] x)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
                sum += x[i];
            return sum / x.Length;
        }
    }
}
