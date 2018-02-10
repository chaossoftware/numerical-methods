using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace MathLib.DrawEngine.Charts {
    /// <summary>
    /// Base class for plot object 
    /// </summary>
    public abstract class PlotObject {
        
        protected Bitmap plotBitmap;                // bitmap to draw plot
        protected Graphics g;

        protected Pen plotPen;
        protected Pen gridPen;
        protected Font gridFont;                    // Font for grid labels
        protected Font axisTitleFont;               // Font for axis titles
        protected SolidBrush br;                    // Brush for grid text

        protected DataPoint PicPtMin;
        protected DataPoint PicPtMax;
        protected DataPoint PicPtCoeff;

        /// <summary>
        /// Base constructor for plot object. initializes necessary objects to create plot
        /// </summary>
        /// <param name="pBox">Picture box control to display plot</param>
        /// <param name="thickness">thickness of plot lines</param>
        public PlotObject(Size bitmapSize, float thickness)
        {
            this.Size = bitmapSize;

            this.LabelX = "x";
            this.LabelY = "y";

            plotPen = new Pen(Color.SteelBlue, thickness);
            gridPen = new Pen(Color.Black, 2);
            gridFont = new Font(new FontFamily("Cambria Math"), 10f);
            axisTitleFont = new Font(new FontFamily("Cambria Math"), 11f, FontStyle.Bold);
            br = new SolidBrush(Color.Black);
        }

        public Size Size { get; set; }

        public string LabelX { get; set; }

        public string LabelY { get; set; }

        /// <summary>
        /// Plot chart
        /// </summary>
        public abstract Bitmap Plot();

        /// <summary>
        /// Draw chart grid
        /// </summary>
        protected abstract void DrawGrid();

        protected string GetAxisValue(double value)
        {
            int decimalPlaces;
            var absValue = Math.Abs(value);

            if (absValue < 10)
            {
                decimalPlaces = 5;
            }
            else if (absValue < 100)
            {
                decimalPlaces = 3;
            }
            else
            {
                decimalPlaces = 1;
            }
            return string.Format("{0:F" + decimalPlaces + "}", value).TrimEnd('0').TrimEnd('.');
        }

        protected void SetDefaultAreaSize(DataPoint amplitude)
        {
            //set plot default area size
            double axisOffset = Math.Max(this.Size.Height * 0.1, 25);
            PicPtMin = new DataPoint(axisOffset, this.Size.Height - axisOffset);
            PicPtMax = new DataPoint(this.Size.Width, 0);
            PicPtCoeff = new DataPoint((PicPtMax.X - PicPtMin.X) / amplitude.X, (PicPtMin.Y - PicPtMax.Y) / amplitude.Y);
        }

        protected void SetAxisValues(string xMin, string xMax, string yMin, string yMax)
        {
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            g.DrawLine(gridPen, PicPtMin.Xint, PicPtMin.Yint, PicPtMin.Xint, PicPtMax.Yint);
            g.DrawLine(gridPen, PicPtMin.Xint, PicPtMin.Yint, PicPtMax.Xint, PicPtMin.Yint);
            
            // x axis text
            float xAxisY = this.Size.Height - axisTitleFont.Size;

            StringFormat FormatX = new StringFormat();
            FormatX.LineAlignment = StringAlignment.Center;

            FormatX.Alignment = StringAlignment.Near;
            g.DrawString(xMin, gridFont, br, PicPtMin.Xint, xAxisY, FormatX);

            FormatX.Alignment = StringAlignment.Far;
            g.DrawString(xMax, gridFont, br, PicPtMax.Xint, xAxisY, FormatX);

            FormatX.Alignment = StringAlignment.Center;
            g.DrawString(LabelX, axisTitleFont, br, PicPtMax.Xint / 2 + PicPtMin.Xint, xAxisY, FormatX);


            //y axis text

            g.RotateTransform(180, MatrixOrder.Append);
            g.TranslateTransform(PicPtMin.Xint, PicPtMin.Yint, MatrixOrder.Append);

            float yAxisX = PicPtMin.Xint - axisTitleFont.Size;

            StringFormat FormatY = new StringFormat();
            FormatY.LineAlignment = StringAlignment.Center;
            FormatY.FormatFlags = StringFormatFlags.DirectionVertical;

            //min value at max as text rotated
            FormatY.Alignment = StringAlignment.Near;
            g.DrawString(yMin, gridFont, br, yAxisX, PicPtMax.Yint, FormatY);

            //max value at min as text rotated
            FormatY.Alignment = StringAlignment.Far;
            g.DrawString(yMax, gridFont, br, yAxisX, PicPtMin.Yint, FormatY);

            FormatY.Alignment = StringAlignment.Center;
            g.DrawString(LabelY, axisTitleFont, br, yAxisX, PicPtMin.Yint / 2, FormatY);

            g.RotateTransform(-180, MatrixOrder.Append);
            g.TranslateTransform(PicPtMin.Xint, PicPtMin.Yint, MatrixOrder.Append);
        }
    }
}
