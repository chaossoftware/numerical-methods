namespace UnitTests.DataGenerators.OdeSys
{
    public class HenonMapLinearized : HenonMap
    {
        private double xl, yl;

        public HenonMapLinearized() : base()
        {
            Rows += EqCount;
        }

        public HenonMapLinearized(double a, double b) : base(a, b)
        {
            Rows += EqCount;
        }

        public override string Name => "Hénon map (linearized)";

        public override void GetDerivatives(double[,] current, double[,] derivs)
        {
            base.GetDerivatives(current, derivs);

            double min2AX = -2.0 * A * current[0, 0]; // speed optimization

            //Linearized Henon map equations:
            for (int i = 0; i < Count; i++)
            {
                xl = current[1, i];
                yl = current[2, i];

                derivs[1, i] = min2AX * xl + B * yl;
                derivs[2, i] = xl;
            }
        }

        public override void SetInitialConditions(double[,] current)
        {
            base.SetInitialConditions(current);

            for (int i = 0; i < Count; i++)
            {
                current[i + 1, i] = 1;
            }
        }
    }
}
