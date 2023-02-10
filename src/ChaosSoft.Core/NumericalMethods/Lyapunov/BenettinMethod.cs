using System;

namespace ChaosSoft.Core.NumericalMethods.Lyapunov
{
    /// <summary>
    /// Lyapunov Methods class
    /// Methods:
    /// - Calculate Lyapunov exponents
    /// - Calculate Kaplan-Yorke dimension
    /// - Calculate KS Entropy (h)
    /// - Calculate Phase volume compression (d)
    /// </summary>
    public sealed class BenettinMethod
    {
        private readonly int _n;            //Number of equations
        private readonly double[] _ltot;    //sum array of lyapunov exponents

        /// <summary>
        /// Lyapunov related methods
        /// </summary>
        /// <param name="equationsCount">Number of equations</param>
        public BenettinMethod(int equationsCount)
        {
            _n = equationsCount;
            _ltot = new double[_n];
            Result = new double[equationsCount];
        }

        public double[] Result { get; }

        /// <summary>
        /// <para>Updating array of Lyapunov exponents (not averaged by time).</para>
        /// </summary>
        /// <param name="rMatrix">Normalized vector (triangular matrix)</param>
        public void CalculateLyapunovSpectrum(double[] rMatrix, double totalTime)
        {
            // update vector magnitudes 
            for (int i = 0; i < _n; i++)
            {
                if (rMatrix[i] > 0)
                {
                    _ltot[i] += Math.Log(rMatrix[i]);
                    Result[i] = _ltot[i] / totalTime;
                }
            }
        }
    }
}