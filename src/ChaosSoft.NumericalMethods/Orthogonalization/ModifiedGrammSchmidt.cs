using System;

namespace ChaosSoft.NumericalMethods.Orthogonalization
{
    /// <summary>
    /// Modified Gramm-Schmidt orthogonalization
    /// </summary>
    public class ModifiedGrammSchmidt : OrthogonalizationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiedGrammSchmidt"/> class for specified equations count.
        /// </summary>
        /// <param name="equationsCount">equations count</param>
        public ModifiedGrammSchmidt(int equationsCount) : base(equationsCount)
        {

        }

        /// <summary>
        /// Performs orthogonalization.
        /// </summary>
        /// <param name="qMatrix">orthogonal matrix</param>
        /// <param name="rMatrix">normalized vector (triangular matrix)</param>
        public override void Perform(double[,] qMatrix, double[] rMatrix)
        {
            //generate a new orthonormal set
            for (int j = 0; j < N; j++)
            {
                rMatrix[j] = 0.0;

                //1. Rmatrix(k,k) = norm (Qmatrix(:,k)) : calculate the vector's norm
                //vector len = sqr (x*x+y*y+z*z)
                for (int k = 1; k <= N; k++)
                {
                    rMatrix[j] += qMatrix[k, j] * qMatrix[k, j];
                }

                rMatrix[j] = Math.Sqrt(rMatrix[j]);

                //2. Qmatrix(:, k) = Qmatrix(:, k) / Rmatrix(k, k)
                //normalize the new vector
                for (int k = 1; k <= N; k++)
                {
                    qMatrix[k, j] /= rMatrix[j];
                    Gsc[k - 1] = 0d;
                }

                //3. Rmatrix(k, k+1:n) = Qmatrix (:, k)' * Qmatrix(:, k+1:n);
                //make gsr coefficients
                for (int k = 1; k <= N; k++)
                {
                    for (int l = j + 1; l < N; l++)
                    {
                        Gsc[l] += qMatrix[k, j] * qMatrix[k, l];
                    }
                }

                //4. Qmatrix(:, k+1:n) = Qmatrix(:, k+1:n) - Qmatrix (:, k) * Rmatrix(k, k+1:n)
                //construct a new vector
                for (int k = 1; k <= N; k++)
                {
                    for (int l = j + 1; l < N; l++)
                    {
                        qMatrix[k, l] -= qMatrix[k, j] * Gsc[l];
                    }
                }
            }
        }
    }
}