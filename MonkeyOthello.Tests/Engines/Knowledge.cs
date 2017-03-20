using MonkeyOthello.Core;
using MonkeyOthello.Engines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Tests.Engines
{
    public class Knowledge : IColosseum
    {
        private string fightPath = Path.Combine(Environment.CurrentDirectory, "fight-hist");
        private string knowledgePath = Path.Combine(Environment.CurrentDirectory, "knowledge");

        public Knowledge()
        {
            if (!Directory.Exists(fightPath))
            {
                Directory.CreateDirectory(fightPath);
            }
            if (!Directory.Exists(knowledgePath))
            {
                Directory.CreateDirectory(knowledgePath);
            }
        }

        public void Fight(IEnumerable<IEngine> engines, int count = 1)
        {
            var i = 0;
            var sw = Stopwatch.StartNew();
            while (i++ < count)
            {
                try
                {
                    engines.PK((e1, e2) =>
                    {
                        Console.Title = $"{i} {e1.Name} vs {e2.Name} [{sw.Elapsed}]";
                        var board = BitBoard.NewGame();
                        Fight(e1, e2, board, i);
                    });

                    Console.WriteLine($"loop {i}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error:{ex}");
                }
            }

            sw.Stop();
            Console.Title += $"[done!] [{sw.Elapsed}]";
        }

        public IEnumerable<IEngine> FindGladiators()
        {
            var enginesPath = Path.Combine(Environment.CurrentDirectory, "engines");
            foreach (var file in Directory.GetFiles(enginesPath))
            {
                var ass = Assembly.LoadFrom(file);
                var engines = ass.GetTypes().Where(t => t.IsSubclassOf(typeof(IEngine)));
                foreach (var engine in engines)
                {
                    yield return (IEngine)Activator.CreateInstance(engine);
                }
            }
        }

        public void Fight(IEngine engineA, IEngine engineB, BitBoard board, int index = 0)
        {
            var indexText = index.ToString().PadLeft(6, '0');
            var targetFile = Path.Combine(fightPath,
                $"{indexText} {DateTime.Now:yyyy-MM-dd HH-mm} {engineA.Name}-{engineB.Name}.tmp");

            FightResult fightResult;
            using (var cc = new ConsoleCopy(targetFile))
            {
                Console.WriteLine("################### Begin #######################");
                Console.WriteLine("{0} ({2}) vs {1} ({3})", engineA.Name, engineB.Name, "Black", "White");

                fightResult = DoFight(engineA, engineB, board);

                Console.WriteLine("################### Result #######################");
                Console.WriteLine("{0}", fightResult);
                Console.WriteLine("#################### End #######################");
            }

            var score = fightResult.WinnerName == engineA.Name ? fightResult.Score : -fightResult.Score;
            var newTargetFile = Path.Combine(fightPath,
                                          $"{indexText} {engineA.Name}-{engineB.Name} ({score}) {DateTime.Now:yyyy-MM-dd HH-mm}.txt");

            File.Move(targetFile, newTargetFile);
        }

        private FightResult DoFight(IEngine engineA, IEngine engineB, BitBoard board)
        {
            var clock = new Clock();
            clock.Start();

            var engines = new IEngine[] { engineA, engineB };
            var timespans = new TimeSpan[] { TimeSpan.Zero, TimeSpan.Zero };
            var colors = new[] { "Black", "White" };
            int turn = 0;
            Console.WriteLine(board.Draw(colors[turn]));

            while (!board.IsFull)
            {
                var empties = board.EmptyPiecesCount();
                if (empties <= 20)
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

                            var sr = AnalyzeEndGame(oppboard, turn);
                            var eval = own ? sr.Score : -sr.Score;//opp's score
                            if (eval > bestScore)
                            {
                                bestScore = eval;
                                bestMove = move;
                            }
                            Console.WriteLine($"color:{turn} move:{move}, score:{eval}");
                        }

                        var winIndex = bestScore >= 0 ? turn : 1 - turn;
                        return new FightResult
                        {
                            WinnerName = engines[winIndex].Name,
                            LoserName = engines[1 - winIndex].Name,
                            WinnerStoneType = colors[winIndex],
                            Score = Math.Abs(bestScore),
                            TimeSpan = clock.Elapsed
                        };
                    }
                }

                var depth = 0;

                var sw = Stopwatch.StartNew();
                var searchResult = engines[turn].Search(board.Copy(), depth);
                sw.Stop();
                timespans[turn] += sw.Elapsed;

                Console.WriteLine($"[{engines[turn].Name}][{board.EmptyPiecesCount()}] {searchResult}");

                if (searchResult.Move < 0 ||
                    searchResult.Move >= Constants.StonesCount ||
                    !Rule.FindMoves(board).Contains(searchResult.Move))
                {
                    clock.Stop();
                    //error
                    Console.WriteLine("----------------Error Move----------------------");
                    Console.WriteLine(board.Draw(colors[turn]));

                    return new FightResult()
                    {
                        WinnerName = engines[1 - turn].Name,
                        LoserName = engines[turn].Name,
                        WinnerStoneType = colors[turn],
                        Score = 1,
                        TimeSpan = clock.Elapsed
                    };
                }
                else
                {
                    board = Rule.MoveSwitch(board, searchResult.Move);
                    Console.WriteLine(board.Draw(colors[turn]));
                }

                turn = 1 ^ turn;

                var canFlip = Rule.CanMove(board);

                if (!canFlip)
                {
                    //pass
                    Console.WriteLine($"{engines[turn].Name} pass");
                    turn = 1 ^ turn;
                    board = board.Switch();
                    canFlip = Rule.CanMove(board);

                    if (!canFlip)
                    {
                        Console.WriteLine($"{engines[turn].Name} pass, game over!");
                        AnalyzeEndGame(board, turn);
                        //both pass
                        break;
                    }
                }
            }

            clock.Stop();
            Console.WriteLine("################### Game Over #######################");

            for (var i = 0; i < 2; i++)
            {
                Console.WriteLine($"Total spent time({engines[i].Name}): {timespans[i]}");
            }

            return new FightResult(board, engines, turn)
            {
                TimeSpan = clock.Elapsed
            };
        }

        private SearchResult AnalyzeEndGame(BitBoard board, int turn)
        {
            var empties = board.EmptyPiecesCount();
            Console.WriteLine($"start to analyze end game. empties:{empties}, color: {turn}");
            var sw = Stopwatch.StartNew();

            SearchResult sr;
            if (board.IsGameOver())
            {
                var eval = board.EndDiffCount(); ;
                sr = new SearchResult { Score = eval, Message = "game over" };
            }
            else
            {
                var endGameEngine = new EndGameEngine();
                sr = endGameEngine.Search(board, empties);
            }
            sw.Stop();
            Console.WriteLine($"finish analyzing end game. spent: {sw.Elapsed}");
            SaveResult(board, sr);

            return sr;
        }

        private void SaveResult(BitBoard board, SearchResult sr)
        {
            var empties = board.EmptyPiecesCount();
            var targetFile = Path.Combine(knowledgePath, $"{empties}-{DateTime.Today:yyyy-MM-dd}.k");

            var content = $"{board.PlayerPieces},{board.OpponentPieces},{sr.Score}";
            File.AppendAllLines(targetFile, new[] { content }, Encoding.UTF8);
        }
    }

}
