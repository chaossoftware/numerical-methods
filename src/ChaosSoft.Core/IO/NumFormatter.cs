using System.Globalization;

namespace ChaosSoft.Core.IO
{
    public static class NumFormatter
    {
        private const string General = "G15";
        private const string Short = "0.#####";

        public static string GetFormattedNumber(double number, string format) =>
            number.ToString(format, CultureInfo.InvariantCulture);

        public static string ToShort(double number) =>
            number.ToString(Short, CultureInfo.InvariantCulture);

        public static string ToLong(double number) =>
            number.ToString(General, CultureInfo.InvariantCulture);
    }
}
