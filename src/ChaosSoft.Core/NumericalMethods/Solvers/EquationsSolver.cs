
namespace ChaosSoft.Core.NumericalMethods.Solvers
{
    /// <summary>
    /// Solvers class
    /// </summary>
    public abstract class EquationsSolver
    {
        /// <summary>
        /// Equations system solver
        /// </summary>
        /// <param name="numberOfEquations">Number of equations</param>
        public EquationsSolver(SystemEquations equations)
        {
            Equations = equations;
            Solution = new double[Equations.TotalEquationsCount, Equations.EquationsCount];
        }

        public double Time { get; set; }

        public double Step { get; set; } = 1d;

        public double[,] Solution { get; set; }

        protected SystemEquations Equations { get; }

        /// <summary>
        /// solve equations system step
        /// </summary>
        public abstract void NexStep();

        public void Init()
        {
            Time = 0;
            Equations.Init(Solution);
        }
    }
}