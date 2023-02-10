using System;

namespace ChaosSoft.Core.NumericalMethods.Equations
{
    /// <summary>
    /// Simple solver
    /// </summary>
    public class DiscreteSolver : SolverBase
    {
        private readonly double[,] _derivs;

        public DiscreteSolver(SystemBase equations, double dt) : base(equations, dt)
        {
            _derivs = new double[equations.Rows, equations.Count];
        }

        public DiscreteSolver(SystemBase equations) : this(equations, 1)
        {
        }

        public override void NexStep()
        {
            for (int i = 0; i < Dt; i++)
            {
                Equations.GetDerivatives(Solution, _derivs);
                Array.Copy(_derivs, Solution, Solution.Length);
            }

            TimeIncrement();
        }
    }
}