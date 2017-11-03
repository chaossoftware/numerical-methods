
namespace MathLib.NeuralNet.Entities
{
    public class HiddenNeuron : Neuron
    {
        public Synapse[] Inputs;
        public Synapse BiasInput;


        public HiddenNeuron(int inputsCount, int outputsCount)
        {
            Inputs = new Synapse[inputsCount];
            Outputs = new Synapse[outputsCount];
            Memory = new double[outputsCount];
            Best = new double[outputsCount];
        }

        public HiddenNeuron(int inputsCount, int outputsCount, double nudge)
        {
            Inputs = new Synapse[inputsCount];
            Outputs = new Synapse[outputsCount];
            Memory = new double[outputsCount];
            Best = new double[outputsCount];
            Nudge = nudge;
        }

    }
}
