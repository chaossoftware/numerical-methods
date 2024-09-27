using ChaosSoft.Core;
using ChaosSoft.NumericalMethods.Algebra;
using ChaosSoft.NumericalMethods.Ode;
using System;
using System.Text;

namespace ChaosSoft.NumericalMethods.Lyapunov;

/// <summary>
/// LLE by Benettin.
/// </summary>
public sealed class LleBenettin : IHasDescription
{
    private readonly int _eqCount;
    private readonly long _iterations;
    private readonly OdeSolverBase _solver1;
    private readonly OdeSolverBase _solver2;

    private double lsum;
    private long nl;

    /// <summary>
    /// Initializes a new instance of the <see cref="LleBenettin"/> class for specific equations system, solver and modelling parameters.
    /// </summary>
    /// <param name="solver1">solver instance</param>
    /// <param name="solver2">second instance of same solver</param>
    /// <param name="iterations">number of iterations to solve</param>
    public LleBenettin(OdeSolverBase solver1, OdeSolverBase solver2, long iterations)
    {
        _solver1 = solver1;
        _solver2 = solver2;
        _eqCount = solver1.OdeSys.EqCount;
        _iterations = iterations;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LleBenettin"/> class for specific equations system, solver and modelling parameters.
    /// </summary>
    /// <param name="odeSys">equations to solve</param>
    /// <param name="solverType">type of solver to use</param>
    /// <param name="initialConditions">initial conditions to set</param>
    /// <param name="dt">solution step</param>
    /// <param name="iterations">number of iterations to solve</param>
    public LleBenettin(IOdeSys odeSys, SolverType solverType, double[] initialConditions, double dt, long iterations)
    {
        _solver1 = SolverFactory.Get(solverType, odeSys, dt);
        _solver2 = SolverFactory.Get(solverType, odeSys, dt);

        _solver1.SetInitialConditions(0, initialConditions);
        _solver2.SetInitialConditions(0, initialConditions);

        _eqCount = odeSys.EqCount;
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
        .AppendLine("Largest Lyapunov exponent by Benettin\n")
        .AppendLine($"system:     {_solver1.OdeSys}")
        .AppendLine($"iterations: {_iterations:#,#}")
        .ToString();

    /// <summary>
    /// Gets help on the method and its params
    /// </summary>
    /// <returns></returns>
    public string Description =>
        throw new NotImplementedException();

    /// <summary>
    /// Makes solving iteration:<br/>
    /// solves next step for pair of systems of equations and tracks orbits divergention
    /// </summary>
    public void MakeIteration()
    {
        _solver1.NextStep();
        _solver2.NextStep();

        double dl2 = 0;

        for (int _i = 0; _i < _eqCount; _i++)
        {
            dl2 += Numbers.QuickPow2(_solver2.Solution[_i] - _solver1.Solution[_i]);
        }

        if (dl2 > 0)
        {
            double df = 1e16 * dl2;
            double rs = 1 / Math.Sqrt(df);

            for (int _i = 0; _i < _eqCount; _i++)
            {
                _solver2.Solution[_i] =
                    _solver1.Solution[_i] + rs * (_solver2.Solution[_i] - _solver1.Solution[_i]);
            }

            lsum += Math.Log(df);
            nl++;
        }

        Result = 0.5 * lsum / nl / Math.Abs(_solver1.Dt);
    }

    /// <summary>
    /// Introduces small difference in initial conditions between two solvers (1e-8)
    /// </summary>
    public void MakeInitialConditionsDifference() =>
        _solver2.Solution[0] += _solver1.Solution[0] + 1e-8;
}
