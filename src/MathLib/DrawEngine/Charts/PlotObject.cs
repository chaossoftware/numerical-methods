using MathLib.Data;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace MathLib.DrawEngine.Charts {
    /// <summary>
    /// Base class for plot object 
    /// </summary>
    public abstract class PlotObject
    {
        protected Graphics g;

        protected Pen plotPen;
        protected Pen gridPen;
        protected Font gridFont;                    // Font for grid labels
        protected Font axisTitleFont;               // Font for axis titles
        protected SolidBrush txtBrush;                    // Brush for grid text
        protected SolidBrush bgBrush;                    // Brush for grid text

        protected DataPoint PicPtMin;
        protected DataPoint PicPtMax;
        protected DataPoint PicPtCoeff;

        /// <summary>
        /// Base constructor for plot object. initializes necessary objects to create plot
        /// </summary>
        /// <param name="pBox">Picture box control to display plot</param>
        /// <param name="thickness">thickness of plot lines</param>
        protected PlotObject(Size bitmapSize, float thickness)
        {
            this.Size = bitmapSize;
            this.Thickness = thickness;

            this.LabelX = "x";
            this.LabelY = "y";

            float gridFontSize = 10, titleFontSize = 11, gridThickness = 2;

            if (this.HasSmallSize)
            {
                gridFontSize = 8;
                titleFontSize = 9;
                gridThickness = 1;
            }

            plotPen = new Pen(Color.SteelBlue, thickness);
            gridPen = new Pen(Color.Black, gridThickness);
            gridFont = new Font(new FontFamily("Cambria Math"), gridFontSize);
            axisTitleFont = new Font(new FontFamily("Cambria Math"), titleFontSize, FontStyle.Bold);
            txtBrush = new SolidBrush(Color.Black);
            bgBrush = new SolidBrush(Color.White);
        }

        public Size Size { get; set; }

        public string LabelX { get; set; }

        public string LabelY { get; set; }

        protected Bitmap PlotBitmap { get; set; }

        protected float Thickness { get; set; }

        protected bool HasSmallSize => this.Size.Width < 216 || this.Size.Height < 161;

        /// <summary>
        /// Plot chart
        /// </summary>
        public abstract Bitmap Plot();

        /// <summary>
        /// Draw chart grid
        /// </summary>
        protected abstract void DrawGrid();

        protected void PrepareChartArea()
        {
            PlotBitmap = new Bitmap(this.Size.Width, this.Size.Height);
            g = Graphics.FromImage(PlotBitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.FillRectangle(bgBrush, 0, 0, this.Size.Width, this.Size.Height);
        }

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

            if (this.HasSmallSize)
            {
                decimalPlaces = 1;
            }

            return string.Format("{0:F" + decimalPlaces + "}", value).TrimEnd('0').TrimEnd('.');
        }

        protected void CalculateChartAreaSize(DataPoint amplitude)
        {
            var minOffset = this.HasSmallSize ? 18 : 25;

            //set plot default area size
            var axisOffset = Math.Max(Math.Min(this.Size.Height, this.Size.Width) * 0.1, minOffset);
            PicPtMin = new DataPoint(axisOffset, this.Size.Height - axisOffset);
            PicPtMax = new DataPoint(this.Size.Width, 0);
            PicPtCoeff = new DataPoint((PicPtMax.X - PicPtMin.X) / amplitude.X, (PicPtMin.Y - PicPtMax.Y - this.Thickness) / amplitude.Y);
        }

        protected void SetAxisValues(string xMin, string xMax, string yMin, string yMax)
        {
            g.DrawLine(gridPen, (float)PicPtMin.X, (float)PicPtMin.Y, (float)PicPtMin.X, (float)PicPtMax.Y);
            g.DrawLine(gridPen, (float)PicPtMin.X, (float)PicPtMin.Y, (float)PicPtMax.X, (float)PicPtMin.Y);
            
            // x axis text
            float xAxisY = this.Size.Height - axisTitleFont.Size;

            var FormatX = new StringFormat();
            FormatX.LineAlignment = StringAlignment.Center;

            FormatX.Alignment = StringAlignment.Near;
            g.DrawString(xMin, gridFont, txtBrush, (float)PicPtMin.X, xAxisY, FormatX);

            FormatX.Alignment = StringAlignment.Far;
            g.DrawString(xMax, gridFont, txtBrush, (float)PicPtMax.X, xAxisY, FormatX);

            FormatX.Alignment = StringAlignment.Center;
            g.DrawString(LabelX, axisTitleFont, txtBrush, (float)PicPtMax.X / 2 + (float)PicPtMin.X, xAxisY, FormatX);


            //y axis text

            g.RotateTransform(180, MatrixOrder.Append);
            g.TranslateTransform((float)PicPtMin.X, (float)PicPtMin.Y, MatrixOrder.Append);

            float yAxisX = (float)PicPtMin.X - axisTitleFont.Size;

            var FormatY = new StringFormat();
            FormatY.LineAlignment = StringAlignment.Center;
            FormatY.FormatFlags = StringFormatFlags.DirectionVertical;

            //min value at max as text rotated
            FormatY.Alignment = StringAlignment.Near;
            g.DrawString(yMin, gridFont, txtBrush, yAxisX, (float)PicPtMax.Y, FormatY);

            //max value at min as text rotated
            FormatY.Alignment = StringAlignment.Far;
            g.DrawString(yMax, gridFont, txtBrush, yAxisX, (float)PicPtMin.Y, FormatY);

            FormatY.Alignment = StringAlignment.Center;
            g.DrawString(LabelY, axisTitleFont, txtBrush, yAxisX, (float)PicPtMin.Y / 2, FormatY);

            g.RotateTransform(-180, MatrixOrder.Append);
            g.TranslateTransform((float)PicPtMin.X, (float)PicPtMin.Y, MatrixOrder.Append);
        }

        protected void NoDataToPlot()
        {
            var format = new StringFormat();
            format.LineAlignment = StringAlignment.Center;
            format.Alignment = StringAlignment.Center;

            g.DrawString("No data to plot.", gridFont, txtBrush, this.Size.Width / 2, this.Size.Height / 2);
        }
    }
}
