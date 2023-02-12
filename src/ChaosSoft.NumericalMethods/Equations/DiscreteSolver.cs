using System;

namespace ChaosSoft.NumericalMethods.Equations
{
    /// <summary>
    /// Solver for discrete systems/maps.
    /// </summary>
    public class DiscreteSolver : SolverBase
    {
        private readonly double[,] _derivs;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscreteSolver"/> class for specified equations and time step.
        /// </summary>
        /// <param name="equations">equations system to solve</param>
        /// <param name="dt">solution step</param>
        public DiscreteSolver(SystemBase equations, double dt) : base(equations, dt)
        {
            _derivs = new double[equations.Rows, equations.Count];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscreteSolver"/> class for specified equations and time step = 1.
        /// </summary>
        /// <param name="equations">equations system to solve</param>
        public DiscreteSolver(SystemBase equations) : this(equations, 1)
        {
        }

        /// <summary>
        /// Solves next step of system of equations.
        /// </summary>
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