using MonkeyOthello.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Learning.Platform
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ConsoleCopy.Create();
                var sw = Stopwatch.StartNew();
                DeepLearning.Test();
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
