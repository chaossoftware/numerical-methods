using MathLib.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Signal plot
    /// </summary>
    public class SignalPlot : PlotObject
    {
        public SignalPlot(Timeseries timeSeries, Size bitmapSize) : this(timeSeries, bitmapSize, 1f)
        {

        }

        public SignalPlot(Timeseries timeSeries, Size bitmapSize, float thickness) : base(bitmapSize, thickness)
        {
            TimeSeries = timeSeries;
            LabelX = "t";
            LabelY = "w(t)";
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

                var points = new List<PointF>();

                foreach (var p in TimeSeries.DataPoints)
                {
                    xPl = PicPtMin.X + (p.X - TimeSeries.Min.X) * PicPtCoeff.X;
                    yPl = PicPtMin.Y - (p.Y - TimeSeries.Min.Y) * PicPtCoeff.Y;
                    points.Add(new PointF((float)xPl, (float)yPl));
                }

                var gp = new GraphicsPath();
                gp.AddLines(points.ToArray());
                g.DrawPath(plotPen, gp);
                gp.Dispose();

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
