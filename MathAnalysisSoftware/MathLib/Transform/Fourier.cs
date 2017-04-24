using MathLib.DrawEngine;
using MathWorks.MATLAB.NET.Arrays;
using MatlabEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Transform
{
    public class Fourier
    {
        public static DataSeries GetFourier(double[] timeSeries, double startFreq, double endFreq, double dt, int logScale)
        {
            MatlabEngine.MatlabEngine signalAnalysis = new MatlabEngine.MatlabEngine();
            MWArray result = null;
            MWNumericArray mw_signalArray = new MWNumericArray(timeSeries);
            result = signalAnalysis.GetFourierData(mw_signalArray, dt, logScale);
            MWNumericArray mw_na_result = (MWNumericArray)result;
            double[,] fourierData = (double[,])mw_na_result.ToArray(MWArrayComponent.Real);

            DataSeries fourier = new DataSeries();
            for (int i = 0; i < fourierData.GetLength(0); i++)
            {
                double x = fourierData[i, 0];
                if (x >= startFreq && x <= endFreq)
                    fourier.AddDataPoint(x, fourierData[i, 1]);
            }

            return fourier;
        }
    }
}
