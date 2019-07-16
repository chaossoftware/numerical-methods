using System;

namespace MathLib.MathMethods.Lyapunov
{
    public abstract class LleMethod : LyapunovMethod
    {
        public LleMethod(double[] timeSeries) : base(timeSeries)
        {
        }

        protected void RescaleData(double[] timeSeries, out double min, out double max)
        {
            max = Ext.countMax(timeSeries);
            min = Ext.countMin(timeSeries);
            max -= min;

            if (max == 0d)
            {
                throw new ArgumentException($"Data ranges from {min} to {min + max}. It makes no sense to continue.");
            }

            for (int i = 0; i < timeSeries.Length; i++)
            {
                timeSeries[i] = (timeSeries[i] - min) / max;
            }
        }

        //shift (Kantz = Tau) (Rosenstein = Tau * (Dim - 1)) (Jakobian = 0)
        protected void PutInBoxes(double[] timeSeries, int[,] box, int[] liste, double eps, int bStart, int bEnd, int shift)
        {
            int boxSize = box.GetLength(0);
            int iBox = boxSize - 1;
            int i, x, y;

            for (x = 0; x < boxSize; x++)
            {
                for (y = 0; y < boxSize; y++)
                {
                    box[x, y] = -1;
                }
            }

            for (i = bStart; i < bEnd; i++)
            {
                x = (int)(timeSeries[i] / eps) & iBox;
                y = (int)(timeSeries[i + shift] / eps) & iBox;
                liste[i] = box[x, y];
                box[x, y] = i;
            }
        }
    }
}
