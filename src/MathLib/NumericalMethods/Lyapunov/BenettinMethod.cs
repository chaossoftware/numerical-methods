using System;

namespace MathLib.NumericalMethods.Lyapunov
{
    /// <summary>
    /// Lyapunov Methods class
    /// Methods:
    /// - Calculae Lyapunov exponents
    /// - Calculate Kaplan-Yorke dimension
    /// - Calculate KS Entropy (h)
    /// - Calculate Phase volume compression (d)
    /// </summary>
    public class BenettinMethod
    {
        private int n;              //Number of equations
        private int i;              //counter
        private double[] ltot;       //sum array of lyapunov exponents

        /// <summary>
        /// Lyapunov related methods
        /// </summary>
        /// <param name="numberOfEquations">Number of equations</param>
        public BenettinMethod(int numberOfEquations)
        {
            this.n = numberOfEquations;
            this.ltot = new double[n];
            this.Result = new LyapunovSpectrum(numberOfEquations);
        }

        public LyapunovSpectrum Result { get; set; }

        /// <summary>
        /// <para>Updating array of Lyapunov exponents (not averaged by time).</para>
        /// </summary>
        /// <param name="rMatrix">Normalized vector (triangular matrix)</param>
        public void CalculateLyapunovSpectrum(double[] rMatrix, double totalTime)
        {
            // update vector magnitudes 
            for (int i = 0; i < n; i++)
            {
                if (rMatrix[i] > 0)
                {
                    ltot[i] += Math.Log(rMatrix[i]);
                    this.Result.Spectrum[i] = ltot[i] / totalTime;
                }
            }
        }
    }
}