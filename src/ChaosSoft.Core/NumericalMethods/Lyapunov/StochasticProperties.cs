using System.Linq;

namespace ChaosSoft.Core.NumericalMethods.Lyapunov
{
    public static class StochasticProperties
    {
        /// <summary>
        /// Gets KS entropy
        /// </summary>
        public static double KSEntropy(double[] leSpectrum) =>
            leSpectrum[0] > 0 ? leSpectrum.Where(le => le > 0).Sum() : 0;

        /// <summary>
        /// Phase volume contraction
        /// </summary>
        public static double PhaseVolumeContractionSpeed(double[] leSpectrum) =>
            leSpectrum.Sum();

        /// <summary>
        /// Gets Kaplan-Yorke dimension
        /// </summary>
        public static double KYDimension(double[] leSpectrum)
        {
            if (leSpectrum[0] <= 0)
            {
                return 0d;
            }

            var lsum = 0d;
            var i = 0;

            do
            {
                lsum += leSpectrum[i];
                i++;
            }
            while ((lsum > 0) && (i < leSpectrum.Length));

            return i - lsum / leSpectrum[i - 1];
        }
    }
}
