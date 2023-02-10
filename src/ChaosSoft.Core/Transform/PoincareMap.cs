using ChaosSoft.Core.Data;
using System;

namespace ChaosSoft.Core.Transform
{
    public class PoincareMap
    {
        private double period;
        private double maxt;
        private double max;
        private double w2;
        private double omega;
        private double saveFrom;

        public PoincareMap(double omega, double saveFrom)
        {
            this.omega = omega;
            this.saveFrom = saveFrom;

            period = 2 * Math.PI / this.omega;
            maxt = max = -1000;
            w2 = 0;

            PoincareOut = new DataSeries();
        }

        public DataSeries PoincareOut { get; protected set; }

        public void NextStep(double t, double val)
        {
            if (t <= saveFrom + period)
            {
                if (max < val)
                {
                    max = val;
                    maxt = t;
                }
            }
            else
            {
                double K = (t - maxt) / period;

                if (Drob(K) < 0.0001 || (1 - Drob(K)) < 0.0001)
                {
                    if (w2 != 0)
                    {
                        PoincareOut.AddDataPoint(w2, val);
                    }

                    w2 = val;
                }
            }
        }

        private double Drob(double x) => x - (long)(x);
    }
}
