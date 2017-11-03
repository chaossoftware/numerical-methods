
namespace MathLib.NeuralNet.Entities
{
    public class BiasNeuron : Neuron
    {

        public BiasNeuron(int outputsCount)
        {
            Outputs = new Synapse[outputsCount];
        }

        public BiasNeuron(int outputsCount, double nudge)
        {
            Outputs = new Synapse[outputsCount];
            Nudge = nudge;
        }
    }
}
