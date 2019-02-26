
using MathLib.NeuralNetwork;

namespace MathLib.NeuralNet.Entities
{
    public class HiddenNeuron : Neuron
    {
        public static ActivationFunction Function;
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

        public override void ProcessInputs()
        {
            double arg = BiasInput.Weight;// + Inputs.Select(;
            foreach (Synapse synapse in Inputs)
                arg += synapse.Signal;


            foreach (Synapse synapse in Outputs)
                synapse.Signal = synapse.Weight * Function.Phi(arg);
        }
    }
}
