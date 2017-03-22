/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart 
//----------------------------------------------------------------*/

using System;
using System.Runtime.InteropServices;

namespace MonkeyOthello.Engines.V2.AI
{ 
    public class MTDSolve
    { 
        public enum Mode
        { 
            DEFAULT = -1, 
            EXECT, 
            WLD,
        }
         
        private int nodes;
        private int bestMove; 

        public MTDSolve()
        {
            nodes = 0; 
        }
 
        public int Nodes
        {
            get { return nodes; }
        }
 
        public int BestMove
        {
            get { return bestMove; }
        }
 
        public double Solve(ChessType[] board, ChessType color, Mode mode, int nbits, int empties, int discdiff)
        {
            int[] myboard = new int[91];
            double eval = 0;
            nodes = 0; bestMove = 0;
            int col = (color == ChessType.WHITE ? 1 : 0);
             
            if (empties > 20)
            {
                MidSolve midSolve = new MidSolve();
                midSolve.SearchDepth = 8; 
                midSolve.PrepareToSolve(board);
                eval = midSolve.Solve(board, -Constants.HighestScore, Constants.HighestScore, color, empties, discdiff, 1);
                bestMove = midSolve.BestMove;
                nodes = midSolve.Nodes;
            }
            else
            {
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
