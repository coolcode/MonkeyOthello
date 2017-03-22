/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
//----------------------------------------------------------------*/

using System;

namespace MonkeyOthello.Engines.V2.AI
{
    public class EndSolve:BaseSolve
    {
        private const int highestScore = Constants.HighestScore;
        
        private uint RegionParity;
        
        public int BestMove
        {
            get { return bestMove; }
        }
        
        public int Nodes
        {
            get { return nodes; }
        }
        
        public EndSolve():base()
        {
        }
        
        public int NoParEndSolve(ChessType[] board, int alpha,int beta, ChessType color, int empties, int discdiff, int prevmove)
        {
            int score =  -highestScore; 
            ChessType oppcolor = 2 - color;
            int sqnum = -1, j, j1;
            int eval;
            Empties em, old_em;
            nodes++;
            for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
            {
                sqnum = em.Square;
                j = RuleUtils.DoFlips(board, sqnum, color, oppcolor);
                if (j > 0)
                {
                    board[sqnum] = color;
                    old_em.Succ = em.Succ;
                    if (empties == 2)
                    {
                        j1 = RuleUtils.CountFlips(board, EmHead.Succ.Square, oppcolor, color);
                        if (j1 > 0)//
                        {
                            eval = discdiff + 2 * (j - j1);
                        }
                        else //pass
                        {
                            j1 = RuleUtils.CountFlips(board, EmHead.Succ.Square, color, oppcolor);
                            eval = discdiff + 2 * j;
                            if (j1 > 0)//
                            {
                                eval += 2 * (j1 + 1);
                            }
                            else
                            {//both pass
                                if (eval >= 0)
                                    eval += 2;
                            }
                        }
                    }
                    else
                    {
                        eval = -NoParEndSolve(board, -beta, -alpha,
                                 oppcolor, empties - 1, -discdiff - 2 * j - 1, sqnum);
                    }
                    RuleUtils.UndoFlips(board, j, oppcolor);
                    board[sqnum] = ChessType.EMPTY;
                    old_em.Succ = em;
                    //purning
                    if (eval > score)
                    {
                        score = eval;
                        
                        if (empties == searchDepth)
                        {
                            bestMove = sqnum;
                        }
                        if (eval > alpha)
                        {
                            if (eval >= beta)
                            { // purning  
                                return score;
                            }
                            alpha = eval;
                        }
                    }
                }
            }

            if (score == -highestScore)
            { 
                if (empties == searchDepth)
                {
                    bestMove = MVPASS;
                }
                if (prevmove == 0)
                {  
                    if (discdiff > 0) return discdiff + empties;
                    if (discdiff < 0) return discdiff - empties;
                    return 0; 
                }
                else
                    return -NoParEndSolve(board, -beta, -alpha, oppcolor, empties, -discdiff, 0);
            }
            return score;
        }
         
        public int ParEndSolve(ChessType[] board, int alpha, int beta, ChessType color, int empties, int discdiff, int prevmove)
        {
            int score = -highestScore; 
            ChessType oppcolor = 2 - color;
            int sqnum = -1;
            int j;
            int eval;
            Empties em, old_em; 
            int par, holepar;
            //计算搜索的结点数
            nodes++;
             
                for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
                {
                    holepar = em.HoleId;
                    holepar = em.Square;
                    holepar = em.HoleId;
                    sqnum = em.Square;
                    j = RuleUtils.DoFlips(board, sqnum, color, oppcolor);

                    if (j > 0)
                    {
                        //下棋
                        board[sqnum] = color;
                        //更新奇偶
                        RegionParity ^= (uint)holepar;
                        //删除空位
                        old_em.Succ = em.Succ;
                        if (empties <= 1 + Constants.Use_Parity)
                        {
                            eval = -NoParEndSolve(board, -beta, -alpha, oppcolor, empties - 1, -discdiff - 2 * j - 1, sqnum);

                        }
                        else
                        {
                            eval = -ParEndSolve(board, -beta, -alpha, oppcolor, empties - 1, -discdiff - 2 * j - 1, sqnum);
                        }
                        //恢复到原来的棋局
                        RuleUtils.UndoFlips(board, j, oppcolor);
                        RegionParity ^= (uint)holepar;
                        board[sqnum] = ChessType.EMPTY;
                        old_em.Succ = em;

                        if (eval > score)
                        {
                            score = eval;
                            //=========================================
                            if (empties == searchDepth)
                            {
                                bestMove = sqnum;
                                //StaticMethod.UpdateMessage(score, nodes,sqnum);
                               // StaticMethod.UpdateThinkingMove(sqnum);
                            }
                            if (eval > alpha)
                            {
                                if (eval >= beta)
                                {
                                    return score;
                                }
                                alpha = eval;
                            }
                        }
                    } 
            }

            if (score == -highestScore)
            { 
                if (empties == searchDepth)
                {
                    bestMove = MVPASS;
                }

                if (prevmove == 0)
                {  
                    if (discdiff > 0) return discdiff + empties;
                    if (discdiff < 0) return discdiff - empties;
                    return 0; 
                }
                else
                    return -ParEndSolve(board, -beta, -alpha, oppcolor, empties, -discdiff, 0);

            }
            return score;
        }
 
        public int FastestFirstEndSolve(ChessType[] board, int alpha, int beta, ChessType color, int empties, int discdiff, int prevmove)
        {
            int i, j;
            int score = -highestScore;
            ChessType oppcolor = 2 - color;
            int sqnum = -1;
            int ev;
            int flipped;
            int moves, mobility;
            int best_value, best_index;
            Empties em, old_em;
            Empties[] move_ptr = new Empties[64];
            int holepar;
            int[] goodness = new int[64];            
            moves = 0; 
                nodes++;
            for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
            {
                sqnum = em.Square;
                flipped = RuleUtils.DoFlips(board, sqnum, color, oppcolor);
                if (flipped > 0)
                { 
                    board[sqnum] = color;
                    old_em.Succ = em.Succ; 
                    mobility = count_mobility(board, oppcolor);                    
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
                    RegionParity ^= (uint)holepar;
                    em.Pred.Succ = em.Succ;
                    if (em.Succ != null)
                        em.Succ.Pred = em.Pred;
                    if (empties <= Constants.Fastest_First + 1)
                        ev = -ParEndSolve(board, -beta, -alpha, oppcolor, empties - 1,
                                   -discdiff - 2 * j - 1, sqnum);
                    else
                        ev = -FastestFirstEndSolve(board, -beta, -alpha, oppcolor,
                                        empties - 1, -discdiff - 2 * j - 1, sqnum);
                    RuleUtils.UndoFlips(board, j, oppcolor);
                    RegionParity ^= (uint)holepar;
                    board[sqnum] = ChessType.EMPTY;
                    em.Pred.Succ = em;
                    if (em.Succ != null)
                        em.Succ.Pred = em;

                    if (ev > score)
                    {
                        score = ev; 
                        if (empties == searchDepth)
                        {
                            bestMove = sqnum; 
                        }
                        if (ev > alpha)
                        {
                            if (ev >= beta)
                                return score;
                            alpha = ev;
                        }
                    }
                }
            }
            else
            {
                if (prevmove == 0)
                {
                    if (discdiff > 0)
                        return discdiff + empties;
                    if (discdiff < 0)
                        return discdiff - empties;
                    return 0;
                }
                else  /* I pass: */
                    score = -FastestFirstEndSolve(board, -beta, -alpha, oppcolor, empties, -discdiff, 0);
                
                if (empties == searchDepth)
                {
                    bestMove = MVPASS;
                }
            }
            return score;
        }
 
        public int Solve(ChessType[] board, int alpha, int beta, ChessType color, int empties, int discdiff, int prevmove)
        { 
            RegionParity = 0;
            for (int i = 10; i <= 80; i++)
            {
                RegionParity ^= HoleId[i];
            }
            searchDepth = empties;
            nodes = 0;
            if (empties > Constants.Fastest_First)
                return FastestFirstEndSolve(board, alpha, beta, color, empties, discdiff, prevmove);
            else
            {
                if (empties <= Constants.Use_Parity)
                    return NoParEndSolve(board, alpha, beta, color, empties, discdiff, prevmove);
                else
                    return ParEndSolve(board, alpha, beta, color, empties, discdiff, prevmove);
            }
        }
    }
}
