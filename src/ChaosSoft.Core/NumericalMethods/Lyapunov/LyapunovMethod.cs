using ChaosSoft.Core.Data;
using System.Text;

namespace ChaosSoft.Core.NumericalMethods.Lyapunov
{
    public abstract class LyapunovMethod
    {
        protected LyapunovMethod(double[] series)
        {
            Series = series;
            Slope = new Timeseries();
            Log = new StringBuilder();
        }

        public Timeseries Slope { get; set; }

        public StringBuilder Log { get; }

        protected double[] Series { get; }

        public abstract void Calculate();

        public abstract string GetResult();

        public abstract string GetHelp();
    }
}
