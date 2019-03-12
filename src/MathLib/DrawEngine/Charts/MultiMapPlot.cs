using MathLib.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Poincare map
    /// </summary>
    public class MultiMapPlot : PlotObject
    {
        private readonly float thickness;

        private DataPoint tsPointMax;
        private DataPoint tsPointMin;
        private DataPoint tsAmplitude;

        protected List<Timeseries> TimeSeriesList;
        protected List<Pen> PlotPens;

        public MultiMapPlot(Size pictureboxSize, float thickness)
            : base(pictureboxSize, thickness)
        {
            this.thickness = thickness;

            this.TimeSeriesList = new List<Timeseries>();
            this.PlotPens = new List<Pen>();
            this.tsAmplitude = new DataPoint(0, 0);
            this.tsPointMax = new DataPoint(double.MinValue, double.MinValue);
            this.tsPointMin = new DataPoint(double.MaxValue, double.MaxValue);
        }

        public MultiMapPlot(Size pictureboxSize) : this(pictureboxSize, 1f)
        {

        }

        public void AddDataSeries(Timeseries dataSeries, Color color)
        {
            this.TimeSeriesList.Add(dataSeries);
            this.PlotPens.Add(new Pen(color, this.thickness));

            foreach (var ds in this.TimeSeriesList)
            {
                this.tsPointMax.X = Math.Max(this.tsPointMax.X, ds.Max.X);
                this.tsPointMax.Y = Math.Max(this.tsPointMax.Y, ds.Max.Y);
                this.tsPointMin.X = Math.Min(this.tsPointMin.X, ds.Min.X);
                this.tsPointMin.Y = Math.Min(this.tsPointMin.Y, ds.Min.Y);
            }

            this.tsAmplitude.X = this.tsPointMax.X - this.tsPointMin.X;
            this.tsAmplitude.Y = this.tsPointMax.Y - this.tsPointMin.Y;
        }

        public override Bitmap Plot()
        {
            SetDefaultAreaSize(this.tsAmplitude);

            plotBitmap = new Bitmap(this.Size.Width, this.Size.Height);
            g = Graphics.FromImage(plotBitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (this.TimeSeriesList.All(ts => ts.Length < 1))
            {
                return null;
            }

            g.FillRectangle(new SolidBrush(Color.White), 0, 0, this.Size.Width, this.Size.Height);

            for (int i = 0; i < this.TimeSeriesList.Count; i++)
            {
                DrawDataSeries(this.TimeSeriesList[i], this.PlotPens[i]);
            }

            DrawGrid();
            g.Dispose();

            return plotBitmap;
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

            foreach (DataPoint dp in ds.DataPoints)
            {
                xPl = PicPtMin.X + (dp.X - this.tsPointMin.X) * PicPtCoeff.X;
                yPl = PicPtMin.Y - (dp.Y - this.tsPointMin.Y) * PicPtCoeff.Y;

                g.DrawEllipse(pen, (int)xPl, (int)yPl, 1, 1);
            }
        }
    }
}
