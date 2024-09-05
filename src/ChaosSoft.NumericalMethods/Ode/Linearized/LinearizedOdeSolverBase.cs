using ChaosSoft.NumericalMethods.Algebra;

namespace ChaosSoft.NumericalMethods.Ode.Linearized;

/// <summary>
/// Provides with abstraction for equations solvers.
/// </summary>
public abstract class LinearizedOdeSolverBase : OdeSolverBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinearizedOdeSolverBase"/> class
    /// with system of equations and solution time step.
    /// </summary>
    /// <param name="equations">system of equations to solve</param>
    /// <param name="dt">solution step</param>
    public LinearizedOdeSolverBase(ILinearizedOdeSys equations, double dt) : base(equations, dt)
    {
        LinearizedOdeSys = equations;
        Linearization = new double[equations.N, equations.N];
    }

    /// <summary>
    /// Gets current solution linearization matrix.
    /// </summary>
    public double[,] Linearization { get; }

    /// <summary>
    /// Gets systems of equations.
    /// </summary>
    protected ILinearizedOdeSys LinearizedOdeSys { get; }

    /// <summary>
    /// Sets initial conditions for the solution.
    /// </summary>
    /// <param name="initialConditions">matrix of initial conditions for linearization</param>
    public void SetLinearInitialConditions(double[,] initialConditions)
    {
        for (int i = 0; i < OdeSys.N; i++)
        {
            for (int j = 0; j < OdeSys.N; j++)
            {
                Linearization[i, j] = initialConditions[i, j];
            }
        }
    }

    /// <summary>
    /// Checks if the solutions contains any NaN of Infinity.
    /// </summary>
    /// <returns>true - if solution has NaN or Infinity members, otherwise - false</returns>
    public override bool IsSolutionDecayed()
    {
        if (base.IsSolutionDecayed())
        {
            return true;
        }

        foreach(double num in Linearization)
        {
            if (Numbers.IsNanOrInfinity(num))
            {
                return true;
            }
        }

        return false;
    }
}