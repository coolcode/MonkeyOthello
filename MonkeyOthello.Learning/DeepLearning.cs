using Accord.Math;
using Accord.Neuro;
using Accord.Neuro.Learning;
using Accord.Neuro.Networks;
using Accord.Statistics;
using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Learning
{
    public class DeepLearning
    {
        private static readonly string networkPath = Path.Combine(Environment.CurrentDirectory, "networks");

        static DeepLearning()
        {
            CreateDirectoryIfNotExist(networkPath);
        }


        public static void Test()
        {
            var from = 29;
            var to = 33;

            PrepareData($@"E:\projects\MonkeyOthello\tests\", from, to);
            TrainAll(from, to);
            TestAll(from, to);
            //LoadItems();
            //VaildateDeepLearningEngine(30);

        }


        public static void TrainAll(int from, int to)
        {
            for (var i = from; i <= to; i++)
            {
                Console.WriteLine($"begin training {i} {"-".Repeat(20)}");
                var sw = Stopwatch.StartNew();
                var networkFile = Path.Combine(networkPath, $@"deeplearning-{i}.net");
                var dataPath = Path.Combine(Environment.CurrentDirectory, $@"knowledge\{i}\");
                Train(dataPath, networkFile);
                sw.Stop();
                Console.WriteLine($"done training {i}, {sw.Elapsed} ");
            }
        }

        public static void TestAll(int from, int to)
        {
            for (var i = from; i <= to; i++)
            {
                Console.WriteLine($"begin test {i} {"-".Repeat(20)}");
                var sw = Stopwatch.StartNew();
                var networkFile = Path.Combine(networkPath, $@"deeplearning-{i}.net");
                var dataPath = Path.Combine(Environment.CurrentDirectory, $@"knowledge\{i}\");
                TestByRate(dataPath, networkFile, 0.2, 0.01, 1.0);
                sw.Stop();
                Console.WriteLine($"done test {i}, {sw.Elapsed} ");
            }
        }

        public static void PrepareData(string basePath, int from, int to)
        {
            var savedBasePath = Path.Combine(Environment.CurrentDirectory, @"knowledge\");
            CreateDirectoryIfNotExist(savedBasePath);

            for (var i = from; i <= to; i++)
            {
                var folder = $@"k-dl-{i}\knowledge\";
                Console.WriteLine($"visit {folder}");
                var path = Path.Combine(basePath, folder);
                var kfiles = Directory.GetFiles(path, "*.k");
                foreach (var kfile in kfiles)
                {
                    var originalFileName = Path.GetFileName(kfile);
                    var index = int.Parse(originalFileName.Split('-')[0]);
                    var savedPath = Path.Combine(savedBasePath, $@"{index}\");
                    CreateDirectoryIfNotExist(savedPath);
                    var savedFilePath = Path.Combine(savedPath, $"[{i}]{originalFileName}");
                    File.Copy(kfile, savedFilePath, true);
                    Console.WriteLine($"copy '{originalFileName}' to '{savedFilePath.Replace(AppContext.BaseDirectory, "")}'");
                }
            }
        }

        private static void Train(string dataPath, string networkFile)
        {
            var items = ConvertData(LoadItems(dataPath));
            var count = items.Length;
            Console.WriteLine($"items: {count}");
            var inputs = items.Select(x => x.Inputs).ToArray();
            var outputs = items.Select(x => x.Outputs).ToArray();

            {
                Learn(networkFile, inputs, outputs, trainRate: 0.99);
            }
            {
                var n = (int)(count * 0.0);
                var testInputs = inputs.Skip(n).ToArray();
                var testOutputs = outputs.Skip(n).ToArray();
                Test(networkFile, testInputs, testOutputs);
            }
        }


        private static void TestByRate(string dataPath, string networkFile, params double[] testRates)
        {
            var items = ConvertData(LoadItems(dataPath));
            var count = items.Length;
            var inputs = items.Select(x => x.Inputs).ToArray();
            var outputs = items.Select(x => x.Outputs).ToArray();

            foreach (var testRate in testRates)
            {
                Console.WriteLine($"test rate: {testRate:p0}");
                var n = (int)(count * (1 - testRate));
                var testInputs = inputs.Skip(n).ToArray();
                var testOutputs = outputs.Skip(n).ToArray();
                Test(networkFile, testInputs, testOutputs);
            }
        }

        private static void Learn(string networkFile, double[][] inputs, double[][] outputs, double trainRate = 0.8)
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
            Test(networkFile, testInputs, testOutputs);
        }


        private static void Test(string networkFile, double[][] inputs, double[][] outputs)
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

        private static void VaildateDeepLearningEngine(int empties)
        {
            var dataPath = Path.Combine(Environment.CurrentDirectory, $@"knowledge\{empties}\");
            var items = ConvertData(LoadItems(dataPath));
            var count = items.Length;
            var inputs = items.Select(x => x.Inputs).ToArray();
            var outputs = items.Select(x => x.Outputs).ToArray();

            Console.WriteLine($"[{empties}] test {inputs.Length} items--------------------------------");

            Console.WriteLine($"DeepBeliefNetwork--------------------------------");

            var networkFile = Path.Combine(networkPath, $@"deeplearning-{empties}.net");
            var network = DeepBeliefNetwork.Load(networkFile);
            Test(networkFile, inputs, outputs);

            Console.WriteLine($"DeepLearningEngine--------------------------------");
            int correct = 0;
            var error = 0.0;
            for (int i = 0; i < inputs.Length; i++)
            {
                var outputValues = DeepLearningEngineSearch(items[i].Board);
                if (Compare(outputValues, outputs[i]))
                {
                    correct++;
                }
                error += Error(outputValues, outputs[i]);

                if ((i + 1) % 10 == 0)
                {
                    Console.WriteLine("Correct " + correct + "/" + (i + 1) + ", " + Math.Round(((double)correct / (double)(i + 1) * 100), 2) + "%");
                }
            }

            Console.WriteLine("Correct " + correct + "/" + inputs.Length + ", " + Math.Round(((double)correct / (double)inputs.Length * 100), 2) + "%");

            error = Math.Sqrt(error / inputs.Length);
            Console.WriteLine($"Error: {error }");
        }

        private static double[] DeepLearningEngineSearch(BitBoard board)
        {
            var engine = new DeepLearningEngine();
            //var result = engine.Search(board, 2);
            var result = engine.Search(board, 4);

            if (result.Score >= 0)
            {
                return new[] { 1.0 };
            }

            return new[] { 0.0 };
        }

        class DataItem
        {
            public BitBoard Board { get; set; }
            public double[] Inputs { get; set; }
            public double[] Outputs { get; set; }
        }

        private static void CreateDirectoryIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static List<string> LoadItems(string dataPath)
        {
            var files = Directory.GetFiles(dataPath, "*.k", SearchOption.AllDirectories);
            Console.WriteLine($"found {files.Length} files");
            var list = new List<string>();
            foreach (var file in files)
            {
                var shortPath = file.Replace(dataPath, "");
                if (Path.GetFileName(file).StartsWith("19"))
                {
                    Console.WriteLine($"skip {shortPath}");
                    continue;
                }
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
                var board = new BitBoard(ulong.Parse(values[0]), ulong.Parse(values[1]));
                list.Add(new DataItem { Board = board, Inputs = inputs, Outputs = outputs });
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
            /*
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
            */
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
