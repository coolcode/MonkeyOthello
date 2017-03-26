using MonkeyOthello.Core;
using MonkeyOthello.Engines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonkeyOthello.OpeningBook
{
    public class OpeningBookItem
    {
        public BitBoard Board { get; set; }

        public int Empties { get; set; }

        public List<EvalItem> EvalList { get; set; } = new List<EvalItem>();

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Board.PlayerPieces);
            sb.Append(",");
            sb.Append(Board.OpponentPieces);
            sb.Append(",");
            sb.Append(Empties);
            sb.Append(",");
            sb.Append(string.Join("/", EvalList.Select(x => $"{x.Move}:{x.Score}")));

            return sb.ToString();
        }

        public static OpeningBookItem Parse(string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                return null;
            }

            var sp = item.Split(',');
            var board = new BitBoard(ulong.Parse(sp[0]), ulong.Parse(sp[1]));
            var empties = int.Parse(sp[2]);
            var el = sp[3].Split('/');
            var evalList = (from q in el
                            let ms = q.Split(':').Select(int.Parse).ToArray()
                            select new EvalItem
                            {
                                Move = ms[0],
                                Score = ms[1]
                            }).ToList();

            return new OpeningBookItem
            {
                Board = board,
                Empties = empties,
                EvalList = evalList
            };
        }
    }


    public class Trainer
    {
        private readonly ConcurrentDictionary<BitBoard, OpeningBookItem> map = new ConcurrentDictionary<BitBoard, OpeningBookItem>();
        private readonly string bookPath = Path.Combine(Environment.CurrentDirectory, @"books\");

        public Trainer()
        {
            if (!Directory.Exists(bookPath))
            {
                Directory.CreateDirectory(bookPath);
            }

            Load();
        }

        private void Load()
        {
            foreach (var file in Directory.GetFiles(bookPath, "*.lmf"))
            {
                var items = File.ReadAllLines(file).Select(OpeningBookItem.Parse).ToList();
                Console.WriteLine($"load {items.Count} items from {Path.GetFileName(file)}");

                foreach (var item in items)
                {
                    map[item.Board] = item;
                }
            }
        }

        public void Train(Func<IEngine> tutorFunc, int depth, int me = 54)
        {
            var beginEmpties = 60;

            var queue = new ConcurrentQueue<BitBoard>();
            if (map.Count == 0)
            {
                queue.Enqueue(BitBoard.NewGame());
            }
            else
            {
                //var me = map.Values.Min(x => x.Empties);
                var items = map.Values.Where(x => x.Empties == me).ToArray();
                foreach (var item in items)
                {
                    queue.Enqueue(item.Board);
                }
                beginEmpties = me;
            }

            var maxThreads = 16;
            var engines = new IEngine[maxThreads];
            for (var i = 0; i < maxThreads; i++)
            {
                engines[i] = tutorFunc();
            }

            //ThreadPool.SetMaxThreads(10, 10);
            var sw = Stopwatch.StartNew();
            Console.WriteLine($"left:{queue.Count}, {sw.Elapsed}");
            while (queue.Count > 0)
            {
                var threadNum = Math.Min(queue.Count, maxThreads);
                Parallel.For(0, threadNum, x =>
                 {
                     BitBoard board;
                     queue.TryDequeue(out board);
                     var empties = board.EmptyPiecesCount();

                     //Console.WriteLine($"visit: {board}, empties:{empties}, left:{queue.Count}");
                     //Console.WriteLine($"left:{queue.Count}");

                     var moves = Rule.FindMoves(board);
                     if (moves.Length == 0)
                     {
                         //board = board.Switch();
                         return;
                     }

                     //var bestScore = -64 - 1;
                     //var bestMove = -1;

                     var evalList = new List<EvalItem>();
                     foreach (var move in moves)
                     {
                         var oppboard = Rule.MoveSwitch(board, move);

                         var own = false;
                         if (!Rule.CanMove(oppboard))
                         {
                             oppboard = oppboard.Switch();
                             own = true;
                         }

                         if (oppboard.EmptyPiecesCount() > beginEmpties - depth)
                         {
                             //extend nodes
                             queue.Enqueue(oppboard);
                             //Console.WriteLine($"add: {oppboard}, empties:{oppboard.EmptyPiecesCount()}, left:{queue.Count}");
                         }

                         OpeningBookItem ob1;
                         if (map.TryGetValue(board, out ob1))
                         {
                             continue;
                         }

                         IEngine tutor = engines[x];

                         var sr = tutor.Search(oppboard, 16);


                         var eval = own ? sr.Score : -sr.Score;//opp's score
                         //if (eval > bestScore)
                         //{
                         //    bestScore = eval;
                         //    bestMove = move;
                         //}

                         evalList.Add(new EvalItem
                         {
                             Move = move,
                             Score = eval,
                         });
                     }

                     OpeningBookItem ob;
                     if (map.TryGetValue(board, out ob))
                     {
                         //Console.WriteLine($"board exists.");
                         return;
                     }

                     var openbookItem = new OpeningBookItem
                     {
                         Board = board,
                         Empties = empties,
                         EvalList = evalList
                     };

                     lock (this)
                     {
                         File.AppendAllLines(Path.Combine(bookPath, $"book-{empties}.lmf"), new[] { openbookItem.ToString() });

                         map[board] = openbookItem;
                     }

                     Console.WriteLine($"add book item: {openbookItem}.");

                 });

                Console.WriteLine($"left:{queue.Count}, {sw.Elapsed}");

            }
        }
    }
}
