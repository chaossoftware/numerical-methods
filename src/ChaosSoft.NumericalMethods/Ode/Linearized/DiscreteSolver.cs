using System;

namespace ChaosSoft.NumericalMethods.Ode.Linearized;

/// <summary>
/// Solver for discrete systems/maps.
/// </summary>
public sealed class DiscreteSolver : LinearizedOdeSolverBase
{
    private readonly double[] _derivs;
    private readonly double[,] _derivsLinear;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscreteSolver"/> class for specified equations and time step.
    /// </summary>
    /// <param name="equations">equations system to solve</param>
    public DiscreteSolver(ILinearizedOdeSys equations) : base(equations, 1)
    {
        _derivs = new double[equations.EqCount];
        _derivsLinear = new double[equations.EqCount, equations.EqCount];
    }


    /// <summary>
    /// Solves next step of system of equations.
    /// </summary>
    public override void NextStep()
    {
        OdeSys.F(T, Solution, _derivs);
        LinearizedOdeSys.F(T, Solution, Linearization, _derivsLinear);
        Array.Copy(_derivs, Solution, Solution.Length);
        Array.Copy(_derivsLinear, Linearization, Linearization.Length);

        TimeIncrement();
    }
}