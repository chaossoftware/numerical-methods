using System.Drawing;

namespace ChaosSoft.Core.DrawEngine.Charts.ColorMaps
{
    public interface IColorMap
    {
        Color GetColor(double value);
    }
}
