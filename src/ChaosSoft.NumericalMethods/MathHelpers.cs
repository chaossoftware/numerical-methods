namespace ChaosSoft.NumericalMethods
{
    internal static class MathHelpers
    {
        internal static double Pow2(double num) =>
            num * num;

        internal static double Fraction(double x) => x - (long)x;
    }
}
