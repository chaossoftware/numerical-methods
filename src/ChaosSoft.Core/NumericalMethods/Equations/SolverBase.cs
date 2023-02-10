namespace ChaosSoft.Core.NumericalMethods.Equations
{
    /// <summary>
    /// Base class for ODE solvers.
    /// </summary>
    public abstract class SolverBase
    {
        private double time = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolverBase"/> class
        /// with system of equations and solution time step.
        /// </summary>
        /// <param name="equations">system of equations to solve</param>
        /// <param name="dt">solution step</param>
        public SolverBase(SystemBase equations, double dt)
        {
            Equations = equations;
            Dt = dt;
            Solution = new double[Equations.Rows, Equations.Count];
            Equations.SetInitialConditions(Solution);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SolverBase"/> class
        /// with system of equations and solution time step = 1.
        /// </summary>
        /// <param name="equations">system of equations to solve</param>
        public SolverBase(SystemBase equations) : this(equations, 1)
        {
        }

        /// <summary>
        /// Gets total modelling time.
        /// </summary>
        public double Time => time;

        /// <summary>
        /// Gets solution step.
        /// </summary>
        public double Dt { get; }

        /// <summary>
        /// Gets current solution matrix.
        /// </summary>
        public double[,] Solution { get; }

        /// <summary>
        /// Gets systems of equations.
        /// </summary>
        protected SystemBase Equations { get; }

        /// <summary>
        /// Sets initial conditions for the solution.
        /// </summary>
        /// <param name="conditions">matrix of initial conditions</param>
        public void SetInitialConditions(double[,] conditions)
        {
            for (int i = 0; i < conditions.GetLength(0); i++)
            {
                for (int j = 0; j < conditions.GetLength(1); j++)
                {
                    Solution[i, j] = conditions[i, j];
                }
            }
        }

        /// <summary>
        /// Solves next step of system of equations.
        /// </summary>
        public abstract void NexStep();

        /// <summary>
        /// Increments total modelling time by solution step.
        /// </summary>
        public void TimeIncrement() =>
            time += Dt;
    }
}