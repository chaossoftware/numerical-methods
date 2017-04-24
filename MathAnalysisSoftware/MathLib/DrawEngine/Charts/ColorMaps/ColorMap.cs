using System.Drawing;

namespace MathLib.DrawEngine.Charts.ColorMaps
{
    public interface ColorMap
    {
        Color GetColor(double value);
    }
}
