using MathLib.Data;
using System.Drawing;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Poincare map
    /// </summary>
    public class MapPlot : PlotObject
    {
        public MapPlot(Timeseries timeSeries, Size pictureboxSize, float thickness)
            : base(pictureboxSize, thickness)
        {
            TimeSeries = timeSeries;
        }

        public MapPlot(Timeseries timeSeries, Size pictureboxSize) : this(timeSeries, pictureboxSize, 1f)
        {

        }

        protected Timeseries TimeSeries { get; set; }

        public override Bitmap Plot()
        {
            PrepareChartArea();

            if (TimeSeries.Length < 1)
            {
                NoDataToPlot();
            }
            else
            {
                CalculateChartAreaSize(TimeSeries.Amplitude);

                double xPl, yPl;

                foreach (var p in TimeSeries.DataPoints)
                {
                    xPl = PicPtMin.X + (p.X - TimeSeries.Min.X) * PicPtCoeff.X;
                    yPl = PicPtMin.Y - (p.Y - TimeSeries.Min.Y) * PicPtCoeff.Y;

                    g.DrawEllipse(plotPen, (float)xPl, (float)yPl, 1f, 1f);
                }

                DrawGrid();
            }
            
            g.Dispose();

            return PlotBitmap;
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
