using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.NeuralNet.Entities
{
    public class Synapse
    {
        public double Weight;
        public double WeightGood;
        public double WeightBest;
        public bool Prune;

        public Synapse()
        {
            Weight = 0;
            WeightGood = 0;
            WeightBest = 0;
            Prune = false;
        }
    }
}
