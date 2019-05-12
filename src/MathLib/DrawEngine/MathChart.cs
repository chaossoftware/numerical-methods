using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using MathLib.Data;

namespace MathLib.DrawEngine
{
    public partial class MathChart : Chart
    {
        public MathChart()
        {
            InitializeComponent();
        }

        public bool HasData => this.Series.Any(s => s.Points.Any());

        //chart.ChartAreas[0].Axes[0].Title = "t";
        //        chart.ChartAreas[0].Axes[1].Title = "Slope";

        public void AddTimeSeries(string seriesName, Timeseries timeseries, SeriesChartType type, Color color, int markerSize)
        {
            var series = new Series();
            series.ChartArea = "ChartArea";
            series.ChartType = type;
            series.Color = color;
            series.MarkerColor = color;
            series.MarkerSize = markerSize;
            series.Name = seriesName;

            foreach (var dp in timeseries.DataPoints)
            {
                series.Points.AddXY(dp.X, dp.Y);
            }

            this.Series.Add(series);

            var newInterval = Math.Max(this.ChartAreas[0].AxisX.Interval, timeseries.Amplitude.X);
            this.ChartAreas[0].AxisX.Interval = newInterval;
            this.ChartAreas[0].RecalculateAxesScale();
            this.Update();
        }

        public void AddTimeSeries(string seriesName, Timeseries timeseries, SeriesChartType type, Color color) =>
            AddTimeSeries(seriesName, timeseries, type, color, 2);

        public void AddTimeSeries(string seriesName, Timeseries timeseries, SeriesChartType type) =>
            AddTimeSeries(seriesName, timeseries, type, Color.SteelBlue, 2);

        public void ClearChart() =>
            this.Series.Clear();
    }
}
