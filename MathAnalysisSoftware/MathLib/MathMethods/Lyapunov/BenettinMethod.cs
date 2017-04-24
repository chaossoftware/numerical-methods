using System;
using System.Globalization;
using System.Text;

namespace MathLib.MathMethods.Lyapunov {

    /// <summary>
    /// Lyapunov Methods class
    /// Methods:
    /// - Calculae Lyapunov exponents
    /// - Calculate Kaplan-Yorke dimension
    /// - Calculate KS Entropy (h)
    /// - Calculate Phase volume compression (d)
    /// </summary>
    public class BenettinMethod {
        private double[] ltot;       //summ array of lyapunov exponents
        public double[] lespec;       //averaged by time array of lyapunov exponents
        private double dky;          //Kaplan-Yorke dimension
        private double h;            //KS entropy
        private double d;            //Phase volume compression
        private double lsum;        //LE sum

        private int n;              //Number of equations
        private int i;              //counter

        /// <summary>
        /// Lyapunov related methods
        /// </summary>
        /// <param name="numberOfEquations">Number of equations</param>
        public BenettinMethod(int numberOfEquations) {
            this.n = numberOfEquations;
            ltot = new double[n];
            lespec = new double[n];
        }


        /// <summary>
        /// <para>Updating array of Lyapunov exponents (not averaged by time).</para>
        /// <para>Result is stored in "Lyapunov.lespec" array</para>
        /// </summary>
        /// <param name="Rmatrix">Normalized vector (triangular matrix)</param>
        public void calculateLE(double[] Rmatrix, double totalTime) {
            // update vector magnitudes 
            for (int i = 0; i < n; i++)
                if (Rmatrix[i] > 0) {
                    ltot[i] += Math.Log(Rmatrix[i]);
                    lespec[i] = ltot[i] / totalTime;
                }
        }


        /// <summary>
        /// <para>Calculatie Kaplan-Yorke dimension from array of Lyapunov exponents</para>
        /// <para>Result stored in "Lyapunov.dky"</para>
        /// <returns>double value</returns>
        /// </summary>
        public double calculateKY() {
            lsum = 0.0;
            i = 0;
            do {
                lsum += lespec[i];
                i++;
            } while ((lsum > 0) && (i < lespec.Length));
            if (lespec[0] > 0)
                dky = i - lsum / lespec[i - 1];
            else dky = 0.0;

            return dky;
        }


        /// <summary>
        /// <para>Calculatie KS entropy</para>
        /// <para>Result stored in "Lyapunov.h" (not averaged by time)</para>
        /// <para>Depends on calculated LE spectra</para>
        /// <returns>double value</returns>
        /// </summary>
        public double calculateKSEntropy() {
            h = 0;

            if (lespec[0] < 0)
                h = lespec[0];
            else
                for (i = 0; i < n; i++)
                    if (lespec[i] > 0)
                        h += lespec[i];
            return h;
        }


        /// <summary>
        /// <para>Calculatie KS entropy and Phase volume compression</para>
        /// <para>Result stored in "Lyapunov.d" (not averaged by time)</para>
        /// <para>Depends on calculated LE spectra</para>
        /// <returns>double value</returns>
        /// </summary>
        public double calculatePVC() {
            d = 0;

            for (i = 0; i < n; i++)
                d += lespec[i];
            return d;
        }


        public BenettinResult GetResults() {
            BenettinResult result = new BenettinResult();
            result.LyapunovSpectrum = lespec;
            result.KSEntropy = calculateKSEntropy();
            result.KYDimension = calculateKY();
            result.PhaseSpaceContraction = calculatePVC();
            return result;
        }
    }


    public class BenettinResult {

        public double[] LyapunovSpectrum { get; set; }
        public double KSEntropy { get; set; }
        public double PhaseSpaceContraction { get; set; }
        public double KYDimension { get; set; }
        public double[,] LeSpectrumInTime { get; set; }

        public string GetInfo() {
            StringBuilder resultBuilder = new StringBuilder();
            resultBuilder.Append("LE Spectrum: ");

            foreach (double le in LyapunovSpectrum) {
                resultBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0:F5} ", le);
            }

            resultBuilder.Append("\n")
                .AppendFormat(CultureInfo.InvariantCulture, "KY dimension: {0:F5}\n", KYDimension)
                .AppendFormat(CultureInfo.InvariantCulture, "KS entropy: {0:F5}\n", KSEntropy)
                .AppendFormat(CultureInfo.InvariantCulture, "Phase volume contraction: {0:F5}\n", PhaseSpaceContraction);

            return resultBuilder.ToString();
        }
    }
}