using System.IO;
using System.Text;
using ChaosSoft.Core.IO;

namespace ChaosSoft.Core.Data
{
    public class SourceData
    {
        private readonly double[][] _dataColumns;

        public SourceData(string filePath, int startOffset, int readLines) :
            this(DataReader.ReadColumnsFromFile(filePath, startOffset, readLines), filePath)
        {
        }

        public SourceData(string filePath) : 
            this(DataReader.ReadColumnsFromFile(filePath, 0, 0), filePath)
        {
        }

        public SourceData(double[][] data, string filePath)
        {
            _dataColumns = data;

            if (!string.IsNullOrEmpty(filePath))
            {
                FileName = Path.GetFileName(filePath);
                Folder = Path.GetDirectoryName(filePath);
            }
            
            LinesCount = _dataColumns[0].Length;
            ColumnsCount = _dataColumns.Length;

            SetTimeSeries(0, 0, LinesCount, 1, false);
        }

        public int LinesCount { get; }

        public int ColumnsCount { get; }

        public string FileName { get; }

        public string Folder { get; }

        public DataSeries TimeSeries { get; protected set; }

        public double Step { get; protected set; }

        public static SourceData FromBytesFile(string filePath)
        {
            double[][] data = DataReader.ReadColumnsFromByteFile(filePath);
            return new SourceData(data, filePath);
        }

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
            TimeSeries = new DataSeries();

            for (int i = 0; i < max; i++)
            {
                int row = startPoint + i * pts;
                var x = timeInFirstColumn ? _dataColumns[0][row] : i + 1;
                var y = _dataColumns[colIndex][row];
                TimeSeries.AddDataPoint(x, y);
            }
                    
            Step = TimeSeries.DataPoints[1].X - TimeSeries.DataPoints[0].X;
        }

        public double[] GetColumn(int index) =>
            _dataColumns[index];

        public string GetTimeSeriesString()
        {
            var timeSeriesOut = new StringBuilder();

            foreach (double value in TimeSeries.YValues)
            {
                timeSeriesOut.AppendLine(NumFormatter.ToLong(value));
            }

            return timeSeriesOut.ToString();
        }

        public override string ToString() =>
            $"File: {FileName}\nLines: {LinesCount}\nColumns: {ColumnsCount}";
    }
}
