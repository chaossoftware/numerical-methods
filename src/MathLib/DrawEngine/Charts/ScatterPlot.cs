using MathLib.Data;
using System.Drawing;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Poincare map
    /// </summary>
    public class ScatterPlot : DataSeriesPlot
    {
        public ScatterPlot(Size pictureboxSize)
            : base(pictureboxSize)
        {
        }

        public ScatterPlot(Size bitmapSize, Timeseries dataSeries, Color color, float thickness) 
            : base(bitmapSize, dataSeries, color, thickness)
        {
        }

        public ScatterPlot(Size bitmapSize, Timeseries dataSeries) 
            : base(bitmapSize, dataSeries)
        {
        }

        protected override void DrawDataSeries(Timeseries ds, Pen pen)
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
