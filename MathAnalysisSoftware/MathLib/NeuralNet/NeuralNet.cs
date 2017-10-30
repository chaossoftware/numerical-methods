using MathLib.MathMethods.Lyapunov;
using MathLib.NeuralNet.Entities;
using System;

namespace MathLib.NeuralNetwork {
    public class NeuralNet {

        public static double arg;

        public static BenettinResult Task_Result;
        public static NeuralNetParams Params;
        public static NeuralNetEquations System_Equations;

        private static Random random = new Random();

        public static InputNeuron[] NeuronsInput;
        public static HiddenNeuron[] NeuronsHidden;
        public static OutputNeuron NeuronOutput;
        public static BiasNeuron NeuronBias;

        //----- input data
        private static long nmax;  //lines in file
        private static double xmax;//max value
        public static double[] xdata;

        //----- pre-calculated constants
        private static double TEN_POW_MIN_PRUNING;      
        private static double MIN_D5_DIV_D;
        private static double NMAX_MINUS_D_XMAX_POW_E;

        private static int neurons;
        private static int dims;
        private static int countnd = 1;    //Allows for introducing n and d gradually

        private static double x;

        private static double ddw;

        //----- arrays
        public static double[] xlast;

        public static int successCount;

        private static int improved = 0;
        private static int seed;

        public static int _c;

        private static bool AdditionalNeuron;


        public NeuralNet(NeuralNetParams taskParams, double[] array) {
            Params = taskParams;
            System_Equations = new NeuralNetEquations(Params.Dimensions, Params.Neurons, Params.ActFunction);

            Init(array);
            AdditionalNeuron = Params.ActFunction.AdditionalNeuron;
        }


        public static MyMethodDelegate LoggingMethod = null;
        public static MyMethodDelegate EndCycleMethod = null;


        public delegate void MyMethodDelegate();

        public void InvokeMethodForNeuralNet(MyMethodDelegate method) {
            method.DynamicInvoke();
        }
        


        public void RunTask() {

            while (successCount < Params.Trainings) {

                if (NeuronOutput.Outputs[0].BestCase == 0) {
                    NeuronOutput.Outputs[0].Memory = Params.TestingInterval;
                }
                else {
                    NeuronOutput.Outputs[0].Memory = 10 * NeuronOutput.Outputs[0].BestCase;
                    ddw = Math.Min(Params.MaxPertrubation, Math.Sqrt(NeuronOutput.Outputs[0].BestCase));
                }

                //update memory with best results
                foreach (InputNeuron neuron in NeuronsInput)
                    neuron.UpdateMemoryWithBestResult();

                foreach (HiddenNeuron neuron in NeuronsHidden)
                    neuron.UpdateMemoryWithBestResult();

                NeuronBias.UpdateMemoryWithBestResult();
                

                // same for Activation function neuron if needed
                if(AdditionalNeuron)
                {
                    Params.ActFunction.Neuron.UpdateMemoryWithBestResult();
                    if (Params.ActFunction.Neuron.Outputs[0].Memory == 0)
                        Params.ActFunction.Neuron.Outputs[0].Memory = 1;
                }


                int N_MUL_D_MINCT_PLUS_1_PLUS_N_PLUS_1 = neurons * (dims - Params.ConstantTerm + 1) + neurons + 1;

                for (_c = 1; _c <= Params.CMax; _c++) {

                    if (Params.Pruning == 0)
                        foreach (HiddenNeuron neuron in NeuronsHidden)
                            foreach (Synapse synapse in neuron.Inputs)
                                synapse.Prune = false;
                        
                    double e1 = 0;
                    int prunes = 0;

                    for (int i = 0; i < neurons; i++)
                        for (int j = Params.ConstantTerm; j <= dims; j++)
                            if (NeuronsInput[j].Outputs[i].Weight == 0)
                                prunes++;

                    //Probability of changing a given parameter at each trial
                    double pc = 1d / Math.Sqrt(N_MUL_D_MINCT_PLUS_1_PLUS_N_PLUS_1 - prunes); // 1d / Sqrt(neurons * (dims - Task_Params.ConstantTerm + 1) + neurons + 1 - prunes)

                    if (Params.BiasTerm == 0)
                    {
                        NeuronBias.Outputs[0].Weight = NeuronBias.Outputs[0].Memory + ddw * (Ext.Gauss2(random) - Params.Nudge * Math.Sign(NeuronBias.Outputs[0].Memory));
                    }
                    else
                        NeuronBias.Outputs[0].Weight = 0;

                    for (int i = 0; i < neurons; i++) {
                        double wBest = NeuronsHidden[i].Outputs[0].Memory;
                        NeuronsHidden[i].Outputs[0].Weight = wBest;
                        if (random.NextDouble() < 9 * pc)
                            NeuronsHidden[i].Outputs[0].Weight += ddw * (Ext.Gauss2(random) - Params.Nudge * Math.Sign(wBest));
            
                        //Eliminates constant term if ct=1
                        for (int j = Params.ConstantTerm; j <= dims; j++) {
                
                            //Reduce neighborhood for large j by a factor of 1-32
                            double dj = 1d / Math.Pow(2, MIN_D5_DIV_D * j);
                            double aBest = NeuronsInput[j].Outputs[i].Memory;
                            NeuronsInput[j].Outputs[i].Weight = aBest;
                
                            if (random.NextDouble() < pc )
                                NeuronsInput[j].Outputs[i].Weight += ddw * dj * (Ext.Gauss2(random) - Params.Nudge * Math.Sign(aBest));
                
                            //This connection has been pruned
                            if (NeuronsInput[j].Outputs[i].Prune)
                                NeuronsInput[j].Outputs[i].Weight = 0; 
                        }
                    }


                    // same for Activation function neuron if needed
                    if (AdditionalNeuron) {
                        for (int j = 0; j < 7; j++) {
                            double cBest = Params.ActFunction.Neuron.Outputs[j].Memory;
                            Params.ActFunction.Neuron.Outputs[j].Weight = cBest;
                            if (random.NextDouble() < pc)
                                Params.ActFunction.Neuron.Outputs[j].Weight += ddw * (Ext.Gauss2(random) - Params.Nudge * Math.Sign(cBest));
                        }
                    }


                    for (int k = Params.Dimensions + 1; k <= nmax; k++) {
            
                        for (int j = 1; j <= Params.Dimensions; j++)
                            xlast[j] = xdata[k - j];
            
                        x = NeuronBias.Outputs[0].Weight;

                        for (int i = 0; i < Params.Neurons; i++) {
                            arg = NeuronsHidden[i].Inputs[0].Weight;

                            for (int j = 1; j <= Params.Dimensions; j++)
                                arg += NeuronsInput[j].Outputs[i].Weight * xlast[j];

                            x += NeuronsHidden[i].Outputs[0].Weight * Params.ActFunction.Phi(arg);
                        }

                        //Error in the prediction of the k-th data point
                        double ex = Math.Abs(x - xdata[k]);
                        e1 += Math.Pow(ex, Params.ErrorsExponent);
                    }


                    //"Mean-square" error (even for e&<>2)
                    e1 = Math.Pow(e1 / NMAX_MINUS_D_XMAX_POW_E, 2 / Params.ErrorsExponent); 

                    if (e1 < NeuronOutput.Outputs[0].Memory) {

                        improved ++;
                        NeuronOutput.Outputs[0].Memory = e1;


                        //memorize current weights
                        foreach (InputNeuron neuron in NeuronsInput)
                            neuron.MemorizeWeights();

                        foreach (HiddenNeuron neuron in NeuronsHidden)
                            neuron.MemorizeWeights();

                        NeuronBias.MemorizeWeights();


                        // same for Activation function neuron if needed
                        if (AdditionalNeuron) {
                            Params.ActFunction.Neuron.MemorizeWeights();
                        }
                    }
                    else if (ddw > 0 && improved == 0) {
                        ddw = -ddw;     //Try going in the opposite direction
                    }
                    //Reseed the random if the trial failed
                    else {
                        seed = random.Next(Int32.MaxValue);     //seed = (int) (1 / Math.Sqrt(e1));
            
                        if (improved > 0) {
                            ddw = Math.Min(Params.MaxPertrubation, (1 + improved / Params.TestingInterval) * Math.Abs(ddw));
                            improved = 0;
                        }
                        else
                            ddw = Params.Eta * Math.Abs(ddw);
                    }

                    random = new Random((int)seed);
        
                    //Testing is costly - don't do it too often
                    if (_c % Params.TestingInterval != 0)
                        continue;


                    InvokeMethodForNeuralNet(LoggingMethod);
        

                    if(NeuronOutput.Outputs[0].Memory > NeuronOutput.Outputs[0].BestCase && NeuronOutput.Outputs[0].BestCase != 0)
                        continue;


                    // same for Activation function neuron if needed
                    if (AdditionalNeuron) {
                        for (int j = 0; j < 7; j++)
                            Params.ActFunction.Neuron.Outputs[j].Weight = Params.ActFunction.Neuron.Outputs[j].Memory;
                    }

     		
                    //Mark the weakconnections for pruning
                    if (Params.Pruning != 0) 
                        for (int i = 0; i < Params.Neurons; i++)
                            for (int j = 0; j <= Params.Dimensions; j++)
                            {
                                double aBest = NeuronsInput[j].Outputs[i].Memory;
                                double bBest = NeuronsHidden[i].Outputs[0].Memory;
                                if (aBest != 0 && Math.Abs(aBest * bBest) < TEN_POW_MIN_PRUNING)
                                    NeuronsInput[j].Outputs[i].Prune = true;
                            }
                }


                if (countnd % 2 != 0)
                    neurons = Math.Min(neurons + 1, Params.Neurons); //Increase the number of neurons slowly
                else
                    dims = Math.Min(dims + 1, Params.Dimensions); //And then increase the number of dimensions

                countnd ++;


                //Save best weights
                foreach (InputNeuron neuron in NeuronsInput)
                    neuron.SaveBestWeights();

                foreach (HiddenNeuron neuron in NeuronsHidden)
                    neuron.SaveBestWeights();

                NeuronBias.SaveBestWeights();

                // same for Activation function neuron if needed
                if (AdditionalNeuron) {
                    Params.ActFunction.Neuron.SaveBestWeights();
                    for (int j = 0; j < 7; j++) {
                        Params.ActFunction.Neuron.Outputs[j].Weight = Params.ActFunction.Neuron.Outputs[j].BestCase;
                    }
                }

                successCount++;

                InvokeMethodForNeuralNet(EndCycleMethod);
            }
        }


        /// <summary>
        /// Init neural network parameters
        /// all arrays should have +1 length
        /// </summary>
        private void Init(double[] sourceArray) {

            nmax = sourceArray.Length;
            xmax = Ext.countMaxAbs(sourceArray);

            // create array with data
            xdata = new double[nmax + 1];
            xdata[0] = 0;

            for (int i = 1; i <= nmax; i++)
                xdata[i] = sourceArray[i - 1];

            ConstructNetwork();


            TEN_POW_MIN_PRUNING = Math.Pow(10, -Params.Pruning);
            MIN_D5_DIV_D = Math.Min(Params.Dimensions, 5) / Params.Dimensions;
            NMAX_MINUS_D_XMAX_POW_E = (nmax - Params.Dimensions) * Math.Pow(xmax, Params.ErrorsExponent);
            
            neurons = Params.Neurons;
            dims = Params.Dimensions;

            xlast = new double[Params.Dimensions + 1];

            ddw = Params.MaxPertrubation;

        }


        private static void ConstructNetwork()
        {
            // init input layer
            NeuronsInput = new InputNeuron[Params.Dimensions + 1];
            for (int i = 0; i < Params.Dimensions + 1; i++)
            {
                NeuronsInput[i] = new InputNeuron();
                NeuronsInput[i].Outputs = new Synapse[Params.Neurons];
            }

            // init hidden layer
            NeuronsHidden = new HiddenNeuron[Params.Neurons];
            for (int i = 0; i < Params.Neurons; i++)
            {
                NeuronsHidden[i] = new HiddenNeuron();
                NeuronsHidden[i].Inputs = new Synapse[Params.Dimensions + 1];
                NeuronsHidden[i].Outputs = new Synapse[1];
            }

            // init bias neuron
            NeuronBias = new BiasNeuron();
            NeuronBias.Outputs = new Synapse[1];
            NeuronBias.Outputs[0] = new Synapse();

            // init output layer
            NeuronOutput = new OutputNeuron();
            NeuronOutput.Inputs = new Synapse[Params.Neurons];
            NeuronOutput.Outputs = new Synapse[1];
            NeuronOutput.Outputs[0] = new Synapse();


            //Connect input and hidden layer neurons
            for (int i = 0; i < Params.Dimensions + 1; i++)
                for(int j = 0; j < Params.Neurons; j++)
                {
                    Synapse synapse = new Synapse();
                    NeuronsInput[i].Outputs[j] = synapse;
                    NeuronsHidden[j].Inputs[i] = synapse;
                }

            //Connect hidden and output layer neurons
            for (int i = 0; i < Params.Neurons; i++)
            {
                Synapse synapse = new Synapse();
                NeuronsHidden[i].Outputs[0] = synapse;
                NeuronOutput.Inputs[i] = synapse;
            }
        }
    }
}
