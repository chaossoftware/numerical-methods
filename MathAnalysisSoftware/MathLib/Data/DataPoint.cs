namespace MathLib.Data
{
    public struct DataPoint
    {
        public DataPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public override string ToString()
        {
            return string.Format("{{{0:G5}, {1:G5}}}", this.X, this.Y);
        }
    }
}
