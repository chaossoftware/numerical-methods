using System.Linq;
using System.Text;
using ChaosSoft.Core.IO;

namespace ChaosSoft.Core.NumericalMethods.Lyapunov
{
    public class LyapunovSpectrum
    {
        public LyapunovSpectrum(int n)
        {
            Spectrum = new double[n];
        }

        public double[] Spectrum { get; set; }

        /// <summary>
        /// Gets KS entropy
        /// </summary>
        public double Eks =>
            Spectrum[0] > 0 ?
            Spectrum.Where(le => le > 0).Sum() :
            Spectrum[0];

        /// <summary>
        /// Phase volume contraction
        /// </summary>
        public double Pvc => Spectrum.Sum();

        /// <summary>
        /// Gets Kaplan-Yorke dimension
        /// </summary>
        public double Dky
        {
            get
            {
                if (Spectrum[0] <= 0)
                {
                    return 0d;
                }

                var lsum = 0d;
                var i = 0;

                do
                {
                    lsum += Spectrum[i];
                    i++;
                }
                while ((lsum > 0) && (i < Spectrum.Length));

                return i - lsum / Spectrum[i - 1];
            }
        }

        public double[,] SpectrumInTime { get; set; }

        public override string ToString() =>
            new StringBuilder().Append("LE Spectrum: ")
            .AppendLine(string.Join("; ", Spectrum.Select(le => NumFormat.ToShort(le))))
            .AppendLine($"Dky: {NumFormat.ToShort(Dky)}")
            .AppendLine($"Eks: {NumFormat.ToShort(Eks)}")
            .AppendLine($"PVC: {NumFormat.ToShort(Pvc)}")
            .ToString();
    }
}
