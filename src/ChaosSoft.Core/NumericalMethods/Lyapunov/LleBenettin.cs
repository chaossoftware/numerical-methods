using ChaosSoft.Core.Extensions;
using ChaosSoft.Core.IO;
using ChaosSoft.Core.NumericalMethods.Equations;
using System;
using System.Text;

namespace ChaosSoft.Core.NumericalMethods.Lyapunov
{
    public class LleBenettin : IDescribable
    {
        private readonly int _eqCount;
        private readonly long _iterations;
        private readonly SolverBase _solver1;
        private readonly SolverBase _solver2;
        private readonly SystemBase _equations;

        private double lsum;
        private long nl;

        public LleBenettin(SystemBase equations, Type solverType, double step, long iterations)
        {
            _equations = equations;
            _solver1 = Activator.CreateInstance(solverType, equations, step) as SolverBase;
            _solver2 = Activator.CreateInstance(solverType, equations, step) as SolverBase;
            
            _eqCount = equations.Count;
            _iterations = iterations;
        }

        public double Result { get; private set; }

        public void Calculate()
        {
            _solver2.Solution[0, 0] += _solver1.Solution[0, 0] + 1e-8;

            for (int i = 0; i < _iterations; i++)
            {
                MakeIteration();
            }
        }

        public override string ToString() =>
            new StringBuilder()
            .AppendLine("Largest Lyapunov exponent by Benettin\n")
            .AppendLine($"system:     {_equations}")
            .AppendLine($"iterations: {_iterations:#,#}")
            .ToString();

        public string GetHelp()
        {
            throw new NotImplementedException();
        }

        public string GetResultAsString() =>
            NumFormatter.ToShort(Result);

        public void MakeIteration()
        {
            _solver1.NexStep();
            _solver2.NexStep();

            double dl2 = 0;

            for (int _i = 0; _i < _eqCount; _i++)
            {
                dl2 += FastMath.Pow2(_solver2.Solution[0, _i] - _solver1.Solution[0, _i]);
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
