/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // ��Ȩ���С� 
          // �����ߣ�Fengart
          // �ļ�����BaseSolve.cs
          // �ļ�����������������������(����)
//----------------------------------------------------------------*/

using MonkeyOthello.Core;
using System;

namespace MonkeyOthello.AI
{
    class BaseSolve
    {
        /// <summary>
        /// ���ĸ���
        /// </summary>
        protected uint[] HoleId;

        /// <summary>
        /// ��λ�б�
        /// </summary>
        protected Empties[] EmList;

        /// <summary>
        /// ��λ�б��ͷ
        /// </summary>
        protected Empties EmHead;

        /// <summary>
        /// �ӻ�λ�õ���λ�õ���������
        /// </summary>
        protected readonly int[] worst2best = new int[]
        {
            //ǰ���ע�ͱ�ʾ�����λ�����Ƶ�λ��
/*B2*/      20 , 25 , 65 , 70 ,
/*B1*/      11 , 16 , 19 , 26 , 64 , 71 , 74 , 79 ,
/*C2*/      21 , 24 , 29 , 34 , 56 , 61 , 66 , 69 ,
/*D2*/      22 , 23 , 38 , 43 , 47 , 52 , 67 , 68 ,
/*D3*/      31 , 32 , 39 , 42 , 48 , 51 , 58 , 59 ,
/*D1*/      13 , 14 , 37 , 44 , 46 , 53 , 76 , 77 ,
/*C3*/      30 , 33 , 57 , 60 ,
/*C1*/      12 , 15 , 28 , 35 , 55 , 62 , 75 , 78 ,
/*A1*/      10 , 17 , 73 , 80 , 
/*D4*/      40 , 41 , 49 , 50
       };


        /// <summary>
        /// ��Чλ��
        /// </summary>
        protected const int MVPASS = -1;

        /// <summary>
        /// �����Ľ����
        /// </summary>
        protected int nodes;

        /// <summary>
        /// �������
        /// </summary>
        protected int searchDepth;

        /// <summary>
        /// ����������õ�����λ��
        /// </summary>
        protected int bestMove;

        /// <summary>
        /// �����Ĺ��캯��
        /// </summary>
        public BaseSolve()
        {
            HoleId = new uint[91];
            EmList = new Empties[Constants.MaxEmpties];
            EmHead = new Empties();
            bestMove = MVPASS;
        }

        /// <summary>
        /// ׼������
        /// </summary>
        public void PrepareToSolve(ChessType[] board)
        {
            int i, sqnum;
            uint k;
            int z;
            const int MAXITERS = 1;
            /* �ҿ�ID: */
            k = 1;
            for (i = 10; i <= 80; i++)
            {
                if (board[i] == ChessType.EMPTY)
                {
                    if (board[i - 10] == ChessType.EMPTY) HoleId[i] = HoleId[i - 10];
                    else if (board[i - 9] == ChessType.EMPTY) HoleId[i] = HoleId[i - 9];
                    else if (board[i - 8] == ChessType.EMPTY) HoleId[i] = HoleId[i - 8];
                    else if (board[i - 1] == ChessType.EMPTY) HoleId[i] = HoleId[i - 1];
                    else { HoleId[i] = k; k <<= 1; }
                }
                else HoleId[i] = 0;
            }

            for (z = MAXITERS; z > 0; z--)
            {
                for (i = 80; i >= 10; i--)
                {
                    if (board[i] == ChessType.EMPTY)
                    {
                        k = HoleId[i];
                        if (board[i + 10] == ChessType.EMPTY) HoleId[i] = minu(k, HoleId[i + 10]);
                        if (board[i + 9] == ChessType.EMPTY) HoleId[i] = minu(k, HoleId[i + 9]);
                        if (board[i + 8] == ChessType.EMPTY) HoleId[i] = minu(k, HoleId[i + 8]);
                        if (board[i + 1] == ChessType.EMPTY) HoleId[i] = minu(k, HoleId[i + 1]);
                    }
                }
                for (i = 10; i <= 80; i++)
                {
                    if (board[i] == ChessType.EMPTY)
                    {
                        k = HoleId[i];
                        if (board[i - 10] == ChessType.EMPTY) HoleId[i] = minu(k, HoleId[i - 10]);
                        if (board[i - 9] == ChessType.EMPTY) HoleId[i] = minu(k, HoleId[i - 9]);
                        if (board[i - 8] == ChessType.EMPTY) HoleId[i] = minu(k, HoleId[i - 8]);
                        if (board[i - 1] == ChessType.EMPTY) HoleId[i] = minu(k, HoleId[i - 1]);
                    }
                }
            }

            /* ��ȡ��λ��*/
            k = 0;
            Empties pt = EmHead;
            for (i = 60 - 1; i >= 0; i--)
            {
                sqnum = worst2best[i];
                if (board[sqnum] == ChessType.EMPTY)
                {
                    EmList[k] = new Empties();
                    pt.Succ = EmList[k];
                    EmList[k].Pred = pt;
                    k++;
                    pt = pt.Succ;
                    pt.Square = sqnum;
                    pt.HoleId = (int)HoleId[sqnum];
                }
                pt.Succ = null;
            }
        }

        /// <summary>
        /// �����ж���
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        protected int count_mobility(ChessType[] board, ChessType color)
        {
            ChessType oppcolor = 2 - color;
            int mobility;
            Empties em;

            mobility = 0;
            for (em = EmHead.Succ; em != null; em = em.Succ)
            {
                if (Board.AnyFlips(board, em.Square, color, oppcolor))
                    mobility++;
            }
            return mobility;
        }

        /// <summary>
        /// ����Сֵ
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static uint minu(uint a, uint b)
        {
            if (a < b) return a;
            return b;
        }

        public UpdateMessageDelegate UpdateMessageAction { get; set; } = null;

        protected void UpdateMessage(double score, int nodes, int square)
        {
            if (UpdateMessageAction != null)
            {
                var msg = string.Format("{0:F2}", score / 100)
                + " | " + string.Format("{0}", (nodes > 1000 ? nodes / 1000 + " K" : nodes.ToString()))
                + " | " + "˼��λ�ã�" + Utils.SquareToString(square) + "...";

                UpdateMessageAction(msg);
            }
        }
    }
}
