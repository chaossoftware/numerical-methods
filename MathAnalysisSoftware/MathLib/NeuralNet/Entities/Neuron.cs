using MathLib.NeuralNetwork;

namespace MathLib.NeuralNet.Entities
{
    public class Neuron
    {
        private ActivationFunction ActivationFunction;

        public Synapse[] Inputs;

        public double Output;


        public Neuron(ActivationFunction activationFunction)
        {
            Output = 0;
            ActivationFunction = activationFunction;
        }

        public Neuron(ActivationFunction activationFunction, int inputsCount)
        {
            Output = 0;
            ActivationFunction = activationFunction;

            Inputs = new Synapse[inputsCount];
            for (int i = 0; i < inputsCount; i++)
                Inputs[i] = new Synapse();
        }


        public void Process()
        {

        }
    }
}
