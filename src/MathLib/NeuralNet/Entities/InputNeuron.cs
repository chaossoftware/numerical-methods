
using System;

namespace MathLib.NeuralNet.Entities
{
    public class InputNeuron : Neuron
    {
        public double Input;


        public InputNeuron(int outputsCount)
        {
            Outputs = new Synapse[outputsCount];
            Memory = new double[outputsCount];
            Best = new double[outputsCount];
            Input = 0;
        }

        public InputNeuron(int outputsCount, double nudge)
        {
            Outputs = new Synapse[outputsCount];
            Memory = new double[outputsCount];
            Best = new double[outputsCount];
            Nudge = nudge;
            Input = 0;
        }

        public override void ProcessInputs()
        {
            foreach (Synapse synapse in Outputs)
                synapse.Signal = Input * synapse.Weight;
        }
    }
}
