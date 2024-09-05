using System;
using ChaosSoft.NumericalMethods.Ode.Linearized;

namespace ChaosSoft.NumericalMethods.Ode;

public static class SolversFactory
{
    public enum Solver
    {
        Discrete,
        RK4,
        RK5
    }

    public static OdeSolverBase Get(Solver solver, IOdeSys odeSys, double dt) =>
        solver switch
        {
            Solver.Discrete => new DiscreteSolver(odeSys),
            Solver.RK4 => new RK4(odeSys, dt),
            Solver.RK5 => new RK5(odeSys, dt),
            _ => throw new NotImplementedException($"Unknown solver {solver}"),
        };

    public static LinearizedOdeSolverBase Get(Solver solver, ILinearizedOdeSys odeSys, double dt) =>
        solver switch
        {
            Solver.Discrete => new Linearized.DiscreteSolver(odeSys),
            Solver.RK4 => new Linearized.RK4(odeSys, dt),
            Solver.RK5 => new Linearized.RK5(odeSys, dt),
            _ => throw new NotImplementedException($"Unknown solver {solver}"),
        };
}
