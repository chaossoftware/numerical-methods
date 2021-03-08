using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ChaosSoft.Core.IO;

namespace ChaosSoft.Core.Data
{
    public class SourceData
    {
        private const string NumberRegex = "\\s+";

        private double[][] dataColumns;

        public SourceData(string filePath, int startOffset, int readLines)
        {
            ReadFromFile(filePath, startOffset, readLines);

            FileName = Path.GetFileName(filePath);
            Folder = Path.GetDirectoryName(filePath);
            LinesCount = dataColumns[0].Length;
            ColumnsCount = dataColumns.Length;

            SetTimeSeries(0, 0, LinesCount, 1, false);
        }

        public SourceData(string filePath) : this (filePath, 0, 0)
        {
        }

        public int LinesCount { get; }

        public int ColumnsCount { get; }

        public string FileName { get; }

        public string Folder { get; }

        public Timeseries TimeSeries { get; protected set; }

        public double Step { get; protected set; }

        /// <summary>
        /// Set current time series from column and data range.
        /// </summary>
        /// <param name="colIndex">index of column</param>
        /// <param name="startPoint">start point for time series</param>
        /// <param name="endPoint">end point for time series</param>
        /// <param name="pts">use each N point from range</param>
        /// <param name="timeInFirstColumn">specify whether to use first column values as time or not</param>
        public void SetTimeSeries(int colIndex, int startPoint, int endPoint, int pts, bool timeInFirstColumn)
        {
            int max = (endPoint - startPoint) / pts;
            TimeSeries = new Timeseries();

            for (int i = 0; i < max; i++)
            {
                int row = startPoint + i * pts;
                var x = timeInFirstColumn ? dataColumns[0][row] : i + 1;
                var y = dataColumns[colIndex][row];
                TimeSeries.AddDataPoint(x, y);
            }
                    
            Step = TimeSeries.DataPoints[1].X - TimeSeries.DataPoints[0].X;
        }

        public double[] GetColumn(int index) =>
            dataColumns[index];

        public string GetTimeSeriesString()
        {
            var timeSeriesOut = new StringBuilder();

            foreach (double value in TimeSeries.YValues)
            {
                timeSeriesOut.AppendLine(NumFormat.ToLong(value));
            }

            return timeSeriesOut.ToString();
        }

        public override string ToString() =>
            $"File: {FileName}\nLines: {LinesCount}\nColumns: {ColumnsCount}";

        private void ReadFromFile(string file, int startOffset, int readLines)
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

            dataColumns = new double[columns][];
            
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
        }
    }
}
