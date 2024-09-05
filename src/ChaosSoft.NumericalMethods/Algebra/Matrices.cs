using System;

namespace ChaosSoft.NumericalMethods.Algebra
{
    /// <summary>
    /// Provides with methods operating with vectors.
    /// </summary>
    public static class Matrices
    {
        /// <summary>
        /// Gets matrix column norm. Matrix column or vector [x,y,z] norm is Sqrt(x*x + y*y + z*z).
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        public static double ColumnNorm(double[,] matrix, int colIndex)
        {
            int length = matrix.GetLength(0);
            double norm = 0d;

            for (int i = 0; i < length; i++)
            {
                norm += matrix[i, colIndex] * matrix[i, colIndex];
            }

            return Math.Sqrt(norm);
        }
    }
}
