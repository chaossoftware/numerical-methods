using System;

namespace MathLib.MathMethods.Orthogonalization {

    /// <summary>
    /// Classic Gramm-Schmidt orthogonalization
    /// </summary>
    public class CGS : Orthogonalization {

        public CGS(int numberOfEquations)
            : base(numberOfEquations) {
        }


        public override void makeOrthogonalization(double[,] Qmatrix, double[] Rmatrix) {
            //normalize the first vector
            Rmatrix[0] = 0.0;
            for (int j = 1; j <= n; j++)
                Rmatrix[0] += Qmatrix[j, 0] * Qmatrix[j, 0];
            Rmatrix[0] = Math.Sqrt(Rmatrix[0]);

            for (int j = 1; j <= n; j++)
                Qmatrix[j, 0] /= Rmatrix[0];

            //generate a new orthonormal set
            for (int j = 1; j < n; j++) {

                //1. Rmatrix(1:k-1, k) = Qmatrix (:, 1:k-1)' * A(:, k);
                //make j-1 gsr coefficients
                for (int k = 0; k < j; k++) {
                    gsc[k] = 0.0;
                    for (int l = 1; l <= n; l++)
                        gsc[k] += Qmatrix[l, j] * Qmatrix[l, k];
                }

                //2. Qmatrix(:, k) = A(:, k) - Qmatrix (:, 1:k-1) * Rmatrix(1:k, k)
                //construct a new vector
                for (int k = 1; k <= n; k++)
                    for (int l = 0; l < j; l++)
                        Qmatrix[k, j] -= gsc[l] * Qmatrix[k, l];

                //3. Rmatrix(k,k) = norm (Qmatrix(:,k)) : calculate the vector's norm
                //vector len = sqr (x*x+y*y+z*z)
                Rmatrix[j] = 0.0;

                for (int k = 1; k <= n; k++)
                    Rmatrix[j] += Qmatrix[k, j] * Qmatrix[k, j];
                Rmatrix[j] = Math.Sqrt(Rmatrix[j]);

                //4. Qmatrix(:, k) = Qmatrix(:, k) / Rmatrix(k, k)
                //normalize the new vector
                for (int k = 1; k <= n; k++)
                    Qmatrix[k, j] /= Rmatrix[j];
            }
        }

    }
}