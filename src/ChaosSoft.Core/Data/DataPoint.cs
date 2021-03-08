using ChaosSoft.Core.IO;

namespace ChaosSoft.Core.Data
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
            $"{NumFormat.ToShort(X)}, {NumFormat.ToShort(Y)}";
    }
}
