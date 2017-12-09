using System;

namespace MathLib.MathMethods.Lyapunov
{
    class LleHelper
    {
        public static void RescaleData(double[] timeSeries, out double min, out double max)
        {
            max = Ext.countMax(timeSeries);
            min = Ext.countMin(timeSeries);

            max -= min;

            if (max != 0.0)
            {
                for (int i = 0; i < timeSeries.Length; i++)
                    timeSeries[i] = (timeSeries[i] - min) / max;
            }
            else
            {
                throw new ArgumentException(
                    $"Data ranges from {min} to {min + max}. It makes no sense to continue.");
            }
        }


        //shift (Kantz = Tau) (Rosenstein = Tau * (Dim - 1))
        public static void PutInBoxes(double[] timeSeries, int[,] box, int[] liste, double eps, int blength, int shift)
        {
            int boxSize = box.GetLength(0);
            int iBox = boxSize - 1;
            int i, j, k;

            for (i = 0; i < boxSize; i++)
                for (j = 0; j < boxSize; j++)
                    box[i, j] = -1;

            for (i = 0; i < blength; i++)
            {
                j = (int)(timeSeries[i] / eps) & iBox;
                k = (int)(timeSeries[i + shift] / eps) & iBox;
                liste[i] = box[j, k];
                box[j, k] = i;
            }
        }
    }
}
