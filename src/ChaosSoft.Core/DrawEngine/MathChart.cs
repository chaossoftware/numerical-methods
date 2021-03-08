using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using ChaosSoft.Core.Data;

namespace ChaosSoft.Core.DrawEngine
{
    public partial class MathChart : Chart
    {
        public MathChart()
        {
            InitializeComponent();
        }

        public MathChart(Size size, string xTitle, string yTitle) : this()
        {
            Size = size;
            ChartAreas[0].AxisX.Title = xTitle;
            ChartAreas[0].AxisY.Title = yTitle;
        }

        public bool HasData => this.Series.Any(s => s.Points.Any());

        public MathChart SetAxisNames(string xTitle, string yTitle)
        {
            ChartAreas[0].AxisX.Title = xTitle;
            ChartAreas[0].AxisY.Title = yTitle;

            return this;
        }

        public MathChart AddTimeSeries(string seriesName, Timeseries timeseries, 
            SeriesChartType type, Color color, int markerSize)
        {
            var series = new Series
            {
                ChartArea = "ChartArea",
                ChartType = type,
                Color = color,
                MarkerColor = color,
                MarkerSize = markerSize,
                Name = seriesName
            };

            foreach (var dp in timeseries.DataPoints)
            {
                series.Points.AddXY(dp.X, dp.Y);
            }

            this.ChartAreas[0].AxisX.Minimum = Math.Min(this.ChartAreas[0].AxisX.Minimum, timeseries.Min.X);
            this.ChartAreas[0].AxisX.Maximum = Math.Max(this.ChartAreas[0].AxisX.Maximum, timeseries.Max.X);
            var amplitude = this.ChartAreas[0].AxisX.Maximum - this.ChartAreas[0].AxisX.Minimum;

            if (amplitude > 100000 || amplitude < 0.1)
            {
                this.ChartAreas[0].AxisX.LabelStyle.Format = "0.00e0";
            }
            else if (amplitude >= 1000)
            {
                this.ChartAreas[0].AxisX.LabelStyle.Format = "#####";
            } 
            else if (amplitude >= 10)
            {
                this.ChartAreas[0].AxisX.LabelStyle.Format = "###.##";
            }

            this.Series.Add(series);
            this.ChartAreas[0].AxisX.Interval = amplitude / 2;
            this.ChartAreas[0].RecalculateAxesScale();
            this.Update();

            return this;
        }

        public MathChart AddTimeSeries(string seriesName, Timeseries timeseries, SeriesChartType type, Color color) =>
            AddTimeSeries(seriesName, timeseries, type, color, 2);

        public MathChart AddTimeSeries(string seriesName, Timeseries timeseries, SeriesChartType type) =>
            AddTimeSeries(seriesName, timeseries, type, Color.SteelBlue, 2);

        public MathChart ClearChart()
        {
            this.Series.Clear();
            this.ChartAreas[0].AxisX.Interval = 0;
            this.ChartAreas[0].AxisX.Minimum = double.MaxValue;
            this.ChartAreas[0].AxisX.Maximum = double.MinValue;

            return this;
        }
    }
}
