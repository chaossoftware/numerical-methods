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

        public double X { get; }

        public double Y { get; }

        public override string ToString() =>
            $"{NumFormatter.ToShort(X)}, {NumFormatter.ToShort(Y)}";
    }
}
