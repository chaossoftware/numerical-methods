using ChaosSoft.Core.Extensions;
using ChaosSoft.Core.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChaosSoft.Core.NumericalMethods.PhaseSpace
{
    /// <summary>
    /// Determines the fraction of false nearest neighbors.
    /// </summary>
    public class FalseNearestNeighbors
    {
        private const ushort BoxSize = 1024;
        private const ushort IBoxSize = BoxSize - 1;

        private readonly int _theiler = 0;
        private readonly int _tau = 1;
        private readonly int _minDim = 1;
        private readonly int _maxDim = 5;
        private readonly double _rt = 10.0;
        
        double eps0 = 1e-5;
        double aveps, vareps;
        double variance;

        int[,] box;
        int[] list;
        uint toolarge;

        public FalseNearestNeighbors(int minDim, int maxDim, int delay, double escapeFactor, int theilerWindow)
        {
            _minDim = minDim;
            _maxDim = maxDim;
            _tau = delay;
            _rt = escapeFactor;
            _theiler = theilerWindow;

            Log = new StringBuilder();
            FalseNeighbors = new Dictionary<int, int>();
        }

        public StringBuilder Log { get; }

        public Dictionary<int, int> FalseNeighbors;

        public void Calculate(double[] timeSeries)
        {
            int i;
            double[] series = new double[timeSeries.Length];
            Array.Copy(timeSeries, series, series.Length);

            // to calculate only if not retrieved in constructor;
            double inter = Arrays.Rescale(series);

            // to calculate only if not retrieved in constructor;
            variance = Statistics.Variance(series);

            list = new int[series.Length];
            bool[] nearest = new bool[series.Length];
            box = new int[BoxSize, BoxSize];

            for (int dim = _minDim; dim <= _maxDim; dim++)
            {
                double epsilon = eps0;
                toolarge = 0;
                bool alldone = false;
                int donesofar = 0;
                aveps = 0.0;
                vareps = 0.0;

                for (i = 0; i < series.Length; i++)
                {
                    nearest[i] = false;
                }

                Log.AppendLine($"Start for dimension = {dim}");
                while (!alldone && (epsilon < 2d * variance / _rt))
                {
                    alldone = true;
                    PutInBox(series, dim, epsilon);

                    for (i = (dim - 1) * _tau; i < series.Length - 1; i++)
                    {
                        if (!nearest[i])
                        {
                            nearest[i] = FindNearest(series, i, dim, epsilon);
                            alldone &= nearest[i];

                            if (nearest[i])
                            {
                                donesofar++;
                            }
                        }
                    }

                    Log.Append($"Found {donesofar} up to epsilon = {NumFormatter.ToShort(epsilon * inter)}");
                    epsilon *= Math.Sqrt(2.0);

                    //if (!donesofar)
                    if (donesofar == 0)
                    {
                        eps0 = epsilon;
                    }
                }

                if (donesofar == 0)
                {
                    throw new ArgumentException("Not enough points found.");
                }

                aveps *= (1d / donesofar);
                vareps *= (1d / donesofar);

                Log.AppendLine($"Dimension: {dim}; False neighbors: {(double)toolarge / donesofar} | {NumFormatter.ToShort(aveps)} | {NumFormatter.ToShort(vareps)}");
                FalseNeighbors.Add(dim, (int)toolarge);
            }
        }

        private bool FindNearest(double[] series, int n, int dim, double eps)
        {
            int x2, y1, i, i1;
            int element, which = -1;
            double dx, maxdx, mindx = 1.1, factor;

            int x = (int)(series[n - (dim - 1) * _tau] / eps) & IBoxSize;
            int y = (int)(series[n] / eps) & IBoxSize;

            for (int x1 = x - 1; x1 <= x + 1; x1++)
            {
                x2 = x1 & IBoxSize;

                for (y1 = y - 1; y1 <= y + 1; y1++)
                {
                    element = box[x2, y1 & IBoxSize];

                    while (element != -1)
                    {
                        if (Math.Abs(element - n) > _theiler)
                        {
                            maxdx = Math.Abs(series[n] - series[element]);

                            for (i = 1; i < dim; i++)
                            {
                                i1 = i * _tau;
                                dx = Math.Abs(series[n - i1] - series[element - i1]);

                                if (dx > maxdx)
                                {
                                    maxdx = dx;
                                }
                            }

                            if (maxdx < mindx && maxdx > 0.0)
                            {
                                which = element;
                                mindx = maxdx;
                            }
                        }
                        element = list[element];
                    }
                }
            }

            if (which != -1 && mindx <= eps && mindx <= variance / _rt)
            {
                aveps += mindx;
                vareps += mindx * mindx;
                factor = Math.Abs(series[n + 1] - series[which + 1]) / mindx;

                if (factor > _rt)
                {
                    toolarge++;
                }

                return true;
            }

            return false;
        }

        private void PutInBox(double[] series, int dim, double eps)
        {
            int x, y;
            int xShift = (dim - 1) * _tau;

            for (x = 0; x < BoxSize; x++)
            {
                for (y = 0; y < BoxSize; y++)
                {
                    box[x, y] = -1;
                }
            }

            for (int i = xShift; i < series.Length - 1; i++)
            {
                x = (int)(series[i - xShift] / eps) & IBoxSize;
                y = (int)(series[i] / eps) & IBoxSize;
                list[i] = box[x, y];
                box[x, y] = i;
            }
        }
    }
}
