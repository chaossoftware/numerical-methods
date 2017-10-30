using MathLib.NeuralNet.Entities;
using System;

namespace MathLib.NeuralNetwork {

    public abstract class ActivationFunction {

        public InputNeuron Neuron;

        public bool AdditionalNeuron = false;

        public abstract double Phi(double arg);

        public abstract double Dphi(double arg);

        public abstract string GetName();

        //Returns hyperbolic secant of arg
        protected double Sech(double arg) {
            if (Math.Abs(arg) < 22d) {
                return 2d / (Math.Exp(arg) + Math.Exp(-arg));
            }
            else {
                return 0d;
            }
        }

        protected void InitNetworkLayer()
        {
            Neuron = new InputNeuron();
            //Neuron.Inputs = new Synapse[7];
            Neuron.Outputs = new Synapse[7];
            //Neuron.Outputs[0] = new Synapse();

            for (int i = 0; i < 7; i++)
                Neuron.Outputs[i] = new Synapse();
        }
    }
}
