using System;
using ChaosSoft.NumericalMethods.Ode.Linearized;

namespace ChaosSoft.NumericalMethods.Ode;
/// <summary>
/// ODE solvers factory.
/// </summary>
public static class SolverFactory
{
    /// <summary>
    /// Gets ODE solver by it's type initialized with given ODE system and solution step.
    /// </summary>
    /// <param name="solver">solver type <see cref="SolverType"/></param>
    /// <param name="odeSys">instance of ODE system</param>
    /// <param name="dt">solution step</param>
    /// <returns><see cref="OdeSolverBase"/> instance</returns>
    /// <exception cref="NotSupportedException"></exception>
    public static OdeSolverBase Get(SolverType solver, IOdeSys odeSys, double dt) =>
        solver switch
        {
            SolverType.Discrete => new DiscreteSolver(odeSys),
            SolverType.RK4 => new RK4(odeSys, dt),
            SolverType.RK5 => new RK5(odeSys, dt),
            _ => throw new NotSupportedException($"Unknown solver {solver}"),
        };

    /// <summary>
    /// Gets linearized ODE solver by it's type initialized with given ODE system with linearization and solution step.
    /// </summary>
    /// <param name="solver">solver type <see cref="SolverType"/></param>
    /// <param name="odeSys">instance of ODE system with linearization</param>
    /// <param name="dt">solution step</param>
    /// <returns><see cref="LinearizedOdeSolverBase"/> instance</returns>
    /// <exception cref="NotSupportedException"></exception>
    public static LinearizedOdeSolverBase Get(SolverType solver, ILinearizedOdeSys odeSys, double dt) =>
        solver switch
        {
            SolverType.Discrete => new Linearized.DiscreteSolver(odeSys),
            SolverType.RK4 => new Linearized.RK4(odeSys, dt),
            SolverType.RK5 => new Linearized.RK5(odeSys, dt),
            _ => throw new NotSupportedException($"Unknown solver {solver}"),
        };
}
