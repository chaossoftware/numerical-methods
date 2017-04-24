using System;

namespace MathLib.MathMethods.Solvers {

    /// <summary>
    /// Solvers class
    /// </summary>
    public abstract class Solver {

        protected SystemEquations Equations;
        public double Time;
        public double Step = 1d;
        public double[,] Solution;

        /// <summary>
        /// Equations system solver
        /// </summary>
        /// <param name="numberOfEquations">Number of equations</param>
        public Solver(SystemEquations equations) {
            Equations = equations;
            Solution = new double[Equations.NN, Equations.N];
        }
        
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