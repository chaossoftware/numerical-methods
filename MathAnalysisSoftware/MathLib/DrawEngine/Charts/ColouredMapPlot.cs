using MathLib.DrawEngine.Charts.ColorMaps;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for map
    /// </summary>
    public class ColouredMapPlot : PlotObject {

        public double Xmin;
        public double Xmax;
        
        public double Ymin;
        public double Ymax;

        private double[,] timeSeries;
        private ColorMap colorCondition;

        DataPoint PointMin;
        DataPoint PointMax;
        DataPoint PointCoeff;

        public ColouredMapPlot(double[,] timeSeries, Size pictureboxSize, ColorMap colorCondition)
            : base(pictureboxSize, 1) {

            this.timeSeries = timeSeries;
            this.colorCondition = colorCondition;

            double axisOffset = Math.Max(BitmapSize.Height * 0.1, MinAxisOffset);

            PointMin = new DataPoint(axisOffset, BitmapSize.Height - axisOffset);
            PointMax = new DataPoint(BitmapSize.Width, 0);
            PointCoeff = new DataPoint((PointMax.X - PointMin.X) / timeSeries.GetLength(0), (PointMin.Y - PointMax.Y) / timeSeries.GetLength(1));
        }


        public override Bitmap Plot() {

            plotBitmap = new Bitmap(BitmapSize.Width, BitmapSize.Height);
            g = Graphics.FromImage(plotBitmap);

            Bitmap mapBitmap = new Bitmap(timeSeries.GetLength(0), timeSeries.GetLength(1));

            Graphics gMap = Graphics.FromImage(mapBitmap);

            if (timeSeries.Length < 1)
                return null;

            g.FillRectangle(new SolidBrush(Color.White), 0, 0, BitmapSize.Width, BitmapSize.Height);

            for (int i = 0; i < timeSeries.GetLength(0); i++)
                for (int j = 0; j < timeSeries.GetLength(1); j++) {
                    Brush brush = new SolidBrush(colorCondition.GetColor(timeSeries[i, j]));
                    gMap.FillRectangle(brush, new Rectangle(i, j, 1, 1));
                }

            DrawGrid();

            int crossX = (int)(PointMin.X + gridPen.Width / 2);
            int crossY = (int)(PointMin.Y);

            g.DrawImage(mapBitmap, new Rectangle(crossX, 0, BitmapSize.Width - crossX, PointMin.Yint));

            g.Dispose();

            return plotBitmap;
        }


        protected override void DrawGrid() {

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            Point crossPoint = new Point(PointMin.Xint, PointMin.Yint);

            g.DrawLine(gridPen, crossPoint, new Point(PointMin.Xint, PointMax.Yint));
            g.DrawLine(gridPen, crossPoint, new Point(PointMax.Xint, PointMin.Yint));


            // x axis text
            float xAxisY = BitmapSize.Height - axisTitleFont.Size;

            StringFormat FormatX = new StringFormat();
            FormatX.LineAlignment = StringAlignment.Center;

            FormatX.Alignment = StringAlignment.Near;
            g.DrawString(GetAxisValue(Xmin), gridFont, br, PointMin.Xint, xAxisY, FormatX);

            FormatX.Alignment = StringAlignment.Far;
            g.DrawString(GetAxisValue(Xmax), gridFont, br, PointMax.Xint, xAxisY, FormatX);

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
            g.DrawString(GetAxisValue(Ymin), gridFont, br, yAxisX, PointMax.Yint, FormatY);

            //max value at min as text rotated
            FormatY.Alignment = StringAlignment.Far;
            g.DrawString(GetAxisValue(Ymax), gridFont, br, yAxisX, PointMin.Yint, FormatY);

            FormatY.Alignment = StringAlignment.Center;
            g.DrawString(LabelY, axisTitleFont, br, yAxisX, PointMin.Yint / 2, FormatY);

            g.RotateTransform(-180, MatrixOrder.Append);
            g.TranslateTransform(PointMin.Xint, PointMin.Yint, MatrixOrder.Append);
        }
    }

}
