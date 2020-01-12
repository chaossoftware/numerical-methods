namespace MathLib.Data
{
    public struct DataPoint
    {
        public DataPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }

        public double Y { get; set; }

        public override string ToString() =>
            string.Format("{{{0:G5}, {1:G5}}}", X, Y);
    }
}
