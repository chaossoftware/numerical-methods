using System;
using System.Collections.Generic;
using System.Linq;

namespace ChaosSoft.Core.Algebra
{
    /// <summary>
    /// Provides with methods operating with vectors.
    /// </summary>
    public static class Vectors
    {
        /// <summary>
        /// Gets vector's norm. Vector [x,y,z] norm is Sqrt(x*x + y*y + z*z).
        /// </summary>
        /// <param name="vector">input vector</param>
        /// <returns>norm value</returns>
        public static double Norm(params double[] vector) =>
            Math.Sqrt(vector.Sum(v => v * v));

        /// <summary>
        /// Gets vector's norm. Vector [x,y,z] norm is Sqrt(x*x + y*y + z*z).
        /// </summary>
        /// <param name="vector">input vector</param>
        /// <returns>norm value</returns>
        public static double Norm(IEnumerable<double> vector) =>
            Math.Sqrt(vector.Sum(v => v * v));

        /// <summary>
        /// Gets a value that does not change during the transfer and rotation of the coordinate axes, 
        /// but changes its sign when replacing the direction of one axis to the opposite
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double Determinant(double x1, double y1, double x2, double y2) =>
            x1 * y2 - y1 * x2;

        public static double DotProduct(double x1, double y1, double x2, double y2) =>
            x1 * x2 + y1 * y2;

        /// <summary>
        /// Gets angle between two vector in radians (see <see cref="Math.Atan2"/>).
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double Angle(double x1, double y1, double x2, double y2) =>
            Math.Atan2(Determinant(x1, y1, x2, y2), DotProduct(x1, y1, x2, y2));

        public static double Module(double value1, double value2) =>
            Math.Sqrt(value1 * value1 + value2 * value2);

        /// <summary>
        /// The Cos theta or cos θ is the ratio of the adjacent side to the hypotenuse.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double CosTheta(double x1, double y1, double x2, double y2)
        {
            double v1 = Module(x1, y1);
            double v2 = Module(x2, y2);

            if (v1 == 0)
            {
                v1 = 1E-5;
            }

            if (v2 == 0)
            {
                v2 = 1E-5;
            }

            double cosTeta = (x1 * x2 + y1 * y2) / (v1 * v2);

            return cosTeta;
        }
    }
}
