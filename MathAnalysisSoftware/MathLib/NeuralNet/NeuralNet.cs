using MathLib.MathMethods.Lyapunov;
using MathLib.NeuralNet.Entities;
using System;

namespace MathLib.NeuralNetwork {
    public class NeuralNet {

        public static BenettinResult Task_Result;
        public static NeuralNetParams Params;
        public static NeuralNetEquations System_Equations;

        public static InputNeuron[] NeuronsInput;
        public static HiddenNeuron[] NeuronsHidden;
        public static OutputNeuron NeuronOutput;
        public static BiasNeuron NeuronBias;
        public static BiasNeuron NeuronConstant;

        //----- input data
        private static long nmax;  //lines in file
        public static double[] xdata;

        //----- pre-calculated constants
        private static double TEN_POW_MIN_PRUNING;      
        private static double MIN_D5_DIV_D;
        private static double NMAX_MINUS_D_XMAX_POW_E;

        private static int neurons, dims;
        private static int countnd = 1;    //Allows for introducing n and d gradually

        private static double ddw;

        //counters
        public static int _c, successCount;

        private static int improved = 0;
        private static int seed;

        private static bool AdditionalNeuron;


        public NeuralNet(NeuralNetParams taskParams, double[] array) {
            Params = taskParams;
            AdditionalNeuron = Params.ActFunction.AdditionalNeuron;
            System_Equations = new NeuralNetEquations(Params.Dimensions, Params.Neurons, Params.ActFunction);
            Init(array);
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
                    neuron.BestToMemory();

                foreach (HiddenNeuron neuron in NeuronsHidden)
                    neuron.BestToMemory();

                NeuronBias.BestToMemory();
                NeuronConstant.BestToMemory();


                // same for Activation function neuron if needed
                if (AdditionalNeuron)
                {
                    Params.ActFunction.Neuron.BestToMemory();
                    if (Params.ActFunction.Neuron.Outputs[0].Memory == 0)
                        Params.ActFunction.Neuron.Outputs[0].Memory = 1;
                }


                int N_MUL_D_MINCT_PLUS_1_PLUS_N_PLUS_1 = neurons * (dims - Params.ConstantTerm + 1) + neurons + 1;

                for (_c = 1; _c <= Params.CMax; _c++) {

                    if (Params.Pruning == 0)
                        foreach (HiddenNeuron neuron in NeuronsHidden)
                            foreach (Synapse synapse in neuron.Inputs)
                                synapse.Prune = false;
                    
                    int prunes = 0;

                    for (int i = 0; i < neurons; i++)
                    {
                        for (int j = 0; j < dims; j++)
                            if (NeuronsInput[j].Outputs[i].Weight == 0)
                                prunes++;

                        if (NeuronConstant.Outputs[i].Weight == 0 && Params.ConstantTerm == 0)
                            prunes++;
                    }


                    //Probability of changing a given parameter at each trial
                    double pc = 1d / Math.Sqrt(N_MUL_D_MINCT_PLUS_1_PLUS_N_PLUS_1 - prunes); // 1d / Sqrt(neurons * (dims - Task_Params.ConstantTerm + 1) + neurons + 1 - prunes)

                    if (Params.BiasTerm == 0)
                    {
                        NeuronBias.CalculateWeight(0, ddw);
                    }
                    else
                        NeuronBias.Outputs[0].Weight = 0;

                    for (int i = 0; i < neurons; i++) {

                        NeuronsHidden[i].CalculateWeight(0, ddw, 9 * pc);

                        if(Params.ConstantTerm == 0)
                        {
                            NeuronConstant.CalculateWeight(i, ddw, pc);

                            //This connection has been pruned
                            if (NeuronConstant.Outputs[i].Prune)
                                NeuronConstant.Outputs[i].Weight = 0;
                        }

                        //Eliminates constant term if ct=1
                        for (int j = 0; j < dims; j++) {
                
                            //Reduce neighborhood for large j by a factor of 1-32
                            double dj = 1d / Math.Pow(2, MIN_D5_DIV_D * j);

                            NeuronsInput[j].CalculateWeight(i, ddw * dj, pc);
                
                            //This connection has been pruned
                            if (NeuronsInput[j].Outputs[i].Prune)
                                NeuronsInput[j].Outputs[i].Weight = 0; 
                        }
                    }


                    // same for Activation function neuron if needed
                    if (AdditionalNeuron) {
                        for (int j = 0; j < 7; j++) {
                            Params.ActFunction.Neuron.CalculateWeight(j, ddw, pc);
                        }
                    }

                    double e1 = 0;

                    for (int k = Params.Dimensions; k < nmax; k++) {
            
                        double x = NeuronBias.Outputs[0].Weight;

                        for (int i = 0; i < Params.Neurons; i++) {
                            double arg = NeuronConstant.Outputs[i].Weight;

                            for (int j = 0; j < Params.Dimensions; j++)
                                arg += NeuronsInput[j].Outputs[i].Weight * xdata[k - j - 1];

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
                            neuron.WeightsToMemory();

                        foreach (HiddenNeuron neuron in NeuronsHidden)
                            neuron.WeightsToMemory();

                        NeuronBias.WeightsToMemory();
                        NeuronConstant.WeightsToMemory();


                        // same for Activation function neuron if needed
                        if (AdditionalNeuron) {
                            Params.ActFunction.Neuron.WeightsToMemory();
                        }
                    }
                    else if (ddw > 0 && improved == 0) {
                        ddw = -ddw;     //Try going in the opposite direction
                    }
                    //Reseed the random if the trial failed
                    else {
                        seed = Neuron.Randomizer.Next(int.MaxValue);     //seed = (int) (1 / Math.Sqrt(e1));
            
                        if (improved > 0) {
                            ddw = Math.Min(Params.MaxPertrubation, (1 + improved / Params.TestingInterval) * Math.Abs(ddw));
                            improved = 0;
                        }
                        else
                            ddw = Params.Eta * Math.Abs(ddw);
                    }

                    Neuron.Randomizer = new Random(seed);
        
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
                        {
                            for (int j = 0; j < Params.Dimensions; j++)
                            {
                                double aBest = NeuronsInput[j].Outputs[i].Memory;
                                double bBest = NeuronsHidden[i].Outputs[0].Memory;
                                if (aBest != 0 && Math.Abs(aBest * bBest) < TEN_POW_MIN_PRUNING)
                                    NeuronsInput[j].Outputs[i].Prune = true;
                            }

                            if (NeuronConstant.Outputs[i].Memory != 0 && Math.Abs(NeuronConstant.Outputs[i].Memory * NeuronsHidden[i].Outputs[0].Memory) < TEN_POW_MIN_PRUNING)
                                NeuronConstant.Outputs[i].Prune = true;
                        }
                }


                if (countnd % 2 != 0)
                    neurons = Math.Min(neurons + 1, Params.Neurons); //Increase the number of neurons slowly
                else
                    dims = Math.Min(dims + 1, Params.Dimensions); //And then increase the number of dimensions

                countnd ++;


                //Save best weights
                foreach (InputNeuron neuron in NeuronsInput)
                    neuron.MemoryToBest();

                foreach (HiddenNeuron neuron in NeuronsHidden)
                    neuron.MemoryToBest();

                NeuronBias.MemoryToBest();
                NeuronConstant.MemoryToBest();

                // same for Activation function neuron if needed
                if (AdditionalNeuron) {
                    Params.ActFunction.Neuron.MemoryToBest();
                    foreach (Synapse synapse in Params.ActFunction.Neuron.Outputs)
                        synapse.Weight = synapse.BestCase;
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
            double xmax = Ext.countMaxAbs(sourceArray);

            // create array with data
            xdata = new double[nmax];

            for (int i = 0; i < nmax; i++)
                xdata[i] = sourceArray[i];

            ConstructNetwork();


            TEN_POW_MIN_PRUNING = Math.Pow(10, -Params.Pruning);
            MIN_D5_DIV_D = Math.Min(Params.Dimensions, 5) / Params.Dimensions;
            NMAX_MINUS_D_XMAX_POW_E = (nmax - Params.Dimensions) * Math.Pow(xmax, Params.ErrorsExponent);
            
            neurons = Params.Neurons;
            dims = Params.Dimensions;

            ddw = Params.MaxPertrubation;

        }


        private static void ConstructNetwork()
        {
            //random = new Random();
            Neuron.Randomizer = new Random();

            // init input layer
            NeuronsInput = new InputNeuron[Params.Dimensions];
            for (int i = 0; i < Params.Dimensions; i++)
            {
                InputNeuron neuron = new InputNeuron(Params.Neurons, Params.Nudge);
                NeuronsInput[i] = neuron;
            }

            // init hidden layer
            NeuronsHidden = new HiddenNeuron[Params.Neurons];
            for (int i = 0; i < Params.Neurons; i++)
            {
                HiddenNeuron neuron = new HiddenNeuron(Params.Dimensions, 1, Params.Nudge);
                NeuronsHidden[i] = neuron;
            }

            // init bias neuron
            NeuronBias = new BiasNeuron(1, Params.Nudge);
            NeuronBias.Outputs[0] = new Synapse();


            // init output layer
            NeuronOutput = new OutputNeuron(Params.Neurons, Params.Nudge);
            NeuronOutput.Outputs[0] = new Synapse();

            NeuronConstant = new BiasNeuron(Params.Neurons, Params.Nudge);
            for (int i = 0; i < Params.Neurons; i++)
                NeuronConstant.Outputs[i] = new Synapse();


            //Connect input and hidden layer neurons
            for (int i = 0; i < Params.Dimensions; i++)
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
