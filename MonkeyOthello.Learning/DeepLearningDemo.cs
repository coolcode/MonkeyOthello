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
    public class DeepLearningDemo
    {
        private static readonly string file = "data.txt";
        private const int count = 10000;

        public static void Run()
        {
            GenData();
            var data = LoadData();
            var inputs = data.Select(x => x.inputs).ToArray();
            var outputs = data.Select(x => x.outputs).ToArray();
            Learn(inputs, outputs);
        }

        private static double[] Func(double[] inputs)
        {
            //var v = inputs[0] * 1.2 + inputs[1] * 6.6 + inputs[2] * 2.1;
            //var o = ((int)v) / 10.0;//0.0-0.9
            var v = inputs.Count(x => x > 0.5);
            var o = Enumerable.Repeat(0.0, 10).ToArray();
            if (v > 0)
            {
                o[v - 1] = 1.0;
            }
            return o;
        }

        private static void GenData()
        {
            var lines = new List<string>();
            var rand = new Random();
            for (var i = 0; i < count; i++)
            {
                var inputs = Enumerable.Range(0, 10).Select(x => rand.NextDouble() > 0.5 ? 1.0 : 0.0).ToArray();
                //var inputs = new double[] { rand.NextDouble(), rand.NextDouble(), rand.NextDouble(), };
                var outputs = Func(inputs);
                lines.Add($"{string.Join(",", inputs)} {string.Join(",", outputs)}");
            }

            File.WriteAllLines(file, lines);
        }

        class DataItem
        {
            public double[] inputs { get; set; }
            public double[] outputs { get; set; }
        }

        private static DataItem[] LoadData()
        {
            var list = new List<DataItem>();
            var lines = File.ReadAllLines(file);
            foreach (var item in lines)
            {
                var values = item.Split(' ');
                var inputs = values[0].Split(',').Select(double.Parse).ToArray();
                var outputs = values[1].Split(',').Select(double.Parse).ToArray();
                list.Add(new DataItem { inputs = inputs, outputs = outputs });
            }

            return list.ToArray();
        }

        public static void Learn(double[][] inputs, double[][] outputs)
        {
            var n = (int)(count * 0.8);
            var testInputs = inputs.Skip(n).ToArray();
            var testOutputs = outputs.Skip(n).ToArray();
            inputs = inputs.Take(n).ToArray();
            outputs = outputs.Take(n).ToArray();

            var network = new DeepBeliefNetwork(inputs.First().Length, 10, 10);
            new GaussianWeights(network, 0.1).Randomize();
            network.UpdateVisibleWeights();

            // Setup the learning algorithm.
            var teacher = new DeepBeliefNetworkLearning(network)
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
            int[] groups = Classes.Random(inputs.Length, batchCount);
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
            for (int i = 0; i < n; i++)
            {
                double error = teacher2.RunEpoch(inputs, outputs) / inputs.Length;
                if (i % 10 == 0)
                {
                    Console.WriteLine(i + ", Error = " + error);
                }
            }

            // Test the resulting accuracy.
            int correct = 0;
            for (int i = 0; i < testInputs.Length; i++)
            {
                double[] outputValues = network.Compute(testInputs[i]);
                if (Compare(outputValues, testOutputs[i]))
                {
                    correct++;
                }
            }

            Console.WriteLine("Correct " + correct + "/" + testInputs.Length + ", " + Math.Round(((double)correct / (double)testInputs.Length * 100), 2) + "%");

        }

        private static bool Compare(double[] d1, double[] d2)
        {
            d1 = d1.Select(x => x > 0.5 ? 1.0 : 0.0).ToArray();
            return d1.Zip(d2, (x1, x2) => x1 == x2).All(r => r);
        }
    }
}
