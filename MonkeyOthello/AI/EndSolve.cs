/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // 版权所有。 
          // 开发者：Fengart
          // 文件名：EndSolve.cs
          // 文件功能描述：终局搜索引擎
//----------------------------------------------------------------*/

using MonkeyOthello.Core;
using System;

namespace MonkeyOthello.AI
{
    /// <summary>
    /// 终局搜索引擎
    /// </summary>
    class EndSolve:BaseSolve
    {

        /*如果棋盘没下满，假如黑棋有34个子，白棋有27个子。此时有两种计分方法：日本的计分法是，剩余的空格全部给胜方，
         * 那么黑棋胜34+3-27=10个子。欧洲的计分法是，剩余的空格双方各得一半，那么黑棋胜(34+1.5)-(27+1.5)=7个子。*/
        /// <summary>
        /// 胜者得空模式(即采用日本的计分方法)
        /// </summary>
        private const bool WINNER_GETS_EMPTIES = true;

        private const int highestScore = Constants.HighestScore;


        /// <summary>
        /// 奇偶性
        /// </summary>
        private uint RegionParity;

        /// <summary>
        /// 搜索到的最好的下棋位置
        /// </summary>
        public int BestMove
        {
            get { return bestMove; }
        }

        /// <summary>
        /// 搜索的结点数
        /// </summary>
        public int Nodes
        {
            get { return nodes; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public EndSolve():base()
        {
        }

        /// <summary>
        /// 无奇偶搜索
        /// </summary>
        /// <param name="board"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="color"></param>
        /// <param name="empties"></param>
        /// <param name="discdiff"></param>
        /// <param name="prevmove"></param>
        /// <returns></returns>
        public int NoParEndSolve(ChessType[] board, int alpha,int beta, ChessType color, int empties, int discdiff, int prevmove)
        {
            int score =  -highestScore; 
            ChessType oppcolor = 2 - color;
            int sqnum = -1, j, j1;
            int eval;
            Empties em, old_em;
            //计算搜索的结点数
            nodes++;
            for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
            {
                sqnum = em.Square;
                j = Board.DoFlips(board, sqnum, color, oppcolor);
                if (j > 0)
                {
                    //下棋 
                    board[sqnum] = color;
                    old_em.Succ = em.Succ;
                    if (empties == 2)
                    {
                        //翻转
                        j1 = Board.CountFlips(board, EmHead.Succ.Square, oppcolor, color);
                        if (j1 > 0)//我下然后对方下
                        {
                            eval = discdiff + 2 * (j - j1);
                        }
                        else //对方Pass
                        {
                            j1 = Board.CountFlips(board, EmHead.Succ.Square, color, oppcolor);
                            eval = discdiff + 2 * j;
                            if (j1 > 0)//我Pass
                            {
                                eval += 2 * (j1 + 1);
                            }
                            else
                            {
                                //双方都无棋可下，游戏结束
                                //if (WINNER_GETS_EMPTIES)
                                //{
                                if (eval >= 0)
                                    eval += 2;
                                //}
                                //else
                                //    ev++;
                            }
                        }
                    }
                    else
                    {
                        eval = -NoParEndSolve(board, -beta, -alpha,
                                 oppcolor, empties - 1, -discdiff - 2 * j - 1, sqnum);
                    }
                    Board.UndoFlips(board, j, oppcolor);
                    board[sqnum] = ChessType.EMPTY;
                    old_em.Succ = em;
                    /*计分*/
                    if (eval > score)
                    {
                        score = eval;

                        //=========================================
                        if (empties == searchDepth)
                        {
                            bestMove = sqnum;
                            UpdateMessage(score, nodes,sqnum);
                           // StaticMethod.UpdateThinkingMove(sqnum);
                        }
                        if (eval > alpha)
                        {
                            if (eval >= beta)
                            { /* 剪枝 */
                                return score;
                            }
                            alpha = eval;
                        }
                    }
                }
            }

            if (score == -highestScore)
            {
                //=========================================
                if (empties == searchDepth)
                {
                    bestMove = MVPASS;
                }
                if (prevmove == 0)
                { /* 游戏结束: */
                    //if (WINNER_GETS_EMPTIES)
                    //{
                    if (discdiff > 0) return discdiff + empties;
                    if (discdiff < 0) return discdiff - empties;
                    return 0;
                    //}
                    //else
                    //    return discdiff;
                }
                else
                    return -NoParEndSolve(board, -beta, -alpha, oppcolor, empties, -discdiff, 0);
            }
            return score;
        }

        /// <summary>
        /// 奇偶搜索
        /// </summary>
        /// <param name="board"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="color"></param>
        /// <param name="empties"></param>
        /// <param name="discdiff"></param>
        /// <param name="prevmove"></param>
        /// <returns></returns>
        public int ParEndSolve(ChessType[] board, int alpha, int beta, ChessType color, int empties, int discdiff, int prevmove)
        {
            int score = -highestScore; 
            ChessType oppcolor = 2 - color;
            int sqnum = -1;
            int j;
            int eval;
            Empties em, old_em;
            uint parity_mask;
            int par, holepar;
            //计算搜索的结点数
            nodes++;
            for (par = 1, parity_mask = RegionParity; par >= 0; par--, parity_mask = ~parity_mask)
            {
                for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
                {
                    holepar = em.HoleId;
                    holepar = em.Square;
                    holepar = em.HoleId;
                    sqnum = em.Square;
                    j = Board.DoFlips(board, sqnum, color, oppcolor);

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
                        Board.UndoFlips(board, j, oppcolor);
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
                                 UpdateMessage(score, nodes,sqnum);
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
            }

            if (score == -highestScore)
            {
                //=========================================
                if (empties == searchDepth)
                {
                    bestMove = MVPASS;
                }

                if (prevmove == 0)
                { /* 游戏结束: */
                    //if (WINNER_GETS_EMPTIES)
                    //{
                    if (discdiff > 0) return discdiff + empties;
                    if (discdiff < 0) return discdiff - empties;
                    return 0;
                    //}
                    //else
                    //    return discdiff;
                }
                else
                    return -ParEndSolve(board, -beta, -alpha, oppcolor, empties, -discdiff, 0);

            }
            return score;
        }

        /// <summary>
        /// 最快优先搜索
        /// </summary>
        /// <param name="board"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="color"></param>
        /// <param name="empties"></param>
        /// <param name="discdiff"></param>
        /// <param name="prevmove"></param>
        /// <returns></returns>
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
            for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
            {
                //计算搜索的结点数
                nodes++;
                sqnum = em.Square;
                flipped = Board.DoFlips(board, sqnum, color, oppcolor);
                if (flipped > 0)
                {
                    //下棋
                    board[sqnum] = color;
                    old_em.Succ = em.Succ;
                    //计算对方行动力
                    mobility = count_mobility(board, oppcolor);
                    //恢复到原来的棋局
                    Board.UndoFlips(board, flipped, oppcolor);
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
                    j = Board.DoFlips(board, sqnum, color, oppcolor);
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
                    Board.UndoFlips(board, j, oppcolor);
                    RegionParity ^= (uint)holepar;
                    board[sqnum] = ChessType.EMPTY;
                    em.Pred.Succ = em;
                    if (em.Succ != null)
                        em.Succ.Pred = em;

                    if (ev > score)
                    {
                        score = ev;
                        //=========================================
                        if (empties == searchDepth)
                        {
                            bestMove = sqnum;
                            UpdateMessage(score, nodes,sqnum);
                           // StaticMethod.UpdateThinkingMove(sqnum);
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
                //=========================================
                if (empties == searchDepth)
                {
                    bestMove = MVPASS;
                }
            }
            return score;
        }

        /// <summary>
        /// 终局搜索
        /// </summary>
        /// <param name="board"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="color"></param>
        /// <param name="empties"></param>
        /// <param name="discdiff"></param>
        /// <param name="prevmove"></param>
        /// <returns></returns>
        public int Solve(ChessType[] board, int alpha, int beta, ChessType color, int empties, int discdiff, int prevmove)
        {
            /* 初始化洞的奇偶,洞空格为奇数,该位为1,否则为0 ;*/
            /* 第一次给某位赋值，该位＝1，第二次就=0了，这样奇数次=1，偶数次=0 ;*/
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
