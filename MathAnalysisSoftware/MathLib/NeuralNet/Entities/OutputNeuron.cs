
namespace MathLib.NeuralNet.Entities
{
    public class OutputNeuron : Neuron
    {
        public Synapse[] Inputs;
        public Synapse BiasInput;


        public OutputNeuron(int inputsCount)
        {
            Outputs = new Synapse[1];
            Memory = new double[1];
            Best = new double[1];
            Inputs = new Synapse[inputsCount];
        }

        public OutputNeuron(int inputsCount, double nudge)
        {
            Outputs = new Synapse[1];
            Memory = new double[1];
            Best = new double[1];
            Inputs = new Synapse[inputsCount];
            Nudge = nudge;
        }

        public override void ProcessInputs()
        {
            double arg = BiasInput.Weight;
            foreach (Synapse synapse in Inputs)
                arg += synapse.Signal;

            Outputs[0].Signal = arg;
        }
    }
}
