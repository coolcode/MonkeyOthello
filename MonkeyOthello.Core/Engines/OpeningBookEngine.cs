using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;
using System.IO;
using System.Diagnostics;

namespace MonkeyOthello.Engines
{
    public class OpeningBookEngine : BaseEngine
    {
        private readonly Dictionary<BitBoard, OpeningBookItem> map = new Dictionary<BitBoard, OpeningBookItem>();
        private readonly string bookPath = Path.Combine(Environment.CurrentDirectory, @"books\");


        public OpeningBookEngine()
        {
            Load();
        }

        public override SearchResult Search(BitBoard board, int depth)
        {
            var sw = Stopwatch.StartNew();

            if (!map.ContainsKey(board))
            {
                throw new KeyNotFoundException(board.ToString());
            }

            var bookItem = map[board];

            var bestScore = bookItem.EvalList.Max(c => c.Score);

            var move = bookItem.EvalList.Where(x => x.Score == bestScore).Select(x => x.Move).First();

            //var oppBoard = Rule.MoveSwitch(board, move);

            sw.Stop();

            return new SearchResult
            {
                Move = move,
                Score = bestScore,
                Nodes = 1,
                Process = 1,
                EvalList = bookItem.EvalList,
                TimeSpan = sw.Elapsed,
            };
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
    }


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

}
