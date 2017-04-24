
namespace MathLib.MathMethods.Solvers {
    public abstract class SystemEquations {

        public string SystemName;

        public int N;   //num of equations linear equations
        public int NN = 1;  //tot. num. of equations (linear + nonlinear)
        protected bool Linearized = false;

        public Solver Solver;

        public SystemEquations(bool linearized)
        {
            Linearized = linearized;
        }

        /// <summary>
        /// System Equations (non-linear and linearized)
        /// </summary>
        /// <param name="x">solution array</param>
        /// <param name="dxdt">derivatives array</param>
        /// <returns></returns>
        public abstract double[,] Derivs(double[,] x, double[,] dxdt);


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
