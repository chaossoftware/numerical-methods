using MathLib.NeuralNetwork;

namespace MathLib.NeuralNet.Entities
{
    public class HiddenNeuron : Neuron
    {
        private ActivationFunction ActivationFunction;

        public Synapse[] Inputs;

        public HiddenNeuron(ActivationFunction activationFunction)
        {
            ActivationFunction = activationFunction;
        }

        public HiddenNeuron()
        {

        }
    }
}
