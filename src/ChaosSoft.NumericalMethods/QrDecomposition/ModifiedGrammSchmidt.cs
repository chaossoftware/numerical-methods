using ChaosSoft.NumericalMethods.Algebra;

namespace ChaosSoft.NumericalMethods.QrDecomposition;

/// <summary>
/// Modified Gramm-Schmidt orthogonalization
/// </summary>
public sealed class ModifiedGrammSchmidt : IQrDecomposition
{
    private readonly int _n;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModifiedGrammSchmidt"/> class for specified equations count.
    /// </summary>
    /// <param name="equationsCount">equations count</param>
    public ModifiedGrammSchmidt(int equationsCount)
    {
        _n = equationsCount;
    }

    /// <summary>
    /// Performs orthogonalization.
    /// </summary>
    /// <param name="qMatrix">orthogonal matrix</param>
    /// <param name="rMatrix">normalized vector (triangular matrix)</param>
    public void Perform(double[,] qMatrix, double[] rMatrix)
    {
        double[] gsc = new double[_n];

        //generate a new orthonormal set
        for (int j = 0; j < _n; j++)
        {
            //1. Rmatrix(k,k) = norm (Qmatrix(:,k)) : calculate the vector's norm
            //vector len = sqr (x*x+y*y+z*z)
            rMatrix[j] = Matrices.ColumnNorm(qMatrix, j);

            //2. Qmatrix(:, k) = Qmatrix(:, k) / Rmatrix(k, k)
            //normalize the new vector
            for (int k = 0; k < _n; k++)
            {
                qMatrix[k, j] /= rMatrix[j];
                gsc[k] = 0d;
            }

            //3. Rmatrix(k, k+1:n) = Qmatrix (:, k)' * Qmatrix(:, k+1:n);
            //make gsr coefficients
            for (int k = 0; k < _n; k++)
            {
                for (int l = j + 1; l < _n; l++)
                {
                    gsc[l] += qMatrix[k, j] * qMatrix[k, l];
                }
            }

            //4. Qmatrix(:, k+1:n) = Qmatrix(:, k+1:n) - Qmatrix (:, k) * Rmatrix(k, k+1:n)
            //construct a new vector
            for (int k = 0; k < _n; k++)
            {
                for (int l = j + 1; l < _n; l++)
                {
                    qMatrix[k, l] -= qMatrix[k, j] * gsc[l];
                }
            }
        }
    }
}