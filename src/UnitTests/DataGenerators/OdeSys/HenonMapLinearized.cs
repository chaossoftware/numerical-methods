using ChaosSoft.NumericalMethods.Ode.Linearized;

namespace UnitTests.DataGenerators.OdeSys;

public class HenonMapLinearized : HenonMap, ILinearizedOdeSys
{
    private double xl, yl;

    public HenonMapLinearized() : base()
    {
    }

    public HenonMapLinearized(double a, double b) : base(a, b)
    {
    }

    public void F(double t, double[] solution, double[,] linearization, double[,] derivs)
    {
        double min2AX = -2.0 * a * solution[0]; // speed optimization

        //Linearized Henon map equations:
        for (int i = 0; i < EqCount; i++)
        {
            xl = linearization[0, i];
            yl = linearization[1, i];

            derivs[0, i] = min2AX * xl + b * yl;
            derivs[1, i] = xl;
        }
    }
}