using System.Globalization;
using System.Text;
using ChaosSoft.Core.IO;

namespace ChaosSoft.NumericalMethods.Transform;

/// <summary>
/// Provides methods to transform series to 3D model files.
/// </summary>
public static class Model3D
{
    /// <summary>
    /// Create file with 3D model in PLY format.<br/>
    /// Accuracy is up to 7 decimal places.
    /// </summary>
    /// <param name="filePath">output 3d model file name</param>
    /// <param name="xt">array of points X coordinates</param>
    /// <param name="yt">array of points Y coordinates</param>
    /// <param name="zt">array of points Z coordinates</param>
    public static void Create3dPlyModelFile(string filePath, double[] xt, double[] yt, double[] zt)
    {
        int pts = xt.Length;

        var model3D = new StringBuilder()
            .AppendLine("ply")
            .AppendLine("format ascii 1.0")
            .AppendLine("comment object: " + "model")
            .AppendFormat("element vertex {0}\n", pts)
            .AppendLine("property double x")
            .AppendLine("property double y")
            .AppendLine("property double z")
            .AppendLine("end_header");

        for (int t = 0; t < pts; t++)
        {
            model3D.AppendFormat(CultureInfo.InvariantCulture, "{0:G7} {1:G7} {2:G7}\n", xt[t], yt[t], zt[t]);
        }

        FileUtils.CreateDataFile(filePath, model3D.ToString());
    }

    /// <summary>
    /// Create file with 3D model in 3DA format (simple list of coordinates (x, y, z)).<br/>
    /// Accuracy is up to 7 decimal places.
    /// </summary>
    /// <param name="filePath">output 3d model file name</param>
    /// <param name="xt">array of points X coordinates</param>
    /// <param name="yt">array of points Y coordinates</param>
    /// <param name="zt">array of points Z coordinates</param>
    public static void Create3daModelFile(string filePath, double[] xt, double[] yt, double[] zt)
    {
        int pts = xt.Length;
        StringBuilder model3D = new StringBuilder();

        for (int t = 0; t < pts; t++)
        {
            model3D.AppendFormat(CultureInfo.InvariantCulture, "{0:G7} {1:G7} {2:G7}\n", xt[t], yt[t], zt[t]);
        }

        FileUtils.CreateDataFile(filePath, model3D.ToString());
    }
}
