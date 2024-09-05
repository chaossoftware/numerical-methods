namespace ChaosSoft.NumericalMethods.Ode.Linearized;

/// <summary>
/// The 5th ordered Runge-Kutta.
/// </summary>
public sealed class RK5 : LinearizedOdeSolverBase
{
    private readonly int _n;
    private readonly double _dtDiv2, _dtDiv4, _dtDiv8, _dtDiv90;

    private readonly double[,] _tempLin, _kLin1, _kLin2, _kLin3, _kLin4, _kLin5, _kLin6;  //arrays for solving
    private readonly double[] _temp, _k1, _k2, _k3, _k4, _k5, _k6;  //arrays for solving

    /// <summary>
    /// Initializes a new instance of the <see cref="RK5"/> class
    /// with system of equations and solution time step.
    /// </summary>
    /// <param name="equations">system of equations to solve</param>
    /// <param name="dt">solution step</param>
    public RK5(ILinearizedOdeSys equations, double dt) : base(equations, dt)
    {
        _n = equations.N;

        _dtDiv2 = dt / 2d;
        _dtDiv4 = dt / 4d;
        _dtDiv8 = dt / 8d;
        _dtDiv90 = dt / 90d;

        _kLin1 = new double[_n, _n];
        _kLin2 = new double[_n, _n];
        _kLin3 = new double[_n, _n];
        _kLin4 = new double[_n, _n];
        _kLin5 = new double[_n, _n];
        _kLin6 = new double[_n, _n];
        _tempLin = new double[_n, _n];

        _k1 = new double[_n];
        _k2 = new double[_n];
        _k3 = new double[_n];
        _k4 = new double[_n];
        _k5 = new double[_n];
        _k6 = new double[_n];
        _temp = new double[_n];
    }

    /// <summary>
    /// Solves next step of system of equations by RK5 method.<para/>
    /// y₁ = y₀ + 1/90 (7k₁ + 32k₃ + 12k₄ + 32k₅ + 7k₆), where: <para/>
    /// k₁ = f(x₀, y₀)<br/>
    /// k₂ = f(x₀ + 1/4 h, y₀ + 1/4 k₁h)<br/>
    /// k₃ = f(x₀ + 1/4 h, y₀ + 1/8 k₁h + 1/8 k₂h)<br/>
    /// k₄ = f(x₀ + 1/2 h, y₀ - 1/2 k₂h + k₃h)<br/>
    /// k₅ = f(x₀ + 3/4 h, y₀ + 3/16 k₁h + 9/16 k₄h)<br/>
    /// k₆ = f(x₀ + h, y₀ - 3/7 k₁h + 2/7 k₂h + 12/7 k₃h - 12/7 k₄h + 8/7 k₅h)<br/>
    /// </summary>
    public override void NextStep()
    {
        OdeSys.F(T, Solution, _k1);
        LinearizedOdeSys.F(T, Solution, Linearization, _kLin1);

        for (int i = 0; i < _n; i++)
        {
            _temp[i] = Solution[i] + _dtDiv4 * _k1[i];

            for (int j = 0; j < _n; j++)
            {
                _tempLin[i, j] = Linearization[i, j] + _dtDiv4 * _kLin1[i, j];
            }
        }

        OdeSys.F(T, _temp, _k2);
        LinearizedOdeSys.F(T, _temp, _tempLin, _kLin2);

        for (int i = 0; i < _n; i++)
        {
            _temp[i] = Solution[i] + _dtDiv8 * _k1[i] + _dtDiv8 * _k2[i];

            for (int j = 0; j < _n; j++)
            {
                _tempLin[i, j] = Linearization[i, j] + _dtDiv8 * _kLin1[i, j] + _dtDiv8 * _kLin2[i, j];
            }
        }

        OdeSys.F(T, _temp, _k3);
        LinearizedOdeSys.F(T, _temp, _tempLin, _kLin3);

        for (int i = 0; i < _n; i++)
        {
            _temp[i] = Solution[i] - _dtDiv2 * _k2[i] + Dt * _k3[i];

            for (int j = 0; j < _n; j++)
            {
                _tempLin[i, j] = Linearization[i, j] - _dtDiv2 * _kLin2[i, j] + Dt * _kLin3[i, j];
            }
        }

        OdeSys.F(T, _temp, _k4);
        LinearizedOdeSys.F(T, _temp, _tempLin, _kLin4);

        for (int i = 0; i < _n; i++)
        {
            _temp[i] = Solution[i] + Dt * 3 / 16 * _k1[i] + Dt * 9 / 16 * _k4[i];

            for (int j = 0; j < _n; j++)
            {
                _tempLin[i, j] = Linearization[i, j] + Dt * 3 / 16 * _kLin1[i, j] + Dt * 9 / 16 * _kLin4[i, j];
            }
        }

        OdeSys.F(T, _temp, _k5);
        LinearizedOdeSys.F(T, _temp, _tempLin, _kLin5);

        for (int i = 0; i < _n; i++)
        {
            _temp[i] = Solution[i] - Dt * 3 / 7 * _k1[i] + Dt * 2 / 7 * _k2[i]
                    + Dt * 12 / 7 * _k3[i] - Dt * 12 / 7 * _k4[i] + Dt * 8 / 7 * _k5[i];

            for (int j = 0; j < _n; j++)
            {
                _tempLin[i, j] = Linearization[i, j] - Dt * 3 / 7 * _kLin1[i, j] + Dt * 2 / 7 * _kLin2[i, j]
                    + Dt * 12 / 7 * _kLin3[i, j] - Dt * 12 / 7 * _kLin4[i, j] + Dt * 8 / 7 * _kLin5[i, j];
            }
        }

        OdeSys.F(T, _temp, _k6);
        LinearizedOdeSys.F(T, _temp, _tempLin, _kLin6);

        for (int i = 0; i < _n; i++)
        {
            Solution[i] += _dtDiv90 * (7d * (_k1[i] + _k6[i]) + 32d * (_k3[i] + _k5[i]) + 12d * _k4[i]);

            for (int j = 0; j < _n; j++)
            {
                Linearization[i, j] += _dtDiv90 *
                    (7d * (_kLin1[i, j] + _kLin6[i, j]) + 32d * (_kLin3[i, j] + _kLin5[i, j]) + 12d * _kLin4[i, j]);
            }
        }

        TimeIncrement();
    }
}