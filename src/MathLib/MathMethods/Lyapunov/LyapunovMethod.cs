using MathLib.Data;
using System.Text;

namespace MathLib.MathMethods.Lyapunov
{
    public abstract class LyapunovMethod
    {
        protected LyapunovMethod(double[] timeSeries)
        {
            this.TimeSeries = timeSeries;
            Slope = new Timeseries();
            Log = new StringBuilder();
        }

        public Timeseries Slope { get; set; }

        public StringBuilder Log { get; private set; }

        protected double[] TimeSeries { get; private set; }

        public abstract void Calculate();

        public abstract string GetResult();

        public abstract string ToString();

        public abstract string GetInfoFull();
    }
}
