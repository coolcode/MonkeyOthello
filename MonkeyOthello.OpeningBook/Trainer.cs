using MonkeyOthello.Core;
using MonkeyOthello.Engines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public IEngine TutorEngine { get; set; }

        private readonly Dictionary<BitBoard, OpeningBookItem> map = new Dictionary<BitBoard, OpeningBookItem>();
        private readonly string bookPath = Path.Combine(Environment.CurrentDirectory, @"books\");

        public Trainer(IEngine tutor)
        {
            TutorEngine = tutor;
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
                var items = File.ReadAllLines(file).Select(OpeningBookItem.Parse);

                foreach (var item in items)
                {
                    map[item.Board] = item;
                }
            }
        }

        public void Train(int depth)
        {
            var beginEmpties = 60;
            var queue = new Queue<BitBoard>();
            if (map.Count == 0)
            {
                queue.Enqueue(BitBoard.NewGame());
            }
            else
            {
                var me = map.Values.Min(x => x.Empties);
                var items = map.Values.Where(x => x.Empties == me).ToArray();
                foreach (var item in items)
                {
                    queue.Enqueue(item.Board);
                }
                beginEmpties = me;
            }

            while (queue.Count > 0)
            {
                var board = queue.Dequeue();
                var empties = board.EmptyPiecesCount();

                Console.WriteLine($"visit: {board}, empties:{empties}, left:{queue.Count}");

                var moves = Rule.FindMoves(board);
                if (moves.Length == 0)
                {
                    //board = board.Switch();
                    continue;
                }

                var bestScore = -64 - 1;
                var bestMove = -1;

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
                        Console.WriteLine($"add: {oppboard}, empties:{oppboard.EmptyPiecesCount()}, left:{queue.Count}");
                    }

                    if (map.ContainsKey(board))
                    {
                        continue;
                    }

                    var sr = TutorEngine.Search(oppboard, 16);
                    var eval = own ? sr.Score : -sr.Score;//opp's score
                    if (eval > bestScore)
                    {
                        bestScore = eval;
                        bestMove = move;
                    }

                    evalList.Add(new EvalItem
                    {
                        Move = move,
                        Score = eval,
                    });
                }

                if (map.ContainsKey(board))
                {
                    Console.WriteLine($"{board} exists.");
                    continue;
                }

                var openbookItem = new OpeningBookItem
                {
                    Board = board,
                    Empties = empties,
                    EvalList = evalList
                };

                File.AppendAllLines(Path.Combine(bookPath, $"book-{empties}.lmf"), new[] { openbookItem.ToString() });

                map[board] = openbookItem;

                Console.WriteLine($"add book item: {openbookItem}.");

            }
        }
    }
}
