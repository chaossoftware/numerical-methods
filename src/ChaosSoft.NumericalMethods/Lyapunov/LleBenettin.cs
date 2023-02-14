using ChaosSoft.Core.IO;
using ChaosSoft.NumericalMethods.Equations;
using System;
using System.Text;

namespace ChaosSoft.NumericalMethods.Lyapunov
{
    /// <summary>
    /// LLE by Benettin.
    /// </summary>
    public sealed class LleBenettin : IDescribable
    {
        private readonly int _eqCount;
        private readonly long _iterations;
        private readonly SolverBase _solver1;
        private readonly SolverBase _solver2;
        private readonly SystemBase _equations;

        private double lsum;
        private long nl;

        /// <summary>
        /// Initializes a new instance of the <see cref="LleBenettin"/> class for specific equations system, solver and modelling parameters.
        /// </summary>
        /// <param name="equations">equations to solve</param>
        /// <param name="solverType">type of solver to use</param>
        /// <param name="step">solution step</param>
        /// <param name="iterations">number of iterations to solve</param>
        public LleBenettin(SystemBase equations, Type solverType, double step, long iterations)
        {
            _equations = equations;
            _solver1 = Activator.CreateInstance(solverType, equations, step) as SolverBase;
            _solver2 = Activator.CreateInstance(solverType, equations, step) as SolverBase;
            
            _eqCount = equations.Count;
            _iterations = iterations;
        }

        /// <summary>
        /// Gets largest Lyapunov exponent.
        /// </summary>
        public double Result { get; private set; }

        /// <summary>
        /// Calculates largest lyapunov exponent by solving same equations with slightly different initial conditions.
        /// The result is stored in <see cref="Result"/>.
        /// </summary>
        public void Calculate()
        {
            _solver2.Solution[0, 0] += _solver1.Solution[0, 0] + 1e-8;

            for (int i = 0; i < _iterations; i++)
            {
                MakeIteration();
            }
        }

        /// <summary>
        /// Gets method setup info (parameters values).
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            new StringBuilder()
            .AppendLine("Largest Lyapunov exponent by Benettin\n")
            .AppendLine($"system:     {_equations}")
            .AppendLine($"iterations: {_iterations:#,#}")
            .ToString();

        /// <summary>
        /// Gets help on the method and its params
        /// </summary>
        /// <returns></returns>
        public string GetHelp() =>
            throw new NotImplementedException();

        /// <summary>
        /// Gets result in string representation (LLE).
        /// </summary>
        /// <returns></returns>
        public string GetResultAsString() =>
            Format.General(Result);

        /// <summary>
        /// Makes solving iteration:<br/>
        /// solves next step for pair of systems of equations and tracks orbits divergention
        /// </summary>
        public void MakeIteration()
        {
            _solver1.NexStep();
            _solver2.NexStep();

            double dl2 = 0;

            for (int _i = 0; _i < _eqCount; _i++)
            {
                dl2 += MathHelpers.Pow2(_solver2.Solution[0, _i] - _solver1.Solution[0, _i]);
            }

            if (dl2 > 0)
            {
                double df = 1e16 * dl2;
                double rs = 1 / Math.Sqrt(df);

                for (int _i = 0; _i < _eqCount; _i++)
                {
                    _solver2.Solution[0, _i] =
                        _solver1.Solution[0, _i] + rs * (_solver2.Solution[0, _i] - _solver1.Solution[0, _i]);
                }

                lsum += Math.Log(df);
                nl++;
            }

            Result = 0.5 * lsum / nl / Math.Abs(_solver1.Dt);
        }
    }
}
