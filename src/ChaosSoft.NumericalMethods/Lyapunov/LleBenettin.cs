using System;
using System.Text;
using ChaosSoft.Core;
using ChaosSoft.NumericalMethods.Algebra;
using ChaosSoft.NumericalMethods.Ode;

namespace ChaosSoft.NumericalMethods.Lyapunov;

/// <summary>
/// LLE by Benettin.
/// </summary>
public sealed class LleBenettin : IHasDescription
{
    private readonly int _eqCount;
    private readonly long _iterations;
    private readonly OdeSolverBase _solver;
    private readonly OdeSolverBase _solverCopy;

    private double lsum;
    private long nl;

    /// <summary>
    /// Initializes a new instance of the <see cref="LleBenettin"/> class for specific equations system, solver and modelling parameters.
    /// </summary>
    /// <param name="solver">solver instance</param>
    /// <param name="solverCopy">second instance of same solver</param>
    /// <param name="iterations">number of iterations to solve</param>
    public LleBenettin(OdeSolverBase solver, OdeSolverBase solverCopy, long iterations)
    {
        _solver = solver;
        _solverCopy = solverCopy;
        _eqCount = solver.OdeSys.EqCount;
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
        // Introduce small difference in intial condtions between two solvers
        MakeInitialConditionsDifference();

        for (int i = 0; i < _iterations; i++)
        {
            MakeIteration();

            if (Numbers.IsNanOrInfinity(Result))
            {
                return;
            }
        }
    }

    /// <summary>
    /// Gets method setup info (parameters values).
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        new StringBuilder()
        .AppendLine("LLE by Benettin")
        .AppendLine($" - system     : {_solver.OdeSys}")
        .AppendLine($" - iterations : {_iterations:#,#}")
        .ToString();

    /// <summary>
    /// Gets help on the method and its params
    /// </summary>
    /// <returns></returns>
    public string Description => "Largest Lyapunov exponent by Benettin";

    /// <summary>
    /// Makes solving iteration:<br/>
    /// solves next step for pair of systems of equations and tracks orbits divergention
    /// </summary>
    public void MakeIteration()
    {
        _solver.NextStep();
        _solverCopy.NextStep();

        double dl2 = 0;

        for (int _i = 0; _i < _eqCount; _i++)
        {
            dl2 += Numbers.FastPow2(_solverCopy.Solution[_i] - _solver.Solution[_i]);
        }

        if (dl2 > 0)
        {
            double df = 1e16 * dl2;
            double rs = 1 / Math.Sqrt(df);

            for (int _i = 0; _i < _eqCount; _i++)
            {
                _solverCopy.Solution[_i] =
                    _solver.Solution[_i] + rs * (_solverCopy.Solution[_i] - _solver.Solution[_i]);
            }

            lsum += Math.Log(df);
            nl++;
        }

        Result = 0.5 * lsum / nl / Math.Abs(_solver.Dt);
    }

    /// <summary>
    /// Introduces small difference in initial conditions between two solvers (1e-8)
    /// </summary>
    public void MakeInitialConditionsDifference() =>
        MakeInitialConditionsDifference(1e-8);

    /// <summary>
    /// Introduces specified small difference in initial conditions between two solvers
    /// </summary>
    /// <param name="epsilon">initial conditions difference</param>
    public void MakeInitialConditionsDifference(double epsilon) =>
        _solverCopy.Solution[0] += _solver.Solution[0] + epsilon;
}
