using System;

namespace MathLib.NumericalMethods.Orthogonalization
{
    /// <summary>
    /// Modified Gramm-Schmidt orthogonalization
    /// </summary>
    public class ModifiedGrammSchmidt : Orthogonalization
    {
        public ModifiedGrammSchmidt(int equationsCount) : base(equationsCount)
        {

        }

        public override void Perform(double[,] qMatrix, double[] rMatrix)
        {
            //generate a new orthonormal set
            for (int j = 0; j < n; j++)
            {
                rMatrix[j] = 0.0;

                //1. Rmatrix(k,k) = norm (Qmatrix(:,k)) : calculate the vector's norm
                //vector len = sqr (x*x+y*y+z*z)
                for (int k = 1; k <= n; k++)
                {
                    rMatrix[j] += qMatrix[k, j] * qMatrix[k, j];
                }
                    
                rMatrix[j] = Math.Sqrt(rMatrix[j]);

                //2. Qmatrix(:, k) = Qmatrix(:, k) / Rmatrix(k, k)
                //normalize the new vector
                for (int k = 1; k <= n; k++)
                {
                    qMatrix[k, j] /= rMatrix[j];
                    gsc[k - 1] = 0.0;
                }

                //3. Rmatrix(k, k+1:n) = Qmatrix (:, k)' * Qmatrix(:, k+1:n);
                //make gsr coefficients
                for (int k = 1; k <= n; k++)
                {
                    for (int l = j + 1; l < n; l++)
                    {
                        gsc[l] += qMatrix[k, j] * qMatrix[k, l];
                    }
                }

                //4. Qmatrix(:, k+1:n) = Qmatrix(:, k+1:n) - Qmatrix (:, k) * Rmatrix(k, k+1:n)
                //construct a new vector
                for (int k = 1; k <= n; k++)
                {
                    for (int l = j + 1; l < n; l++)
                    {
                        qMatrix[k, l] -= qMatrix[k, j] * gsc[l];
                    }
                }
            }
        }
    }
}