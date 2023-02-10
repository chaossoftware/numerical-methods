using ChaosSoft.Core.Algebra;
using ChaosSoft.Core.Extensions;
using System.Linq;

namespace ChaosSoft.Core.NumericalMethods.Orthogonalization
{
    /// <summary>
    /// Classic Gramm-Schmidt orthogonalization
    /// </summary>
    public class ClassicGrammSchmidt : OrthogonalizationBase
    {
        public ClassicGrammSchmidt(int equationsCount) : base(equationsCount)
        {

        }

        public override void Perform(double[,] qMatrix, double[] rMatrix)
        {
            rMatrix[0] = Vectors.Norm(Matrixes.GetRow(qMatrix, 0).Skip(1));

            for (int j = 1; j <= N; j++)
            {
                qMatrix[j, 0] /= rMatrix[0];
            }

            //generate a new orthonormal set
            for (int j = 1; j < N; j++)
            {
                //1. Rmatrix(1:k-1, k) = Qmatrix (:, 1:k-1)' * A(:, k);
                //make j-1 gsr coefficients
                for (int k = 0; k < j; k++)
                {
                    Gsc[k] = 0.0;

                    for (int l = 1; l <= N; l++)
                    {
                        Gsc[k] += qMatrix[l, j] * qMatrix[l, k];
                    }
                }

                //2. Qmatrix(:, k) = A(:, k) - Qmatrix (:, 1:k-1) * Rmatrix(1:k, k)
                //construct a new vector
                for (int k = 1; k <= N; k++)
                {
                    for (int l = 0; l < j; l++)
                    {
                        qMatrix[k, j] -= Gsc[l] * qMatrix[k, l];
                    }
                }

                //3. Rmatrix(k,k) = norm (Qmatrix(:,k)) : calculate the vector's norm
                //vector len = sqr (x*x+y*y+z*z)
                rMatrix[j] = Vectors.Norm(Matrixes.GetRow(qMatrix, j).Skip(1));

                //4. Qmatrix(:, k) = Qmatrix(:, k) / Rmatrix(k, k)
                //normalize the new vector
                for (int k = 1; k <= N; k++)
                {
                    qMatrix[k, j] /= rMatrix[j];
                }
            }
        }
    }
}