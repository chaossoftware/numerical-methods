
namespace MathLib.MathMethods.EmbeddingDimension
{
    public class BoxAssistedFnn
    {
        private readonly ushort boxSize;

        public BoxAssistedFnn(ushort boxSize, int timeSeriesSize)
        {
            this.boxSize = boxSize;
            this.MaxBoxIndex = (ushort)(boxSize - 1);
            this.Boxes = new int[boxSize, boxSize];
            this.List = new int[timeSeriesSize];
            this.Found = new int[timeSeriesSize];
        }

        public int[,] Boxes { get; protected set; }

        public ushort MaxBoxIndex { get; protected set; }

        public int[] List { get; protected set; }

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
                    this.Boxes[x, y] = -1;
                }
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                var x = (int)(timeSeries[i + xShift] / epsilon) & MaxBoxIndex;
                var y = (int)(timeSeries[i + yShift] / epsilon) & MaxBoxIndex;
                this.List[i] = this.Boxes[x, y];
                this.Boxes[x, y] = i;
            }
        }

    }
}
