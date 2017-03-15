/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // 版权所有。 
          // 开发者：Fengart
          // 文件名：MTDSolve.cs
          // 文件功能描述：终局搜索引擎(使用MTD)
//----------------------------------------------------------------*/

using System;
using System.Runtime.InteropServices;

namespace MonkeyOthello.AI
{
    /// <summary>
    /// 带MTD的终局搜索引擎
    /// </summary>
    class MTDSolve
    {

        /// <summary>
        /// MTD搜索模式
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// 默认
            /// </summary>
            DEFAULT=-1,

            /// <summary>
            /// 终局搜索
            /// </summary>
            EXECT,

            /// <summary>
            /// 胜负平搜索
            /// </summary>
            WLD,
        }

       private static string engineName = Config.Instance.EGEngine;

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

        private int nodes;
        private int bestMove;
        private bool existEGEngine;

        public MTDSolve()
        {
            nodes = 0;
            try
            {
                existEGEngine = System.IO.File.Exists(engineName);
            }
            catch
            {
                existEGEngine = false;
            }
        }

        /// <summary>
        /// 搜索的结点数
        /// </summary>
        public int Nodes
        {
            get { return nodes; }
        }

        /// <summary>
        /// 搜索到的最好的下棋位置
        /// </summary>
        public int BestMove
        {
            get { return bestMove; }
        }

        /// <summary>
        /// 是否存在引擎文件(dll)
        /// </summary>
        public bool ExistEGEngine
        {
            get { return existEGEngine; }
        }

        /// <summary>
        /// 搜索深度设置
        /// </summary>
        /// <param name="midDepth"></param>
        /// <param name="wldDepth"></param>
        /// <param name="endDepth"></param>
        public void SetDepth(int midDepth, int wldDepth, int endDepth)
        {
            if (existEGEngine)
                MyDllAI_SetDepth(midDepth, wldDepth, endDepth);
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="board">棋盘</param>
        /// <param name="color">颜色</param>
        /// <param name="mode">搜索模式</param>
        /// <param name="nbits">置换表大小(15≈1M,16≈2M,17≈4M,...,20≈32M</param>
        /// <returns></returns>
        public double Solve(ChessType[] board, ChessType color, Mode mode, int nbits, int empties, int discdiff)
        {
            int[] myboard=new int[91];
            double eval=0 ;
            nodes = 0; bestMove = 0;
            int col = (color == ChessType.WHITE ? 1 : 0);
            
            if (existEGEngine)
            {
                for (int i = 0; i < 91; i++)
                    myboard[i] = (int)board[i];
                MyDllAI_Slove(myboard, col, (int)mode, nbits);
                bestMove = MyDllAI_GetBestMove();
            }
            if (bestMove >= 10 && bestMove <= 80 && board[bestMove] == ChessType.EMPTY &&
                Board.AnyFlips(board, bestMove, color, 2 - color))
            {
                nodes = MyDllAI_GetNodes();
                eval = MyDllAI_GetEval();
                return (eval > 64 ? 64 : eval);
            }
            else
            {
                //当MTD搜索返回错误时,如果空格>20时调用中局搜索,否则进行不带HashTable的终局搜索;
                if (empties > 20)
                {
                    //中局搜索
                    MidSolve midSolve = new MidSolve();
                    midSolve.SearchDepth = Config.Instance.MidDepth;
                    midSolve.PrepareToSolve(board);
                    eval=  midSolve.Solve(board, -Constants.HighestScore, Constants.HighestScore, color, empties, discdiff, 1);                    
                    bestMove = midSolve.BestMove;
                    nodes = midSolve.Nodes;
                }
                else
                {
                    //不带HashTable的终局搜索
                    EndSolve endSolve = new EndSolve();
                    endSolve.PrepareToSolve(board);
                    if (empties > 16)
                    {
                        eval = endSolve.Solve(board, -1, 1, color, empties, discdiff, 1);
                    }
                    else
                    {
                        eval = endSolve.Solve(board, -64, 64, color, empties, discdiff, 1);
                    }
                    bestMove = endSolve.BestMove;
                    nodes = endSolve.Nodes;
                }
                return eval;
            }
        }
    }
}
