using MathLib.Data;
using System;

namespace MathLib.Transform
{
    public class PoincareMap {
        double period;
        double maxt;
        double Max;
        double w2;

        double omega;
        double saveFrom;
        public Timeseries PoincareOut;

        public PoincareMap(double omega, double saveFrom) {
            this.omega = omega;
            this.saveFrom = saveFrom;

            period = 2 * Math.PI / this.omega;
            maxt = Max = -1000;
            w2 = 0;

            PoincareOut = new Timeseries();
        }

        public void nextStep(double t, double val) {
            if (t <= saveFrom + period) {
                if (Max < val) {
                    Max = val;
                    maxt = t;
                }
            }
            else {
                double K = (t - maxt) / period;

                if (drob(K) < 0.0001 || (1 - drob(K)) < 0.0001) {
                    if (w2 != 0)
                        PoincareOut.AddDataPoint(w2, val);
                    w2 = val;
                }
            }
        }

        double drob(double x) {
            return x - (long)(x);
        }
    }
}
