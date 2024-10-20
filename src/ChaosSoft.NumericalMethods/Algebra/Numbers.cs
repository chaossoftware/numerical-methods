namespace ChaosSoft.NumericalMethods.Algebra;

/// <summary>
/// Contains helpers and utilities to work with numbers.
/// </summary>
public static class Numbers
{
    /// <summary>
    /// Checks if provided number is NaN or Infinity
    /// </summary>
    /// <param name="value">number to check</param>
    /// <returns>true - if number is NaN or an Infinity; otherwise - false</returns>
    public static bool IsNanOrInfinity(double value) =>
        double.IsInfinity(value) || double.IsNaN(value);

    /// <summary>
    /// Returns square of given number by simple multiplying without any additional handlng.
    /// It's more performant than Math.Pow
    /// </summary>
    /// <param name="value">number to raise in power</param>
    /// <returns>square of a number</returns>
    public static double FastPow2(double value) =>
        value * value;

    /// <summary>
    /// Gets fractional part from a given number.
    /// </summary>
    /// <param name="value">number to get fraction of</param>
    /// <returns>fractional part as double</returns>
    public static double Fraction(double value) =>
        value - (long)value;
}
