using System;

namespace MathLib.MathMethods.Solvers {

    /// <summary>
    /// Simple solver
    /// </summary>
    public class SimpleSolver : Solver {

        private double[,] dxdt;

        public SimpleSolver(SystemEquations equations)
            : base(equations) {
                dxdt = new double[equations.NN, equations.N];
        }


        public override void NexStep() {
            for (int i = 0; i < Step; i++)
            {
                Equations.Derivs(Solution, dxdt);
                Array.Copy(dxdt, Solution, Solution.Length);
                
            }
            Time += Step;
        }
    }
}