using MathLib.Data;
using System.Text;

namespace MathLib.NumericalMethods.Lyapunov
{
    public abstract class LyapunovMethod
    {
        protected LyapunovMethod(double[] timeSeries)
        {
            TimeSeries = timeSeries;
            Slope = new Timeseries();
            Log = new StringBuilder();
        }

        public Timeseries Slope { get; set; }

        public StringBuilder Log { get; }

        protected double[] TimeSeries { get; }

        public abstract void Calculate();

        public abstract string GetResult();

        public abstract string GetHelp();
    }
}
