﻿using System;
using ChaosSoft.NumericalMethods.Algebra;

namespace ChaosSoft.NumericalMethods.PhaseSpace
{
    internal class BoxAssistedFnn
    {
        private readonly ushort _boxSize;
        private readonly ushort _iBoxSize;
        private readonly int[,] _boxes;
        private readonly int[] _list;

        internal BoxAssistedFnn(ushort boxSize, int timeSeriesSize)
        {
            _boxSize = boxSize;
            _iBoxSize = (ushort)(boxSize - 1);
            _boxes = new int[boxSize, boxSize];
            _list = new int[timeSeriesSize];
            Found = new int[timeSeriesSize];
        }

        internal int[] Found { get; }

        /// <summary>
        /// Optimized False Nearest Neighbors (FNN):
        /// Box-assisted algorithm, consisting of dividing the phase space into a grid of boxes of eps side length. 
        /// Then, each point falls into one of these boxes. 
        /// All its neighbors closer than eps have to lie in either the same box or one of the adjacent ones
        /// </summary>
        /// <param name="series"></param>
        /// <param name="epsilon"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="xShift"></param>
        /// <param name="yShift"></param>
        // Shift (Kantz = Tau) (Rosenstein = Tau * (Dim - 1)) (Jakobian = 0)
        internal void PutInBoxes(double[] series, double epsilon, int startIndex, int endIndex, int xShift, int yShift)
        {
            for (var x = 0; x < _boxSize; x++)
            {
                for (var y = 0; y < _boxSize; y++)
                {
                    _boxes[x, y] = -1;
                }
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                var x = (int)(series[i + xShift] / epsilon) & _iBoxSize;
                var y = (int)(series[i + yShift] / epsilon) & _iBoxSize;
                _list[i] = _boxes[x, y];
                _boxes[x, y] = i;
            }
        }

        internal int FindNeighborsJ(double[] series, int eDim, int tau, double epsilon, int act)
        {
            int element;
            int nf = 0;
            int x, y, i, i1, j, k, k1;
            double dx = 0.0;

            x = (int)(series[act] / epsilon) & _iBoxSize;
            y = x;

            for (i = x - 1; i <= x + 1; i++)
            {
                i1 = i & _iBoxSize;

                for (j = y - 1; j <= y + 1; j++)
                {
                    element = _boxes[i1, j & _iBoxSize];

                    while (element != -1)
                    {
                        for (k = 0; k < eDim; k++)
                        {
                            k1 = -k * tau;

                            dx = Math.Abs(series[k1 + act] - series[element + k1]);

                            if (dx > epsilon)
                            {
                                break;
                            }
                        }

                        if (dx <= epsilon)
                        {
                            Found[nf++] = element;
                        }

                        element = _list[element];
                    }
                }
            }

            return nf;
        }

        internal int FindNeighborsK(double[] series, int eDim, int tau, double epsilon, int act, int window)
        {
            int element;
            int nf = 0;
            int x, y, i, i1, j, k, k1;
            double dx, eps2 = Numbers.FastPow2(epsilon);

            x = (int)(series[act] / epsilon) & _iBoxSize;
            y = (int)(series[act + tau] / epsilon) & _iBoxSize;

            for (i = x - 1; i <= x + 1; i++)
            {
                i1 = i & _iBoxSize;

                for (j = y - 1; j <= y + 1; j++)
                {
                    element = _boxes[i1, j & _iBoxSize];

                    while (element != -1)
                    {
                        if (element < (act - window) || element > (act + window))
                        {
                            dx = Numbers.FastPow2(series[act] - series[element]);

                            if (dx <= eps2)
                            {
                                k = eDim - 1;
                                k1 = k * tau;
                                dx += Numbers.FastPow2(series[act + k1] - series[element + k1]);

                                if (dx > eps2)
                                {
                                    break;
                                }

                                k1 = k - 1;
                                Found[nf++] = element;
                            }
                        }

                        element = _list[element];
                    }
                }
            }

            return nf;
        }

        internal bool FindNeighborsR(double[] timeSeries, int eDim, int tau, double epsilon, long act, int window, out int minelement)
        {
            int element;
            bool ok = false;
            minelement = -1;
            int x, y, i1, k, del1 = eDim * tau;
            
            double dx, eps2 = Numbers.FastPow2(epsilon), mindx = 1.0;

            x = (int)(timeSeries[act] / epsilon) & _iBoxSize;
            y = (int)(timeSeries[act + tau * (eDim - 1)] / epsilon) & _iBoxSize;

            for (int i = x - 1; i <= x + 1; i++)
            {
                i1 = i & _iBoxSize;

                for (int j = y - 1; j <= y + 1; j++)
                {
                    element = _boxes[i1, j & _iBoxSize];

                    while (element != -1)
                    {
                        if (Math.Abs(act - element) > window)
                        {
                            dx = 0.0;

                            for (k = 0; k < del1; k += tau)
                            {
                                dx += Numbers.FastPow2(timeSeries[act + k] - timeSeries[element + k]);

                                if (dx > eps2)
                                {
                                    break;
                                }
                            }

                            if (k == del1 && dx < mindx)
                            {
                                ok = true;

                                if (dx > 0.0)
                                {
                                    mindx = dx;
                                    minelement = element;
                                }
                            }
                        }

                        element = _list[element];
                    }
                }
            }

            return ok;
        }
    }
}
