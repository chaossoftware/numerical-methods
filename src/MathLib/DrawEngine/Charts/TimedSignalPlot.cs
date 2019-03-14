using MathLib.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Signal plot
    /// </summary>
    public class TimedSignalPlot : PlotObject
    {
        protected List<Timeseries> HistoricalData;
        private DataPoint tsPointMax;
        private DataPoint tsPointMin;
        private DataPoint tsAmplitude;

        private int currentStep = 0;

        public TimedSignalPlot(List<Timeseries> historicalData, Size bitmapSize) 
            : this(historicalData, bitmapSize, 1f)
        {

        }

        public TimedSignalPlot(List<Timeseries> historicalData, Size bitmapSize, float thickness)
            : base(bitmapSize, thickness)
        {
            HistoricalData = historicalData;

            tsPointMax = new DataPoint(double.MinValue, double.MinValue);
            tsPointMin = new DataPoint(double.MaxValue, double.MaxValue);
            tsAmplitude = new DataPoint(0, 0);

            foreach (Timeseries da in HistoricalData)
            {
                tsPointMax.X = Math.Max(tsPointMax.X, da.Max.X);
                tsPointMax.Y = Math.Max(tsPointMax.Y, da.Max.Y);
                tsPointMin.X = Math.Min(tsPointMin.X, da.Min.X);
                tsPointMin.Y = Math.Min(tsPointMin.Y, da.Min.Y);
            }

            tsAmplitude.X = tsPointMax.X - tsPointMin.X;
            tsAmplitude.Y = tsPointMax.Y - tsPointMin.Y;
        }

        public override Bitmap Plot()
        {
            PrepareChartArea();

            if (HistoricalData.Count < 1 || HistoricalData[0].Length < 1)
            {
                NoDataToPlot();
            }
            else
            {
                CalculateChartAreaSize(tsAmplitude);
                DrawGrid();
            }
            
            g.Dispose();

            return PlotBitmap;
        }

        public Bitmap PlotNextStep()
        {
            g = Graphics.FromImage(PlotBitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            var da = HistoricalData[currentStep];

            currentStep++;

            g.FillRectangle(bgBrush, (int)(this.Size.Height * 0.1), 0, this.Size.Width - (int)(this.Size.Height * 0.1), (int)(this.Size.Height * 0.9));

            double xPl, yPl;

            var points = new List<PointF>();

            foreach (var p in da.DataPoints)
            {
                xPl = PicPtMin.X + (p.X - tsPointMin.X) * PicPtCoeff.X;
                yPl = PicPtMin.Y - (p.Y - tsPointMin.Y) * PicPtCoeff.Y;
                points.Add(new PointF((float)xPl, (float)yPl));
            }

            var gp = new GraphicsPath();
            gp.AddLines(points.ToArray());
            g.DrawPath(plotPen, gp);

            var formatT = new StringFormat();
            formatT.LineAlignment = StringAlignment.Center;
            formatT.Alignment = StringAlignment.Far;

            g.DrawString(GetAxisValue(double.Parse(da.Name)), gridFont, txtBrush, (int)PicPtMax.X, this.Size.Height - 4 * axisTitleFont.Size, formatT);

            gp.Dispose();
            g.Dispose();

            return PlotBitmap;
        }

        protected override void DrawGrid()
        {
            SetAxisValues(
                GetAxisValue(tsPointMin.X),
                GetAxisValue(tsPointMax.X),
                GetAxisValue(tsPointMin.Y),
                GetAxisValue(tsPointMax.Y)
            );
        }
    }
}
