using ChaosSoft.Core.IO;
using System.Globalization;
using System.Text;

namespace ChaosSoft.Core.Transform
{
    public class Model3D
    {
        /// <summary>
        /// Create file with 3D model in PLY format
        /// </summary>
        /// <param name="filePath">output 3d model file name</param>
        /// <param name="xt">array of points X coordinates</param>
        /// <param name="yt">array of points Y coordinates</param>
        /// <param name="zt">array of points Z coordinates</param>
        public static void Create3dPlyModelFile(string filePath, double[] xt, double[] yt, double[] zt)
        {
            long pts = xt.Length;

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
                model3D.AppendFormat(CultureInfo.InvariantCulture, "{0:G14} {1:G14} {2:G14}\n", xt[t], yt[t], zt[t]);
            }

            DataWriter.CreateDataFile(filePath, model3D.ToString());
        }

        /// <summary>
        /// Create file with 3D model in 3DA format
        /// </summary>
        /// <param name="filePath">output 3d model file name</param>
        /// <param name="xt">array of points X coordinates</param>
        /// <param name="yt">array of points Y coordinates</param>
        /// <param name="zt">array of points Z coordinates</param>
        public static void Create3daModelFile(string filePath, double[] xt, double[] yt, double[] zt)
        {

            long pts = xt.Length;
            var model3D = new StringBuilder();

            for (int t = 0; t < pts; t++)
            {
                model3D.AppendFormat(CultureInfo.InvariantCulture, "{0:G14} {1:G14} {2:G14}\n", xt[t], yt[t], zt[t]);
            }

            DataWriter.CreateDataFile(filePath, model3D.ToString());
        }
    }
}
