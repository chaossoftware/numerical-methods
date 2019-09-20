using System;

namespace MathLib.NumericalMethods.Orthogonalization
{
    /// <summary>
    /// Classic Gramm-Schmidt orthogonalization
    /// </summary>
    public class ClassicGrammSchmidt : Orthogonalization
    {
        public ClassicGrammSchmidt(int equationsCount) : base(equationsCount)
        {

        }

        public override void Perform(double[,] qMatrix, double[] rMatrix)
        {
            //normalize the first vector
            rMatrix[0] = 0.0;

            for (int j = 1; j <= n; j++)
            {
                rMatrix[0] += qMatrix[j, 0] * qMatrix[j, 0];
            }
                
            rMatrix[0] = Math.Sqrt(rMatrix[0]);

            for (int j = 1; j <= n; j++)
            {
                qMatrix[j, 0] /= rMatrix[0];
            }

            //generate a new orthonormal set
            for (int j = 1; j < n; j++)
            {
                //1. Rmatrix(1:k-1, k) = Qmatrix (:, 1:k-1)' * A(:, k);
                //make j-1 gsr coefficients
                for (int k = 0; k < j; k++)
                {
                    gsc[k] = 0.0;

                    for (int l = 1; l <= n; l++)
                    {
                        gsc[k] += qMatrix[l, j] * qMatrix[l, k];
                    }
                }

                //2. Qmatrix(:, k) = A(:, k) - Qmatrix (:, 1:k-1) * Rmatrix(1:k, k)
                //construct a new vector
                for (int k = 1; k <= n; k++)
                {
                    for (int l = 0; l < j; l++)
                    {
                        qMatrix[k, j] -= gsc[l] * qMatrix[k, l];
                    }
                }

                //3. Rmatrix(k,k) = norm (Qmatrix(:,k)) : calculate the vector's norm
                //vector len = sqr (x*x+y*y+z*z)
                rMatrix[j] = 0.0;

                for (int k = 1; k <= n; k++)
                {
                    rMatrix[j] += qMatrix[k, j] * qMatrix[k, j];
                }
                    
                rMatrix[j] = Math.Sqrt(rMatrix[j]);

                //4. Qmatrix(:, k) = Qmatrix(:, k) / Rmatrix(k, k)
                //normalize the new vector
                for (int k = 1; k <= n; k++)
                {
                    qMatrix[k, j] /= rMatrix[j];
                }
            }
        }
    }
}