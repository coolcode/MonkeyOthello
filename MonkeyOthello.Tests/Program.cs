using MonkeyOthello.Learning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Utils.ConsoleCopy.Create();
                var sw = Stopwatch.StartNew();
                //Console.WriteLine(int.MaxValue);
                //Console.WriteLine(ulong.MaxValue);
                //Parallel.For(0, 1, Console.WriteLine);
                Test.TrainOpeningBook();
                //Controller.Run();
                //Test.ValidateDeepLearningResult();
                //DeepLearning.Test();
                //DeepLearningDemo.Run();
                //Test.TestEndGameEngine();
                //Test.GenerateKnowledge();
                //Test.Fight();
                //Test.TestBitBoard();
                //Test.TestLinkedList();
                //Test.GenTestData();
                //Test.TestEndGameSearch();
                //Test.TestV2IndexToV3Index();
                //Test.TestFlips();
                sw.Stop();
                Console.WriteLine($"done! {sw.Elapsed}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.Read();
        }
    }
}
