using ChaosSoft.NumericalMethods.Ode;

namespace UnitTests.DataGenerators.OdeSys;

public class LorenzAttractor : IOdeSys
{
    protected const int EqCount = 3;
    protected double sg;
    protected double r;
    protected double b;

    public LorenzAttractor() : this(10, 28, 8d / 3d)
    {
    }

    public LorenzAttractor(double sg, double r, double b)
    {
        this.sg = sg;
        this.r = r;
        this.b = b;
        N = EqCount;
    }

    public int N { get; }

    public virtual string Name { get; } = "Lorenz system";

    public void SetParameters(params double[] parameters)
    {
        sg = parameters[0];
        r = parameters[1];
        b = parameters[2];
    }

    public void F(double t, double[] solution, double[] derivs)
    {
        double x = solution[0];
        double y = solution[1];
        double z = solution[2];

        derivs[0] = sg * (y - x);
        derivs[1] = x * (r - z) - y;
        derivs[2] = x * y - b * z;
    }
}