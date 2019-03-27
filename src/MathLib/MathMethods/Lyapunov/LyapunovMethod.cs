using MathLib.Data;
using System.Text;

namespace MathLib.MathMethods.Lyapunov
{
    public abstract class LyapunovMethod
    {
        public LyapunovMethod(double[] timeSeries)
        {
            this.TimeSeries = timeSeries;
            Slope = new Timeseries();
            Log = new StringBuilder();
        }

        public Timeseries Slope { get; set; }

        protected StringBuilder Log { get; set; }

        protected double[] TimeSeries { get; set; }

        public abstract void Calculate();

        public abstract string GetInfoShort();

        public abstract string GetInfoFull();
    }
}
