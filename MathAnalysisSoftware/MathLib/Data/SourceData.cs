using System.IO;
using System.Text;
using System.Globalization;

namespace MathLib.Data
{
    /// <summary>
    /// Class representing source data object
    /// </summary>
    public class SourceData
    {
        public int timeSeriesLength;        // number of points in 
        public int columnsCount;            // number of columns in file
        public string fileName;       // name of source file
        public string folder;         // parent forlder of source file

        public double Step;

        public double[,] dataColumns;      // 2-D array of file content

        public DataSeries TimeSeries;


        public SourceData(double[,] dataColumns, string filePath) {
            this.dataColumns = dataColumns;
            this.fileName = filePath.Substring(filePath.LastIndexOf("\\") + 1);
            this.folder = Path.GetDirectoryName(filePath);
            this.timeSeriesLength = dataColumns.GetLength(0);
            this.columnsCount = dataColumns.GetLength(1);
            SetTimeSeries(0, 0, timeSeriesLength, 1, false);
        }



        /// <summary>
        /// Set current time series from column and data range
        /// </summary>
        /// <param name="colIndex">index of column</param>
        /// <param name="startPoint">start point for time series</param>
        /// <param name="endPoint">end point for time series</param>
        /// <param name="pts">use each N point from range</param>
        public void SetTimeSeries(int colIndex, int startPoint, int endPoint, int pts, bool timeInFirstColumn) {
            int max = (endPoint - startPoint) / pts;

            TimeSeries = new DataSeries();

            if (timeInFirstColumn)
                for (int i = 0; i < max; i++) {
                    int row = startPoint + i * pts;
                    TimeSeries.AddDataPoint(dataColumns[row, 0], dataColumns[row, colIndex]);
                }
            else
                for (int i = 0; i < max; i++)
                    TimeSeries.AddDataPoint(dataColumns[startPoint + i * pts, colIndex]);
            Step = TimeSeries.DataPoints[1].X - TimeSeries.DataPoints[0].X;
        }


        public override string ToString()
        {
            return $"File: {this.fileName}\nLines: {timeSeriesLength}\nColumns: {columnsCount}";
        }


        public string GetTimeSeriesString(bool withTime) {
            StringBuilder timeSeriesOut = new StringBuilder();

            foreach (double value in TimeSeries.YValues)
                timeSeriesOut.AppendFormat(CultureInfo.InvariantCulture, "{0:F14}\n", value);

            return timeSeriesOut.ToString();
        }
    }
}
