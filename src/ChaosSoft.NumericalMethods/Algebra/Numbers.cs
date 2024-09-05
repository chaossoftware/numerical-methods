namespace ChaosSoft.NumericalMethods.Algebra;

public class Numbers
{
    /// <summary>
    /// Checks if provided number is NaN or Infinity
    /// </summary>
    /// <param name="value">number to check</param>
    /// <returns></returns>
    public static bool IsNanOrInfinity(double value) =>
        double.IsInfinity(value) || double.IsNaN(value);

    public static double QuickPow2(double num) =>
        num * num;

    public static double Fraction(double x) =>
        x - (long)x;
}
