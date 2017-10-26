
namespace MathLib.NeuralNet.Entities
{
    public abstract class Neuron
    {

        public Synapse[] Outputs;


        public void UpdateMemoryWithBestResult()
        {
            foreach (Synapse synapse in Outputs)
                synapse.Memory = synapse.BestCase;
        }

        public void MemorizeWeights()
        {
            foreach (Synapse synapse in Outputs)
                synapse.Memory = synapse.Weight;
        }

        public void SaveBestWeights()
        {
            foreach (Synapse synapse in Outputs)
                synapse.BestCase = synapse.Memory;
        }


        public abstract void Process();
    }
}
