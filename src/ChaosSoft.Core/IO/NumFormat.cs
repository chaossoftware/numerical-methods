using System.Globalization;

namespace ChaosSoft.Core.IO
{
    public static class NumFormat
    {
        private const string General = "G15";
        private const string Short = "G5";

        public static string GetFormattedNumber(double number, string format) =>
            number.ToString(format, CultureInfo.InvariantCulture);

        public static string ToShort(double number) =>
            number.ToString(Short, CultureInfo.InvariantCulture);

        public static string ToLong(double number) =>
            number.ToString(General, CultureInfo.InvariantCulture);
    }
}
