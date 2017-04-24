using System;

namespace MathLib.MathMethods.Orthogonalization {

    /// <summary>
    /// Modified Gramm-Schmidt orthogonalization
    /// </summary>
    public class MGS : Orthogonalization {

        public MGS(int numberOfEquations)
            : base(numberOfEquations) {
        }


        public override void makeOrthogonalization(double[,] Qmatrix, double[] Rmatrix) {
            //generate a new orthonormal set
            for (int j = 0; j < n; j++) {
                Rmatrix[j] = 0.0;

                //1. Rmatrix(k,k) = norm (Qmatrix(:,k)) : calculate the vector's norm
                //vector len = sqr (x*x+y*y+z*z)
                for (int k = 1; k <= n; k++)
                    Rmatrix[j] += Qmatrix[k, j] * Qmatrix[k, j];
                Rmatrix[j] = Math.Sqrt(Rmatrix[j]);

                //2. Qmatrix(:, k) = Qmatrix(:, k) / Rmatrix(k, k)
                //normalize the new vector
                for (int k = 1; k <= n; k++) {
                    Qmatrix[k, j] /= Rmatrix[j];
                    gsc[k - 1] = 0.0;
                }

                //3. Rmatrix(k, k+1:n) = Qmatrix (:, k)' * Qmatrix(:, k+1:n);
                //make gsr coefficients
                for (int k = 1; k <= n; k++)
                    for (int l = j + 1; l < n; l++)
                        gsc[l] += Qmatrix[k, j] * Qmatrix[k, l];

                //4. Qmatrix(:, k+1:n) = Qmatrix(:, k+1:n) - Qmatrix (:, k) * Rmatrix(k, k+1:n)
                //construct a new vector
                for (int k = 1; k <= n; k++)
                    for (int l = j + 1; l < n; l++)
                        Qmatrix[k, l] -= Qmatrix[k, j] * gsc[l];
            }
        }

    }
}