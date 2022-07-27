using ChaosSoft.Core.Data;
using System.Drawing;

namespace ChaosSoft.Core.DrawEngine.Charts
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
                yPl = PicPtMin.Y - (p.Y - this.tsPointMin.Y) * PicPtCoeff.Y - this.Thickness / 2d;

                g.DrawEllipse(pen, (float)xPl, (float)yPl, pen.Width, pen.Width);
            }
        }
    }
}
