namespace ChaosSoft.Core.NumericalMethods.Equations
{
    /// <summary>
    /// The 4th ordered Runge-Kutta<para/>
    /// y₁ = y₀ + 1/6 (k₁ + 2k₂ + 2k₃ + k₄) h, where: <para/>
    /// k₁ = f(x₀, y₀)<br/>
    /// k₂ = f(x₀ + 1/2 h, y₀ + 1/2 hk₁)<br/>
    /// k₃ = f(x₀ + 1/2 h, y₀ + 1/2 hk₂)<br/>
    /// k₄ = f(x₀ + h, y₀ + hk₃)<br/>
    /// </summary>
    public class RK4 : SolverBase
    {
        private readonly int _n;
        private readonly int _nn;
        private readonly double _dtDiv2;
        private readonly double _dtDiv6;

        private readonly double[,] _temp, _k1, _k2, _k3, _k4;  //arrays for solving

        /// <summary>
        /// Initializes a new instance of the <see cref="RK4"/> class
        /// with system of equations and solution time step.
        /// </summary>
        /// <param name="equations">system of equations to solve</param>
        /// <param name="dt">solution step</param>
        public RK4(SystemBase equations, double dt) : base(equations, dt)
        {
            _n = equations.Count;
            _nn = equations.Rows;
            _dtDiv2 = dt / 2d;
            _dtDiv6 = dt / 6d;

            _k1 = new double[_nn, _n];
            _k2 = new double[_nn, _n];
            _k3 = new double[_nn, _n];
            _k4 = new double[_nn, _n];
            _temp = new double[_nn, _n];
        }

        /// <summary>
        /// Solves next step of system of equations by RK4 method.
        /// </summary>
        public override void NexStep()
        {
            Equations.GetDerivatives(Solution, _k1);

            for (int i = 0; i < _nn; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    _temp[i, j] = Solution[i, j] + _dtDiv2 * _k1[i, j];
                }
            }

            Equations.GetDerivatives(_temp, _k2);

            for (int i = 0; i < _nn; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    _temp[i, j] = Solution[i, j] + _dtDiv2 * _k2[i, j];
                }
            }

            Equations.GetDerivatives(_temp, _k3);

            for (int i = 0; i < _nn; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    _temp[i, j] = Solution[i, j] + Dt * _k3[i, j];
                }
            }

            Equations.GetDerivatives(_temp, _k4);

            for (int i = 0; i < _nn; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    Solution[i, j] += _dtDiv6 *
                        (_k1[i, j] + _k4[i, j] + 2 * (_k2[i, j] + _k3[i, j]));
                }
            }

            TimeIncrement();
        }
    }
}