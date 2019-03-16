using System;

namespace MathLib.MathMethods.Orthogonalization
{
    /// <summary>
    /// Orthogonalization class
    /// </summary>
    public abstract class Orthogonalization
    {
        protected int n;          //number of equations
        protected double[] gsc;   //gramm-schmidt coefficients matrix

        /// <summary>
        /// Orthogonalization methods
        /// </summary>
        /// <param name="equationsCount">Number of equations</param>
        public Orthogonalization(int equationsCount)
        {
            this.n = equationsCount;
            this.gsc = new double[n];
        }

        /// <summary>
        /// Make orthogonalization
        /// </summary>
        /// <param name="Qmatrix">Orthogonal matrix</param>
        /// <param name="Rmatrix">Normalized vector (triangular matrix)</param>
        public abstract void Perform(double[,] qMatrix, double[] rMatrix);
    }
}