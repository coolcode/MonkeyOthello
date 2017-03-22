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
                GenerateKnowledge();
                sw.Stop();
                Console.WriteLine($"done! {sw.Elapsed}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.Read();
        }

        public static void GenerateKnowledge()
        {
            var timeout = 60;
            var engines = new IEngine[] {
                new EdaxEngine { Timeout = timeout },
                new FuzzyEngine(new EdaxEngine { Timeout = timeout }, new RandomEngine())
            };

            foreach (var engine in engines)
            {
                engine.UpdateProgress = UpdateProgress;
            }

            var endEngine = new EdaxEngine { Timeout = timeout, UpdateProgress = UpdateProgress };

            var trainDepth = 20;
            int.TryParse(ConfigurationManager.AppSettings["TrainDepth"], out trainDepth);
            var midGameDepth = 16;
            int.TryParse(ConfigurationManager.AppSettings["MidGameDepth"], out midGameDepth);
            var endGameDepth = EdaxEngine.EndGameDepth;

            Console.WriteLine($"TrainDepth: {trainDepth}, MidGameDepth:{midGameDepth}");

            IColosseum game = new TrainableColosseum(endEngine, trainDepth, midGameDepth, endGameDepth);
            game.Fight(engines, 5000);
        }

        private static void UpdateProgress(SearchResult result)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {result}");
        }

    }
}
