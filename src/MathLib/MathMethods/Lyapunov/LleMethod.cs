using System;

namespace MathLib.MathMethods.Lyapunov
{
    public abstract class LleMethod : LyapunovMethod
    {
        public LleMethod(double[] timeSeries) : base(timeSeries)
        {
        }

        protected void RescaleData(double[] timeSeries, out double min, out double max)
        {
            max = Ext.countMax(timeSeries);
            min = Ext.countMin(timeSeries);
            max -= min;

            if (max == 0d)
            {
                throw new ArgumentException($"Data ranges from {min} to {min + max}. It makes no sense to continue.");
            }

            for (int i = 0; i < timeSeries.Length; i++)
            {
                timeSeries[i] = (timeSeries[i] - min) / max;
            }
        }



    }
}
