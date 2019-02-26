
namespace MathLib.NeuralNet.Entities
{
    public class Synapse
    {
        public double Weight;
        public double Signal;
        public bool Prune;

        public Synapse()
        {
            Weight = 0;
            Signal = 0;
            Prune = false;
        }
    }
}
