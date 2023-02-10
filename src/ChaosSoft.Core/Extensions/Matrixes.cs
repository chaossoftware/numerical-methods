
namespace ChaosSoft.Core.Extensions
{
    public static class Matrixes
    {
        public static double[,] Create(int rows, int columns, double initialValue)
        {
            double[,] matrix = new double[rows, columns];

            if (initialValue != 0)
            {
                FillWith(matrix, initialValue);
            }

            return matrix;
        }

        public static double Max(double[,] matrix)
        {
            double maxVal = double.MinValue;

            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    maxVal = FastMath.Max(maxVal, matrix[x, y]);
                }
            }

            return maxVal;
        }

        public static double Min(double[,] matrix)
        {
            double min = double.MaxValue;

            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    min = FastMath.Min(min, matrix[x, y]);
                }
            }

            return min;
        }

        public static void FillWith(double[,] matrix, double value)
        {
            int i, j;
            int xLen = matrix.GetLength(0);
            int yLen = matrix.GetLength(1);

            for (i = 0; i < xLen; i++)
            {
                for (j = 0; j < yLen; j++)
                {
                    matrix[i, j] = value;
                }
            }
        }

        public static double[] GetColumn(double[,] matrix, int index)
        {
            int length = matrix.GetLength(1);
            double[] row = new double[length];

            for (int i = 0; i < length; i++)
            {
                row[i] = matrix[index, i];
            }

            return row;
        }

        public static double[] GetRow(double[,] matrix, int index)
        {
            int length = matrix.GetLength(0);
            double[] column = new double[length];

            for (int i = 0; i < length; i++)
            {
                column[i] = matrix[i, index];
            }

            return column;
        }
    }

}
