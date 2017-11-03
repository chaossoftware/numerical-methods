
namespace MathLib.NeuralNet.Entities
{
    public class Synapse
    {
        public double Weight;
        //public double Memory;
        //public double BestCase;
        public bool Prune;

        public Synapse()
        {
            Weight = 0;
            //Memory = 0;
            //BestCase = 0;
            Prune = false;
        }
    }
}
