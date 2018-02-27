using MathLib.Data;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace MathLib.IO {
    /// <summary>
    /// Class to read data from file
    /// </summary>
    public class DataReader {

        /// <summary>
        /// Read data to SourceData object from file
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns></returns>
        public static SourceData readTimeSeries(string fileName) {
            int timeSeriesLength;
            int timeSeriesWidth;

            string[] sourceData = File.ReadAllLines(fileName);
            string[] line;


            timeSeriesLength = sourceData.Length;
            timeSeriesWidth = Regex.Split(sourceData[0].ToString().Trim(), "\\s+").Length;

            double[,] timeSeries = new double[timeSeriesLength, timeSeriesWidth];

            for (int i = 0; i < timeSeriesLength; i++) {
                line = Regex.Split(sourceData[i].Trim(), "\\s+");

                for (int j = 0; j < timeSeriesWidth; j++)
                    try {
                        timeSeries[i, j] = double.Parse(line[j], NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (FormatException ex) {
                        throw new ArgumentException(ex.Message + "\nLine: " + j + ". Value: " + line[j]);
                    }
            }

            return new SourceData(timeSeries, fileName);
        }
    }
}
