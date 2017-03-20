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
                var sw = Stopwatch.StartNew();
                //DeepLearning.Run();
                // Test.TestEndGameEngine();
                 //Test.GenerateKnowledge();
                //DeepLearningDemo.Run();
                //DLDemo.Run();
                 //Test.Fight();
                //Test.TestBitBoard();
                //Test.TestLinkedList();
                //Test.GenTestData();
              Test.TestEndGameSearch();
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
