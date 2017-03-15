/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // ��Ȩ���С� 
          // �����ߣ�Fengart
          // �ļ�����
          // �ļ�������������������
//----------------------------------------------------------------*/

using MonkeyOthello.Core;
using System;

namespace MonkeyOthello.AI
{
    /// <summary>
    /// ��������
    /// </summary>
    class Engine
    {
        /// <summary>
        /// ���п�������������:��ʣ��ո���>emptiesOfStartGame (40)ʱ;
        /// </summary>
        private int emptiesOfStartGame = 40;
        private StaSolve staSolve;
        private MidSolve midSolve;
        private EndSolve endSolve;
        private MTDSolve mtdSolve;

        private int endDepth;
        private int wldDepth;

        private ChessType color;
        private BoardState boardState;
        private int bestMove;
        private double bestScore;
        private int nodes;
        private double time;

        //private ComputerAI ai;

        /// <summary>
        /// ����ѡ���������ɫ
        /// </summary>
        public ChessType Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// ����״̬
        /// </summary>
        public BoardState BoardState
        {
            get { return boardState; }
        }

        /// <summary>
        /// �����������λ��
        /// </summary>
        public int BestMove
        {
            get { return bestMove; }
        }

        /// <summary>
        /// �����ķ���
        /// </summary>
        public double BestScore
        {
            get { return bestScore; }
        }

        /// <summary>
        /// �����Ľ����
        /// </summary>
        public int Nodes
        {
            get { return nodes; }
        }

        /// <summary>
        /// �������ѵ�ʱ��
        /// </summary>
        public double Time
        {
            get { return time; }
        }


        /// <summary>
        /// ���캯��
        /// </summary>
        public Engine()
        {
            staSolve = new StaSolve();
            midSolve = new MidSolve();
            endSolve = new EndSolve();
            mtdSolve = new MTDSolve();
            staSolve.SearchDepth = Config.Instance.MidDepth;
            midSolve.SearchDepth = Config.Instance.MidDepth;
            endDepth = Config.Instance.EndDepth;
            wldDepth = Config.Instance.WldDepth;
            boardState = BoardState.START;
            emptiesOfStartGame = 32 + midSolve.SearchDepth;
        }
        
        /// <summary>
        /// �������濪��
        /// </summary>
        /// <param name="curboard"></param>
        /// <param name="col"></param>
        /// <param name="useBook"></param>
        /// <returns></returns>
        public void Search(ChessType[] curboard,ChessType col)
        {
            int empties=0;
            int white=0;
            int black=0;
            int discdiff;
            double start_time;
            ChessType[] board = new ChessType[91];
            //����board
            for (int i = 0; i < 91; i++)
                board[i] = curboard[i];
            for (int i = 10; i <= 80; i++)
            {
                switch (board[i])
                {
                    case ChessType.EMPTY:
                        empties++;
                        break;
                    case ChessType.BLACK:
                        black++;
                        break;
                    case ChessType.WHITE:
                        white++;
                        break;
                    default:
                        break;
                }
            }
            discdiff = (col == ChessType.BLACK ? black - white : white - black);
            start_time = getCurTime();
            if (empties > emptiesOfStartGame)
            {
                //���п�������
                boardState = BoardState.START;
                staSolve.PrepareToSolve(board);
                bestScore = staSolve.Solve(board, -Constants.HighestScore, Constants.HighestScore, col, empties, discdiff, 1);
                
                bestMove = staSolve.BestMove;
                nodes = staSolve.Nodes;
            }
            else if (empties > wldDepth)
            {
                //�����о�����
                //StaticMethod.UpdateMessage("�о�����...");
                boardState = BoardState.MIDDLE;
                midSolve.PrepareToSolve(board);
                bestScore = midSolve.Solve(board, -Constants.HighestScore, Constants.HighestScore, col, empties, discdiff, 1);
                bestMove = midSolve.BestMove;
                nodes = midSolve.Nodes;
            }
            else
            {
                //�����վ�����
                //StaticMethod.UpdateMessage("�վ�����,���Ժ�...");
                boardState = BoardState.WLD;
                mtdSolve.SetDepth(6, wldDepth, endDepth);
                bestScore = mtdSolve.Solve(board, col, MTDSolve.Mode.DEFAULT, Config.Instance.HashSize, empties, discdiff);
                bestMove = mtdSolve.BestMove;
                nodes = mtdSolve.Nodes;
            }
            time = (getCurTime() - start_time) / 10000000.0;
            if (Config.Instance.GameMode !=GameMode.CvsC && empties > wldDepth)
                time-=1;//��ȥ1����Ϊ�˼������廨�ѵ�ʱ��
            if (time < 0) time = 0;
           // str += String.Format("��ֵ��{0:F3}(�ո�={1:D2} ����={2:D2} ����={3:D2}  ��ֵ={4})", bestScore, empties, white, black, discdiff);

           // return str;
        }

        /// <summary>
        /// �����������
        /// </summary>
        public void AISet(ComputerAI ai)
        {
            switch (ai)
            {
                case ComputerAI.LOWEST:
                    staSolve.SearchDepth = 2;
                    midSolve.SearchDepth = 2;
                    Config.Instance.MidDepth = 2;
                    break;
                case ComputerAI.LOW:
                    staSolve.SearchDepth = 4;
                    midSolve.SearchDepth = 4;
                    Config.Instance.MidDepth = 4;
                    break;
                case ComputerAI.NORMAL:
                    staSolve.SearchDepth = 6;
                    midSolve.SearchDepth = 6;
                    Config.Instance.MidDepth = 6;
                    break;
                case ComputerAI .HIGH:
                    staSolve.SearchDepth = 8;
                    midSolve.SearchDepth = 8;
                    Config.Instance.MidDepth = 8;
                    break;
                case ComputerAI .MOREHIGH:
                    staSolve.SearchDepth =9;
                    midSolve.SearchDepth =9;
                    Config.Instance.MidDepth= 9;
                    break;
                case ComputerAI.EVOLUTION:
                    staSolve.SearchDepth = 10;
                    midSolve.SearchDepth = 10;
                    Config.Instance.MidDepth = 10;
                    break;
                default:
                    staSolve.SearchDepth = 6;
                    midSolve.SearchDepth = 6;
                    Config.Instance.MidDepth = 6;
                    break;
            }
        }

        /// <summary>
        /// ��ȡʱ��
        /// </summary>
        /// <returns></returns>
        private double getCurTime()
        {
            long ticks = 0;
            ticks = DateTime.Now.Ticks;
            return ticks;
        }
    }
}
