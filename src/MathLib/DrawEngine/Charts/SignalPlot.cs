using MathLib.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Signal plot
    /// </summary>
    public class SignalPlot : PlotObject {
        
        protected Timeseries TimeSeries;

        public SignalPlot(Timeseries timeSeries, Size bitmapSize) : this(timeSeries, bitmapSize, 1f)
        {

        }

        public SignalPlot(Timeseries timeSeries, Size bitmapSize, float thickness) : base(bitmapSize, thickness)
        {
            TimeSeries = timeSeries;
            LabelX = "t";
            LabelY = "w(t)";
        }

        public override Bitmap Plot()
        {
            SetDefaultAreaSize(TimeSeries.Amplitude);

            plotBitmap = new Bitmap(this.Size.Width, this.Size.Height);
            g = Graphics.FromImage(plotBitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (TimeSeries.Length < 1)
            {
                return null;
            }

            g.FillRectangle(new SolidBrush(Color.White), 0, 0, this.Size.Width, this.Size.Height);

            double xPl, yPl;

            List<Point> points = new List<Point>();

            foreach (DataPoint dp in TimeSeries.DataPoints)
            {
                xPl = PicPtMin.X + (dp.X - TimeSeries.Min.X) * PicPtCoeff.X;
                yPl = PicPtMin.Y - (dp.Y - TimeSeries.Min.Y) * PicPtCoeff.Y;
                points.Add(new Point((int)xPl, (int)yPl));
            }

            var gp = new GraphicsPath();
            gp.AddLines(points.ToArray());
            g.DrawPath(plotPen, gp);
            DrawGrid();

            gp.Dispose();
            g.Dispose();

            return plotBitmap;
        }

        protected override void DrawGrid()
        {
            SetAxisValues(
                GetAxisValue(TimeSeries.Min.X),
                GetAxisValue(TimeSeries.Max.X),
                GetAxisValue(TimeSeries.Min.Y),
                GetAxisValue(TimeSeries.Max.Y)
            );
        }
    }
}
