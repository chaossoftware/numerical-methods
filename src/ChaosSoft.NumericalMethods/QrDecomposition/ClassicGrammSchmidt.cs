using System;
using ChaosSoft.NumericalMethods.Algebra;

namespace ChaosSoft.NumericalMethods.QrDecomposition;

/// <summary>
/// Classic Gramm-Schmidt orthogonalization
/// </summary>
public sealed class ClassicGrammSchmidt : IQrDecomposition
{
    private readonly int _n;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassicGrammSchmidt"/> class for specified equations count.
    /// </summary>
    /// <param name="equationsCount">equations count</param>
    public ClassicGrammSchmidt(int equationsCount)
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

        //normalize the first vector
        rMatrix[0] = Matrices.ColumnNorm(qMatrix, 0);

        for (int j = 0; j < _n; j++)
        {
            qMatrix[j, 0] /= rMatrix[0];
        }

        //generate a new orthonormal set
        for (int j = 1; j < _n; j++)
        {
            //1. Rmatrix(1:k-1, k) = Qmatrix (:, 1:k-1)' * A(:, k);
            //make j-1 gsr coefficients
            for (int k = 0; k < j; k++)
            {
                gsc[k] = 0.0;

                for (int l = 0; l < _n; l++)
                {
                    gsc[k] += qMatrix[l, j] * qMatrix[l, k];
                }
            }

            //2. Qmatrix(:, k) = A(:, k) - Qmatrix (:, 1:k-1) * Rmatrix(1:k, k)
            //construct a new vector
            for (int k = 0; k < _n; k++)
            {
                for (int l = 0; l < j; l++)
                {
                    qMatrix[k, j] -= gsc[l] * qMatrix[k, l];
                }
            }

            //3. Rmatrix(k,k) = norm (Qmatrix(:,k)) : calculate the vector's norm
            //vector len = sqr (x*x+y*y+z*z)
            rMatrix[j] = Matrices.ColumnNorm(qMatrix, j);

            //4. Qmatrix(:, k) = Qmatrix(:, k) / Rmatrix(k, k)
            //normalize the new vector
            for (int k = 0; k < _n; k++)
            {
                qMatrix[k, j] /= rMatrix[j];
            }
        }
    }
}