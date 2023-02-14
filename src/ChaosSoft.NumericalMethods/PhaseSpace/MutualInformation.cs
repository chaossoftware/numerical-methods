using ChaosSoft.Core.Data;
using ChaosSoft.Core.DataUtils;
using System;

namespace ChaosSoft.NumericalMethods.PhaseSpace
{
    /// <summary>
    /// Estimates the time delayed mutual information of the data using fixed mesh of boxes.
    /// </summary>
    public class MutualInformation
    {
        private readonly int _partitions;
        private int corrLength;

        private int[] array;
        private int[] h1;
        private int[] h11;
        private int[,] h2;

        /// <summary>
        /// Initializes a new instance of the <see cref="MutualInformation"/> class for 
        /// specific count of boxes and max time delay.
        /// </summary>
        /// <param name="partitions">number of boxes for the partition</param>
        /// <param name="corrLength">max time delay</param>
        public MutualInformation(int partitions, int corrLength) 
        { 
            _partitions = partitions;
            this.corrLength = corrLength;

            EntropySlope = new DataSeries();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MutualInformation"/> class for 16 boxes in partition and 
        /// max time delay 20
        /// </summary>
        public MutualInformation() : this(16, 20)
        {
        }

        /// <summary>
        /// Gets slope of entropy values by delays.
        /// </summary>
        public DataSeries EntropySlope { get; }

        /// <summary>
        /// Calculates time delayed mutual information of the series.
        /// The result is stored in <see cref="EntropySlope"/>.
        /// </summary>
        /// <param name="series"></param>
        public void Calculate(double[] series)
        {
            double[] data = new double[series.Length];
            Array.Copy(series, data, data.Length);

            Vector.Rescale(data);

            h1 = new int[_partitions];
            h11 = new int[_partitions];
            h2 = new int[_partitions, _partitions];
            array = new int[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                array[i] = data[i] < 1.0 ? (int)(data[i] * _partitions) : _partitions - 1;
            }

            double shannon = GetShannonEntropy(0, data.Length);

            if (corrLength >= data.Length)
            {
                corrLength = data.Length - 1;
            }

            Console.WriteLine($"#shannon = {shannon}");
            Console.WriteLine($"{0} {shannon}");

            for (int tau = 1; tau <= corrLength; tau++)
            {
                double entropy = GetShannonEntropy(tau, data.Length);
                EntropySlope.AddDataPoint(tau, entropy);
                Console.WriteLine($"{tau} {entropy}");
            }
        }

        private double GetShannonEntropy(int t, int length)
        {
            int i, j, hi, hii, count = 0;
            double hpi, hpj, pij, norm;
            double cond_ent = 0.0;

            for (i = 0; i < _partitions; i++)
            {
                h1[i] = h11[i] = 0;

                for (j = 0; j < _partitions; j++)
                {
                    h2[i, j] = 0;
                }
            }

            for (i = 0; i < length; i++)
            {
                if (i >= t)
                {
                    hii = array[i];
                    hi = array[i - t];
                    h1[hi]++;
                    h11[hii]++;
                    h2[hi, hii]++;
                    count++;
                }
            }

            norm = 1.0 / count;

            for (i = 0; i < _partitions; i++)
            {
                hpi = h1[i] * norm;

                if (hpi > 0.0)
                {
                    for (j = 0; j < _partitions; j++)
                    {
                        hpj = h11[j] * norm;

                        if (hpj > 0.0)
                        {
                            pij = h2[i, j] * norm;

                            if (pij > 0.0)
                            {
                                cond_ent += pij * Math.Log(pij / hpj / hpi);
                            }
                        }
                    }
                }
            }

            return cond_ent;
        }
    }
}
