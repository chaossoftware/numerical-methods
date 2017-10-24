using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.NeuralNet.Entities
{
    public class Synapse
    {
        public double Weight;
        public double WBest;
        public double WVeryBest;
        public bool Prune;

        public Synapse()
        {
            Weight = 0;
            WBest = 0;
            WVeryBest = 0;
            Prune = false;
        }
    }
}
