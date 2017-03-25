using MonkeyOthello.Core;
using MonkeyOthello.Engines;
using MonkeyOthello.Utils;
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
    public interface IColosseum
    {
        IEnumerable<IEngine> FindGladiators();
        void Fight(IEnumerable<IEngine> engines, int count = 1);
    }

    public class Colosseum : IColosseum
    {
        private string targetPath = Path.Combine(Environment.CurrentDirectory, "fight-results");

        public Colosseum()
        {
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
        }

        public void Fight(IEnumerable<IEngine> engines, int count = 1)
        {
            foreach (var engine in engines)
            {
                engine.UpdateProgress = r => Console.WriteLine($"[{engine.Name}] {r}");
            }

            var i = 0;
            while (i++ < count)
            {
                engines.PK((e1, e2) =>
                {
                    var board = BitBoard.NewGame();
                    Fight(e1, e2, targetPath, board);
                });

            }
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

        public void Fight(IEngine engineA, IEngine engineB, string targetPath, BitBoard board)
        {
            var targetFile = Path.Combine(targetPath,
                                          string.Format("{0:yyyy-MM-dd HH-mm} {1}-{2}.txt", DateTime.Now, engineA.Name, engineB.Name));

            using (var cc =  ConsoleCopy.Create(targetFile))
            {
                Console.WriteLine("################### Begin #######################");
                Console.WriteLine("{0} ({2}) vs {1} ({3})", engineA.Name, engineB.Name, "Black", "White");

                var fightResult = Fight(engineA, engineB, board);

                Console.WriteLine("################### Result #######################");
                Console.WriteLine("{0}", fightResult);
                Console.WriteLine("#################### End #######################");
            }
        }

        private FightResult Fight(IEngine engineA, IEngine engineB, BitBoard board)
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
                var depth = 8;

                var sw = Stopwatch.StartNew();
                var searchResult = engines[turn].Search(board.Copy(), depth);
                sw.Stop();
                timespans[turn] += sw.Elapsed;

                Console.Title = $"[{board.EmptyPiecesCount()}][{depth}] [{engines[turn].Name}] {board.PlayerPiecesCount()}:{board.OpponentPiecesCount()}";
                Console.Title += $" {searchResult.Score}";
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

    }

    static class Extensions
    {
        public static void PK<T>(this IEnumerable<T> list, Action<T, T> action) where T : class
        {
            foreach (T t1 in list)
                foreach (T t2 in list)
                    if (t1 != t2)
                        action(t1, t2);
        }
    }

    public class FightResult
    {

        public FightResult()
        {

        }

        public FightResult(BitBoard board, IEngine[] engines, int turn)
        {
            var diff = board.PlayerPiecesCount() - board.OpponentPiecesCount();

            var winnerIndex = diff > 0 ? turn : 1 - turn;

            WinnerName = engines[winnerIndex].Name;
            LoserName = engines[1 - winnerIndex].Name;
            WinnerStoneType = winnerIndex == 0 ? "Black" : "White";
            Score = Math.Abs(diff);
        }

        public string WinnerName { get; set; }
        public string LoserName { get; set; }
        public string WinnerStoneType { get; set; }
        public int Score { get; set; }
        public TimeSpan TimeSpan { get; set; }

        public override string ToString()
        {
            return string.Format("Winner:{0},{1} Loser:{2}, Score:{3}, TimeSpan:{4}",
                                 WinnerName,
                                 WinnerStoneType,
                                 LoserName,
                                 Score,
                                 TimeSpan);
        }
    }
}
