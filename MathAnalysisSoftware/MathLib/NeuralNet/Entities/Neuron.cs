
using System;

namespace MathLib.NeuralNet.Entities
{
    public abstract class Neuron
    {
        public static Random Randomizer;

        public Synapse[] Outputs;
        public double[] Memory;
        public double[] Best;
        public double Nudge;


        public virtual void CalculateWeight(int index, double pertrubation)
        {
            Outputs[index].Weight = Memory[index] + pertrubation * (Gauss2() - Nudge * Math.Sign(Memory[index]));
        }

        public virtual void CalculateWeight(int index, double pertrubation, double lowerThan)
        {
            Outputs[index].Weight = Memory[index];

            if(Randomizer.NextDouble() < lowerThan)
                Outputs[index].Weight += pertrubation * (Gauss2() - Nudge * Math.Sign(Memory[index]));
        }


        public void BestToMemory()
        {
            for(int i = 0; i < Outputs.Length; i++)
                Memory[i] = Best[i];
        }

        public void WeightsToMemory()
        {
            for (int i = 0; i < Outputs.Length; i++)
                Memory[i] = Outputs[i].Weight;
        }

        public void MemoryToWeights()
        {
            for (int i = 0; i < Outputs.Length; i++)
                Outputs[i].Weight = Memory[i];
        }

        public void BestToWeights()
        {
            for (int i = 0; i < Outputs.Length; i++)
                Outputs[i].Weight = Best[i];
        }

        public void MemoryToBest()
        {
            for (int i = 0; i < Outputs.Length; i++)
                Best[i] = Memory[i];
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
