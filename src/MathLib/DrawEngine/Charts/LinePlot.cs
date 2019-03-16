using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using MathLib.Data;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Multi Signal plot
    /// </summary>
    public class LinePlot : DataSeriesPlot
    {
        public LinePlot(Size bitmapSize) 
            : base(bitmapSize)
        {
        }

        public LinePlot(Size bitmapSize, Timeseries dataSeries, Color color, float thickness) 
            : base(bitmapSize, dataSeries, color, thickness)
        {
        }

        public LinePlot(Size bitmapSize, Timeseries dataSeries) 
            : base(bitmapSize, dataSeries)
        {
        }

        protected override void DrawDataSeries(Timeseries ds, Pen pen)
        {
            double xPl, yPl;
            var points = new List<PointF>();

            foreach (var p in ds.DataPoints)
            {
                xPl = PicPtMin.X + (p.X - this.tsPointMin.X) * PicPtCoeff.X;
                yPl = PicPtMin.Y - (p.Y - this.tsPointMin.Y) * PicPtCoeff.Y;
                points.Add(new PointF((float)xPl, (float)yPl));
            }

            var gp = new GraphicsPath();
            gp.AddLines(points.ToArray());
            g.DrawPath(pen, gp);
            gp.Dispose();
        }
    }
}
