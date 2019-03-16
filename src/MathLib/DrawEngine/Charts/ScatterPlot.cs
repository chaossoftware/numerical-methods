using MathLib.Data;
using System.Drawing;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Poincare map
    /// </summary>
    public class ScatterPlot : PlotObject
    {
        private DataPoint tsPointMax;
        private DataPoint tsPointMin;
        private DataPoint tsAmplitude;

        protected List<Timeseries> TimeSeriesList;
        protected List<Pen> PlotPens;

        public ScatterPlot(Size pictureboxSize)
            : base(pictureboxSize, 1f)
        {
            this.TimeSeriesList = new List<Timeseries>();
            this.PlotPens = new List<Pen>();
            this.tsAmplitude = new DataPoint(0, 0);
            this.tsPointMax = new DataPoint(double.MinValue, double.MinValue);
            this.tsPointMin = new DataPoint(double.MaxValue, double.MaxValue);
        }

        public ScatterPlot(Size bitmapSize, Timeseries dataSeries, Color color, float thickness) 
            : this(bitmapSize)
        {
            this.AddDataSeries(dataSeries, color, thickness);
        }

        public ScatterPlot(Size bitmapSize, Timeseries dataSeries) 
            : this(bitmapSize, dataSeries, Color.SteelBlue, 1f)
        {
        }

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

        private void DrawDataSeries(Timeseries ds, Pen pen)
        {
            double xPl, yPl;

            foreach (var p in ds.DataPoints)
            {
                xPl = PicPtMin.X + (p.X - this.tsPointMin.X) * PicPtCoeff.X;
                yPl = PicPtMin.Y - (p.Y - this.tsPointMin.Y) * PicPtCoeff.Y;

                g.DrawEllipse(pen, (float)xPl, (float)yPl, pen.Width, pen.Width);
            }
        }
    }
}
