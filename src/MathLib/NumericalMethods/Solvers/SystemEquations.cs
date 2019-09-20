
namespace MathLib.NumericalMethods.Solvers
{
    public abstract class SystemEquations
    {
        protected bool linearized = false;

        public SystemEquations(bool linearized)
        {
            this.linearized = linearized;
        }

        public abstract string Name { get; }

        /// <summary>
        /// Count of original system equations
        /// </summary>
        public int EquationsCount { get; protected set; }  //num of equations

        /// <summary>
        /// total count of equations (linear + nonlinear)
        /// </summary>
        public int TotalEquationsCount { get; protected set; } = 1;

        public EquationsSolver Solver { get; set; }

        /// <summary>
        /// System Equations (non-linear and linearized)
        /// </summary>
        /// <param name="x">solution array</param>
        /// <param name="dxdt">derivatives array</param>
        /// <returns></returns>
        public abstract double[,] Derivatives(double[,] x, double[,] dxdt);

        /// <summary>
        /// Initial conditions for the system
        /// </summary>
        /// <param name="x">solution array</param>
        public abstract void Init(double[,] x);

        public abstract string ToFileName();

        public abstract string GetInfoShort();

        public abstract string GetInfoFull();
    }
}
