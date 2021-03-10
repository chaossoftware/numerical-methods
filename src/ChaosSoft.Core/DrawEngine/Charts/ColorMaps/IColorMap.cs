using System.Drawing;

namespace ChaosSoft.Core.DrawEngine.Charts.ColorMaps
{
    public enum ColorMap
    {
        Orange,
        Parula
    }

    public interface IColorMap
    {
        Color GetColor(double value);
    }
}
