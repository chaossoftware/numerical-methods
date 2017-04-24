using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Signal plot
    /// </summary>
    public class TimedSignalPlot : PlotObject {
        
        protected List<DDArray> HistoricalData;
        private DataPoint TsPointMax;
        private DataPoint TsPointMin;
        private DataPoint TsAmplitude;

        DataPoint PointMin;
        DataPoint PointMax;
        DataPoint PointCoeff;

        int currentStep = 0;

        public TimedSignalPlot(List<DDArray> historicalData, Size bitmapSize, float thickness)
            : base(bitmapSize, thickness) {
            HistoricalData = historicalData;
            init();
        }


        protected void init() {
            LabelX = "t";
            LabelY = "w(t)";
            TsPointMax = new DataPoint(double.MinValue, double.MinValue);
            TsPointMin = new DataPoint(double.MaxValue, double.MaxValue);
            TsAmplitude = new DataPoint(0, 0);

            foreach (DDArray da in HistoricalData)
            {
                TsPointMax.X = Math.Max(TsPointMax.X, da.data.PointMax.X);
                TsPointMax.Y = Math.Max(TsPointMax.Y, da.data.PointMax.Y);
                TsPointMin.X = Math.Min(TsPointMin.X, da.data.PointMin.X);
                TsPointMin.Y = Math.Min(TsPointMin.Y, da.data.PointMin.Y);
            }

            TsAmplitude.X = TsPointMax.X - TsPointMin.X;
            TsAmplitude.Y = TsPointMax.Y - TsPointMin.Y;

            PointMin = new DataPoint(BitmapSize.Height * 0.1, BitmapSize.Height * 0.9);
            PointMax = new DataPoint(BitmapSize.Width, 0);
            PointCoeff = new DataPoint((PointMax.X - PointMin.X) / TsAmplitude.X, (PointMin.Y - PointMax.Y) / TsAmplitude.Y);
        }


        public override Bitmap Plot() {

            if (HistoricalData.Count < 1 || HistoricalData[0].data.Length < 1)
                return null;

            plotBitmap = new Bitmap(BitmapSize.Width, BitmapSize.Height);
            g = Graphics.FromImage(plotBitmap);

            DrawGrid();
            g.Dispose();

            return plotBitmap;
        }


        public Bitmap PlotNextStep()
        {
            g = Graphics.FromImage(plotBitmap);
            DDArray da = HistoricalData[currentStep];

            currentStep++;

            g.FillRectangle(new SolidBrush(Color.White), (int)(BitmapSize.Height * 0.1), 0, BitmapSize.Width - (int)(BitmapSize.Height * 0.1), (int)(BitmapSize.Height * 0.9));

            double xPl, yPl;

            List<Point> points = new List<Point>();

            foreach (DataPoint dp in da.data.ListDataPoints)
            {
                xPl = PointMin.X + (dp.X - TsPointMin.X) * PointCoeff.X;
                yPl = PointMin.Y - (dp.Y - TsPointMin.Y) * PointCoeff.Y;
                points.Add(new Point((int)xPl, (int)yPl));
            }

            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(points.ToArray());
            g.DrawPath(plotPen, gp);

            g.DrawString(GetAxisValue(da.t), gridFont, br, (float)PointMax.X, (float)(PointMin.Y - 20), FormatR);

            gp.Dispose();
            g.Dispose();

            return plotBitmap;
        }
         

        protected override void DrawGrid() {
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            string timeStart, timeEnd;

            timeStart = GetAxisValue(TsPointMin.X, 4);
            timeEnd = GetAxisValue(TsPointMax.X, 4);

            Point crossPoint = new Point(PointMin.Xint, PointMin.Yint);

            g.DrawLine(gridPen, crossPoint, new Point(PointMin.Xint, PointMax.Yint));
            g.DrawLine(gridPen, crossPoint, new Point(PointMax.Xint, PointMin.Yint));

            g.DrawString(timeStart, gridFont, br, PointMin.Xint, PointMin.Yint + 3);
            g.DrawString(timeEnd, gridFont, br, PointMax.Xint, PointMin.Yint + 3, FormatR);
            g.DrawString(LabelX, axisTitleFont, br, PointMax.Xint / 2, BitmapSize.Height - gridFont.Height);

            g.DrawString(GetAxisValue(TsPointMax.Y), gridFont, br, PointMin.Xint - gridFont.Height, PointMax.Yint, FormatV);
            g.DrawString(GetAxisValue(TsPointMin.Y), gridFont, br, PointMin.Xint - gridFont.Height, PointMin.Yint, FormatVR);
            g.DrawString(LabelY, axisTitleFont, br, 0, PointMin.Yint / 2 + 15, FormatVR);
        }

    }
}
