using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MathLib.Data;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Multi Signal plot
    /// </summary>
    public abstract class DataSeriesPlot : PlotObject
    {
        public DataSeriesPlot(Size bitmapSize) 
            : base(bitmapSize, 1f)
        {
            this.TimeSeriesList = new List<Timeseries>();
            this.PlotPens = new List<Pen>();
            this.tsAmplitude = new DataPoint(0, 0);
            this.tsPointMax = new DataPoint(double.MinValue, double.MinValue);
            this.tsPointMin = new DataPoint(double.MaxValue, double.MaxValue);
        }

        public DataSeriesPlot(Size bitmapSize, Timeseries dataSeries, Color color, float thickness) 
            : this(bitmapSize)
        {
            this.AddDataSeries(dataSeries, color, thickness);
        }

        public DataSeriesPlot(Size bitmapSize, Timeseries dataSeries) 
            : this(bitmapSize, dataSeries, Color.SteelBlue, 1f)
        {
        }

        protected List<Timeseries> TimeSeriesList { get; set; }

        protected List<Pen> PlotPens { get; set; }

        protected DataPoint tsPointMax;

        protected DataPoint tsPointMin;

        protected DataPoint tsAmplitude;

        public void AddDataSeries(Timeseries dataSeries, Color color, float thickness)
        {
            this.TimeSeriesList.Add(dataSeries);
            this.PlotPens.Add(new Pen(color, thickness));

            this.tsPointMax.X = Math.Max(this.tsPointMax.X, dataSeries.Max.X);
            this.tsPointMax.Y = Math.Max(this.tsPointMax.Y, dataSeries.Max.Y);
            this.tsPointMin.X = Math.Min(this.tsPointMin.X, dataSeries.Min.X);
            this.tsPointMin.Y = Math.Min(this.tsPointMin.Y, dataSeries.Min.Y);

            this.tsAmplitude.X = this.tsPointMax.X - this.tsPointMin.X;
            this.tsAmplitude.Y = this.tsPointMax.Y - this.tsPointMin.Y;
        }

        public void AddDataSeries(Timeseries dataSeries, Color color) =>
            AddDataSeries(dataSeries, color, 1f);

        public override Bitmap Plot()
        {
            PrepareChartArea();

            if (this.TimeSeriesList.All(ts => ts.Length < 1))
            {
                NoDataToPlot();
            }
            else
            {
                CalculateChartAreaSize(this.tsAmplitude);

                for (int i = 0; i < this.TimeSeriesList.Count; i++)
                {
                    DrawDataSeries(this.TimeSeriesList[i], this.PlotPens[i]);
                }

                DrawGrid();
            }

            g.Dispose();

            return PlotBitmap;
        }

        protected override void DrawGrid()
        {
            SetAxisValues(
                GetAxisValue(this.tsPointMin.X),
                GetAxisValue(this.tsPointMax.X),
                GetAxisValue(this.tsPointMin.Y),
                GetAxisValue(this.tsPointMax.Y)
            );
        }

        protected abstract void DrawDataSeries(Timeseries ds, Pen pen);
    }
}
