
namespace MathLib.NeuralNet.Entities
{
    public class OutputNeuron : Neuron
    {
        public Synapse[] Inputs;
        public Synapse BiasInput;


        public OutputNeuron(int inputsCount)
        {
            Outputs = new Synapse[1];
            Inputs = new Synapse[inputsCount];
        }

        public OutputNeuron(int inputsCount, double nudge)
        {
            Outputs = new Synapse[1];
            Inputs = new Synapse[inputsCount];
            Nudge = nudge;
        }
    }
}
