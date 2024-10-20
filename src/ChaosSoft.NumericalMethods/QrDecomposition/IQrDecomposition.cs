namespace ChaosSoft.NumericalMethods.QrDecomposition;

/// <summary>
/// Base class for orthogonalization implementations.
/// </summary>
public interface IQrDecomposition
{
    /// <summary>
    /// Performs orthogonalization.
    /// </summary>
    /// <param name="qMatrix">orthogonal matrix</param>
    /// <returns>normalized vector (triangular matrix) as double[] </returns>
    double[] Perform(double[,] qMatrix);
}