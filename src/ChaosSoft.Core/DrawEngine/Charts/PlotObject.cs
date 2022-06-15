using ChaosSoft.Core.Data;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace ChaosSoft.Core.DrawEngine.Charts {
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

        protected float gridFontSizePx;

        /// <summary>
        /// Base constructor for plot object. initializes necessary objects to create plot
        /// </summary>
        /// <param name="pBox">Picture box control to display plot</param>
        /// <param name="thickness">thickness of plot lines</param>
        protected PlotObject(Size bitmapSize, float thickness)
        {
            Size = bitmapSize;
            Thickness = thickness;

            LabelX = "x";
            LabelY = "y";

            float gridFontSize = 10, titleFontSize = 11, gridThickness = 2;

            if (HasSmallSize)
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

        protected bool HasSmallSize => Size.Width < 216 || Size.Height < 161;

        public bool NeedToDrawGrid { get; set; } = true;

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
            PlotBitmap = new Bitmap(Size.Width, Size.Height);
            g = Graphics.FromImage(PlotBitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.FillRectangle(bgBrush, 0, 0, Size.Width, Size.Height);

            gridFontSizePx = gridFont.SizeInPoints / 72 * g.DpiX;
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

            if (HasSmallSize)
            {
                decimalPlaces = 1;
            }

            return string.Format("{0:F" + decimalPlaces + "}", value).TrimEnd('0').TrimEnd('.');
        }

        protected void CalculateChartAreaSize(DataPoint amplitude)
        {
            var minOffset = HasSmallSize ? 18 : 25;

            //set plot default area size

            var axisOffset = NeedToDrawGrid ?
                Math.Max(Math.Min(Size.Height, Size.Width) * 0.1, minOffset) :
                0;

            PicPtMin = new DataPoint(axisOffset, Size.Height - axisOffset);
            PicPtMax = new DataPoint(Size.Width, 0);
            PicPtCoeff = new DataPoint((PicPtMax.X - PicPtMin.X) / amplitude.X, (PicPtMin.Y - PicPtMax.Y - Thickness) / amplitude.Y);
        }

        protected void SetAxisValues(string xMin, string xMax, string yMin, string yMax)
        {
            g.DrawLine(gridPen, (float)PicPtMin.X, (float)PicPtMin.Y, (float)PicPtMin.X, (float)PicPtMax.Y);
            g.DrawLine(gridPen, (float)PicPtMin.X, (float)PicPtMin.Y, (float)PicPtMax.X, (float)PicPtMin.Y);
            
            // x axis text
            float xAxisY = Size.Height - axisTitleFont.Size;

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

            g.DrawString("No data to plot.", gridFont, txtBrush, Size.Width / 2, Size.Height / 2);
        }
    }
}
