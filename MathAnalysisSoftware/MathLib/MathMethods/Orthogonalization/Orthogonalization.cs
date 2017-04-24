using System;

namespace MathLib.MathMethods.Orthogonalization {

    /// <summary>
    /// Orthogonalization class
    /// </summary>
    public abstract class Orthogonalization {

        protected int n;          //number of equations
        protected double[] gsc;   //gramm-schmidt coefficients matrix

        /// <summary>
        /// Orthogonalization methods
        /// </summary>
        /// <param name="numberOfEquations">Number of equations</param>
        public Orthogonalization(int numberOfEquations) {
            this.n = numberOfEquations;
            gsc = new double[n];
        }

        /// <summary>
        /// Make orthogonalization
        /// </summary>
        /// <param name="Qmatrix">Orthogonal matrix</param>
        /// <param name="Rmatrix">Normalized vector (triangular matrix)</param>
        public abstract void makeOrthogonalization(double[,] Qmatrix, double[] Rmatrix);
    }
}