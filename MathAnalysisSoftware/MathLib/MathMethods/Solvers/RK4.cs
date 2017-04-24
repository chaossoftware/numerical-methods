using System;

namespace MathLib.MathMethods.Solvers {

    /// <summary>
    /// 4th ordered Runge-Kutta
    /// </summary>
    public class RK4 : Solver {

        private int N;
        private int NN;
        private double stepSize;                //step size
        private double stepDiv2;
        private double stepDiv6;

        private double[,] x, dxdt, A, B, C, D;  //arrays for solving

        /// <summary>
        /// Solvers
        /// </summary>
        /// <param name="numberOfEquations">Number of equations</param>
        /// <param name="stepSize">Step size</param>
        public RK4(SystemEquations equations, double stepSize)
            : base(equations) {
            
            N = equations.N;
            NN = equations.NN;
            this.stepSize = stepSize;

            x = new double[NN, N];
            dxdt = new double[NN, N];
            A = new double[NN, N];
            B = new double[NN, N];
            C = new double[NN, N];
            D = new double[NN, N];
            stepDiv2 = stepSize / 2.0;
            stepDiv6 = stepSize / 6.0;
        }
        

        public override void NexStep() {

            Equations.Derivs(Solution, dxdt);

            Array.Copy(dxdt, A, A.Length);

            for (int i = 0; i < NN; i++)
                for (int j = 0; j < N; j++)
                    x[i, j] = Solution[i, j] + stepDiv2 * A[i, j];

            Equations.Derivs(x, dxdt);

            Array.Copy(dxdt, B, B.Length); ;

            for (int i = 0; i < NN; i++)
                for (int j = 0; j < N; j++)
                    x[i, j] = Solution[i, j] + stepDiv2 * B[i, j];

            Equations.Derivs(x, dxdt);

            Array.Copy(dxdt, C, C.Length);

            for (int i = 0; i < NN; i++)
                for (int j = 0; j < N; j++)
                    x[i, j] = Solution[i, j] + stepSize * C[i, j];

            Equations.Derivs(x, dxdt);

            Array.Copy(dxdt, D, D.Length);

            for (int i = 0; i < NN; i++)
                for (int j = 0; j < N; j++)
                    Solution[i, j] += stepDiv6 *
                        (A[i, j] + D[i, j] + 2.0 * (B[i, j] + C[i, j]));

            Time += Step;
        }

    }
}