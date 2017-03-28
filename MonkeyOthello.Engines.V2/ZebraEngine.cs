using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace MonkeyOthello.Engines.V2
{
    public class ZebraEngine : V2Engine
    {
        protected override SearchResult Solve(int[] board, char color)
        {
            if (!File.Exists("EGEngineNativeMethods.dll"))
            {
                throw new FileNotFoundException("fail to load zebra engine", "EGEngineNativeMethods.dll");
            }

            EGEngineNativeMethods.SetDepth(6, 20, 16);  

            var col = (color == 'w' ? 1 : 0);

            var mode = -1;
            var nbits = 20;
            var sw = Stopwatch.StartNew();
            EGEngineNativeMethods.Slove(board, col, mode, nbits);
            var bestMove = EGEngineNativeMethods.GetBestMove();
            sw.Stop();

            var sr = new SearchResult()
            {
                Move = V2SquareToV3(bestMove)
            };

            if (bestMove >= 10 && bestMove <= 80 && board[bestMove] == 1)
            {
                sr.Nodes = (ulong)EGEngineNativeMethods.GetNodes();
                sr.Score = EGEngineNativeMethods.GetEval();
                sr.TimeSpan = sw.Elapsed;
            }

            return sr;
        }
    }
}
