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
    public class DeepLearning
    {
        private static readonly string dataPath = @"k-data\";//edax-fuzzy\random\
        private static int empties = 19;
        private static readonly string networkPath = Path.Combine(Environment.CurrentDirectory, "networks");
        private static string networkFile;

        static DeepLearning()
        {
            if (!Directory.Exists(networkPath))
            {
                Directory.CreateDirectory(networkPath);
            }

            networkFile = Path.Combine(networkPath, $@"all-deeplearning-{empties}.net");
        }

        public static void Test()
        {
            //LoadItems();
            TrainTest();
        }

        private static void TrainTest()
        {
            var items = ConvertData(LoadItems());
            var count = items.Length;
            Console.WriteLine($"items: {items.Length}");
            var inputs = items.Select(x => x.inputs).ToArray();
            var outputs = items.Select(x => x.outputs).ToArray();

            {
                Learn(inputs, outputs, trainRate: 0.99);
            }
            {
                var n = (int)(count * 0.8);
                var testInputs = inputs.Skip(n).ToArray();
                var testOutputs = outputs.Skip(n).ToArray();
                Test(testInputs, testOutputs);
            }
            {
                var n = (int)(count * 0.0);
                var testInputs = inputs.Skip(n).ToArray();
                var testOutputs = outputs.Skip(n).ToArray();
                Test(testInputs, testOutputs);
            }
        }

        private static void Test(double[][] inputs, double[][] outputs)
        {
            Console.WriteLine($"test {inputs.Length} items--------------------------------");
            var network = DeepBeliefNetwork.Load(networkFile);
            // Test the resulting accuracy.
            int correct = 0;
            var error = 0.0;
            for (int i = 0; i < inputs.Length; i++)
            {
                double[] outputValues = network.Compute(inputs[i]);
                if (Compare(outputValues, outputs[i]))
                {
                    correct++;
                }
                error += Error(outputValues, outputs[i]);
            }

            Console.WriteLine("Correct " + correct + "/" + inputs.Length + ", " + Math.Round(((double)correct / (double)inputs.Length * 100), 2) + "%");

            error = Math.Sqrt(error / inputs.Length);
            Console.WriteLine($"Error: {error }");
        }

        class DataItem
        {
            public double[] inputs { get; set; }
            public double[] outputs { get; set; }
        }

        private static List<string> LoadItems()
        {
            var files = Directory.GetFiles(dataPath, "*.k", SearchOption.AllDirectories);
            Console.WriteLine($"found {files.Length} files");
            var list = new List<string>();
            foreach (var file in files)
            {
                var shortPath = file.Replace(dataPath, "");
                Console.WriteLine($"reading {shortPath}");
                var lines = File.ReadAllLines(file);
                list.AddRange(lines);
            }

            var total = list.Count;
            Console.WriteLine($"total: {total} items");
            var uni = list.Distinct().ToList();
            Console.WriteLine($"uni: {uni.Count} items");
            Console.WriteLine($"duplicae: {total - uni.Count} items");

            return uni;
        }

        private static DataItem[] ConvertData(IEnumerable<string> items)
        {
            var list = new List<DataItem>();
            foreach (var item in items)
            {
                var values = item.Split(',');
                var inputs = ToInputValues(values[0]).Concat(ToInputValues(values[1])).ToArray();
                var outputs = ToOutputValues(values[2]);
                list.Add(new DataItem { inputs = inputs, outputs = outputs });
            }

            return list.ToArray();
        }

        private static double[] ToInputValues(string text)
        {
            var bits = ulong.Parse(text);
            var values = new double[64];
            for (var i = 0; i < values.Length; i++)
            {
                if ((bits & (1UL << i)) != 0)
                {
                    values[i] = 1.0;
                }
                else
                {
                    values[i] = 0.0;
                }
            }

            return values;
        }

        private static double[] ToOutputValues(string text)
        {
            //>=0 win, otherwise lose
            return int.Parse(text) >= 0 ? new[] { 1.0 } : new[] { 0.0 };
            var eval = int.Parse(text);
            if (eval == 0)
            {
                return new[] { 0.0, 0.0, 0.0 };
            }
            else if (eval > 0)
            {
                return eval > 20 ? new[] { 1.0, 1.0, 1.0 } : new[] { 1.0, 1.0, 0.0 };
            }
            else
            {
                return eval < -20 ? new[] { 0.0, 0.0, 0.1 } : new[] { 0.0, 1.0, 0.0 };
            }

            //data range:[-64,64] total: 129, belongs to(2^7,2^8) so use 8 bits array to store outputs
            //0: -64, 1:-63 ... 64:128
            /*var eval = int.Parse(text) + 64;
            
            var values = new double[8];
            for (var i = 0; i < values.Length; i++)
            {
                if ((eval & (1 << i)) != 0)
                {
                    values[i] = 1.0;
                }
                else
                {
                    values[i] = 0.0;
                }
            }*/

            /*
            var values = Enumerable.Repeat(0.0,128).ToArray();
            if (eval > 0)
            {
                values[eval - 1] = 1.0;
            }
           
            return values; */
        }

        private static void Learn(double[][] inputs, double[][] outputs, double trainRate = 0.8)
        {
            var count = inputs.Length;
            var n = (int)(count * trainRate);
            var trainedInputs = inputs.Take(n).ToArray();
            var trainedOutputs = outputs.Take(n).ToArray();
            var testInputs = inputs.Skip(n).ToArray();
            var testOutputs = outputs.Skip(n).ToArray();

            Console.WriteLine($"trained items: {trainedInputs.Length}, tested items: {testInputs.Length}");

            var network = new DeepBeliefNetwork(trainedInputs.First().Length, 10, trainedOutputs.First().Length);
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
            int batchCount = Math.Max(1, trainedInputs.Length / 100);
            // Create mini-batches to speed learning.
            int[] groups = Classes.Random(trainedInputs.Length, batchCount);
            double[][][] batches = trainedInputs.Subgroups(groups);
            // Learning data for the specified layer.
            double[][][] layerData;

            // Unsupervised learning on each hidden layer, except for the output layer.
            for (int layerIndex = 0; layerIndex < network.Machines.Count - 1; layerIndex++)
            {
                teacher.LayerIndex = layerIndex;
                layerData = teacher.GetLayerInput(batches);
                for (int i = 0; i < 200; i++)
                {
                    double error = teacher.RunEpoch(layerData) / trainedInputs.Length;
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
            for (int i = 0; i < Math.Min(2000, n); i++)
            {
                double error = teacher2.RunEpoch(trainedInputs, trainedOutputs) / trainedInputs.Length;
                if (i % 10 == 0)
                {
                    Console.WriteLine(i + ", Error = " + error);
                }
            }
            network.Save(networkFile);
            Console.WriteLine($"save network: {networkFile}");

            // Test the resulting accuracy.
            Test(testInputs, testOutputs);
        }

        private static bool Compare(double[] d1, double[] d2)
        {
            d1 = d1.Select(x => x > 0.5 ? 1.0 : 0.0).ToArray();
            return d1.Zip(d2, (x1, x2) => x1 == x2).All(r => r);
        }

        private static double Error(double[] d1, double[] d2)
        {
            var v1 = ToInt(d1);
            var v2 = ToInt(d2);

            return Math.Pow(v1 - v2, 2);
        }

        private static int ToInt(double[] d)
        {
            var v = 0;
            for (var i = 0; i < d.Length; i++)
            {
                if (d[i] > 0.5)
                {
                    v ^= (1 << i);
                }
            }

            return v;
        }
    }
}
