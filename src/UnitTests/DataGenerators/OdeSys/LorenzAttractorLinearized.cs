namespace UnitTests.DataGenerators.OdeSys
{
    public class LorenzAttractorLinearized : LorenzAttractor
    {
        private double xl, yl, zl;

        public LorenzAttractorLinearized() : base()
        {
            Rows += EqCount;
        }

        public LorenzAttractorLinearized(double sigma, double rho, double b) : base(sigma, rho, b)
        {
            Rows += EqCount;
        }

        public override string Name => "Lorenz (linearized)";

        public override void GetDerivatives(double[,] current, double[,] derivs)
        {
            base.GetDerivatives(current, derivs);

            //Linearized Lorenz equations:
            for (int i = 0; i < Count; i++)
            {
                xl = current[1, i];
                yl = current[2, i];
                zl = current[3, i];

                derivs[1, i] = Sg * (yl - xl);
                derivs[2, i] = xl * (R - current[0, 2]) - yl - current[0, 0] * zl;
                derivs[3, i] = xl * current[0, 1] + current[0, 0] * yl - B * zl;
            }
        }

        public override void SetInitialConditions(double[,] current)
        {
            base.SetInitialConditions(current);

            //set diagonal and first n elements to 1
            for (int i = 0; i < Count; i++)
            {
                current[i + 1, i] = 1.0;
            }
        }
    }
}
