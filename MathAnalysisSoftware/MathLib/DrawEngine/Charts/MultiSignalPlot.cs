using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathLib.DrawEngine.Charts {
    /// <summary>
    /// Class for Multi Signal plot
    /// </summary>
    public class MultiSignalPlot : PlotObject {
        
        private double tStart = -1;
        private double tEnd = -1;

        protected double amplitude;
        protected double minVal;
        private double maxVal;
        List<double[]> multiSignal;
        double[] timeSeries;

        public MultiSignalPlot(List<double[,]> multiSignal, Size bitmapSize, float thickness)
            : base(bitmapSize, thickness) {
                init(multiSignal);
        }


        public MultiSignalPlot(List<double[,]> multiSignal, Size bitmapSize, float thickness, double tStart, double tEnd)
            : base(bitmapSize, thickness) {
            this.tStart = tStart;
            this.tEnd = tEnd;
            init(multiSignal);
        }

        protected void init(List<double[,]> multiSignal) {
            this.minVal = Ext.countMin(timeSeries);
            this.maxVal = Ext.countMax(timeSeries);
            this.amplitude = maxVal - minVal;
            LabelX = "t";
            LabelY = "w(t)";
        }


        public override Bitmap Plot() {

            plotBitmap = new Bitmap(BitmapSize.Width, BitmapSize.Height);
            g = Graphics.FromImage(plotBitmap);

            int xPl, yPl;
            double yCoeff = GetYCoefficient(amplitude / 2);
            double xCoeff = GetXCoefficient(timeSeries.Length);

            if (timeSeries.Length < 1) {
                return null;
            }

            Point[] points = new Point[timeSeries.Length];

            for (int i = 0; i < timeSeries.Length; i++) {
                xPl = (int)(i * xCoeff + BitmapSize.Height * 0.1);
                yPl = BitmapSize.Height - (int)(BitmapSize.Height * 0.1) - (int)((timeSeries[i] - minVal) * yCoeff);
                points[i] = new Point(xPl, yPl);
            }

            GraphicsPath gp = new GraphicsPath();
            gp.AddLines(points);
            g.DrawPath(plotPen, gp);
            DrawGrid();

            gp.Dispose();
            g.Dispose();

            return plotBitmap;
        }


        protected override void DrawGrid() {
            int crossX = (int)(BitmapSize.Height * 0.1);
            int crossY = (int)(BitmapSize.Height * 0.9);
            int xRight = BitmapSize.Width;
            int yUp = 0;

            string timeStart, timeEnd;

            if (tEnd == -1 || tStart == -1) {
                timeStart = "0";
                timeEnd = String.Format("{0:F4}", timeSeries.Length).TrimEnd('0').TrimEnd('.');
            }
            else {
                timeStart = String.Format("{0:F4}", tStart).TrimEnd('0').TrimEnd('.');
                timeEnd = String.Format("{0:F4}", tEnd).TrimEnd('0').TrimEnd('.');
            }

            Point crossPoint = new Point(crossX, crossY);

            g.DrawLine(gridPen, crossPoint, new Point(crossX, (int)(0)));
            g.DrawLine(gridPen, crossPoint, new Point(xRight, crossY));

            g.DrawString(timeStart, gridFont, br, (float)crossX, (float)crossY + 3);
            g.DrawString(timeEnd, gridFont, br, (float)xRight, (float)crossY + 3, FormatR);
            g.DrawString(LabelX, axisTitleFont, br, (float)xRight / 2, BitmapSize.Height - gridFont.Height);

            g.DrawString(String.Format("{0:F4}", maxVal).TrimEnd('0').TrimEnd('.'), gridFont, br, (float)crossX - gridFont.Height, (float)yUp, FormatV);
            g.DrawString(String.Format("{0:F4}", minVal).TrimEnd('0').TrimEnd('.'), gridFont, br, (float)crossX - gridFont.Height, (float)crossY, FormatVR);
            g.DrawString(LabelY, axisTitleFont, br, 0, (float)crossY / 2 + 15, FormatVR);
        }


        protected double GetXCoefficient(double maxVal) {
            return (BitmapSize.Width - BitmapSize.Height * 0.1) / maxVal;
        }

        protected double GetYCoefficient(double maxVal)
        {
            return (BitmapSize.Width - BitmapSize.Height * 0.1) / maxVal;
        }

    }
}
