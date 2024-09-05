namespace ChaosSoft.NumericalMethods.Ode;

/// <summary>
/// The 4th ordered Runge-Kutta.
/// </summary>
public sealed class RK4 : OdeSolverBase
{
    private readonly int _n;
    private readonly double _dtDiv2;
    private readonly double _dtDiv6;

    private readonly double[] _temp, _k1, _k2, _k3, _k4;  //arrays for solving

    /// <summary>
    /// Initializes a new instance of the <see cref="RK4"/> class
    /// with system of equations and solution time step.
    /// </summary>
    /// <param name="equations">system of equations to solve</param>
    /// <param name="dt">solution step</param>
    public RK4(IOdeSys equations, double dt) : base(equations, dt)
    {
        _n = equations.N;
        _dtDiv2 = dt / 2d;
        _dtDiv6 = dt / 6d;

        _k1 = new double[_n];
        _k2 = new double[_n];
        _k3 = new double[_n];
        _k4 = new double[_n];
        _temp = new double[_n];
    }

    /// <summary>
    /// Solves next step of system of equations by RK4 method:<para/>
    /// y₁ = y₀ + 1/6 (k₁ + 2k₂ + 2k₃ + k₄) h, where: <para/>
    /// k₁ = f(x₀, y₀)<br/>
    /// k₂ = f(x₀ + 1/2 h, y₀ + 1/2 hk₁)<br/>
    /// k₃ = f(x₀ + 1/2 h, y₀ + 1/2 hk₂)<br/>
    /// k₄ = f(x₀ + h, y₀ + hk₃)<br/>
    /// </summary>
    public override void NextStep()
    {
        OdeSys.F(T, Solution, _k1);

        for (int i = 0; i < _n; i++)
        {
            _temp[i] = Solution[i] + _dtDiv2 * _k1[i];
        }

        OdeSys.F(T, _temp, _k2);

        for (int i = 0; i < _n; i++)
        {
            _temp[i] = Solution[i] + _dtDiv2 * _k2[i];
        }

        OdeSys.F(T, _temp, _k3);

        for (int i = 0; i < _n; i++)
        {
            _temp[i] = Solution[i] + Dt * _k3[i];
        }

        OdeSys.F(T, _temp, _k4);

        for (int i = 0; i < _n; i++)
        {
            Solution[i] += _dtDiv6 * (_k1[i] + _k4[i] + 2 * (_k2[i] + _k3[i]));
        }

        TimeIncrement();
    }
}