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


            StringFormat FormatT = new StringFormat();
            FormatT.LineAlignment = StringAlignment.Center;
            FormatT.Alignment = StringAlignment.Far;

            g.DrawString(GetAxisValue(da.t), gridFont, br, PointMax.Xint, BitmapSize.Height - 4 * axisTitleFont.Size, FormatT);

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

            // x axis text
            float xAxisY = BitmapSize.Height - axisTitleFont.Size;

            StringFormat FormatX = new StringFormat();
            FormatX.LineAlignment = StringAlignment.Center;

            FormatX.Alignment = StringAlignment.Near;
            g.DrawString(timeStart, gridFont, br, PointMin.Xint, xAxisY, FormatX);

            FormatX.Alignment = StringAlignment.Far;
            g.DrawString(timeEnd, gridFont, br, PointMax.Xint, xAxisY, FormatX);

            FormatX.Alignment = StringAlignment.Center;
            g.DrawString(LabelX, axisTitleFont, br, PointMax.Xint / 2 + PointMin.Xint, xAxisY, FormatX);


            //y axis text

            g.RotateTransform(180, MatrixOrder.Append);
            g.TranslateTransform(PointMin.Xint, PointMin.Yint, MatrixOrder.Append);

            float yAxisX = PointMin.Xint - axisTitleFont.Size;

            StringFormat FormatY = new StringFormat();
            FormatY.LineAlignment = StringAlignment.Center;
            FormatY.FormatFlags = StringFormatFlags.DirectionVertical;

            //min value at max as text rotated
            FormatY.Alignment = StringAlignment.Near;
            g.DrawString(GetAxisValue(TsPointMin.Y), gridFont, br, yAxisX, PointMax.Yint, FormatY);

            //max value at min as text rotated
            FormatY.Alignment = StringAlignment.Far;
            g.DrawString(GetAxisValue(TsPointMax.Y), gridFont, br, yAxisX, PointMin.Yint, FormatY);

            FormatY.Alignment = StringAlignment.Center;
            g.DrawString(LabelY, axisTitleFont, br, yAxisX, PointMin.Yint / 2, FormatY);

            g.RotateTransform(-180, MatrixOrder.Append);
            g.TranslateTransform(PointMin.Xint, PointMin.Yint, MatrixOrder.Append);
        }

    }
}
