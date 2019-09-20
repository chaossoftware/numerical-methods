
namespace MathLib.NumericalMethods.Solvers
{
    /// <summary>
    /// Solvers class
    /// </summary>
    public abstract class EquationsSolver
    {
        protected SystemEquations equations;

        /// <summary>
        /// Equations system solver
        /// </summary>
        /// <param name="numberOfEquations">Number of equations</param>
        public EquationsSolver(SystemEquations equations)
        {
            this.equations = equations;
            Solution = new double[this.equations.TotalEquationsCount, this.equations.EquationsCount];
        }

        public double Time { get; set; }

        public double Step { get; set; } = 1d;

        public double[,] Solution { get; set; }

        /// <summary>
        /// solve equations system step
        /// </summary>
        public abstract void NexStep();

        public void Init()
        {
            this.Time = 0;
            this.equations.Init(this.Solution);
        }
    }
}