using System.Drawing;

namespace ChaosSoft.Core.DrawEngine.Charts.ColorMaps
{
    public class OrangeColorMap : IColorMap
    {
        private static readonly Color[] Colors;

        private readonly double _step;
        private readonly double _min;

        static OrangeColorMap()
        {
            Colors = new Color[5];
            Colors[0] = ColorTranslator.FromHtml("#fff018");
            Colors[1] = ColorTranslator.FromHtml("#ffc000");
            Colors[2] = ColorTranslator.FromHtml("#ff9000");
            Colors[3] = ColorTranslator.FromHtml("#ff6000");
            Colors[4] = ColorTranslator.FromHtml("#f04830");
        }

        public OrangeColorMap(double min, double max)
        {
            _min = min;
            _step = (max - min) / 5;
        }

        public Color GetColor(double value)
        {
            double current = _min;
            int counter = -1;

            do
            {
                current += _step;
                counter++;
            }
            while (current < value);

            return Colors[counter];
        }
    }
}
