using System;

namespace MathLib.DrawEngine
{
    public class DataPoint : IComparable
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

        public int CompareTo(object obj)
        {
            DataPoint dp = obj as DataPoint;

            if (this.X == dp.X && this.Y == dp.Y)
            {
                return 0;
            }

            if (this.X == dp.X && this.Y > dp.Y)
            {
                return 1;
            }
                
            if (this.X == dp.X && this.Y < dp.Y)
            {
                return -1;
            }

            if (this.X > dp.X)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
