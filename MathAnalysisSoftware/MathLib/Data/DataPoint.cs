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

        public int Xint => (int)this.X;

        public int Yint => (int)this.Y;
    }
}
