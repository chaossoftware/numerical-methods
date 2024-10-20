namespace ChaosSoft.NumericalMethods.Ode;

/// <summary>
/// ODE solver types.
/// </summary>
public enum SolverType
{
    /// <summary>
    /// Discrete solver with fixed solution step == 1
    /// </summary>
    Discrete,
    /// <summary>
    /// Runge-Kutta 4th order
    /// </summary>
    RK4,
    /// <summary>
    /// Runge-Kutta 5th order
    /// </summary>
    RK5
}
