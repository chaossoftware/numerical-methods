using System;

namespace MathLib.MathMethods.Orthogonalization {

    /// <summary>
    /// Householder orthogonalization
    /// </summary>
    public class HH : Orthogonalization {

        public HH(int numberOfEquations)
            : base(numberOfEquations) {
        }

        public override void makeOrthogonalization(double[,] Qmatrix, double[] Rmatrix) {
            // Initialize.
            int nn = n + 1;
            double[,] QR = new double[nn, n];
            Array.Copy(Qmatrix, QR, Qmatrix.Length);

            // Main loop.
            for (int k = 0; k < n; k++) {
                // Compute 2-norm of k-th column without under/overflow.
                double nrm = 0;
                for (int i = k + 1; i < nn; i++) {
                    nrm = Math.Sqrt(nrm * nrm + QR[i, k] * QR[i, k]);
                }

                if (nrm != 0.0) {
                    // Form k-th Householder vector.
                    if (QR[k + 1, k] < 0) {
                        nrm = -nrm;
                    }
                    for (int i = k + 1; i < nn; i++) {
                        QR[i, k] /= nrm;
                    }
                    QR[k + 1, k] += 1.0;

                    // Apply transformation to remaining columns.
                    for (int j = k + 1; j < n; j++) {
                        double s = 0.0;
                        for (int i = k + 1; i < nn; i++) {
                            s += QR[i, k] * QR[i, j];
                        }
                        s = -s / QR[k + 1, k];
                        for (int i = k + 1; i < nn; i++) {
                            QR[i, j] += s * QR[i, k];
                        }
                    }
                }
                //Rmatrix[k] = -nrm; ///why not works???
                Rmatrix[k] = nrm;
            }

            for (int k = n - 1; k >= 0; k--) {
                for (int i = 1; i < nn; i++) {
                    Qmatrix[i, k] = 0.0;
                }
                Qmatrix[k + 1, k] = 1.0;
                for (int j = k; j < n; j++) {
                    if (QR[k + 1, k] != 0) {
                        double s = 0.0;
                        for (int i = k + 1; i < nn; i++) {
                            s += QR[i, k] * Qmatrix[i, j];
                        }
                        s = -s / QR[k + 1, k];
                        for (int i = k + 1; i < nn; i++) {
                            Qmatrix[i, j] += s * QR[i, k];
                        }
                    }
                }
            }

            //------------------------------------------
            for (int i = 1; i < nn; i++)
                for (int j = 0; j < n; j++)
                    Qmatrix[i, j] = -Qmatrix[i, j];
            //---------------------------------------------
        }
    }
}