using MathLib.Data;
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
        
        protected List<Timeseries> HistoricalData;
        private DataPoint TsPointMax;
        private DataPoint TsPointMin;
        private DataPoint TsAmplitude;

        int currentStep = 0;

        public TimedSignalPlot(List<Timeseries> historicalData, Size bitmapSize, float thickness)
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

            foreach (Timeseries da in HistoricalData)
            {
                TsPointMax.X = Math.Max(TsPointMax.X, da.Max.X);
                TsPointMax.Y = Math.Max(TsPointMax.Y, da.Max.Y);
                TsPointMin.X = Math.Min(TsPointMin.X, da.Min.X);
                TsPointMin.Y = Math.Min(TsPointMin.Y, da.Min.Y);
            }

            TsAmplitude.X = TsPointMax.X - TsPointMin.X;
            TsAmplitude.Y = TsPointMax.Y - TsPointMin.Y;

            SetDefaultAreaSize(TsAmplitude);
        }


        public override Bitmap Plot() {

            if (HistoricalData.Count < 1 || HistoricalData[0].Length < 1)
                return null;

            plotBitmap = new Bitmap(this.Size.Width, this.Size.Height);
            g = Graphics.FromImage(plotBitmap);

            DrawGrid();
            g.Dispose();

            return plotBitmap;
        }


        public Bitmap PlotNextStep()
        {
            g = Graphics.FromImage(plotBitmap);
            Timeseries da = HistoricalData[currentStep];

            currentStep++;

            g.FillRectangle(new SolidBrush(Color.White), (int)(this.Size.Height * 0.1), 0, this.Size.Width - (int)(this.Size.Height * 0.1), (int)(this.Size.Height * 0.9));

            double xPl, yPl;

            List<Point> points = new List<Point>();

            foreach (DataPoint dp in da.DataPoints)
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

            g.DrawString(GetAxisValue(double.Parse(da.Name)), gridFont, br, (int)PicPtMax.X, this.Size.Height - 4 * axisTitleFont.Size, FormatT);

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
