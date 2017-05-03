using System;

namespace MathLib.DrawEngine
{
    public class DataPoint : IComparable
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

        public int CompareTo(object obj)
        {
            DataPoint dp = obj as DataPoint;

            if (X == dp.X && Y == dp.Y)
                return 0;

            if (X == dp.X && Y > dp.Y)
                return 1;
            if (X == dp.X && Y < dp.Y)
                return -1;

            if (X > dp.X)
                return 1;
            else
                return -1;
        }
    }
}
