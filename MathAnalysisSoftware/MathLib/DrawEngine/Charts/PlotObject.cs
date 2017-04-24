using System.Drawing;

namespace MathLib.DrawEngine.Charts {
    /// <summary>
    /// Base class for plot object 
    /// </summary>
    public abstract class PlotObject {
        
        public Size BitmapSize; 
        protected Bitmap plotBitmap;                // bitmap to draw plot
        protected Graphics g;

        protected Pen plotPen;
        protected Pen gridPen;
        protected Font gridFont;                    // Font for grid labels
        protected Font axisTitleFont;               // Font for axis titles
        protected SolidBrush br;                    // Brush for grid text

        protected StringFormat FormatR;
        protected StringFormat FormatV;
        protected StringFormat FormatVR;
        protected StringFormat FormatLX;

        public string LabelX = "x";                       // Label for X axis
        public string LabelY = "y";                       // Label for Y axis

        protected const int MinAxisOffset = 25;

        /// <summary>
        /// Base constructor for plot object. initializes necessary objects to create plot
        /// </summary>
        /// <param name="timeSeries">time series to plot</param>
        /// <param name="pBox">Picture box control to display plot</param>
        /// <param name="thickness">thickness of plot lines</param>
        public PlotObject(Size bitmapSize, float thickness) {
            BitmapSize = bitmapSize;

            plotPen = new Pen(Color.SteelBlue, thickness);
            gridPen = new Pen(Color.Black, 2);
            gridFont = new Font(new FontFamily("Cambria Math"), 13f);
            axisTitleFont = new Font(new FontFamily("Cambria Math"), 12f, FontStyle.Bold);
            br = new SolidBrush(Color.Black);

            FormatR = new StringFormat();
            FormatR.Alignment = StringAlignment.Far;
            FormatR.LineAlignment = StringAlignment.Center;

            FormatV = new StringFormat();
            FormatV.FormatFlags = StringFormatFlags.DirectionVertical;
            FormatV.Alignment = StringAlignment.Near;
            FormatV.LineAlignment = StringAlignment.Center;

            FormatVR = new StringFormat();
            FormatVR.FormatFlags = StringFormatFlags.DirectionVertical;
            FormatVR.Alignment = StringAlignment.Far;
            FormatVR.LineAlignment = StringAlignment.Center;
        }


        /// <summary>
        /// Plot chart
        /// </summary>
        public abstract Bitmap Plot();

        /// <summary>
        /// Draw chart grid
        /// </summary>
        protected abstract void DrawGrid();


        /// <summary>
        /// get X coefficient for coordinates to plot Phase portrait
        /// </summary>
        /// <param name="pBoxSize">PictureBox height</param>
        /// <param name="maxVal">maximum absolute value from signal</param>
        /// <returns></returns>
        protected void GetXCoefficient() { }


        /// <summary>
        /// get Y coefficient for coordinates to plot Phase portrait
        /// </summary>
        /// <param name="pBoxSize">PictureBox height</param>
        /// <param name="maxVal">maximum absolute value from signal</param>
        /// <returns></returns>
        protected void GetYCoefficient() { }

        protected string GetAxisValue(double value, int decPlaces = 5)
        {
            return string.Format("{0:F" + decPlaces + "}", value).TrimEnd('0').TrimEnd('.');
        }
    }
}
