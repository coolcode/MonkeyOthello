/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
//----------------------------------------------------------------*/

using System;
namespace MonkeyOthello.Engines.V2.AI
{
    public class Engine
    {
        private int emptiesOfStartGame = 40;
        private StaSolve staSolve;
        private MidSolve midSolve;
        private EndSolve endSolve;
        private MTDSolve mtdSolve;
        
        private int wldDepth;

        private ChessType color;
        private BoardState boardState;
        private int bestMove;
        private double bestScore;
        private int nodes;
        private double time;

        public ChessType Color
        {
            get { return color; }
            set { color = value; }
        }
        
        public BoardState BoardState
        {
            get { return boardState; }
        }

        public int BestMove
        {
            get { return bestMove; }
        }
        
        public double BestScore
        {
            get { return bestScore; }
        }
        
        public int Nodes
        {
            get { return nodes; }
        }
        
        public double Time
        {
            get { return time; }
        }

        
        public Engine()
        {
            staSolve = new StaSolve();
            midSolve = new MidSolve();
            endSolve = new EndSolve();
            mtdSolve = new MTDSolve();
            var depth = 8;
            staSolve.SearchDepth = depth;
            midSolve.SearchDepth = depth;
            wldDepth = 20;
            boardState = BoardState.START;
            emptiesOfStartGame = 32 + midSolve.SearchDepth;
        }

       
        public void Search(ChessType[] curboard, ChessType col)
        {
            int empties = 0;
            int white = 0;
            int black = 0;
            int discdiff;
            double start_time;
            ChessType[] board = new ChessType[91];
            
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
                boardState = BoardState.START;
                staSolve.PrepareToSolve(board);
                bestScore = staSolve.Solve(board, -Constants.HighestScore, Constants.HighestScore, col, empties, discdiff, 1);

                bestMove = staSolve.BestMove;
                nodes = staSolve.Nodes;
            }
            else if (empties > wldDepth)
            {
                boardState = BoardState.MIDDLE;
                midSolve.PrepareToSolve(board);
                bestScore = midSolve.Solve(board, -Constants.HighestScore, Constants.HighestScore, col, empties, discdiff, 1);
                bestMove = midSolve.BestMove;
                nodes = midSolve.Nodes;
            }
            else
            {
                boardState = BoardState.WLD;
                bestScore = mtdSolve.Solve(board, col, MTDSolve.Mode.DEFAULT, 0, empties, discdiff);
                bestMove = mtdSolve.BestMove;
                nodes = mtdSolve.Nodes;
            }
            time = (getCurTime() - start_time) / 10000000.0;
            if (time < 0) time = 0;
        }

        
        private double getCurTime()
        {
            long ticks = 0;
            ticks = DateTime.Now.Ticks;
            return ticks;
        }
    }
}
