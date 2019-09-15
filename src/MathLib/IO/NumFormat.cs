using System.Globalization;

namespace MathLib.IO
{
    public static class NumFormat
    {
        public const string General = "G15";
        public const string Short = "G5";

        public static string GetFormattedNumber(double number, string format) =>
            number.ToString(format, CultureInfo.InvariantCulture);

        public static string ToShort(double number) =>
            number.ToString(Short, CultureInfo.InvariantCulture);

        public static string ToLong(double number) =>
            number.ToString(General, CultureInfo.InvariantCulture);
    }
}
