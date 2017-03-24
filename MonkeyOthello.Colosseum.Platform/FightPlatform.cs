using MonkeyOthello.Core;
using MonkeyOthello.Engines;
using MonkeyOthello.Engines.X;
using MonkeyOthello.Learning;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Colosseum.Platform
{
    class FightPlatform
    {
        private const int timeout = 30;

        private static readonly IEngine endEngine = new EdaxEngine
        {
            Timeout = timeout,
            UpdateProgress = UpdateProgress
        };

        public static void GenerateKnowledge()
        {
            var engines = new IEngine[] {
                new EdaxEngine { Timeout = timeout },
                new FuzzyEngine(new EdaxEngine { Timeout = timeout }, new RandomEngine())
            };

            foreach (var engine in engines)
            {
                engine.UpdateProgress = UpdateProgress;
            }

            //var endEngine = new EdaxEngine { Timeout = timeout, UpdateProgress = UpdateProgress };

            var trainDepth = 20;
            int.TryParse(ConfigurationManager.AppSettings["TrainDepth"], out trainDepth);
            var midGameDepth = 16;
            int.TryParse(ConfigurationManager.AppSettings["MidGameDepth"], out midGameDepth);
            var endGameDepth = EdaxEngine.EndGameDepth;

            Console.WriteLine($"TrainDepth: {trainDepth}, MidGameDepth:{midGameDepth}");

            IColosseum game = new TrainableColosseum(endEngine, trainDepth, midGameDepth, endGameDepth);
            game.Fight(engines, 5000);
        }


        public static void GenerateKnowledgeBaseOnNetwork()
        {
            var engines = new IEngine[] {
                new EdaxEngine { Timeout = timeout },
                new FuzzyEngine(new EdaxEngine { Timeout = timeout }, new RandomEngine())
            };

            foreach (var engine in engines)
            {
                engine.UpdateProgress = UpdateProgress;
            }

            var endEngine = new DeepLearningEngine();
                       
            var trainDepth = 8;
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

        public static void Analyze(string pattern)
        {
            //var pattern = "-X-O------XOXX--OOOXXXX-OOOOXXX-OXXOXXX-O-XX------XX------------O";
            var color = 'w';

            var board = BitBoard.Parse(pattern, color != 'w');

            Analyze(board);
        }


        public static void Analyze(BitBoard board)
        {
            var moves = Rule.FindMoves(board);
            if (moves.Length > 0)
            {
                var bestScore = -64;
                var bestMove = -1;
                foreach (var move in moves)
                {
                    var oppboard = Rule.MoveSwitch(board, move);

                    var own = false;
                    if (!Rule.CanMove(oppboard))
                    {
                        oppboard = oppboard.Switch();
                        own = true;
                    }

                    var sr = AnalyzeEndGame(oppboard);
                    var eval = own ? sr.Score : -sr.Score;//opp's score
                    if (eval > bestScore)
                    {
                        bestScore = eval;
                        bestMove = move;
                    }
                    Console.WriteLine($"move:{move} {move.ToNotation()}, score:{eval}, result: {sr}");
                }
            }
        }

        private static SearchResult AnalyzeEndGame(BitBoard board)
        {
            var empties = board.EmptyPiecesCount();
            Console.WriteLine($"start to analyze end game. empties:{empties} ");
            var sw = Stopwatch.StartNew();

            SearchResult sr;
            if (board.IsGameOver())
            {
                var eval = board.EndDiffCount(); ;
                sr = new SearchResult { Score = eval, Message = "game over" };
            }
            else
            {
                sr = endEngine.Search(board, empties);
            }
            sw.Stop();
            Console.WriteLine($"finish analyzing end game. spent: {sw.Elapsed}");
            //SaveResult(board, sr);

            return sr;
        }
    }
}
