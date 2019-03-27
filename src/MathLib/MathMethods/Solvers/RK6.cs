using System;

namespace MathLib.MathMethods.Solvers
{
    /// <summary>
    /// 4th ordered Runge-Kutta
    /// </summary>
    public class RK6 : EquationsSolver
    {
        private int n;
        private int nn;
        private double stepSize;                //step size
        private double stepDiv2;
        private double stepDiv6;

        private double[,] x, dxdt, A, B, C, D;  //arrays for solving

        /// <summary>
        /// Solvers
        /// </summary>
        /// <param name="numberOfEquations">Number of equations</param>
        /// <param name="stepSize">Step size</param>
        public RK6(SystemEquations equations, double stepSize)
            : base(equations)
        {
            n = equations.EquationsCount;
            nn = equations.TotalEquationsCount;
            this.stepSize = stepSize;

            x = new double[nn, n];
            dxdt = new double[nn, n];
            A = new double[nn, n];
            B = new double[nn, n];
            C = new double[nn, n];
            D = new double[nn, n];
            stepDiv2 = stepSize / 2.0;
            stepDiv6 = stepSize / 6.0;
        }

        public override void NexStep()
        {
            equations.Derivatives(Solution, dxdt);

            Array.Copy(dxdt, A, A.Length);

            for (int i = 0; i < nn; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    x[i, j] = Solution[i, j] + stepDiv2 * A[i, j];
                }
            }

            equations.Derivatives(x, dxdt);

            Array.Copy(dxdt, B, B.Length);

            for (int i = 0; i < nn; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    x[i, j] = Solution[i, j] + stepDiv2 * B[i, j];
                }
            }

            equations.Derivatives(x, dxdt);

            Array.Copy(dxdt, C, C.Length);

            for (int i = 0; i < nn; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    x[i, j] = Solution[i, j] + stepSize * C[i, j];
                }
            }

            equations.Derivatives(x, dxdt);

            Array.Copy(dxdt, D, D.Length);

            for (int i = 0; i < nn; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Solution[i, j] += stepDiv6 *
                        (A[i, j] + D[i, j] + 2.0 * (B[i, j] + C[i, j]));
                }
            }
        }
    }
}