using System;

namespace MathLib.NeuralNetwork {

    public class BinaryShiftFunction : ActivationFunction {

        public override double Phi(double arg) {
            return arg % 1d;
        }


        public override double Dphi(double arg) {
            return 1d;
        }

        public override string GetName() {
            return "Binary shift";
        }
    }


    public class GaussianFunction : ActivationFunction {

        public override double Phi(double arg) {
            return arg * (1d - arg);
        }


        public override double Dphi(double arg) {
            return 1d - 2d * arg;
        }

        public override string GetName() {
            return "Gaussian";
        }
    }


    public class GaussianDerivativeFunction : ActivationFunction {

        public override double Phi(double arg) {
            return -arg * Math.Exp(-arg * arg);
        }


        public override double Dphi(double arg) {
            return (2d * arg - 1d) * Math.Exp(-arg * arg);
        }

        public override string GetName() {
            return "Gaussian Derivative";
        }
    }


    public class LogisticFunction : ActivationFunction {

        public override double Phi(double arg) {
            return arg * (1d - arg);
        }


        public override double Dphi(double arg) {
            return 1d - 2d * arg;
        }

        public override string GetName() {
            return "Logistic";
        }
    }


    public class LinearFunction : ActivationFunction {

        public override double Phi(double arg) {
            return arg;
        }


        public override double Dphi(double arg) {
            return 2d * arg;
        }

        public override string GetName() {
            return "Linear";
        }
    }


    public class PiecewiseLinearFunction : ActivationFunction {

        public override double Phi(double arg) {
            if (Math.Abs(arg) < 1d)
                return arg;
            else 
                return Math.Sign(arg);
        }


        public override double Dphi(double arg) {
            if (Math.Abs(arg) < 1d)
                return 1;
            else
                return 0;
        }

        public override string GetName() {
            return "Piecewise Linear";
        }
    }


    public class ExponentialFunction : ActivationFunction {

        public override double Phi(double arg) {
            return Math.Exp(arg);
        }


        public override double Dphi(double arg) {
            return Math.Exp(arg);
        }

        public override string GetName() {
            return "Exponential";
        }
    }


    public class CosineFunction : ActivationFunction {

        public override double Phi(double arg) {
            return Math.Cos(arg);
        }


        public override double Dphi(double arg) {
            return Math.Cos(arg);
        }

        public override string GetName() {
            return "Cosine";
        }
    }


    public class SigmoidFunction : ActivationFunction {

        public override double Phi(double arg) {
            if (arg < -44d) {
                return 0d;
            }
            else if (arg > 44d) {
                return 1d;
            }
            else {
                return 1d / (1d + Math.Exp(-arg));
            }
        }


        public override double Dphi(double arg) {
            if (Math.Abs(arg) > 44d) {
                return 0d;
            }
            else {
                double argExp = Math.Exp(arg);
                double _v = (1d + argExp);
                return argExp / (_v * _v);
            }
        }

        public override string GetName() {
            return "Sigmoid";
        }
    }


    public class HyperbolicTangentFunction : ActivationFunction {

        public override double Phi(double arg) {
            if (arg < 22d) {
                return 1d - 2d / (Math.Exp(2d * arg) + 1d);
            }
            else {
                return Math.Sign(arg);
            }
        }


        public override double Dphi(double arg) {
            double tmp = Sech(arg);
            return tmp * tmp;
        }


        public override string GetName() {
            return "Hyperbolic tangent";
        }
    }


    public class PolynomialSixOrderFunction : ActivationFunction {

        public bool additionalArray = true;

        public override double Phi(double arg) {
            return c[0] + arg * (c[1] + arg * (c[2] + arg * (c[3] + arg * (c[4] + arg * (c[5] + arg * c[6])))));
        }


        public override double Dphi(double arg) {
            return c[1] + arg * (2d * c[2] + arg * (3d * c[3] + arg *(4d * c[4] + arg * (5d * c[5] + arg * 6d * c[6]))));
        }


        public override string GetName() {
            return "Polynomial (6 order)";
        }
    }


    public class RationalFunction : ActivationFunction {

        public bool additionalArray = true;

        public override double Phi(double arg) {
            return (c[0] + arg * (c[1] + arg * (c[2] + arg * c[3]))) / (1d + arg * (c[4] + arg * (c[5] + arg * c[6])));
        }


        public override double Dphi(double arg) {
            double f = c[0] + arg * (c[1] + arg * (c[2] + arg * c[3]));
            double df = c[1] + arg * (2d * c[2] + arg * 3d * c[3]);
            double g = 1d +arg * (c[4] + arg * (c[5] + arg * c[6]));
            double dg = c[4] + arg * (2d * c[5] + arg * 3d * c[6]);
            return (g * df - f * dg) / (g * g);
        }

        public override string GetName() {
            return "Rational";
        }
    }


    public class SpecialFunction : ActivationFunction {

        public bool additionalArray = true;

        public override double Phi(double arg) {
            if (Math.Abs(arg) < 22d)
                return c[0] + arg * (c[1] + arg * c[2]) + c[3] * (1d - 2d / (Math.Exp(2d * arg) + 1d));
            else
                return c[0] + arg * (c[1] + arg * c[2]) + c[3] * Math.Sign(arg);
        }


        public override double Dphi(double arg) {
            return c[1] + arg * 2d * c[2] + c[3] * Math.Pow(Sech(arg), 2);
        }


        public override string GetName() {
            return "Special";
        }
    }
}
