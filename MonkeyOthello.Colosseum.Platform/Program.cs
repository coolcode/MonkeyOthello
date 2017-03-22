using MonkeyOthello.Core;
using MonkeyOthello.Engines;
using MonkeyOthello.Engines.X;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Colosseum.Platform
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var sw = Stopwatch.StartNew();

                //var ownBoard = BitBoard.Parse("-X-O------XOXX--OOOXXXX-OOOOXXX-OXXOXXX-O-XX------XX------------");
                //var oppBoard = Rule.MoveSwitch(ownBoard, 2);

                //var oppBoard = BitBoard.Parse("----------XOXX--OOOXXXX-OOOOXXX-OXXOXXX-------------------------");
                //FightPlatform.Analyze(oppBoard);

                FightPlatform. GenerateKnowledge();

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
