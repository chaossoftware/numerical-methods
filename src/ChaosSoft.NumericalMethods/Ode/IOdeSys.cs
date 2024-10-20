namespace ChaosSoft.NumericalMethods.Ode;

/// <summary>
/// Provides with abstraction for ODE systems implementations.
/// </summary>
public interface IOdeSys
{
    /// <summary>
    /// Gets count of system equations.
    /// </summary>
    int EqCount { get; }

    /// <summary>
    /// Gets derivatives from current solution based on defined equations.
    /// </summary>
    /// <param name="t">current system time</param>
    /// <param name="solution">current solution</param>
    /// <param name="derivs">derivatives</param>
    /// <returns></returns>
    void F(double t, double[] solution, double[] derivs);
}
