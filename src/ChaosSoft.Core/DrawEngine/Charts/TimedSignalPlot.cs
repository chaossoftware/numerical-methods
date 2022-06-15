using ChaosSoft.Core.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ChaosSoft.Core.DrawEngine.Charts
{
    /// <summary>
    /// Class for Signal plot
    /// </summary>
    public class TimedSignalPlot : DataSeriesPlot
    {
        private int currentStep = 0;

        public TimedSignalPlot(List<Timeseries> historicalData, Size bitmapSize) 
            : this(historicalData, bitmapSize, 1f)
        {

        }

        public TimedSignalPlot(List<Timeseries> historicalData, Size bitmapSize, float thickness)
            : base(bitmapSize)
        {
            this.Thickness = thickness;

            historicalData.ForEach(ds => AddDataSeries(ds, Color.SteelBlue, thickness));
        }

        public override Bitmap Plot()
        {
            PrepareChartArea();

            var ts = this.TimeSeriesList[currentStep];

            if (ts.Length < 1)
            {
                NoDataToPlot();
            }
            else
            {
                if (currentStep == 0)
                {
                    CalculateChartAreaSize(tsAmplitude);
                }

                DrawDataSeries(ts, PlotPens[currentStep]);

                if (NeedToDrawGrid)
                {
                    DrawGrid();
                }

                var formatT = new StringFormat();
                formatT.LineAlignment = StringAlignment.Center;
                formatT.Alignment = StringAlignment.Far;

                float offset = NeedToDrawGrid ? 2 * gridFontSizePx : 1 * gridFontSizePx;

                g.DrawString(GetAxisValue(double.Parse(ts.Name)), gridFont, txtBrush, (int)PicPtMax.X, Size.Height - offset, formatT);
            }

            g.Dispose();

            return PlotBitmap;
        }

        public Bitmap PlotNextStep()
        {
            Plot();
            currentStep++;
            return PlotBitmap;
        }

        protected override void DrawGrid()
        {
            SetAxisValues(
                GetAxisValue(tsPointMin.X),
                GetAxisValue(tsPointMax.X),
                GetAxisValue(tsPointMin.Y),
                GetAxisValue(tsPointMax.Y)
            );
        }

        protected override void DrawDataSeries(Timeseries ds, Pen pen)
        {
            double xPl, yPl;

            var points = new List<PointF>();

            foreach (var p in ds.DataPoints)
            {
                xPl = PicPtMin.X + (p.X - tsPointMin.X) * PicPtCoeff.X;
                yPl = PicPtMin.Y - (p.Y - tsPointMin.Y) * PicPtCoeff.Y - Thickness / 2d;
                points.Add(new PointF((float)xPl, (float)yPl));
            }

            var gp = new GraphicsPath();
            gp.AddLines(points.ToArray());
            g.DrawPath(pen, gp);
            gp.Dispose();
        }
    }
}
