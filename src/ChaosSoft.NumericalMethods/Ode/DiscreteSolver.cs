using System;

namespace ChaosSoft.NumericalMethods.Ode;

/// <summary>
/// Solver for discrete systems/maps.
/// </summary>
public sealed class DiscreteSolver : OdeSolverBase
{
    private readonly double[] _derivs;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteSolver"/> class for specified equations and time step = 1.
    /// </summary>
    /// <param name="equations">equations system to solve</param>
    public DiscreteSolver(IOdeSys equations) : base(equations, 1)
    {
        _derivs = new double[equations.EqCount];
    }

    /// <summary>
    /// Solves next step of system of equations.
    /// </summary>
    public override void NextStep()
    {
        for (int i = 0; i < Dt; i++)
        {
            OdeSys.F(T, Solution, _derivs);
            Array.Copy(_derivs, Solution, Solution.Length);
        }

        TimeIncrement();
    }
}