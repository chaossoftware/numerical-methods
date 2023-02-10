using ChaosSoft.Core.Algebra;
using ChaosSoft.Core.Extensions;
using System.Linq;

namespace ChaosSoft.Core.NumericalMethods.Orthogonalization
{
    /// <summary>
    /// Modified Gramm-Schmidt orthogonalization
    /// </summary>
    public class ModifiedGrammSchmidt : OrthogonalizationBase
    {
        public ModifiedGrammSchmidt(int equationsCount) : base(equationsCount)
        {

        }

        public override void Perform(double[,] qMatrix, double[] rMatrix)
        {
            //generate a new orthonormal set
            for (int j = 0; j < N; j++)
            {
                //1. Rmatrix(k,k) = norm (Qmatrix(:,k)) : calculate the vector's norm
                //vector len = sqr (x*x+y*y+z*z)
                rMatrix[j] = Vectors.Norm(Matrixes.GetRow(qMatrix, j).Skip(1));

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