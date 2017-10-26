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

        public static Neuron[] HiddenNeurons;
        public static Neuron OutputNeuron;

        public static Synapse Bias;

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

        public static double verybest, ebest;
        private static double ddw;

        //----- arrays
        public static double[] ex, exbest;

        public static double[] xlast;

        public static int successCount;

        private static int improved = 0;
        private static int seed;

        public static int _c;

        private static bool additionalArray;


        public NeuralNet(NeuralNetParams taskParams, double[] array) {
            Params = taskParams;
            System_Equations = new NeuralNetEquations(Params.Dimensions, Params.Neurons, Params.ActFunction);

            Init(array);
            additionalArray = Params.ActFunction.additionalArray;
        }


        public static MyMethodDelegate LoggingMethod = null;
        public static MyMethodDelegate EndCycleMethod = null;


        public delegate void MyMethodDelegate();

        public void InvokeMethodForNeuralNet(MyMethodDelegate method) {
            method.DynamicInvoke();
        }
        


        public void RunTask() {

            while (successCount < Params.Trainings) {

                if (verybest == 0) {
                    ebest = Params.TestingInterval;
                }
                else {
                    ebest = 10 * verybest;
                    ddw = Math.Min(Params.MaxPertrubation, Math.Sqrt(verybest));
                }

                Bias.WeightGood = Bias.WeightBest;
                foreach (Synapse synapse in OutputNeuron.Inputs)
                    synapse.WeightGood = synapse.WeightBest;

                foreach (Neuron neuron in HiddenNeurons) {
                    for (int j = 0; j <= Params.Dimensions; j++) {
                        neuron.Inputs[j].WeightGood = neuron.Inputs[j].WeightBest;
                    }
                }


                // TODO: 
                //----------------------------------------------------------------
                for (int j = 0; j <= 6; j++)
                    Params.ActFunction.cbest[j] = Params.ActFunction.cverybest[j];

                if (Params.ActFunction.cbest[0] == 0)
                    Params.ActFunction.cbest[0] = 1;
                //----------------------------------------------------------------

                int N_MUL_D_MINCT_PLUS_1_PLUS_N_PLUS_1 = neurons * (dims - Params.ConstantTerm + 1) + neurons + 1;

                for (_c = 1; _c <= Params.CMax; _c++) {

                    if (Params.Pruning == 0)
                        foreach (Neuron neuron in HiddenNeurons)
                            foreach (Synapse synapse in neuron.Inputs)
                                synapse.Prune = false;
                        
                    double e1 = 0;
                    int prunes = 0;

                    for (int i = 0; i < neurons; i++)
                        for (int j = Params.ConstantTerm; j <= dims; j++)
                            if (HiddenNeurons[i].Inputs[j].Weight == 0)
                                prunes++;

                    //Probability of changing a given parameter at each trial
                    double pc = 1d / Math.Sqrt(N_MUL_D_MINCT_PLUS_1_PLUS_N_PLUS_1 - prunes); // 1d / Sqrt(neurons * (dims - Task_Params.ConstantTerm + 1) + neurons + 1 - prunes)

                    if (Params.BiasTerm == 0)
                    {
                        Bias.Weight = Bias.WeightGood + ddw * (Ext.Gauss2(random) - Params.Nudge * Math.Sign(Bias.WeightGood));
                    }
                    else
                        Bias.Weight = 0;

                    for (int i = 0; i < neurons; i++) {
                        double wBest = OutputNeuron.Inputs[i].WeightGood;
                        OutputNeuron.Inputs[i].Weight = wBest;
                        if (random.NextDouble() < 9 * pc)
                            OutputNeuron.Inputs[i].Weight += ddw * (Ext.Gauss2(random) - Params.Nudge * Math.Sign(wBest));
            
                        //Eliminates constant term if ct=1
                        for (int j = Params.ConstantTerm; j <= dims; j++) {
                
                            //Reduce neighborhood for large j by a factor of 1-32
                            double dj = 1d/Math.Pow(2, MIN_D5_DIV_D * j);
                            double aBest = HiddenNeurons[i].Inputs[j].WeightGood;
                            HiddenNeurons[i].Inputs[j].Weight = aBest;
                
                            if (random.NextDouble() < pc )
                                HiddenNeurons[i].Inputs[j].Weight += ddw * dj * (Ext.Gauss2(random) - Params.Nudge * Math.Sign(aBest));
                
                            //This connection has been pruned
                            if (HiddenNeurons[i].Inputs[j].Prune)
                                HiddenNeurons[i].Inputs[j].Weight = 0; 
                        }
                    }


                    // TODO: 
                    //----------------------------------------------------------------
                    if (additionalArray) {
                        for (int j = 0; j <= 6; j++) {
                            Params.ActFunction.c[j] = Params.ActFunction.cbest[j];
                            if (random.NextDouble() < pc)
                                Params.ActFunction.c[j] = Params.ActFunction.cbest[j] + ddw * (Ext.Gauss2(random) - Params.Nudge * Math.Sign(Params.ActFunction.cbest[j]));
                        }
                    }
                    //----------------------------------------------------------------


                    for (int k = Params.Dimensions + 1; k <= nmax; k++) {
            
                        for (int j = 1; j <= Params.Dimensions; j++)
                            xlast[j] = xdata[k - j];
            
                        x = Bias.Weight;

                        for (int i = 0; i < Params.Neurons; i++) {
                            arg = HiddenNeurons[i].Inputs[0].Weight;

                            for (int j = 1; j <= Params.Dimensions; j++)
                                arg += HiddenNeurons[i].Inputs[j].Weight * xlast[j];

                            x += OutputNeuron.Inputs[i].Weight * Params.ActFunction.Phi(arg);
                        }

                        //Error in the prediction of the k-th data point
                        ex[k] = Math.Abs(x - xdata[k]);
                        e1 += Math.Pow(ex[k], Params.ErrorsExponent);
                    }


                    //"Mean-square" error (even for e&<>2)
                    e1 = Math.Pow(e1 / NMAX_MINUS_D_XMAX_POW_E, 2 / Params.ErrorsExponent); 

                    if (e1 < ebest) {

                        improved ++;
                        ebest = e1;

                        Bias.WeightGood = Bias.Weight;
                        foreach (Synapse synapse in OutputNeuron.Inputs)
                            synapse.WeightGood = synapse.Weight;
            
                        foreach (Neuron neuron in HiddenNeurons) {
                            for (int j = 0; j <= Params.Dimensions; j++)
                                neuron.Inputs[j].WeightGood = neuron.Inputs[j].Weight;
                        }


                        // TODO: 
                        //----------------------------------------------------------------
                        if (additionalArray) {
                            for (int j = 0; j <= 6; j++)
                                Params.ActFunction.cbest[j] = Params.ActFunction.c[j];
                        }
                        //----------------------------------------------------------------
                        

                        for (int k = 0; k <= nmax; k++)
                            exbest[k] = ex[k];
                    }
                    else if (ddw > 0 && improved == 0) {
                        ddw = -ddw;     //Try going in the opposite direction
                    }
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


                    // TODO: 
                    //-------------------------------------------------------------------
                    InvokeMethodForNeuralNet(LoggingMethod);
        

                    if(ebest > verybest && verybest != 0)
                        continue;


                    // TODO: 
                    //----------------------------------------------------------------
                    if (additionalArray) {
                        for (int j = 0; j <= 6; j++)
                            Params.ActFunction.c[j] = Params.ActFunction.cbest[j];
                    }
                    //----------------------------------------------------------------

     		
                    //Mark the weakconnections for pruning
                    if (Params.Pruning != 0) 
                        for (int i = 0; i < Params.Neurons; i++)
                            for (int j = 0; j <= Params.Dimensions; j++)
                            {
                                double aBest = HiddenNeurons[i].Inputs[j].WeightGood;
                                double bBest = OutputNeuron.Inputs[i].WeightGood;
                                if (aBest != 0 && Math.Abs(aBest * bBest) < TEN_POW_MIN_PRUNING)
                                    HiddenNeurons[i].Inputs[j].Prune = true;
                            }
                                
                }


                if (countnd % 2 != 0)
                    neurons = Math.Min(neurons + 1, Params.Neurons); //Increase the number of neurons slowly
                else
                    dims = Math.Min(dims + 1, Params.Dimensions); //And then increase the number of dimensions

                countnd ++;

                //Save the very best case
                verybest = ebest;

                Bias.WeightBest = Bias.WeightGood;
                foreach (Synapse synapse in OutputNeuron.Inputs)
                    synapse.WeightBest = synapse.WeightGood;
                
                foreach (Neuron neuron in HiddenNeurons) {
                    for (int j = 0; j <= Params.Dimensions; j++)
                        neuron.Inputs[j].WeightBest = neuron.Inputs[j].WeightGood;
                }


                // TODO: 
                //----------------------------------------------------------------
                if (additionalArray) {
                    for (int j = 0; j <= 6; j++) {
                        Params.ActFunction.cverybest[j] = Params.ActFunction.cbest[j];
                        Params.ActFunction.c[j] = Params.ActFunction.cverybest[j];
                    }
                }
                //----------------------------------------------------------------


                successCount++;

                // TODO: 
                //-------------------------------------------------------------------
                InvokeMethodForNeuralNet(EndCycleMethod);
            }
        }


        /// <summary>
        /// Init neural network parameters
        /// all arrays should have +1 length
        /// </summary>
        private void Init(double[] sourceArray) {

            HiddenNeurons = new Neuron[Params.Neurons];
            for (int i = 0; i < Params.Neurons; i++)
                HiddenNeurons[i] = new Neuron(Params.ActFunction, Params.Dimensions + 1);

            OutputNeuron = new Neuron(Params.ActFunction, Params.Neurons);

            Bias = new Synapse();

            nmax = sourceArray.Length;
            xmax = Ext.countMaxAbs(sourceArray);

            // create array with data
            xdata = new double[nmax + 1];
            xdata[0] = 0;

            for (int i = 1; i <= nmax; i++)
                xdata[i] = sourceArray[i - 1];
            
            TEN_POW_MIN_PRUNING = Math.Pow(10, -Params.Pruning);
            MIN_D5_DIV_D = Math.Min(Params.Dimensions, 5) / Params.Dimensions;
            NMAX_MINUS_D_XMAX_POW_E = (nmax - Params.Dimensions) * Math.Pow(xmax, Params.ErrorsExponent);
            
            neurons = Params.Neurons;
            dims = Params.Dimensions;

            ex = new double[nmax + 1];
            exbest = new double[nmax + 1];

            xlast = new double[Params.Dimensions + 1];

            ddw = Params.MaxPertrubation;

        }

    }
}
