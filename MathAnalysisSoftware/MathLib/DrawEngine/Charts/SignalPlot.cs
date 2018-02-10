using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Signal plot
    /// </summary>
    public class SignalPlot : PlotObject {
        
        protected DataSeries TimeSeries;

        public SignalPlot(DataSeries timeSeries, Size bitmapSize, float thickness)
            : base(bitmapSize, thickness) {
            TimeSeries = timeSeries;
            LabelX = "t";
            LabelY = "w(t)";
        }


        public override Bitmap Plot() {
            SetDefaultAreaSize(TimeSeries.Amplitude);

            plotBitmap = new Bitmap(BitmapSize.Width, BitmapSize.Height);
            g = Graphics.FromImage(plotBitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (TimeSeries.Length < 1)
                return null;

            g.FillRectangle(new SolidBrush(Color.White), 0, 0, BitmapSize.Width, BitmapSize.Height);

            double xPl, yPl;

            List<Point> points = new List<Point>();

            foreach (DataPoint dp in TimeSeries.ListDataPoints)
            {
                xPl = PicPtMin.X + (dp.X - TimeSeries.PointMin.X) * PicPtCoeff.X;
                yPl = PicPtMin.Y - (dp.Y - TimeSeries.PointMin.Y) * PicPtCoeff.Y;
                points.Add(new Point((int)xPl, (int)yPl));
            }

            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(points.ToArray());
            g.DrawPath(plotPen, gp);
            DrawGrid();

            gp.Dispose();
            g.Dispose();

            return plotBitmap;
        }


        protected override void DrawGrid() {
            SetAxisValues(
                GetAxisValue(TimeSeries.PointMin.X, 4),
                GetAxisValue(TimeSeries.PointMax.X, 4),
                GetAxisValue(TimeSeries.PointMin.Y),
                GetAxisValue(TimeSeries.PointMax.Y)
            );
        }
    }
}
