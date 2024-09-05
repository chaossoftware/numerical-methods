namespace ChaosSoft.NumericalMethods.Orthogonalization
{
    /// <summary>
    /// Base class for orthogonalization implementations.
    /// </summary>
    public interface IQrDecomposition
    {
        /// <summary>
        /// Performs orthogonalization.
        /// </summary>
        /// <param name="qMatrix">orthogonal matrix</param>
        /// <param name="rMatrix">normalized vector (triangular matrix)</param>
        void Perform(double[,] qMatrix, double[] rMatrix);
    }
}