using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.Learning;
using Accord.Neuro.Networks;
using Accord.Statistics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Learning
{
    public class DLDemo
    {
        public static void Run()
        {
            double[][] inputs;
            double[][] outputs;
            double[][] testInputs;
            double[][] testOutputs;

            // Load ascii digits dataset.
            inputs = DataManager.Load(@"data2.txt", out outputs);

            // The first 500 data rows will be for training. The rest will be for testing.
            testInputs = inputs.Skip(500).ToArray();
            testOutputs = outputs.Skip(500).ToArray();
            inputs = inputs.Take(500).ToArray();
            outputs = outputs.Take(500).ToArray();

            // Setup the deep belief network and initialize with random weights.
            DeepBeliefNetwork network = new DeepBeliefNetwork(inputs.First().Length, 10, 10);
            new GaussianWeights(network, 0.1).Randomize();
            network.UpdateVisibleWeights();

            // Setup the learning algorithm.
            DeepBeliefNetworkLearning teacher = new DeepBeliefNetworkLearning(network)
            {
                Algorithm = (h, v, i) => new ContrastiveDivergenceLearning(h, v)
                {
                    LearningRate = 0.1,
                    Momentum = 0.5,
                    Decay = 0.001,
                }
            };

            // Setup batches of input for learning.
            int batchCount = Math.Max(1, inputs.Length / 100);
            // Create mini-batches to speed learning.
            int[] groups = Accord.Statistics.Tools.RandomGroups(inputs.Length, batchCount);
            double[][][] batches = inputs.Subgroups(groups);
            // Learning data for the specified layer.
            double[][][] layerData;

            // Unsupervised learning on each hidden layer, except for the output layer.
            for (int layerIndex = 0; layerIndex < network.Machines.Count - 1; layerIndex++)
            {
                teacher.LayerIndex = layerIndex;
                layerData = teacher.GetLayerInput(batches);
                for (int i = 0; i < 200; i++)
                {
                    double error = teacher.RunEpoch(layerData) / inputs.Length;
                    if (i % 10 == 0)
                    {
                        Console.WriteLine(i + ", Error = " + error);
                    }
                }
            }

            // Supervised learning on entire network, to provide output classification.
            var teacher2 = new BackPropagationLearning(network)
            {
                LearningRate = 0.1,
                Momentum = 0.5
            };

            // Run supervised learning.
            for (int i = 0; i < 500; i++)
            {
                double error = teacher2.RunEpoch(inputs, outputs) / inputs.Length;
                if (i % 10 == 0)
                {
                    Console.WriteLine(i + ", Error = " + error);
                }
            }

            // Test the resulting accuracy.
            int correct = 0;
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] outputValues = network.Compute(testInputs[i]);
                if (DataManager.FormatOutputResult(outputValues) == DataManager.FormatOutputResult(testOutputs[i]))
                {
                    correct++;
                }
            }

            Console.WriteLine("Correct " + correct + "/" + inputs.Length + ", " + Math.Round(((double)correct / (double)inputs.Length * 100), 2) + "%");
            Console.Write("Press any key to quit ..");
            Console.ReadKey();


        }
    }

    public static class DataManager
    {
        public static double[][] Load(string pathName, out double[][] outputs)
        {
            List<double[]> list = new List<double[]>();
            List<double[]> output = new List<double[]>();

            // Read data file.
            using (FileStream fs = File.Open(pathName, FileMode.Open, FileAccess.Read))
            {
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        List<double> row = new List<double>();

                        bool readOutput = false;

                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            // Collect each 0 and 1 from the data.
                            foreach (char ch in line)
                            {
                                if (!readOutput)
                                {
                                    // Reading input.
                                    if (ch != ' ' && ch != '\n')
                                    {
                                        // Add this digit to our input.
                                        row.Add(Double.Parse(ch.ToString()));
                                    }
                                    else if (ch == ' ')
                                    {
                                        // End of input reached. Store the input row.
                                        list.Add(row.ToArray());

                                        // Start a new input row.
                                        row = new List<double>();

                                        // Set flag to read output label.
                                        readOutput = true;
                                    }
                                }
                                else
                                {
                                    // Read output label.
                                    output.Add(FormatOutputVector(Double.Parse(ch.ToString())));

                                    // Set flag to read inputs for next row.
                                    readOutput = false;
                                }
                            }
                        }
                    }
                }
            }

            // Set outputs.
            outputs = output.ToArray();

            // Return inputs;
            return list.ToArray();
        }

        #region Utility Methods

        /// <summary>
        /// Converts a numeric output label (0, 1, 2, 3, etc) to its cooresponding array of doubles, where all values are 0 except for the index matching the label (ie., if the label is 2, the output is [0, 0, 1, 0, 0, ...]).
        /// </summary>
        /// <param name="label">double</param>
        /// <returns>double[]</returns>
        public static double[] FormatOutputVector(double label)
        {
            double[] output = new double[10];

            for (int i = 0; i < output.Length; i++)
            {
                if (i == label)
                {
                    output[i] = 1;
                }
                else
                {
                    output[i] = 0;
                }
            }

            return output;
        }

        /// <summary>
        /// Finds the largest output value in an array and returns its index. This allows for sequential classification from the outputs of a neural network (ie., if output at index 2 is the largest, the classification is class "3" (zero-based)).
        /// </summary>
        /// <param name="output">double[]</param>
        /// <returns>double</returns>
        public static double FormatOutputResult(double[] output)
        {
            return output.ToList().IndexOf(output.Max());
        }

        #endregion
    }
}
