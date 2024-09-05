using ChaosSoft.NumericalMethods.Equations;

namespace UnitTests.DataGenerators.OdeSys
{
    public class LorenzAttractor : SystemBase
    {
        protected const int EqCount = 3;

        private double x, y, z;

        public LorenzAttractor() : this(10, 28, 8d / 3d)
        {
        }

        public LorenzAttractor(double sg, double r, double b) : base(EqCount)
        {
            Sg = sg;
            R = r;
            B = b;
        }

        public double Sg { get; private set; }

        public double R { get; private set; }

        public double B { get; private set; }

        public override string Name => "Lorenz system";

        public override void SetParameters(params double[] parameters)
        {
            Sg = parameters[0];
            R = parameters[1];
            B = parameters[2];
        }

        public override void GetDerivatives(double[,] current, double[,] derivs)
        {
            x = current[0, 0];
            y = current[0, 1];
            z = current[0, 2];

            derivs[0, 0] = Sg * (y - x);
            derivs[0, 1] = x * (R - z) - y;
            derivs[0, 2] = x * y - B * z;
        }

        public override void SetInitialConditions(double[,] current)
        {
            for (int i = 0; i < Count; i++)
            {
                current[0, i] = 1;
            }
        }

        public override string ToString() => string.Empty;

        public override string ToFileName() => string.Empty;
    }
}
