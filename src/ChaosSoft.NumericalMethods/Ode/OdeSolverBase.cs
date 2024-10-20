using ChaosSoft.NumericalMethods.Algebra;

namespace ChaosSoft.NumericalMethods.Ode;

/// <summary>
/// Provides with abstraction for equations solvers.
/// </summary>
public abstract class OdeSolverBase
{
    private double t = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="OdeSolverBase"/> class
    /// with system of equations and solution time step.
    /// </summary>
    /// <param name="equations">system of equations to solve</param>
    /// <param name="dt">solution step</param>
    public OdeSolverBase(IOdeSys equations, double dt)
    {
        OdeSys = equations;
        Dt = dt;
        Solution = new double[equations.EqCount];
    }

    /// <summary>
    /// Gets total modelling time.
    /// </summary>
    public double T => t;

    /// <summary>
    /// Gets solution step.
    /// </summary>
    public double Dt { get; }

    /// <summary>
    /// Gets current solution matrix.
    /// </summary>
    public double[] Solution { get; }

    /// <summary>
    /// Gets systems of equations.
    /// </summary>
    public IOdeSys OdeSys { get; }

    /// <summary>
    /// Sets initial conditions for the solution.
    /// </summary>
    /// <param name="t0">initial time</param>
    /// <param name="conditions">matrix of initial conditions</param>
    public void SetInitialConditions(double t0, double[] conditions)
    {
        t = t0;

        for (int i = 0; i < OdeSys.EqCount; i++)
        {
            Solution[i] = conditions[i];
        }
    }

    /// <summary>
    /// Solves next step of system of equations.
    /// </summary>
    public abstract void NextStep();

    /// <summary>
    /// Increments total modelling time by solution step.
    /// </summary>
    public void TimeIncrement() =>
        t += Dt;

    /// <summary>
    /// Checks if the solutions contains any NaN of Infinity.
    /// </summary>
    /// <returns>true - if solution has NaN or Infinity members, otherwise - false</returns>
    public virtual bool IsSolutionDecayed()
    {
        for (int i = 0; i < OdeSys.EqCount; i++)
        {
            if (Numbers.IsNanOrInfinity(Solution[i]))
            {
                return true;
            }
        }

        return false;
    }
}