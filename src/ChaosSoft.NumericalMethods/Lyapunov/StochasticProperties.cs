using System.Linq;

namespace ChaosSoft.NumericalMethods.Lyapunov
{
    /// <summary>
    /// Provides with methods for attractor stochastic properties calculation based on lyapunov spectrum.
    /// </summary>
    public static class StochasticProperties
    {
        /// <summary>
        /// Gets Kolmogorov-Sinai entropy.
        /// </summary>
        /// <param name="leSpectrum">lyapunov exponents spectrum</param>
        /// <returns></returns>
        public static double KSEntropy(double[] leSpectrum) =>
            leSpectrum[0] > 0 ? leSpectrum.Where(le => le > 0).Sum() : 0;

        /// <summary>
        /// Gets phase volume contraction speed.
        /// </summary>
        /// <param name="leSpectrum">lyapunov exponents spectrum</param>
        /// <returns></returns>
        public static double PhaseVolumeContractionSpeed(double[] leSpectrum) =>
            leSpectrum.Sum();

        /// <summary>
        /// Gets Kaplan-Yorke dimension.
        /// </summary>
        /// <param name="leSpectrum">lyapunov exponents spectrum</param>
        /// <returns></returns>
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
