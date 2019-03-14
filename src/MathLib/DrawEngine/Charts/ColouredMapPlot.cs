using MathLib.Data;
using MathLib.DrawEngine.Charts.ColorMaps;
using System.Drawing;

namespace MathLib.DrawEngine.Charts
{
    /// <summary>
    /// Class for map
    /// </summary>
    public class ColouredMapPlot : PlotObject {

        public double Xmin;
        public double Xmax;
        
        public double Ymin;
        public double Ymax;

        private double[,] timeSeries;
        private ColorMap colorCondition;

        public ColouredMapPlot(double[,] timeSeries, Size pictureboxSize, ColorMap colorCondition)
            : base(pictureboxSize, 1) {

            this.timeSeries = timeSeries;
            this.colorCondition = colorCondition;
        }


        public override Bitmap Plot() {

            CalculateChartAreaSize(new DataPoint(timeSeries.GetLength(0), timeSeries.GetLength(1)));

            PlotBitmap = new Bitmap(this.Size.Width, this.Size.Height);
            g = Graphics.FromImage(PlotBitmap);

            Bitmap mapBitmap = new Bitmap(timeSeries.GetLength(0), timeSeries.GetLength(1));

            Graphics gMap = Graphics.FromImage(mapBitmap);

            if (timeSeries.Length < 1)
                return null;

            g.FillRectangle(new SolidBrush(Color.White), 0, 0, this.Size.Width, this.Size.Height);

            for (int i = 0; i < timeSeries.GetLength(0); i++)
                for (int j = 0; j < timeSeries.GetLength(1); j++) {
                    Brush brush = new SolidBrush(colorCondition.GetColor(timeSeries[i, j]));
                    gMap.FillRectangle(brush, new Rectangle(i, j, 1, 1));
                }

            DrawGrid();

            int crossX = (int)(PicPtMin.X + gridPen.Width / 2);
            int crossY = (int)(PicPtMin.Y);

            g.DrawImage(mapBitmap, new Rectangle(crossX, 0, this.Size.Width - crossX, (int)PicPtMin.Y));

            g.Dispose();

            return PlotBitmap;
        }


        protected override void DrawGrid() {
            SetAxisValues(
                GetAxisValue(Xmin),
                GetAxisValue(Xmax),
                GetAxisValue(Ymin),
                GetAxisValue(Ymax)
            );
        }
    }

}
