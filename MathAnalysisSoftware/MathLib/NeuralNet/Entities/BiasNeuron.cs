
namespace MathLib.NeuralNet.Entities
{
    public class BiasNeuron : Neuron
    {

        public BiasNeuron(int outputsCount)
        {
            Outputs = new Synapse[outputsCount];
            Memory = new double[outputsCount];
            Best = new double[outputsCount];
        }

        public BiasNeuron(int outputsCount, double nudge)
        {
            Outputs = new Synapse[outputsCount];
            Memory = new double[outputsCount];
            Best = new double[outputsCount];
            Nudge = nudge;
        }
    }
}
