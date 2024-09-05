namespace ChaosSoft.NumericalMethods;

public class NumUtils
{
    /// <summary>
    /// Checks if provided number is NaN or Infinity
    /// </summary>
    /// <param name="value">number to check</param>
    /// <returns></returns>
    public static bool IsNanOrInfinity(double value) =>
        double.IsInfinity(value) || double.IsNaN(value);
}
