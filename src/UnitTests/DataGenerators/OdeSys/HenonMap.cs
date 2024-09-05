using ChaosSoft.NumericalMethods.Equations;

namespace UnitTests.DataGenerators.OdeSys
{
    public class HenonMap : SystemBase
    {
        protected const int EqCount = 2;
        private double x, y;

        public HenonMap() : this(1.4, 0.3)
        {
        }

        public HenonMap(double a, double b) : base(EqCount)
        {
            A = a;
            B = b;
        }

        public double A { get; private set; }

        public double B { get; private set; }

        public override string Name => "Hénon map";

        public override void SetParameters(params double[] parameters)
        {
            A = parameters[0];
            B = parameters[1];
        }

        public override void GetDerivatives(double[,] current, double[,] derivs)
        {
            x = current[0, 0];
            y = current[0, 1];

            derivs[0, 0] = 1 - A * x * x + y;
            derivs[0, 1] = B * x;
        }

        public override void SetInitialConditions(double[,] current)
        {
            for (int i = 0; i < Count; i++)
            {
                current[0, i] = 0;
            }
        }

        public override string ToString() => string.Empty;

        public override string ToFileName() => string.Empty;
    }
}
