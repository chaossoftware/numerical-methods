using System;

namespace MathLib.NumericalMethods.Solvers
{
    /// <summary>
    /// Simple solver
    /// </summary>
    public class SimpleSolver : EquationsSolver
    {
        private double[,] dxdt;

        public SimpleSolver(SystemEquations equations) : base(equations)
        {
            dxdt = new double[equations.TotalEquationsCount, equations.EquationsCount];
        }

        public override void NexStep()
        {
            for (int i = 0; i < Step; i++)
            {
                base.equations.Derivatives(base.Solution, dxdt);
                Array.Copy(dxdt, base.Solution, Solution.Length);
            }

            base.Time += Step;
        }
    }
}