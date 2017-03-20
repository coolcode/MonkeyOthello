using MonkeyOthello.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;
using System.Diagnostics;

namespace MonkeyOthello.Tests.Engines
{
    public class ZebraEngine : V2Engine
    {
        protected override SearchResult Solve(int[] board, char color)
        {        
            EGEngine.SetDepth(6, 20, 16);  

            var col = (color == 'w' ? 1 : 0);

            var mode = -1;
            var nbits = 20;
            var sw = Stopwatch.StartNew();
            EGEngine.Slove(board, col, mode, nbits);
            var bestMove = EGEngine.GetBestMove();
            sw.Stop();
             
            var sr = new SearchResult();
            sr.Move = V2SquareToV3(bestMove);
            if (bestMove >= 10 && bestMove <= 80 && board[bestMove] == 1)
            {
                sr.Nodes = EGEngine.GetNodes();
                sr.Score = EGEngine.GetEval();
                sr.TimeSpan = sw.Elapsed;
            }

            return sr;
        }
    }

    /// <summary>
    /// Zebra end game engine
    /// </summary>
    public class EGEngine
    {
        [DllImport("EGEngine", EntryPoint = "SetDepth", SetLastError = true)]
        public static extern void SetDepth(int mid, int wld, int end);

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="board">array 1*91</param>
        /// <param name="color">0:black, 1:white</param>
        /// <param name="mode">-1:default, engine decides by itself; 0:return winning or losing pieces; 1:return win or lose only</param>
        /// <param name="n_bits">hashtable size: 15≈1M,16≈2M,17≈4M,...,20≈32M</param>
        [DllImport("EGEngine", EntryPoint = "AI_Slove", SetLastError = true)]
        public static extern void Slove(int[] board, int color, int mode, int nbits);

        [DllImport("EGEngine", EntryPoint = "AI_GetEval", SetLastError = true)]
        public static extern int GetEval();

        [DllImport("EGEngine", EntryPoint = "AI_GetNodes", SetLastError = true)]
        public static extern int GetNodes();

        [DllImport("EGEngine", EntryPoint = "AI_GetBestMove", SetLastError = true)]
        public static extern int GetBestMove();
    }
}
