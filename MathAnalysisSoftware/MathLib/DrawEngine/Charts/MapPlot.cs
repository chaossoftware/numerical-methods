using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Poincare map
    /// </summary>
    public class MapPlot : PlotObject {

        DataSeries TimeSeries;
        DataPoint PointMin;
        DataPoint PointMax;
        DataPoint PointCoeff;

        public MapPlot(DataSeries timeSeries, Size pictureboxSize, float thickness)
            : base(pictureboxSize, thickness) {
            TimeSeries = timeSeries;
        }


        public override Bitmap Plot() {

            PointMin = new DataPoint(BitmapSize.Height * 0.1, BitmapSize.Height * 0.9);
            PointMax = new DataPoint(BitmapSize.Width, 0);
            PointCoeff = new DataPoint((PointMax.X - PointMin.X) / TimeSeries.Amplitude.X, (PointMin.Y - PointMax.Y) / TimeSeries.Amplitude.Y);

            plotBitmap = new Bitmap(BitmapSize.Width, BitmapSize.Height);
            g = Graphics.FromImage(plotBitmap);

            if (TimeSeries.Length < 1)
                return null;

            g.FillRectangle(new SolidBrush(Color.White), 0, 0, BitmapSize.Width, BitmapSize.Height);

            double xPl, yPl;

            foreach (DataPoint dp in TimeSeries.ListDataPoints)
            {
                xPl = PointMin.X + (dp.X - TimeSeries.PointMin.X) * PointCoeff.X;
                yPl = PointMin.Y - (dp.Y - TimeSeries.PointMin.Y) * PointCoeff.Y;
                
                g.DrawEllipse(plotPen, (int)xPl, (int)yPl, 1, 1);
            }

            DrawGrid();
            g.Dispose();

            return plotBitmap;
        }


        protected override void DrawGrid() {
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            string timeStart, timeEnd;

            timeStart = GetAxisValue(TimeSeries.PointMin.X, 4);
            timeEnd = GetAxisValue(TimeSeries.PointMax.X, 4);

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
            g.DrawString(GetAxisValue(TimeSeries.PointMin.Y), gridFont, br, yAxisX, PointMax.Yint, FormatY);

            //max value at min as text rotated
            FormatY.Alignment = StringAlignment.Far;
            g.DrawString(GetAxisValue(TimeSeries.PointMax.Y), gridFont, br, yAxisX, PointMin.Yint, FormatY);

            FormatY.Alignment = StringAlignment.Center;
            g.DrawString(LabelY, axisTitleFont, br, yAxisX, PointMin.Yint / 2, FormatY);

            g.RotateTransform(-180, MatrixOrder.Append);
        }
    }
}
