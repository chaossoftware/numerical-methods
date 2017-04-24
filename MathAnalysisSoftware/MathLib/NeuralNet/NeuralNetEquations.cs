using System;
using MathLib.MathMethods.Solvers;

namespace MathLib.NeuralNetwork {
    public class NeuralNetEquations : SystemEquations {

        private readonly ActivationFunction Activation_Function;
        private readonly int Neurons;

        public NeuralNetEquations(int dimensions, int neurons, ActivationFunction activationFunction) : base(true)
        {
            SystemName = "Neural Net";
            Linearized = true;
            N = dimensions;
            NN = dimensions + 1;
            Neurons = neurons;
            Activation_Function = activationFunction;
        }


        public override double[,] Derivs(double[,] x, double[,] dxdt) { throw new NotImplementedException(); }


        public double[,] Derivs(double[,] x, double[,] a, double[] b) {
            double[] df = new double[N];
            double[,] xnew = new double[NN, N];
            double arg = 0;

            /*
             * Nonlinear neural net equations:
             */
            xnew[0, 0] = b[0];

            for (int i = 1; i <= Neurons; i++) {
                arg = a[i, 0];

                for (int j = 1; j <= N; j++)
                    arg += a[i, j] * x[0, j - 1];

                xnew[0, 0] += b[i] * Activation_Function.Phi(arg);
            }

            for (int j = 1; j < N; j++)
                xnew[0, j] = x[0, j - 1];


            /*
             * Linearized neural net equations:
             */
            for (int k = 1; k <= N; k++) {
                df[k-1] = 0;

                for (int i = 1; i <= Neurons; i++) {
                    arg = a[i, 0];

                    for (int j = 1; j <= N; j++)
                        arg += a[i, j] * x[0, j-1];

                    df[k-1] += b[i] * a[i, k] * Activation_Function.Dphi(arg);
                }
            }

            for (int k = 0; k < N; k++) {
                xnew[1, k] = 0;

                for (int j = 0; j < N; j++)
                    xnew[1, k] += df[j] * x[j + 1, k];//xnew(k) + df(j) * x(k + d * (j - 1))
            }

            for (int k = 2; k < N + 1; k++) {
                for (int j = 0; j < N; j++) {
                    xnew[k, j] = x[k - 1, j];
                }
            }

            return xnew;
        }


        public override void Init(double[,] x) { throw new NotImplementedException(); }


        public void Init(double[,] x, double[] xdata) {
            // initial conditions for nonlinear map
            for (int i = 0; i < N; i++) {
                x[0, i] = xdata[N - i];   //was xdata[dimensions - i + 1]
            }

            // initial conditions for linearized maps
            for (int i = 1; i < NN; i++) {
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
