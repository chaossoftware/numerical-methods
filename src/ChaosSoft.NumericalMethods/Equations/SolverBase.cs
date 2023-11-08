namespace ChaosSoft.NumericalMethods.Equations
{
    /// <summary>
    /// Provides with abstraction for equations solvers.
    /// </summary>
    public abstract class SolverBase
    {
        private readonly int _eqRows;
        private readonly int _eqCount;
        private readonly double[,] _solution;

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
            _eqRows = Equations.Rows;
            _eqCount = Equations.Count;
            _solution = new double[_eqRows, _eqCount];
            Equations.SetInitialConditions(_solution);
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
        public double[,] Solution => _solution;

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
            for (int i = 0; i < _eqRows; i++)
            {
                for (int j = 0; j < _eqCount; j++)
                {
                    _solution[i, j] = conditions[i, j];
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

        /// <summary>
        /// Checks if the solutions contains any NaN of Infinity.
        /// </summary>
        /// <returns>true - if solution has NaN or Infinity members, otherwise - false</returns>
        public bool IsSolutionDecayed()
        {
            for (int i = 0; i < _eqRows; i++)
            {
                for (int j = 0; j < _eqCount; j++)
                {
                    if (IsNanOrInfinity(_solution[i, j]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsNanOrInfinity(double value) =>
            double.IsInfinity(value) || double.IsNaN(value);
    }
}