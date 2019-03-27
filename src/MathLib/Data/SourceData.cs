using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MathLib.IO;

namespace MathLib.Data
{
    public class SourceData
    {
        private double[,] dataColumns;

        public SourceData(string filePath)
        {
            ReadFromFile(filePath);

            this.FileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
            this.Folder = Path.GetDirectoryName(filePath);
            this.Length = dataColumns.GetLength(0);
            this.ColumnsCount = dataColumns.GetLength(1);

            SetTimeSeries(0, 0, Length, 1, false);
        }

        public Timeseries TimeSeries { get; protected set; }

        public int Length { get; protected set; }

        public int ColumnsCount { get; protected set; }

        public double Step { get; protected set; }

        public string FileName { get; protected set; }

        public string Folder { get; protected set; }

        /// <summary>
        /// Set current time series from column and data range
        /// </summary>
        /// <param name="colIndex">index of column</param>
        /// <param name="startPoint">start point for time series</param>
        /// <param name="endPoint">end point for time series</param>
        /// <param name="pts">use each N point from range</param>
        public void SetTimeSeries(int colIndex, int startPoint, int endPoint, int pts, bool timeInFirstColumn)
        {
            int max = (endPoint - startPoint) / pts;
            this.TimeSeries = new Timeseries();

            for (int i = 0; i < max; i++)
            {
                int row = startPoint + i * pts;
                var x = timeInFirstColumn ? dataColumns[row, 0] : i + 1;
                var y = dataColumns[row, colIndex];
                this.TimeSeries.AddDataPoint(x, y);
            }
                    
            this.Step = this.TimeSeries.DataPoints[1].X - this.TimeSeries.DataPoints[0].X;
        }

        public string GetTimeSeriesString(bool withTime)
        {
            var timeSeriesOut = new StringBuilder();

            foreach (double value in TimeSeries.YValues)
            {
                timeSeriesOut.AppendLine(value.ToString(NumFormat.General, CultureInfo.InvariantCulture));
            }

            return timeSeriesOut.ToString();
        }

        public override string ToString()
        {
            return $"File: {this.FileName}\nLines: {this.Length}\nColumns: {this.ColumnsCount}";
        }

        private void ReadFromFile(string file)
        {
            var sourceData = File.ReadAllLines(file);
            var timeSeriesWidth = Regex.Split(sourceData[0].ToString().Trim(), "\\s+").Length;

            this.dataColumns = new double[sourceData.Length, timeSeriesWidth];

            for (int i = 0; i < sourceData.Length; i++)
            {
                var numbers = Regex.Split(sourceData[i].Trim(), "\\s+");

                for (int j = 0; j < timeSeriesWidth; j++)
                {
                    try
                    {
                        dataColumns[i, j] = double.Parse(numbers[j], NumberStyles.Any, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException ex)
                    {
                        throw new ArgumentException($"{ex.Message}\nLine: {j} , value: {numbers[j]}");
                    }
                }
            }
        }
    }
}
