using MonkeyOthello.Engines.V2.AI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Engines.V2
{
    public class MonkeyV2Engine : V2Engine
    {
        protected override SearchResult Solve(int[] board, char color)
        {
            var col = (color == 'w' ? ChessType.WHITE : ChessType.BLACK);

            var sw = Stopwatch.StartNew();
            var engine = new Engine();
            engine.Search(board.Select(c=>(ChessType)c).ToArray(), col);
            var bestMove = engine.BestMove; 
            sw.Stop();

            var sr = new SearchResult();
            sr.Move = V2SquareToV3(bestMove);
            if (bestMove >= 10 && bestMove <= 80 && (ChessType)board[bestMove] == ChessType.EMPTY)
            {
                sr.Nodes = engine.Nodes;
                sr.Score = (int)engine.BestScore;
                sr.TimeSpan = sw.Elapsed;
            }

            return sr;
        }
    }
}
