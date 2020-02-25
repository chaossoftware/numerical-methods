using System;
using System.Linq;
using System.Threading.Tasks;

namespace MathLib.NumericalMethods
{
    /**
     * In order to get the Correlation Coefficient (Pearson number) of a time series, we need 
     * to be able to compute the average, variance, and standard deviation of the series. 
     * The Autocorrelation method simply takes the input series, splits it into two arrays, 
     * and then steps through the computation each time incrementing the starting point of the 
     * second series by one. The first coefficient in the resultset will always be 1.0, 
     * since the data is being correlated with an exact copy of itself. 
     * Each subsequent coefficient in the resultset will be different as the second "half" series 
     * is moved forward one item. It is the slope and particularly the peaks in the result series 
     * that are of interest. High relative peaks indicate periodicities or fundamental 
     * frequencies of cycles in the data.
     */
    public class AutoCorrelationFunction
    {
        public double[] GetFromSeries(double[] timeSeries)
        {
            var half = timeSeries.Length / 2;
            var autoCorrelation = new double[half];
            var a = new double[half];
            var b = new double[half];

            for (int i = 0; i < half; i++)
            {
                a[i] = timeSeries[i];
                b[i] = timeSeries[i + i];
                autoCorrelation[i] = GetCorrelation(a, b);
            }

            return autoCorrelation;
        }

        public double[] GetAutoCorrelationOfSeriesOParallel(double[] timeSeries)
        {
            var half = timeSeries.Length / 2;
            var tasks = new Task[half];
            var autoCorrelation = new double[half];
            var a = new double[half];
            var b = new double[half];

            for (int i = 0; i < half; i++)
            {
                a[i] = timeSeries[i];
                b[i] = timeSeries[i + i];

                var task = Task.Factory.StartNew(() => GetCorrelation(a, b), TaskCreationOptions.LongRunning);
                tasks[i] = task;
                autoCorrelation[i] = task.Result;
            }

            Task.WaitAll(tasks);
            return autoCorrelation;
        }

        private double GetCorrelation(double[] x, double[] y)
        {
            if (x.Length != y.Length)
            {
                throw new CalculationException("Auto-Correlation: length of sources is different.");
            }

            var avgX = GetAverage(x);
            var stdevX = GetStdev(x);
            var avgY = GetAverage(y);
            var stdevY = GetStdev(y);
            var covXY = 0d;
            int len = x.Length;

            for (int i = 0; i < len; i++)
            {
                covXY += (x[i] - avgX) * (y[i] - avgY);
            }

            covXY /= len;
            var pearson = covXY / (stdevX * stdevY);
            
            return pearson;
        }

        private double GetAverage(double[] data)
        {
            int len = data.Length;

            if (len == 0)
            {
                throw new CalculationException("Auto-Correlation: no data.");
            }

            return data.Sum() / len;
        }

        private double GetVariance(double[] data)
        {
            double avg = GetAverage(data);
            return data.Sum(dp => Math.Pow((dp - avg), 2)) / data.Length;
        }

        private double GetStdev(double[] data) =>
            Math.Sqrt(GetVariance(data));
    }
}
