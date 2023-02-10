namespace ChaosSoft.Core.NumericalMethods.Orthogonalization
{
    /// <summary>
    /// Base class for orthogonalization implementations.
    /// </summary>
    public abstract class OrthogonalizationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrthogonalizationBase"/> class for specified equations count.
        /// </summary>
        /// <param name="equationsCount">number of equations</param>
        protected OrthogonalizationBase(int equationsCount)
        {
            N = equationsCount;
            Gsc = new double[N];
        }

        /// <summary>
        /// Gets number of equations.
        /// </summary>
        protected int N { get; }

        /// <summary>
        /// Gets gramm-schmidt coefficients vector.
        /// </summary>
        protected double[] Gsc { get; }

        /// <summary>
        /// Make orthogonalization
        /// </summary>
        /// <param name="qMatrix">orthogonal matrix</param>
        /// <param name="rMatrix">normalized vector (triangular matrix)</param>
        public abstract void Perform(double[,] qMatrix, double[] rMatrix);
    }
}