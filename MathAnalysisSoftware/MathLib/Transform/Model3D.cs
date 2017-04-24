using MathLib.IO;
using System.Globalization;
using System.Text;

namespace MathLib.Transform
{
    public class Model3D {

        /// <summary>
        /// Create file with 3D model in PLY format
        /// </summary>
        /// <param name="filePath">output 3d model file name</param>
        /// <param name="xt">array of points X coordinates</param>
        /// <param name="yt">array of points Y coordinates</param>
        /// <param name="zt">array of points Z coordinates</param>
        public static void Create3dModelFile(string filePath, double[] xt, double[] yt, double[] zt) {

            long pts = xt.Length;

            StringBuilder model3D = new StringBuilder();
            model3D.AppendLine("ply").AppendLine("format ascii 1.0");
            model3D.AppendLine("comment object: " + "model");
            model3D.AppendFormat("element vertex {0}\n", pts);
            model3D.AppendLine("property double x");
            model3D.AppendLine("property double y");
            model3D.AppendLine("property double z");
            model3D.AppendLine("end_header");

            for (int t = 0; t < pts; t++)
                model3D.AppendFormat(CultureInfo.InvariantCulture, "{0:F10} {1:F10} {2:F10}\n", xt[t], yt[t], zt[t]);

            DataWriter.CreateDataFile(filePath, model3D.ToString());
        }
    }
}
