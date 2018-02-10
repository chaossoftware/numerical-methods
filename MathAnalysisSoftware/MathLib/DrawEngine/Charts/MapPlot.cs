using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Poincare map
    /// </summary>
    public class MapPlot : PlotObject {

        DataSeries TimeSeries;
        

        public MapPlot(DataSeries timeSeries, Size pictureboxSize, float thickness)
            : base(pictureboxSize, thickness) {
            TimeSeries = timeSeries;
        }


        public override Bitmap Plot() {
            SetDefaultAreaSize(TimeSeries.Amplitude);

            plotBitmap = new Bitmap(this.Size.Width, this.Size.Height);
            g = Graphics.FromImage(plotBitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (TimeSeries.Length < 1)
                return null;

            g.FillRectangle(new SolidBrush(Color.White), 0, 0, this.Size.Width, this.Size.Height);

            double xPl, yPl;

            foreach (DataPoint dp in TimeSeries.ListDataPoints)
            {
                xPl = PicPtMin.X + (dp.X - TimeSeries.PointMin.X) * PicPtCoeff.X;
                yPl = PicPtMin.Y - (dp.Y - TimeSeries.PointMin.Y) * PicPtCoeff.Y;
                
                g.DrawEllipse(plotPen, (int)xPl, (int)yPl, 1, 1);
            }

            DrawGrid();
            g.Dispose();

            return plotBitmap;
        }


        protected override void DrawGrid() {
            SetAxisValues(
                GetAxisValue(TimeSeries.PointMin.X), 
                GetAxisValue(TimeSeries.PointMax.X), 
                GetAxisValue(TimeSeries.PointMin.Y), 
                GetAxisValue(TimeSeries.PointMax.Y)
            );
        }
    }
}
