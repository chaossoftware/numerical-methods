using System;
using ChaosSoft.NumericalMethods.Ode.Linearized;

namespace ChaosSoft.NumericalMethods.Ode;

public static class SolverFactory
{
    public static OdeSolverBase Get(SolverType solver, IOdeSys odeSys, double dt) =>
        solver switch
        {
            SolverType.Discrete => new DiscreteSolver(odeSys),
            SolverType.RK4 => new RK4(odeSys, dt),
            SolverType.RK5 => new RK5(odeSys, dt),
            _ => throw new NotImplementedException($"Unknown solver {solver}"),
        };

    public static LinearizedOdeSolverBase Get(SolverType solver, ILinearizedOdeSys odeSys, double dt) =>
        solver switch
        {
            SolverType.Discrete => new Linearized.DiscreteSolver(odeSys),
            SolverType.RK4 => new Linearized.RK4(odeSys, dt),
            SolverType.RK5 => new Linearized.RK5(odeSys, dt),
            _ => throw new NotImplementedException($"Unknown solver {solver}"),
        };
}
