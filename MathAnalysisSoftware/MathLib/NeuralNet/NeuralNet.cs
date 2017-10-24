using MathLib.MathMethods.Lyapunov;
using MathLib.NeuralNet.Entities;
using System;

namespace MathLib.NeuralNetwork {
    public class NeuralNet {

        public static double arg;

        public static BenettinResult Task_Result;
        public static NeuralNetParams Task_Params;
        public static NeuralNetEquations System_Equations;

        private static Random random = new Random();

        public static Neuron[] HiddenNeurons;
        public static Neuron OutputNeuron;

        //----- input data
        private static long nmax;  //lines in file
        private static double xmax;//max value
        public static double[] xdata;

        //----- pre-calculated constants
        private static double TEN_POW_MIN_PRUNING;      
        private static double MIN_D5_DIV_D;
        private static double NMAX_MINUSD_XMAX_POW2;

        private static int neurons;
        private static int dims;
        private static int countnd = 1;    //Allows for introducing n and d gradually

        private static double x, y;

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
            Task_Params = taskParams;
            System_Equations = new NeuralNetEquations(Task_Params.Dimensions, Task_Params.Neurons, Task_Params.ActFunction);

            Init(array);
            additionalArray = Task_Params.ActFunction.additionalArray;
        }


        public static MyMethodDelegate LoggingMethod = null;
        public static MyMethodDelegate EndCycleMethod = null;


        public delegate void MyMethodDelegate();

        public void InvokeMethodForNeuralNet(MyMethodDelegate method) {
            method.DynamicInvoke();
        }
        


        public void RunTask() {

            while (successCount < Task_Params.Trainings) {

                if (verybest == 0) {
                    ebest = Task_Params.TestingInterval;
                }
                else {
                    ebest = 10 * verybest;
                    ddw = Math.Min(Task_Params.MaxPertrubation, Math.Sqrt(verybest));
                }

                foreach (Synapse synapse in OutputNeuron.Inputs)
                    synapse.WBest = synapse.WVeryBest;

                for (int i = 1; i <= Task_Params.Neurons; i++) {
                    for (int j = 0; j <= Task_Params.Dimensions; j++) {
                        HiddenNeurons[i].Inputs[j].WBest = HiddenNeurons[i].Inputs[j].WVeryBest;
                    }
                }


                // TODO: 
                //----------------------------------------------------------------
                for (int j = 0; j <= 6; j++)
                    Task_Params.ActFunction.cbest[j] = Task_Params.ActFunction.cverybest[j];

                if (Task_Params.ActFunction.cbest[0] == 0)
                    Task_Params.ActFunction.cbest[0] = 1;
                //----------------------------------------------------------------

                int N_MUL_D_MINCT_PLUS_1_PLUS_N_PLUS_1 = neurons * (dims - Task_Params.ConstantTerm + 1) + neurons + 1;

                for (_c = 1; _c <= Task_Params.CMax; _c++) {

                    if (Task_Params.Pruning == 0)
                        foreach (Neuron neuron in HiddenNeurons)
                            foreach (Synapse synapse in neuron.Inputs)
                                synapse.Prune = false;
                        
                    double e1 = 0;
                    int prunes = 0;

                    for (int i = 1; i <= neurons; i++)
                        for (int j = Task_Params.ConstantTerm; j <= dims; j++)
                            if (HiddenNeurons[i].Inputs[j].Weight == 0)
                                prunes++;

                    //Probability of changing a given parameter at each trial
                    //double pc = 1d / Math.Sqrt(neurons * (dims - Task_Params.ConstantTerm + 1) + neurons + 1 - prunes);
                    double pc = 1d / Math.Sqrt(N_MUL_D_MINCT_PLUS_1_PLUS_N_PLUS_1 - prunes); // optimization

                    if (Task_Params.BiasTerm == 0)
                    {
                        double wBest = OutputNeuron.Inputs[0].WBest;
                        OutputNeuron.Inputs[0].Weight = wBest + ddw * (Ext.Gauss2(random) - Task_Params.Nudge * Math.Sign(wBest));
                    }
                    else
                        OutputNeuron.Inputs[0].Weight = 0;

                    for (int i = 1; i <= neurons; i++) {
                        double wBest = OutputNeuron.Inputs[i].WBest;
                        OutputNeuron.Inputs[i].Weight = wBest;
                        if (random.NextDouble() < 9 * pc)
                            OutputNeuron.Inputs[i].Weight += ddw * (Ext.Gauss2(random) - Task_Params.Nudge * Math.Sign(wBest));
            
                        //Eliminates constant term if ct=1
                        for (int j = Task_Params.ConstantTerm; j <= dims; j++) {
                
                            //Reduce neighborhood for large j by a factor of 1-32
                            double dj = 1d/Math.Pow(2, MIN_D5_DIV_D * j);
                            double aBest = HiddenNeurons[i].Inputs[j].WBest;
                            HiddenNeurons[i].Inputs[j].Weight = aBest;
                
                            if (random.NextDouble() < pc )
                                HiddenNeurons[i].Inputs[j].Weight += ddw * dj * (Ext.Gauss2(random) - Task_Params.Nudge * Math.Sign(aBest));
                
                            //This connection has been pruned
                            if (HiddenNeurons[i].Inputs[j].Prune)
                                HiddenNeurons[i].Inputs[j].Weight = 0; 
                        }
                    }


                    // TODO: 
                    //----------------------------------------------------------------
                    if (additionalArray) {
                        for (int j = 0; j <= 6; j++) {
                            Task_Params.ActFunction.c[j] = Task_Params.ActFunction.cbest[j];
                            if (random.NextDouble() < pc)
                                Task_Params.ActFunction.c[j] = Task_Params.ActFunction.cbest[j] + ddw * (Ext.Gauss2(random) - Task_Params.Nudge * Math.Sign(Task_Params.ActFunction.cbest[j]));
                        }
                    }
                    //----------------------------------------------------------------


                    for (int k = Task_Params.Dimensions + 1; k <= nmax; k++) {
            
                        for (int j = 1; j <= Task_Params.Dimensions; j++)
                            xlast[j] = xdata[k - j];
            
                        x = OutputNeuron.Inputs[0].Weight;

                        for (int i = 1; i <= Task_Params.Neurons; i++) {
                            arg = HiddenNeurons[i].Inputs[0].Weight;

                            for (int j = 1; j <= Task_Params.Dimensions; j++)
                                arg += HiddenNeurons[i].Inputs[j].Weight * xlast[j];

                            x += OutputNeuron.Inputs[i].Weight * Task_Params.ActFunction.Phi(arg);
                        }

                        //Error in the prediction of the k-th data point
                        //ex[k] = Math.Abs(x - xdata[k]);
                        ex[k] = x - xdata[k]; // for e == 2

                        //---------------- optimization
                        //if (e == 2) { 
                        e1 += ex[k] * ex[k];  // for e == 2
                        //}
                        //else { 
                            //e1 += Math.Pow(ex[k], e);
                        //}
                    }

                    //---------------- optimization
                    //e1 = Math.Pow(e1 / ((nmax - d) * Math.Pow(xmax, e)), 2 / e); //"Mean-square" error (even for e&<>2)
                    e1 /= NMAX_MINUSD_XMAX_POW2; //"Mean-square" error

                    if (e1 < ebest) {

                        improved ++;
                        ebest = e1;

                        foreach (Synapse synapse in OutputNeuron.Inputs)
                            synapse.WBest = synapse.Weight;
            
                        for (int i = 1; i <= Task_Params.Neurons; i++) {
                            for (int j = 0; j <= Task_Params.Dimensions; j++)
                                HiddenNeurons[i].Inputs[j].WBest = HiddenNeurons[i].Inputs[j].Weight;
                        }


                        // TODO: 
                        //----------------------------------------------------------------
                        if (additionalArray) {
                            for (int j = 0; j <= 6; j++)
                                Task_Params.ActFunction.cbest[j] = Task_Params.ActFunction.c[j];
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
                            ddw = Math.Min(Task_Params.MaxPertrubation, (1 + improved / Task_Params.TestingInterval) * Math.Abs(ddw));
                            improved = 0;
                        }
                        else
                            ddw = Task_Params.Eta * Math.Abs(ddw);
                    }

                    random = new Random((int)seed);
        
                    //Testing is costly - don't do it too often
                    if (_c % Task_Params.TestingInterval != 0) continue;


                    // TODO: 
                    //-------------------------------------------------------------------
                    InvokeMethodForNeuralNet(LoggingMethod);
        

                    if(ebest > verybest && verybest != 0) continue;


                    // TODO: 
                    //----------------------------------------------------------------
                    if (additionalArray) {
                        for (int j = 0; j <= 6; j++)
                            Task_Params.ActFunction.c[j] = Task_Params.ActFunction.cbest[j];
                    }
                    //----------------------------------------------------------------

     		
                    //Mark the weakconnections for pruning
                    if (Task_Params.Pruning != 0) 
                        for (int i = 1; i <= Task_Params.Neurons; i++)
                            for (int j = 0; j <= Task_Params.Dimensions; j++)
                            {
                                double aBest = HiddenNeurons[i].Inputs[j].WBest;
                                double bBest = OutputNeuron.Inputs[i].WBest;
                                if (aBest != 0 && Math.Abs(aBest * bBest) < TEN_POW_MIN_PRUNING)
                                    HiddenNeurons[i].Inputs[j].Prune = true;
                            }
                                
                }


                if (countnd % 2 != 0)
                    neurons = Math.Min(neurons + 1, Task_Params.Neurons); //Increase the number of neurons slowly
                else
                    dims = Math.Min(dims + 1, Task_Params.Dimensions); //And then increase the number of dimensions

                countnd ++;

                //Save the very best case
                verybest = ebest;

                foreach (Synapse synapse in OutputNeuron.Inputs)
                    synapse.WVeryBest = synapse.WBest;
                
                for (int i = 1; i <= Task_Params.Neurons; i++) {
                    for (int j = 0; j <= Task_Params.Dimensions; j++)
                        HiddenNeurons[i].Inputs[j].WVeryBest = HiddenNeurons[i].Inputs[j].WBest;
                }


                // TODO: 
                //----------------------------------------------------------------
                if (additionalArray) {
                    for (int j = 0; j <= 6; j++) {
                        Task_Params.ActFunction.cverybest[j] = Task_Params.ActFunction.cbest[j];
                        Task_Params.ActFunction.c[j] = Task_Params.ActFunction.cverybest[j];
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

            HiddenNeurons = new Neuron[Task_Params.Neurons + 1];
            for (int i = 0; i < Task_Params.Neurons + 1; i++)
                HiddenNeurons[i] = new Neuron(Task_Params.ActFunction, Task_Params.Dimensions + 1);

            OutputNeuron = new Neuron(Task_Params.ActFunction, Task_Params.Neurons + 1);


            nmax = sourceArray.Length;
            xmax = Ext.countMaxAbs(sourceArray);

            // create array with data
            xdata = new double[nmax + 1];
            xdata[0] = 0;

            for (int i = 1; i <= nmax; i++)
                xdata[i] = sourceArray[i - 1];
            
            TEN_POW_MIN_PRUNING = Math.Pow(10, -Task_Params.Pruning);
            MIN_D5_DIV_D = Math.Min(Task_Params.Dimensions, 5) / Task_Params.Dimensions;
            NMAX_MINUSD_XMAX_POW2 = (nmax - Task_Params.Dimensions) * xmax * xmax;
            
            neurons = Task_Params.Neurons;
            dims = Task_Params.Dimensions;

            ex = new double[nmax + 1];
            exbest = new double[nmax + 1];

            

            xlast = new double[Task_Params.Dimensions + 1];

            ddw = Task_Params.MaxPertrubation;

        }

    }
}
