using System;

namespace ChaosSoft.Core.NumericalMethods.Solvers
{
    /// <summary>
    /// Simple solver
    /// </summary>
    public class SimpleSolver : EquationsSolver
    {
        private readonly double[,] _dxdt;

        public SimpleSolver(SystemEquations equations) : base(equations)
        {
            _dxdt = new double[equations.TotalEquationsCount, equations.EquationsCount];
        }

        public override void NexStep()
        {
            for (int i = 0; i < Step; i++)
            {
                Equations.Derivatives(Solution, _dxdt);
                Array.Copy(_dxdt, Solution, Solution.Length);
            }

            base.Time += Step;
        }
    }
}