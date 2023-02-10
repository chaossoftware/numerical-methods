using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace ChaosSoft.Core.IO
{
    public static class DataReader
    {
        private const string NumberRegex = "\\s+";

        public static double[][] ReadColumnsFromFile(string file, int startOffset, int readLines)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("Source data file not found.", file);
            }

            int i, j;

            var sourceData = File.ReadAllLines(file);

            // Determine how many numbers in line.
            var columns = Regex.Split(sourceData[startOffset].Trim(), NumberRegex).Length;

            var length = readLines == 0 ? sourceData.Length - startOffset : readLines;

            double[][] dataColumns = new double[columns][];

            for (i = 0; i < dataColumns.Length; i++)
            {
                dataColumns[i] = new double[length];
            }

            for (i = startOffset; i < length + startOffset; i++)
            {
                var numbers = Regex.Split(sourceData[i].Trim(), NumberRegex);

                for (j = 0; j < columns; j++)
                {
                    if (double.TryParse(numbers[j], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        dataColumns[j][i - startOffset] = value;
                    }
                    else
                    {
                        throw new ArgumentException($"Unable to parse value (Line: {i + 1}, Column: {j} [value: {numbers[j]}])");
                    }
                }
            }

            return dataColumns;
        }

        public static double[][] ReadColumnsFromByteFile(string fileName)
        {
            byte[] bytes = File.ReadAllBytes(fileName);

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ms.Seek(0, 0);
                return (double[][])(new BinaryFormatter().Deserialize(ms));
            }
        }
    }
}
