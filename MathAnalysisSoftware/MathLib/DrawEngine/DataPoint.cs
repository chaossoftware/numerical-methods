namespace MathLib.DrawEngine
{
    public class DataPoint
    {

        public DataPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X;
        public double Y;

        public int Xint
        {
            get
            {
                return (int)X;
            }
        }

        public int Yint
        {
            get
            {
                return (int)Y;
            }
        }
    }
}
