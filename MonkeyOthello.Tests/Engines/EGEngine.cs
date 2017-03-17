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
    public class EndGameEngine : BaseEngine
    {
        public override SearchResult Search(BitBoard bb, int depth)
        {
            EGEngine.MyDllAI_SetDepth(6, 20, 18);

            var board = new int[91];
            for (var j = 0; j < board.Length; j++)
            {
                board[j] = 3;// ChessType.DUMMY;
            }

            int x, y, k, empties = 0;
            for (var j = 0; j < 64; j++)
            {
                x = j & 7;
                y = (j >> 3) & 7;
                k = x + 10 + 9 * y;

                var w = bb.PlayerPieces & (1ul << j);
                var b = bb.OpponentPieces & (1ul << j);

                if (w > 0)
                {
                    board[k] = 0;//white
                }
                else if (b > 0)
                {
                    board[k] = 2;//black
                }
                else
                {
                    board[k] = 1;//empty
                    empties++;
                }
            }

            var r = Solve(board, 'w');

            return r;
        } 

        protected static SearchResult Solve(int[] board, char color)
        {
            var sw = Stopwatch.StartNew();
            var col = (color == 'w' ? 1 : 0);

            //-1代表电脑默认搜索，0代表终局确切比分搜索，1代表胜负平搜索
            var mode = -1;
            //置换表大小(15≈1M,16≈2M,17≈4M,...,20≈32M
            var nbits = 20;
            EGEngine.MyDllAI_Slove(board, col, mode, nbits);
            var bestMove = EGEngine.MyDllAI_GetBestMove();
            sw.Stop();

            var m = (bestMove - 9) % 9 - 1;
            var n = (bestMove - 9) / 9;

            var sr = new SearchResult();
            sr.Move = n*8+m;
            if (bestMove >= 10 && bestMove <= 80 && board[bestMove] == 1)
            {
                sr.Nodes = EGEngine.MyDllAI_GetNodes();
                sr.Score = EGEngine.MyDllAI_GetEval();
                sr.TimeSpan = sw.Elapsed;
            }
            return sr;
        }
    }

    public class EGEngine
    {
        /// <summary>
        /// 搜索深度设置
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="wld"></param>
        /// <param name="end"></param>
        [DllImport("EGEngine", EntryPoint = "SetDepth", SetLastError = true)]
        public static extern void MyDllAI_SetDepth(int mid, int wld, int end);

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="curboard">1*91的数组</param>
        /// <param name="color">0代表黑色，1代表白色</param>
        /// <param name="mode">-1代表电脑默认搜索，0代表终局确切比分搜索，1代表胜负平搜索</param>
        /// <param name="n_bits"></param>
        [DllImport("EGEngine", EntryPoint = "AI_Slove", SetLastError = true)]
        public static extern void MyDllAI_Slove(int[] curboard, int color, int mode, int n_bits);

        /// <summary>
        /// 获取搜索的估值
        /// </summary>
        /// <returns></returns>
        [DllImport("EGEngine", EntryPoint = "AI_GetEval", SetLastError = true)]
        public static extern int MyDllAI_GetEval();

        /// <summary>
        /// 获取搜索的结点数
        /// </summary>
        /// <returns></returns>
        [DllImport("EGEngine", EntryPoint = "AI_GetNodes", SetLastError = true)]
        public static extern int MyDllAI_GetNodes();

        /// <summary>
        /// 获取搜索的最佳走步
        /// </summary>
        /// <returns></returns>
        [DllImport("EGEngine", EntryPoint = "AI_GetBestMove", SetLastError = true)]
        public static extern int MyDllAI_GetBestMove();
    }
}
