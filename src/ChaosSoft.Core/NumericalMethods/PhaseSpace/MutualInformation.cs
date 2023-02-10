using ChaosSoft.Core.Data;
using ChaosSoft.Core.Extensions;
using System;

namespace ChaosSoft.Core.NumericalMethods.PhaseSpace
{
    public class MutualInformation
    {
        private readonly int _partitions;
        private int corrLength;

        private int[] array;
        private int[] h1;
        private int[] h11;
        private int[,] h2;

        public MutualInformation(int partitions, int corrlength) 
        { 
            _partitions = partitions;
            corrLength = corrlength;

            Slope = new DataSeries();
        }

        public MutualInformation() : this(16, 20)
        {
        }

        public DataSeries Slope { get; }

        public void Calculate(double[] timeSeries)
        {
            double[] series = new double[timeSeries.Length];
            Array.Copy(timeSeries, series, series.Length);

            Arrays.Rescale(series);

            h1 = new int[_partitions];
            h11 = new int[_partitions];
            h2 = new int[_partitions, _partitions];
            array = new int[series.Length];

            for (int i = 0; i < series.Length; i++)
            {
                array[i] = series[i] < 1.0 ? (int)(series[i] * _partitions) : _partitions - 1;
            }

            double shannon = GetShannonEntropy(0, series.Length);

            if (corrLength >= series.Length)
            {
                corrLength = series.Length - 1;
            }

            Console.WriteLine($"#shannon = {shannon}");
            Console.WriteLine($"{0} {shannon}");

            for (int tau = 1; tau <= corrLength; tau++)
            {
                double entropy = GetShannonEntropy(tau, series.Length);
                Slope.AddDataPoint(tau, entropy);
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
