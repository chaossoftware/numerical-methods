using System;
using ChaosSoft.Core.DataUtils;

namespace ChaosSoft.NumericalMethods.Orthogonalization {

    /// <summary>
    /// Householder orthogonalization
    /// </summary>
    public sealed class HouseholderTransformation : IQrDecomposition
    {
        private readonly int _n;

        /// <summary>
        /// Initializes a new instance of the <see cref="HouseholderTransformation"/> class for specified equations count.
        /// </summary>
        /// <param name="equationsCount">equations count</param>
        public HouseholderTransformation(int equationsCount)
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
            int n = _n;
            double[,] Q = new double[n, n];
            double[,] R = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                double[] ai = Matrix.GetRow(qMatrix, i);
                double normAi = Math.Sqrt(ai[0] * ai[0] + ai[1] * ai[1]);
                double s = ai[0] > 0 ? normAi : -normAi;
                double t = 2 * s / (s + normAi);
                double c = 1 + t;
                double[] v = new double[] { t * c, t * c };

                for (int j = i + 1; j < n; j++)
                {
                    double temp = 0;
                    for (int k = 0; k <= i; k++)
                    {
                        temp += Q[j, k] * v[k];
                    }
                    for (int k = i; k < n; k++)
                    {
                        double u = qMatrix[j, k] - temp;
                        v[k - i] = u;
                    }
                }

                for (int j = 0; j <= i; j++)
                {
                    double temp = 0;
                    for (int k = 0; k <= i; k++)
                    {
                        temp += Q[j, k] * v[k];
                    }
                    R[j, i] = temp;
                }

                for (int j = i + 1; j < n; j++)
                {
                    double temp = 0;
                    for (int k = 0; k <= i; k++)
                    {
                        temp += Q[j, k] * v[k];
                    }
                    R[j, i] = temp;
                    for (int k = i; k < n; k++)
                    {
                        double u = qMatrix[j, k] - temp;
                        v[k - i] = u;
                    }
                }

                for (int j = 0; j < n; j++)
                {
                    Q[j, i] = v[j];
                }
            }

            Array.Copy(Q, qMatrix, qMatrix.Length);
            for (int i = 0; i < n; i++)
            {
                rMatrix[i] = R[i, i];
            }
        }
    }
}