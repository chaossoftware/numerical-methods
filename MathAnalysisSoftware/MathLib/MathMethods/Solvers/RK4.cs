using System;

namespace MathLib.MathMethods.Solvers
{
    /// <summary>
    /// 4th ordered Runge-Kutta
    /// </summary>
    public class RK4 : EquationsSolver
    {
        private int n;
        private int nn;
        private double stepSize;
        private double stepDiv2;
        private double stepDiv6;

        private double[,] x, dxdt, a, b, c, d;  //arrays for solving

        /// <summary>
        /// Solvers
        /// </summary>
        /// <param name="numberOfEquations">Number of equations</param>
        /// <param name="stepSize">Step size</param>
        public RK4(SystemEquations equations, double stepSize) : base(equations)
        {
            n = equations.EquationsCount;
            nn = equations.TotalEquationsCount;
            this.stepSize = stepSize;

            x = new double[nn, n];
            dxdt = new double[nn, n];
            a = new double[nn, n];
            b = new double[nn, n];
            c = new double[nn, n];
            d = new double[nn, n];
            stepDiv2 = stepSize / 2.0;
            stepDiv6 = stepSize / 6.0;
        }

        public override void NexStep()
        {
            base.equations.Derivatives(base.Solution, dxdt);

            Array.Copy(dxdt, a, a.Length);

            for (int i = 0; i < nn; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    x[i, j] = base.Solution[i, j] + stepDiv2 * a[i, j];
                }
            }

            base.equations.Derivatives(x, dxdt);

            Array.Copy(dxdt, b, b.Length);

            for (int i = 0; i < nn; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    x[i, j] = base.Solution[i, j] + stepDiv2 * b[i, j];
                }
            }

            base.equations.Derivatives(x, dxdt);

            Array.Copy(dxdt, c, c.Length);

            for (int i = 0; i < nn; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    x[i, j] = base.Solution[i, j] + stepSize * c[i, j];
                }
            }

            base.equations.Derivatives(x, dxdt);

            Array.Copy(dxdt, d, d.Length);

            for (int i = 0; i < nn; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    base.Solution[i, j] += stepDiv6 *
                        (a[i, j] + d[i, j] + 2.0 * (b[i, j] + c[i, j]));
                }
            }

            base.Time += Step;
        }
    }
}