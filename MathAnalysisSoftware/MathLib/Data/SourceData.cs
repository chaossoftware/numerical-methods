using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MathLib.Data
{
    public class SourceData
    {
        private double[,] dataColumns;
        private int length;
        private double step;
        private int columnsCount;
        private string fileName;
        private string folder;
        private Timeseries timeSeries;

        public SourceData(string filePath)
        {
            ReadFromFile(filePath);

            this.fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
            this.folder = Path.GetDirectoryName(filePath);
            this.length = dataColumns.GetLength(0);
            this.columnsCount = dataColumns.GetLength(1);

            SetTimeSeries(0, 0, length, 1, false);
        }

        public Timeseries TimeSeries => this.timeSeries;

        public int Length => this.length;

        public double Step => this.step;

        public int ColumnsCount => this.columnsCount;

        public string FileName => this.fileName;

        public string Folder => this.folder;

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

            this.timeSeries = new Timeseries();

            if (timeInFirstColumn)
                for (int i = 0; i < max; i++) {
                    int row = startPoint + i * pts;
                    this.timeSeries.AddDataPoint(dataColumns[row, 0], dataColumns[row, colIndex]);
                }
            else
                for (int i = 0; i < max; i++)
                    this.timeSeries.AddDataPoint(dataColumns[startPoint + i * pts, colIndex]);
            this.step = this.timeSeries.DataPoints[1].X - this.timeSeries.DataPoints[0].X;
        }

        public string GetTimeSeriesString(bool withTime)
        {
            var timeSeriesOut = new StringBuilder();

            foreach (double value in TimeSeries.YValues)
            {
                timeSeriesOut.AppendFormat(CultureInfo.InvariantCulture, "{0:F14}\n", value);
            }

            return timeSeriesOut.ToString();
        }

        public override string ToString()
        {
            return $"File: {this.fileName}\nLines: {this.length}\nColumns: {this.columnsCount}";
        }

        private void ReadFromFile(string file)
        {
            var sourceData = File.ReadAllLines(fileName);
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
