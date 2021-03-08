using ChaosSoft.Core.Data;
using MathWorks.MATLAB.NET.Arrays;

namespace ChaosSoft.Core.Transform
{
    public class Fourier
    {
        public static Timeseries GetFourier(double[] timeSeries, double startFreq, double endFreq, double dt, int logScale)
        {
            MatlabEngine.MatlabEngine signalAnalysis = new MatlabEngine.MatlabEngine();

            var mw_signalArray = new MWNumericArray(timeSeries);
            var result = signalAnalysis.GetFourierData(mw_signalArray, dt, logScale);
            var mw_na_result = (MWNumericArray)result;
            var fourierData = (double[,])mw_na_result.ToArray(MWArrayComponent.Real);
            var fourier = new Timeseries();

            for (int i = 0; i < fourierData.GetLength(0); i++)
            {
                double x = fourierData[i, 0];

                if (x >= startFreq && x <= endFreq)
                {
                    fourier.AddDataPoint(x, fourierData[i, 1]);
                }
            }

            return fourier;
        }
    }
}
