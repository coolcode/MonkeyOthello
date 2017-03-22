/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
//----------------------------------------------------------------*/

using System;

namespace MonkeyOthello.Engines.V2.AI
{
    public class BaseSolve
    {
        protected uint[] HoleId;
        
        protected Empties[] EmList;
        
        protected Empties EmHead;
        
        protected readonly int[] worst2best = new int[]
        {
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
        
        protected const int MVPASS = -1;
        
        protected int nodes;
        
        protected int searchDepth;
        
        protected int bestMove;
        
        public BaseSolve()
        {
            HoleId = new uint[91];
            EmList = new Empties[Constants.MaxEmpties];
            EmHead = new Empties();
            bestMove = MVPASS;
        }
        
        public void PrepareToSolve(ChessType[] board)
        {
            int i, sqnum;
            uint k;
            int z;
            const int MAXITERS = 1;
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
        
        
        protected int count_mobility(ChessType[] board, ChessType color)
        {
            ChessType oppcolor = 2 - color;
            int mobility;
            Empties em;

            mobility = 0;
            for (em = EmHead.Succ; em != null; em = em.Succ)
            {
                if (RuleUtils.AnyFlips(board, em.Square, color, oppcolor))
                    mobility++;
            }
            return mobility;
        }
        
        private static uint minu(uint a, uint b)
        {
            if (a < b) return a;
            return b;
        }
    }
}
