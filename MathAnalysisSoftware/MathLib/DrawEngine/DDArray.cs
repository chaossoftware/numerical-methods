
namespace MathLib.DrawEngine
{
    public class DDArray
    {
        public double t;
        public DataSeries data;

        public DDArray(double t, DataSeries data)
        {
            this.t = t;
            this.data = data;
        }

        public double T => this.t;

        public DataSeries Data => this.data;
    }
}
