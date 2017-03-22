/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
//----------------------------------------------------------------*/

using System;

namespace MonkeyOthello.Engines.V2.AI
{ 
    public class MidSolve:BaseSolve
    {

        private const int MidBetterScore = 1800;
        private Evalation evalation; 

        public int SearchDepth
        {
            get { return searchDepth; }
            set { searchDepth = value; }
        }

        public int Nodes
        {
            get { return nodes; }
        }
        
        public int BestMove
        {
            get { return bestMove; }
        }

        public MidSolve()
            : base()
        {
            searchDepth = 8;
            evalation = new Evalation();
            nodes = 0;
        }       

        private int FastestFirstSolve(ChessType[] board, int alpha,int beta, ChessType color,int depth, int empties, int discdiff, int prevmove)
        {
            lock (this)
            {
                int i, j;
                int score = -Constants.HighestScore-1;
                ChessType oppcolor = 2 - color;
                int sqnum;
                int eval;
                int flipped;
                int moves, mobility;
                int best_value, best_index;
                Empties em, old_em;
                Empties[] move_ptr = new Empties[Constants.MaxEmpties];
                int holepar;
                int[] goodness = new int[Constants.MaxEmpties];
                //
                bool fFoundPv = false;
                //
                //nodes++;
                if (depth == 0)
                {
                    nodes++;
                    //return evalation.MidEval2(board, color, oppcolor, empties,EmHead);
                    return QuiescenceSolve(board, alpha, beta, color, 2, empties, discdiff, prevmove);
                }

                moves = 0;
                for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
                {
                    sqnum = em.Square;
                    flipped = RuleUtils.DoFlips(board, sqnum, color, oppcolor);
                    if (flipped > 0)
                    {
                        //
                        board[sqnum] = color;
                        old_em.Succ = em.Succ;
                        //
                        mobility = count_mobility(board, oppcolor);
                        //
                        RuleUtils.UndoFlips(board, flipped, oppcolor);
                        old_em.Succ = em;
                        board[sqnum] = ChessType.EMPTY;
                        move_ptr[moves] = em;
                        goodness[moves] = -mobility;
                        moves++;
                    }
                }

                if (moves != 0)
                {
                    for (i = 0; i < moves; i++)
                    {
                        //
                        best_value = goodness[i];
                        best_index = i;
                        for (j = i + 1; j < moves; j++)
                            if (goodness[j] > best_value)
                            {
                                best_value = goodness[j];
                                best_index = j;
                            }
                        em = move_ptr[best_index];
                        move_ptr[best_index] = move_ptr[i];
                        goodness[best_index] = goodness[i];
                        //
                        sqnum = em.Square;
                        holepar = em.HoleId;
                        j = RuleUtils.DoFlips(board, sqnum, color, oppcolor);
                        board[sqnum] = color;

                        em.Pred.Succ = em.Succ;
                        if (em.Succ != null)
                            em.Succ.Pred = em.Pred;

                        nodes++;
                        //
                        if (fFoundPv) 
                        {
                            //
                            eval = -FastestFirstSolve(board, -alpha - 1, -alpha, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
                            if ((eval > alpha) && (eval < beta))
                            {
                                eval = -FastestFirstSolve(board, -beta, -eval, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
                               //eval = -FastestFirstMidSolve(board, -beta, -alpha, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
                            }
                        }
                        else
                        {
                            eval = -FastestFirstSolve(board, -beta, -alpha, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
                        }
                        //
                        RuleUtils.UndoFlips(board, j, oppcolor);
                        board[sqnum] = ChessType.EMPTY;
                        em.Pred.Succ = em;
                        if (em.Succ != null)
                            em.Succ.Pred = em;

                        if (eval > score)
                        {
                            score = eval;
                            if (eval > alpha)
                            {
                                if (eval >= beta)
                                {
                                    //
                                    return score;
                                }
                                alpha = eval;
                                fFoundPv = true;
                            }
                        }
                    }
                }
                else
                {
                    if (prevmove == 0)//
                    {
                        if (discdiff > 0)//
                            return Constants.HighestScore;
                        if (discdiff < 0)//
                            return -Constants.HighestScore;
                        return 0;//
                    }
                    else  /* I pass: */
                        score = -FastestFirstSolve(board, -beta, -alpha, oppcolor,depth, empties, -discdiff, 0);
                    
                }
                return score;
            }
        }

        private int QuiescenceSolve(ChessType[] board, int alpha, int beta, ChessType color, int depth, int empties, int discdiff, int prevmove)
        {
            lock (this)
            {
                int i, j;
                int score1;
                int score2 = -Constants.HighestScore;

                ChessType oppcolor = 2 - color;
                int sqnum;
                int eval;
                int flipped;
                int moves, mobility;
                int best_value, best_index;
                Empties em, old_em;
                Empties[] move_ptr = new Empties[Constants.MaxEmpties];
                int holepar;
                int[] goodness = new int[Constants.MaxEmpties];

                //
                //nodes++;
                score1 = evalation.MidEval2(board, color, oppcolor, empties,EmHead);
                if (score1 >=beta || score1>=3000 || depth == 0) return score1;
                else if (score1 > alpha)
                    alpha = score1;
                moves = 0;
                for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
                {
                    sqnum = em.Square;
                    flipped = RuleUtils.DoFlips(board, sqnum, color, oppcolor);
                    if (flipped > 0)
                    {
                        //
                        board[sqnum] = color;
                        old_em.Succ = em.Succ;
                        //
                        mobility = count_mobility(board, oppcolor);
                        //
                        RuleUtils.UndoFlips(board, flipped, oppcolor);
                        old_em.Succ = em;
                        board[sqnum] = ChessType.EMPTY;
                        move_ptr[moves] = em;
                        goodness[moves] = -mobility;
                        moves++;
                    }
                }

                if (moves != 0)
                {
                    for (i = 0; i < moves; i++)
                    {
                        best_value = goodness[i];
                        best_index = i;
                        for (j = i + 1; j < moves; j++)
                            if (goodness[j] > best_value)
                            {
                                best_value = goodness[j];
                                best_index = j;
                            }
                        em = move_ptr[best_index];
                        move_ptr[best_index] = move_ptr[i];
                        goodness[best_index] = goodness[i];

                        sqnum = em.Square;
                        holepar = em.HoleId;
                        j = RuleUtils.DoFlips(board, sqnum, color, oppcolor);
                        board[sqnum] = color;

                        em.Pred.Succ = em.Succ;
                        if (em.Succ != null)
                            em.Succ.Pred = em.Pred;

                        //
                        nodes++;
                        //
                        eval = -evalation.MidEval2(board, oppcolor, color, empties - 1, EmHead);
                        // -QuiescenceSolve(board, -beta, -alpha, oppcolor, depth - 1, empties - 1, -discdiff - j - j - 1, sqnum);

                        RuleUtils.UndoFlips(board, j, oppcolor);
                        board[sqnum] = ChessType.EMPTY;
                        em.Pred.Succ = em;
                        if (em.Succ != null)
                            em.Succ.Pred = em;

                        if (eval > score2)
                        {
                            score2 = eval;
                            if (eval > alpha)
                            {
                                if (eval >= beta)
                                {
                                    //
                                    return score2;
                                }
                                alpha = eval;
                            }
                        }
                    }
                }
                else
                {
                    if (prevmove == 0)//
                    {
                        if (discdiff > 0)
                            return Constants.HighestScore;
                        if (discdiff < 0)
                            return -Constants.HighestScore;
                        return 0;
                    }
                    else  /* I pass: */
                    {
                        nodes++;
                        score2 = -QuiescenceSolve(board, -beta, -alpha, oppcolor, depth, empties, -discdiff, 0);
                        return score2;
                    }
                }
                return (2 * score1 + score2) / 3; // (5 * score1 + 3 * score2) / 8;
            }
        }

        private int RootMidSolve(ChessType[] board, int alpha, int beta, ChessType color, int depth, int empties, int discdiff, int prevmove)
        {
            lock (this)
            {
                int i, j;
                int score = -Constants.HighestScore - 1;
                ChessType oppcolor = 2 - color;
                int sqnum;
                int eval;
                int flipped;
                int moves, mobility;
                int best_value, best_index;
                Empties em, old_em;
                Empties[] move_ptr = new Empties[Constants.MaxEmpties];
                int holepar;
                int[] goodness = new int[Constants.MaxEmpties];
                //
                bool fFoundPv = false;
                
                moves = 0;
                for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
                {
                    sqnum = em.Square;
                    flipped = RuleUtils.DoFlips(board, sqnum, color, oppcolor);
                    if (flipped > 0)
                    {
                        //
                        board[sqnum] = color;
                        old_em.Succ = em.Succ;
                        //
                        mobility = -count_mobility(board, oppcolor);
                       // mobility = -FastestFirstSolve(board, -beta, -alpha, oppcolor, 3, empties - 1, -discdiff - 2 * flipped - 1, sqnum);
                        //
                        RuleUtils.UndoFlips(board, flipped, oppcolor);
                        old_em.Succ = em;
                        board[sqnum] = ChessType.EMPTY;
                        move_ptr[moves] = em;
                        goodness[moves] = mobility;
                        moves++;
                    }
                }

                if (moves != 0)
                {
                    for (i = 0; i < moves; i++)
                    {
                        //
                        best_value = goodness[i];
                        best_index = i;
                        for (j = i + 1; j < moves; j++)
                            if (goodness[j] > best_value)
                            {
                                best_value = goodness[j];
                                best_index = j;
                            }
                        em = move_ptr[best_index];
                        move_ptr[best_index] = move_ptr[i];
                        goodness[best_index] = goodness[i];
                        //
                        sqnum = em.Square;
                        holepar = em.HoleId;
                        j = RuleUtils.DoFlips(board, sqnum, color, oppcolor);
                        board[sqnum] = color;

                        em.Pred.Succ = em.Succ;
                        if (em.Succ != null)
                            em.Succ.Pred = em.Pred;

                        nodes++;
                        //
                        if (fFoundPv)
                        {
                            //
                            eval = -FastestFirstSolve(board, -alpha - 1, -alpha, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
                            if ((eval > alpha) && (eval < beta))
                            {
                                eval = -FastestFirstSolve(board, -beta, -eval, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
                                //eval = -FastestFirstMidSolve(board, -beta, -alpha, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
                            }
                        }
                        else
                        {
                            eval = -FastestFirstSolve(board, -beta, -alpha, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
                        }
                        //
                        RuleUtils.UndoFlips(board, j, oppcolor);
                        board[sqnum] = ChessType.EMPTY;
                        em.Pred.Succ = em;
                        if (em.Succ != null)
                            em.Succ.Pred = em;

                        if (eval > score)
                        {
                            score = eval;
                            //
                            bestMove = sqnum;
                            if (eval > alpha)
                            {
                                if (eval >= beta)
                                {
                                    //
                                    return score;
                                }
                                alpha = eval;
                                fFoundPv = true;
                            }
                        }
                    }
                }
                else
                {  /* I pass: */
                        score = -FastestFirstSolve(board, -beta, -alpha, oppcolor, depth, empties, -discdiff, 0);
                    //
                    if (depth == searchDepth)
                    {
                        bestMove = MVPASS;
                    }
                }
                return score;
            }
        }

         
        public double Solve(ChessType[] board, int alpha, int beta, ChessType color, int empties, int discdiff, int prevmove)
        {
            nodes = 0;
            //searchDepth = 8;
            double eval=RootMidSolve(board, alpha, beta, color, searchDepth, empties, discdiff, prevmove);
            //double eval = MPCMidSolve(board, alpha, beta, color, searchDepth, empties, discdiff, prevmove);
            eval=eval/100.0;
            if (eval > 64)
                return 64;
            if (eval < -64)
                return -64;
            return eval;       
        }
    }
}
