using ChaosSoft.NumericalMethods.Ode;

namespace UnitTests.DataGenerators.OdeSys;

public class HenonMap : IOdeSys
{
    protected const int EqCount = 2;
    protected double a;
    protected double b;

    public HenonMap() : this(1.4, 0.3)
    {
    }

    public HenonMap(double a, double b)
    {
        this.a = a;
        this.b = b;
        N = EqCount;
    }

    public int N { get; }

    public string Name { get; } = "Hénon map";

    public void SetParameters(params double[] parameters)
    {
        a = parameters[0];
        b = parameters[1];
    }

    public void F(double t, double[] solution, double[] derivs)
    {
        derivs[0] = 1 - a * solution[0] * solution[0] + solution[1];
        derivs[1] = b * solution[0];
    }
}
