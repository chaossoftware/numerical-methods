using MathWorks.MATLAB.NET.Arrays;

namespace ChaosSoft.Core.Transform
{
    public class Wavelet
    {
        public static void BuildWavelet(double[] timeSeries, string tmpFileName, string wName, 
            double tStart, double tEnd, double fStart, double fEnd, double dt, string colMap, double width, double height)
        {
            var matlabEngine = new MatlabEngine.MatlabEngine();
            var mwFolder = new MWCharArray(string.Empty);
            var mwfileName = new MWCharArray(tmpFileName);
            var mwWname = new MWCharArray(wName);
            var mwColMap = new MWCharArray(colMap);
            var mwSignalArray = new MWNumericArray(timeSeries);

            matlabEngine.Get2DWavelet(
                mwSignalArray, mwFolder, mwfileName, mwWname, tStart, tEnd, fStart, fEnd, dt, mwColMap, width, height);
        }
    }
}
