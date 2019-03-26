using System.Drawing;

namespace MathLib.DrawEngine.Charts.ColorMaps
{
    public interface IColorMap
    {
        Color GetColor(double value);
    }
}
