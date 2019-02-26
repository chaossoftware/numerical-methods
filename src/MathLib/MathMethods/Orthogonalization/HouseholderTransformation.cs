using System;

namespace MathLib.MathMethods.Orthogonalization {

    /// <summary>
    /// Householder orthogonalization
    /// </summary>
    public class HouseholderTransformation : Orthogonalization
    {
        public HouseholderTransformation(int equationsCount) : base(equationsCount) { }

        public override void Perform(double[,] qMatrix, double[] rMatrix)
        {
            int nn = n + 1;
            double[,] qr = new double[nn, n];

            Array.Copy(qMatrix, qr, qMatrix.Length);

            // Main loop.
            for (int k = 0; k < n; k++)
            {
                // Compute 2-norm of k-th column without under/overflow.
                double nrm = 0;

                for (int i = k + 1; i < nn; i++)
                {
                    nrm = Math.Sqrt(nrm * nrm + qr[i, k] * qr[i, k]);
                }

                if (nrm != 0.0) {
                    // Form k-th Householder vector.
                    if (qr[k + 1, k] < 0)
                    {
                        nrm = -nrm;
                    }

                    for (int i = k + 1; i < nn; i++)
                    {
                        qr[i, k] /= nrm;
                    }

                    qr[k + 1, k] += 1.0;

                    // Apply transformation to remaining columns.
                    for (int j = k + 1; j < n; j++)
                    {
                        double s = 0.0;

                        for (int i = k + 1; i < nn; i++)
                        {
                            s += qr[i, k] * qr[i, j];
                        }

                        s = -s / qr[k + 1, k];

                        for (int i = k + 1; i < nn; i++)
                        {
                            qr[i, j] += s * qr[i, k];
                        }
                    }
                }
                
                rMatrix[k] = nrm; //Rmatrix[k] = -nrm; ///why doe not work???
            }

            for (int k = n - 1; k >= 0; k--)
            {
                for (int i = 1; i < nn; i++)
                {
                    qMatrix[i, k] = 0.0;
                }

                qMatrix[k + 1, k] = 1.0;

                for (int j = k; j < n; j++)
                {
                    if (qr[k + 1, k] != 0)
                    {
                        double s = 0.0;

                        for (int i = k + 1; i < nn; i++)
                        {
                            s += qr[i, k] * qMatrix[i, j];
                        }

                        s = -s / qr[k + 1, k];

                        for (int i = k + 1; i < nn; i++)
                        {
                            qMatrix[i, j] += s * qr[i, k];
                        }
                    }
                }
            }

            for (int i = 1; i < nn; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    qMatrix[i, j] = -qMatrix[i, j];
                }
            }
        }
    }
}