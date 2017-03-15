/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // ��Ȩ���С� 
          // �����ߣ�Fengart
          // �ļ�����MTDSolve.cs
          // �ļ������������վ���������(ʹ��MTD)
//----------------------------------------------------------------*/

using System;
using System.Runtime.InteropServices;

namespace MonkeyOthello.AI
{
    /// <summary>
    /// ��MTD���վ���������
    /// </summary>
    class MTDSolve
    {

        /// <summary>
        /// MTD����ģʽ
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Ĭ��
            /// </summary>
            DEFAULT=-1,

            /// <summary>
            /// �վ�����
            /// </summary>
            EXECT,

            /// <summary>
            /// ʤ��ƽ����
            /// </summary>
            WLD,
        }

       private static string engineName = Config.Instance.EGEngine;

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="mid"></param>
        /// <param name="wld"></param>
        /// <param name="end"></param>
        [DllImport("EGEngine", EntryPoint = "SetDepth", SetLastError = true)]
        public static extern void MyDllAI_SetDepth(int mid, int wld, int end);

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="curboard">1*91������</param>
        /// <param name="color">0�����ɫ��1�����ɫ</param>
        /// <param name="mode">-1�������Ĭ��������0�����վ�ȷ�бȷ�������1����ʤ��ƽ����</param>
        /// <param name="n_bits"></param>
        [DllImport("EGEngine", EntryPoint = "AI_Slove", SetLastError = true)]
        public static extern void MyDllAI_Slove(int[] curboard, int color, int mode, int n_bits);

        /// <summary>
        /// ��ȡ�����Ĺ�ֵ
        /// </summary>
        /// <returns></returns>
        [DllImport("EGEngine", EntryPoint = "AI_GetEval", SetLastError = true)]
        public static extern int MyDllAI_GetEval();

        /// <summary>
        /// ��ȡ�����Ľ����
        /// </summary>
        /// <returns></returns>
        [DllImport("EGEngine", EntryPoint = "AI_GetNodes", SetLastError = true)]
        public static extern int MyDllAI_GetNodes();

        /// <summary>
        /// ��ȡ����������߲�
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
        /// �����Ľ����
        /// </summary>
        public int Nodes
        {
            get { return nodes; }
        }

        /// <summary>
        /// ����������õ�����λ��
        /// </summary>
        public int BestMove
        {
            get { return bestMove; }
        }

        /// <summary>
        /// �Ƿ���������ļ�(dll)
        /// </summary>
        public bool ExistEGEngine
        {
            get { return existEGEngine; }
        }

        /// <summary>
        /// �����������
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
        /// ����
        /// </summary>
        /// <param name="board">����</param>
        /// <param name="color">��ɫ</param>
        /// <param name="mode">����ģʽ</param>
        /// <param name="nbits">�û����С(15��1M,16��2M,17��4M,...,20��32M</param>
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
                //��MTD�������ش���ʱ,����ո�>20ʱ�����о�����,������в���HashTable���վ�����;
                if (empties > 20)
                {
                    //�о�����
                    MidSolve midSolve = new MidSolve();
                    midSolve.SearchDepth = Config.Instance.MidDepth;
                    midSolve.PrepareToSolve(board);
                    eval=  midSolve.Solve(board, -Constants.HighestScore, Constants.HighestScore, color, empties, discdiff, 1);                    
                    bestMove = midSolve.BestMove;
                    nodes = midSolve.Nodes;
                }
                else
                {
                    //����HashTable���վ�����
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
