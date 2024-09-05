namespace ChaosSoft.NumericalMethods.Ode.Linearized;

/// <summary>
/// Provides with abstraction for ODE systems implementations.
/// </summary>
public interface ILinearizedOdeSys : IOdeSys
{
    /// <summary>
    /// Gets derivatives from current solution based on defined equations.
    /// </summary>
    /// <param name="t">current system time</param>
    /// <param name="solution">current solution</param>
    /// <param name="linearization">solution linearization</param>
    /// <param name="derivs">derivatives</param>
    /// <returns></returns>
    void F(double t, double[] solution, double[,] linearization, double[,] derivs);
}
