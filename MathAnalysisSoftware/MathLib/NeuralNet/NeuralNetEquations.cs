using System;
using MathLib.MathMethods.Solvers;
using MathLib.NeuralNet.Entities;

namespace MathLib.NeuralNetwork {
    public class NeuralNetEquations : SystemEquations
    {

        private readonly ActivationFunction Activation_Function;
        private readonly int Neurons;

        public NeuralNetEquations(int dimensions, int neurons, ActivationFunction activationFunction) : base(true)
        {
            linearized = true;
            EquationsCount = dimensions;
            TotalEquationsCount = dimensions + 1;
            Neurons = neurons;
            Activation_Function = activationFunction;
        }

        public override string Name => "Neural Net";

        public override double[,] Derivatives(double[,] x, double[,] dxdt) { throw new NotImplementedException(); }


        public double[,] Derivs(double[,] x, double[,] a, double[] b, double bias, BiasNeuron constant) {
            double[] df = new double[EquationsCount];
            double[,] xnew = new double[TotalEquationsCount, EquationsCount];
            double arg = 0;

            /*
             * Nonlinear neural net equations:
             */
            xnew[0, 0] = bias;

            for (int i = 0; i < Neurons; i++) {
                arg = constant.Outputs[i].Weight;

                for (int j = 0; j < EquationsCount; j++)
                    arg += a[i, j] * x[0, j];

                xnew[0, 0] += b[i] * Activation_Function.Phi(arg);
            }

            for (int j = 1; j < EquationsCount; j++)
                xnew[0, j] = x[0, j - 1];


            /*
             * Linearized neural net equations:
             */
            for (int k = 0; k < EquationsCount; k++) {
                df[k] = 0;

                for (int i = 0; i < Neurons; i++) {
                    arg = constant.Outputs[i].Weight;

                    for (int j = 0; j < EquationsCount; j++)
                        arg += a[i, j] * x[0, j];

                    df[k] += b[i] * a[i, k] * Activation_Function.Dphi(arg);
                }
            }

            for (int k = 0; k < EquationsCount; k++) {
                xnew[1, k] = 0;

                for (int j = 0; j < EquationsCount; j++)
                    xnew[1, k] += df[j] * x[j + 1, k];//xnew(k) + df(j) * x(k + d * (j - 1))
            }

            for (int k = 2; k < EquationsCount + 1; k++) {
                for (int j = 0; j < EquationsCount; j++) {
                    xnew[k, j] = x[k - 1, j];
                }
            }

            return xnew;
        }


        public override void Init(double[,] x) { throw new NotImplementedException(); }


        public void Init(double[,] x, double[] xdata) {
            // initial conditions for nonlinear map
            for (int i = 0; i < EquationsCount; i++) {
                x[0, i] = xdata[EquationsCount - i - 1];   //was xdata[dimensions - i + 1]
            }

            // initial conditions for linearized maps
            for (int i = 1; i < TotalEquationsCount; i++) {
                x[i, i - 1] = 1;
            }
        }


        public override string ToFileName()
        {
            throw new NotImplementedException();
        }


        public override string GetInfoShort() {
            throw new NotImplementedException();
        }


        public override string GetInfoFull() {
            throw new NotImplementedException();
        }
    }
}
