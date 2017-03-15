using System;
using System.Collections.Generic;
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
                Test.TestEndGameSearch();
                Console.WriteLine("done!");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            Console.Read();
        }
    }
}
