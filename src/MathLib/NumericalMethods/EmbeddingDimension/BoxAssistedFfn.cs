
using System;

namespace MathLib.NumericalMethods.EmbeddingDimension
{
    public class BoxAssistedFnn
    {
        private readonly ushort boxSize;
        private readonly ushort iBoxSize;
        private readonly int[,] boxes;
        private readonly int[] list;

        public BoxAssistedFnn(ushort boxSize, int timeSeriesSize)
        {
            this.boxSize = boxSize;
            this.iBoxSize = (ushort)(boxSize - 1);
            this.boxes = new int[boxSize, boxSize];
            this.list = new int[timeSeriesSize];
            this.Found = new int[timeSeriesSize];
        }

        public int[] Found { get; protected set; }

        /// <summary>
        /// Optimized False Nearest Neighbors (FNN):
        /// Box-assisted algorithm, consisting of dividing the phase space into a grid of boxes of eps side length. 
        /// Then, each point falls into one of these boxes. 
        /// All its neighbors closer than eps have to lie in either the same box or one of the adjacent ones
        /// </summary>
        /// <param name="timeSeries"></param>
        /// <param name="list"></param>
        /// <param name="epsilon"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="xShift"></param>
        /// <param name="yShift"></param>
        //shift (Kantz = Tau) (Rosenstein = Tau * (Dim - 1)) (Jakobian = 0)
        public void PutInBoxes(double[] timeSeries, double epsilon, int startIndex, int endIndex, int xShift, int yShift)
        {
            for (var x = 0; x < boxSize; x++)
            {
                for (var y = 0; y < boxSize; y++)
                {
                    this.boxes[x, y] = -1;
                }
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                var x = (int)(timeSeries[i + xShift] / epsilon) & iBoxSize;
                var y = (int)(timeSeries[i + yShift] / epsilon) & iBoxSize;
                this.list[i] = this.boxes[x, y];
                this.boxes[x, y] = i;
            }
        }


        public int FindNeighborsJ(double[] timeSeries, int eDim, int tau, double epsilon, int act)
        {
            int element;
            int nf = 0;
            int x, y, i, i1, j, k, k1;
            double dx = 0.0;

            x = (int)(timeSeries[act] / epsilon) & iBoxSize;
            y = (int)(timeSeries[act] / epsilon) & iBoxSize;

            for (i = x - 1; i <= x + 1; i++)
            {
                i1 = i & iBoxSize;

                for (j = y - 1; j <= y + 1; j++)
                {
                    element = boxes[i1, j & iBoxSize];

                    while (element != -1)
                    {
                        for (k = 0; k < eDim; k++)
                        {
                            k1 = -k * tau;

                            dx = Math.Abs(timeSeries[k1 + act] - timeSeries[element + k1]);

                            if (dx > epsilon)
                            {
                                break;
                            }
                        }

                        if (dx <= epsilon)
                        {
                            Found[nf++] = element;
                        }

                        element = list[element];
                    }
                }
            }

            return nf;
        }

        public int FindNeighborsK(double[] timeSeries, int eDim, int tau, double epsilon, int act, int window)
        {
            int element;
            int nf = 0;
            int x, y, i, i1, j, k, k1;
            double dx, eps2 = Math.Pow(epsilon, 2);

            x = (int)(timeSeries[act] / epsilon) & iBoxSize;
            y = (int)(timeSeries[act + tau] / epsilon) & iBoxSize;

            for (i = x - 1; i <= x + 1; i++)
            {
                i1 = i & iBoxSize;

                for (j = y - 1; j <= y + 1; j++)
                {
                    element = boxes[i1, j & iBoxSize];

                    while (element != -1)
                    {
                        if (element < (act - window) || element > (act + window))
                        {
                            dx = Math.Pow(timeSeries[act] - timeSeries[element], 2);

                            if (dx <= eps2)
                            {
                                k = eDim - 1;
                                k1 = k * tau;
                                dx += Math.Pow(timeSeries[act + k1] - timeSeries[element + k1], 2);

                                if (dx > eps2)
                                {
                                    break;
                                }

                                k1 = k - 1;
                                Found[nf++] = element;
                            }
                        }

                        element = list[element];
                    }
                }
            }

            return nf;
        }

        public bool FindNeighborsR(double[] timeSeries, int eDim, int tau, double epsilon, long act, int window, out int minelement)
        {
            int element;
            bool ok = false;
            minelement = -1;
            int x, y, i1, k, del1 = eDim * tau;
            
            double dx, eps2 = Math.Pow(epsilon, 2), mindx = 1.0;

            x = (int)(timeSeries[act] / epsilon) & iBoxSize;
            y = (int)(timeSeries[act + tau * (eDim - 1)] / epsilon) & iBoxSize;

            for (int i = x - 1; i <= x + 1; i++)
            {
                i1 = i & iBoxSize;

                for (int j = y - 1; j <= y + 1; j++)
                {
                    element = boxes[i1, j & iBoxSize];

                    while (element != -1)
                    {
                        if (Math.Abs(act - element) > window)
                        {
                            dx = 0.0;

                            for (k = 0; k < del1; k += tau)
                            {
                                dx += Math.Pow(timeSeries[act + k] - timeSeries[element + k], 2);

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

                        element = list[element];
                    }
                }
            }

            return ok;
        }
    }
}
