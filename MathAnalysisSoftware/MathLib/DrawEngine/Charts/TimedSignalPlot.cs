using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for Signal plot
    /// </summary>
    public class TimedSignalPlot : PlotObject {
        
        protected List<DDArray> HistoricalData;
        private DataPoint TsPointMax;
        private DataPoint TsPointMin;
        private DataPoint TsAmplitude;

        int currentStep = 0;

        public TimedSignalPlot(List<DDArray> historicalData, Size bitmapSize, float thickness)
            : base(bitmapSize, thickness) {
            HistoricalData = historicalData;
            init();
        }


        protected void init() {
            LabelX = "t";
            LabelY = "w(t)";
            TsPointMax = new DataPoint(double.MinValue, double.MinValue);
            TsPointMin = new DataPoint(double.MaxValue, double.MaxValue);
            TsAmplitude = new DataPoint(0, 0);

            foreach (DDArray da in HistoricalData)
            {
                TsPointMax.X = Math.Max(TsPointMax.X, da.data.PointMax.X);
                TsPointMax.Y = Math.Max(TsPointMax.Y, da.data.PointMax.Y);
                TsPointMin.X = Math.Min(TsPointMin.X, da.data.PointMin.X);
                TsPointMin.Y = Math.Min(TsPointMin.Y, da.data.PointMin.Y);
            }

            TsAmplitude.X = TsPointMax.X - TsPointMin.X;
            TsAmplitude.Y = TsPointMax.Y - TsPointMin.Y;

            SetDefaultAreaSize(TsAmplitude);
        }


        public override Bitmap Plot() {

            if (HistoricalData.Count < 1 || HistoricalData[0].data.Length < 1)
                return null;

            plotBitmap = new Bitmap(BitmapSize.Width, BitmapSize.Height);
            g = Graphics.FromImage(plotBitmap);

            DrawGrid();
            g.Dispose();

            return plotBitmap;
        }


        public Bitmap PlotNextStep()
        {
            g = Graphics.FromImage(plotBitmap);
            DDArray da = HistoricalData[currentStep];

            currentStep++;

            g.FillRectangle(new SolidBrush(Color.White), (int)(BitmapSize.Height * 0.1), 0, BitmapSize.Width - (int)(BitmapSize.Height * 0.1), (int)(BitmapSize.Height * 0.9));

            double xPl, yPl;

            List<Point> points = new List<Point>();

            foreach (DataPoint dp in da.data.ListDataPoints)
            {
                xPl = PicPtMin.X + (dp.X - TsPointMin.X) * PicPtCoeff.X;
                yPl = PicPtMin.Y - (dp.Y - TsPointMin.Y) * PicPtCoeff.Y;
                points.Add(new Point((int)xPl, (int)yPl));
            }

            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(points.ToArray());
            g.DrawPath(plotPen, gp);


            StringFormat FormatT = new StringFormat();
            FormatT.LineAlignment = StringAlignment.Center;
            FormatT.Alignment = StringAlignment.Far;

            g.DrawString(GetAxisValue(da.t), gridFont, br, PicPtMax.Xint, BitmapSize.Height - 4 * axisTitleFont.Size, FormatT);

            gp.Dispose();
            g.Dispose();

            return plotBitmap;
        }
         

        protected override void DrawGrid() {
            SetAxisValues(
                GetAxisValue(TsPointMin.X),
                GetAxisValue(TsPointMax.X),
                GetAxisValue(TsPointMin.Y),
                GetAxisValue(TsPointMax.Y)
            );
        }

    }
}
