using ChaosSoft.Core.Data;
using ChaosSoft.NumericalMethods.Algebra;
using System;

namespace ChaosSoft.NumericalMethods.PhaseSpace
{
    /// <summary>
    /// Provides with functionality to build poincare map.
    /// </summary>
    public sealed class PoincareMap
    {
        private readonly double _period;
        private double maxt;
        private double max;
        private double w2;
        private readonly double _omega;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoincareMap"/> class for specific frequency value.
        /// </summary>
        /// <param name="omega">frequency</param>
        public PoincareMap(double omega)
        {
            _omega = omega;
            _period = 2 * Math.PI / _omega;
            maxt = max = double.MinValue;
            w2 = 0;

            MapData = new DataSeries();
        }

        /// <summary>
        /// Gets map data as <see cref="DataSeries"/>
        /// </summary>
        public DataSeries MapData { get; }

        /// <summary>
        /// Performs next section iteration based on current time and value.
        /// </summary>
        /// <param name="t">current time</param>
        /// <param name="value">current value</param>
        public void NextStep(double t, double value)
        {
            if (t <= _period)
            {
                if (max < value)
                {
                    max = value;
                    maxt = t;
                }
            }
            else
            {
                double K = (t - maxt) / _period;

                double frac = Numbers.Fraction(K);

                if (frac < 0.0001 || 1 - frac < 0.0001)
                {
                    if (w2 != 0)
                    {
                        MapData.AddDataPoint(w2, value);
                    }

                    w2 = value;
                }
            }
        }
    }
}
