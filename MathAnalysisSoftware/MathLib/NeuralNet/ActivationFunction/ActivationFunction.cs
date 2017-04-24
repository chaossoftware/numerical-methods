using System;

namespace MathLib.NeuralNetwork {

    public abstract class ActivationFunction {

        public double[] c = new double[7];
        public double[] cbest = new double[7];
        public double[] cverybest = new double[7];

        public bool additionalArray = false;

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
    }
}
