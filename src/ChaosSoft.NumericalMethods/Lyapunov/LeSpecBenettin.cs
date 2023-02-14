using System;

namespace ChaosSoft.NumericalMethods.Lyapunov
{
    /// <summary>
    /// Wrapper to calculate lyapunov spectrum from Normalized vector (triangular matrix).
    /// </summary>
    public sealed class LeSpecBenettin
    {
        private readonly int _n;            //Number of equations
        private readonly double[] _ltot;    //sum array of lyapunov exponents

        /// <summary>
        /// Initializes a new instance of the<see cref="LeSpecSanoSawada"/> class for specific number of equations.
        /// </summary>
        /// <param name="equationsCount">number of equations</param>
        public LeSpecBenettin(int equationsCount)
        {
            _n = equationsCount;
            _ltot = new double[_n];
            Result = new double[equationsCount];
        }

        /// <summary>
        /// Gets lyapunov exponents spectrum.
        /// </summary>
        public double[] Result { get; }

        /// <summary>
        /// Updating array of Lyapunov exponents (not averaged by time).
        /// </summary>
        /// <param name="rMatrix">Normalized vector (triangular matrix)</param>
        /// <param name="totalTime">total modelling time passed</param>
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