using System;
using ChaosSoft.Core.DataUtils;
using ChaosSoft.Core;
using ChaosSoft.NumericalMethods.Ode.Linearized;
using ChaosSoft.NumericalMethods.QrDecomposition;
using System.Text;

namespace ChaosSoft.NumericalMethods.Lyapunov;

/// <summary>
/// Lyapunov exponents spectrum by Benettin.
/// </summary>
public sealed class LeSpecBenettin : IHasDescription
{
    private readonly LinearizedOdeSolverBase _solver;
    private readonly int _eqCount;
    private readonly long _iterations;
    private readonly IQrDecomposition _qrDecomposition;
    private readonly int _qrInterval;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeSpecBenettin"/> class for
    /// initialized instance of solver, solution iterations and QR decomposition.
    /// </summary>
    /// <param name="solver">solver instance</param>
    /// <param name="iterations">number of iterations to solve</param>
    /// <param name="qrDecomposition">instance of QR decomposition</param>
    /// <param name="qrInterval">steps between QR decomposition executions</param>
    public LeSpecBenettin(LinearizedOdeSolverBase solver, long iterations, IQrDecomposition qrDecomposition, int qrInterval)
    {
        _solver = solver;
        _eqCount = solver.OdeSys.EqCount;
        _iterations = iterations;
        _qrInterval = qrInterval;
        _qrDecomposition = qrDecomposition;
        
        Result = new double[_eqCount];
    }

    /// <summary>
    /// Gets lyapunov exponents spectrum.
    /// </summary>
    public double[] Result { get; }

    /// <summary>
    /// Gets help on the method and its params
    /// </summary>
    /// <returns></returns>
    public string Description => "Lyapunov exponents spectrum by Benettin";

    /// <summary>
    /// 
    /// </summary>
    public void Calculate()
    {
        double[] rMatrix;       //normalized vector (triangular matrix)
        double[] leSpecAccum = new double[_eqCount];

        for (int i = 0; i < _iterations; i += _qrInterval)
        {
            for (int j = 0; j < _qrInterval; j++)
            {
                _solver.NextStep();
            }

            if (_solver.IsSolutionDecayed())
            {
                Vector.FillWith(Result, double.NaN);
                return;
            }

            rMatrix = _qrDecomposition.Perform(_solver.Linearization);

            // update vector magnitudes 
            for (int k = 0; k < _eqCount; k++)
            {
                if (rMatrix[k] > 0)
                {
                    leSpecAccum[k] += Math.Log(rMatrix[k]);
                }
            }
        }

        for (int i = 0; i < _eqCount; i++)
        {
            Result[i] = leSpecAccum[i] / _solver.T;
        }
    }

    /// <summary>
    /// Gets method setup info (parameters values).
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        new StringBuilder()
        .AppendLine("LES by Benettin")
        .AppendLine($" - system     : {_solver.OdeSys}")
        .AppendLine($" - iterations : {_iterations:#,#}")
        .AppendLine($" - QR         : {_qrDecomposition.GetType().Name} (each {_qrInterval} step(s))")
        .ToString();
}