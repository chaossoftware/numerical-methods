
using System;

namespace MathLib.NeuralNet.Entities
{
    public abstract class Neuron
    {
        public static Random Randomizer;

        public Synapse[] Outputs;
        public double Nudge;


        public virtual void CalculateWeight(int index, double pertrubation)
        {
            Outputs[index].Weight = Outputs[index].Memory + pertrubation * (Gauss2() - Nudge * Math.Sign(Outputs[index].Memory));
        }

        public virtual void CalculateWeight(int index, double pertrubation, double lowerThan)
        {
            Outputs[index].Weight = Outputs[index].Memory;

            if(Randomizer.NextDouble() < lowerThan)
                Outputs[index].Weight += pertrubation * (Gauss2() - Nudge * Math.Sign(Outputs[index].Memory));
        }


        public void BestToMemory()
        {
            foreach (Synapse synapse in Outputs)
                synapse.Memory = synapse.BestCase;
        }

        public void WeightsToMemory()
        {
            foreach (Synapse synapse in Outputs)
                synapse.Memory = synapse.Weight;
        }

        public void MemoryToBest()
        {
            foreach (Synapse synapse in Outputs)
                synapse.BestCase = synapse.Memory;
        }


        /// <summary>
        /// Returns the product of two normally (Gaussian) distributed random 
        /// deviates with meanof zero and standard deviation of 1.0
        /// </summary>
        /// <returns></returns>
        private double Gauss2()
        {
            double v1, v2, _arg;
            do
            {
                v1 = 2d * Randomizer.NextDouble() - 1d;
                v2 = 2d * Randomizer.NextDouble() - 1d;
                _arg = v1 * v1 + v2 * v2;
            }
            while (_arg >= 1d || _arg == 0d);

            return v1 * v2 * (-2d + Math.Log(_arg) / _arg);
        }
        
    }
}
