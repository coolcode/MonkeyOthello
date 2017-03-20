using FengartOthello;
using FengartOthello.AI;
using MonkeyOthello.Core;
using MonkeyOthello.Engines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Tests.Engines
{
    public class MonkeyOthelloV2Engine : BaseEngine
    {
        public override string Name
        {
            get
            {
                return "MonkeyV2";
            }
        }

        public override SearchResult Search(BitBoard bb, int depth)
        {
            var btext = bb.Draw();
            ChessType color = ChessType.WHITE;
            ChessType[] board = new ChessType[91];
            for (var j = 0; j <= 90; j++)
            {
                board[j] = ChessType.DUMMY;
            }
            int white = 0, black = 0, empties = 0;
            for (var j = 0; j < 64; j++)
            {
                var s = btext[j];
                var x = j & 7;
                var y = (j >> 3) & 7;
                var k = x + 10 + 9 * y;
                if (s == 'w')
                { board[k] = ChessType.WHITE; white++; }
                else if (s == 'b')
                { board[k] = ChessType.BLACK; black++; }
                else if (s == '.')
                { board[k] = ChessType.EMPTY; empties++; }
            }

            var discdiff = (color == ChessType.BLACK ? black - white : white - black);

            var sw = Stopwatch.StartNew();
            var engine = new Engine();
            engine.Search(board, color);
            var bestMove = engine.BestMove;

            var m = (bestMove - 9) % 9 - 1;
            var n = (bestMove - 9) / 9;

            sw.Stop();

            var sr = new SearchResult();
            sr.Move = n * 8 + m;
            if (bestMove >= 10 && bestMove <= 80 && board[bestMove] == ChessType.EMPTY)
            {
                sr.Nodes = engine.Nodes;
                sr.Score = (int) engine.BestScore;
                sr.TimeSpan = sw.Elapsed;
            }

            return sr;
        }
    }
}
