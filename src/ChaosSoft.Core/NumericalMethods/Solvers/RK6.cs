using System;

namespace ChaosSoft.Core.NumericalMethods.Solvers
{
    /// <summary>
    /// 4th ordered Runge-Kutta
    /// </summary>
    public class RK6 : EquationsSolver
    {
        private readonly int _n;
        private readonly int _nn;
        private readonly double _stepSize;                //step size
        private readonly double _stepDiv2;
        private readonly double _stepDiv6;

        private readonly double[,] _x, _dxdt, _a, _b, _c, _d;  //arrays for solving

        /// <summary>
        /// Solvers
        /// </summary>
        /// <param name="numberOfEquations">Number of equations</param>
        /// <param name="stepSize">Step size</param>
        public RK6(SystemEquations equations, double stepSize)
            : base(equations)
        {
            _n = equations.EquationsCount;
            _nn = equations.TotalEquationsCount;
            _stepSize = stepSize;

            _x = new double[_nn, _n];
            _dxdt = new double[_nn, _n];
            _a = new double[_nn, _n];
            _b = new double[_nn, _n];
            _c = new double[_nn, _n];
            _d = new double[_nn, _n];
            _stepDiv2 = stepSize / 2.0;
            _stepDiv6 = stepSize / 6.0;
        }

        public override void NexStep()
        {
            Equations.Derivatives(Solution, _dxdt);

            Array.Copy(_dxdt, _a, _a.Length);

            for (int i = 0; i < _nn; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    _x[i, j] = Solution[i, j] + _stepDiv2 * _a[i, j];
                }
            }

            Equations.Derivatives(_x, _dxdt);

            Array.Copy(_dxdt, _b, _b.Length);

            for (int i = 0; i < _nn; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    _x[i, j] = Solution[i, j] + _stepDiv2 * _b[i, j];
                }
            }

            Equations.Derivatives(_x, _dxdt);

            Array.Copy(_dxdt, _c, _c.Length);

            for (int i = 0; i < _nn; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    _x[i, j] = Solution[i, j] + _stepSize * _c[i, j];
                }
            }

            Equations.Derivatives(_x, _dxdt);

            Array.Copy(_dxdt, _d, _d.Length);

            for (int i = 0; i < _nn; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    Solution[i, j] += _stepDiv6 *
                        (_a[i, j] + _d[i, j] + 2.0 * (_b[i, j] + _c[i, j]));
                }
            }
        }
    }
}