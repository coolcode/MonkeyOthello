/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // 版权所有。 
          // 开发者：Fengart
          // 文件名：MidSolve.cs
          // 文件功能描述：中局搜索引擎
//----------------------------------------------------------------*/

using MonkeyOthello.Core;
using System;

namespace MonkeyOthello.AI
{
    /// <summary>
    /// 中局搜索引擎
    /// </summary>
    class MidSolve:BaseSolve
    {

        private const int MidBetterScore = 1800;
        private Evalation evalation;
        /// <summary>
        /// 搜索深度 
        /// </summary>
        public int SearchDepth
        {
            get { return searchDepth; }
            set { searchDepth = value; }
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

        public MidSolve()
            : base()
        {
            searchDepth = Config.Instance.MidDepth;
            evalation = new Evalation();
            nodes = 0;
        }       

        /// <summary>
        /// 最快优先搜索
        /// </summary>
        /// <param name="board">棋盘</param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="color">当前方的颜色</param>
        /// <param name="empties">空格数</param>
        /// <param name="discdiff">双方棋子数差</param>
        /// <param name="prevmove">上一步是否有棋可下</param>
        /// <returns></returns>
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
                //是否调用零窗口的标志
                bool fFoundPv = false;
                //计算搜索的结点数
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
                    flipped = Board.DoFlips(board, sqnum, color, oppcolor);
                    if (flipped > 0)
                    {
                        //下棋 
                        board[sqnum] = color;
                        old_em.Succ = em.Succ;
                        //计算对方行动力（下棋后，计算对方行动力，少的优先）
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
                        //优先选择
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
                        //下棋
                        sqnum = em.Square;
                        holepar = em.HoleId;
                        j = Board.DoFlips(board, sqnum, color, oppcolor);
                        board[sqnum] = color;

                        em.Pred.Succ = em.Succ;
                        if (em.Succ != null)
                            em.Succ.Pred = em.Pred;

                        nodes++;
                        //检测
                        if (fFoundPv) 
                        {
                            //调用零窗口
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
                        //恢复到上一步
                        Board.UndoFlips(board, j, oppcolor);
                        board[sqnum] = ChessType.EMPTY;
                        em.Pred.Succ = em;
                        if (em.Succ != null)
                            em.Succ.Pred = em;

                        if (eval > score)
                        {
                            score = eval;
                            //如果当前结点已经搜索完毕,就更新位置
                            //if (depth == searchDepth)
                            //{
                            //    bestMove = sqnum;
                            //    StaticMethod.UpdateMessage(score, nodes);
                            //    StaticMethod.UpdateThinkingMove(sqnum);
                            //}
                            if (eval > alpha)
                            {
                                if (eval >= beta)
                                {
                                    //剪枝
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
                    if (prevmove == 0)//游戏结束
                    {
                        if (discdiff > 0)//自己赢
                            return Constants.HighestScore;
                        if (discdiff < 0)//对方赢
                            return -Constants.HighestScore;
                        return 0;//平手
                    }
                    else  /* I pass: */
                        score = -FastestFirstSolve(board, -beta, -alpha, oppcolor,depth, empties, -discdiff, 0);
                    //如果当前结点已经搜索完毕,就更新位置
                    //if (depth == searchDepth)
                    //{
                    //    bestMove = MVPASS;
                    //}
                }
                return score;
            }
        }

        /// <summary>
        /// 宁静搜索
        /// </summary>
        /// <param name="board"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="color"></param>
        /// <param name="empties"></param>
        /// <param name="discdiff"></param>
        /// <param name="prevmove"></param>
        /// <returns></returns>
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

                //计算搜索的结点数
                //nodes++;
                score1 = evalation.MidEval2(board, color, oppcolor, empties,EmHead);
                if (score1 >=beta || score1>=3000 || depth == 0) return score1;
                else if (score1 > alpha)
                    alpha = score1;
                moves = 0;
                for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
                {
                    sqnum = em.Square;
                    flipped = Board.DoFlips(board, sqnum, color, oppcolor);
                    if (flipped > 0)
                    {
                        //下棋 
                        board[sqnum] = color;
                        old_em.Succ = em.Succ;
                        //计算行动力
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

                        em.Pred.Succ = em.Succ;
                        if (em.Succ != null)
                            em.Succ.Pred = em.Pred;

                        //计算搜索的结点数
                        nodes++;
                        //调用自身
                        eval = -evalation.MidEval2(board, oppcolor, color, empties - 1, EmHead);
                        // -QuiescenceSolve(board, -beta, -alpha, oppcolor, depth - 1, empties - 1, -discdiff - j - j - 1, sqnum);

                        Board.UndoFlips(board, j, oppcolor);
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
                                    //剪枝
                                    return score2;
                                }
                                alpha = eval;
                            }
                        }
                    }
                }
                else
                {
                    if (prevmove == 0)//游戏结束
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

        /// <summary>
        /// 中局根结点搜索
        /// </summary>
        /// <param name="board">棋盘</param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="color">当前方的颜色</param>
        /// <param name="empties">空格数</param>
        /// <param name="discdiff">双方棋子数差</param>
        /// <param name="prevmove">上一步是否有棋可下</param>
        /// <returns></returns>
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
                //是否调用零窗口的标志
                bool fFoundPv = false;
                
                moves = 0;
                for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
                {
                    sqnum = em.Square;
                    flipped = Board.DoFlips(board, sqnum, color, oppcolor);
                    if (flipped > 0)
                    {
                        //下棋 
                        board[sqnum] = color;
                        old_em.Succ = em.Succ;
                        //计算对方行动力（下棋后，计算对方行动力，少的优先）
                        mobility = -count_mobility(board, oppcolor);
                       // mobility = -FastestFirstSolve(board, -beta, -alpha, oppcolor, 3, empties - 1, -discdiff - 2 * flipped - 1, sqnum);
                        //恢复到原来的棋局
                        Board.UndoFlips(board, flipped, oppcolor);
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
                        //优先选择
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
                        //下棋
                        sqnum = em.Square;
                        holepar = em.HoleId;
                        j = Board.DoFlips(board, sqnum, color, oppcolor);
                        board[sqnum] = color;

                        em.Pred.Succ = em.Succ;
                        if (em.Succ != null)
                            em.Succ.Pred = em.Pred;

                        nodes++;
                        //检测
                        if (fFoundPv)
                        {
                            //调用零窗口
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
                        //恢复到上一步
                        Board.UndoFlips(board, j, oppcolor);
                        board[sqnum] = ChessType.EMPTY;
                        em.Pred.Succ = em;
                        if (em.Succ != null)
                            em.Succ.Pred = em;

                        if (eval > score)
                        {
                            score = eval;
                            //更新位置
                            bestMove = sqnum;
                            UpdateMessage(score, nodes,sqnum);
                            if (eval > alpha)
                            {
                                if (eval >= beta)
                                {
                                    //剪枝
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
                    //如果当前结点已经搜索完毕,就更新位置
                    if (depth == searchDepth)
                    {
                        bestMove = MVPASS;
                    }
                }
                return score;
            }
        }

        //private const int tryDepth = 4;
        //private const double pp = 1.5, psigma = 200,pb=10,pa=0.8;

        ///// <summary>
        ///// 最快优先搜索
        ///// </summary>
        ///// <param name="board">棋盘</param>
        ///// <param name="alpha"></param>
        ///// <param name="beta"></param>
        ///// <param name="color">当前方的颜色</param>
        ///// <param name="empties">空格数</param>
        ///// <param name="discdiff">双方棋子数差</param>
        ///// <param name="prevmove">上一步是否有棋可下</param>
        ///// <returns></returns>
        //public int MPCMidSolve(ChessType[] board, int alpha, int beta, ChessType color, int depth, int empties, int discdiff, int prevmove)
        //{
        //    lock (this)
        //    {
        //        int i, j;
        //        int score = -Constants.HighestScore - 1;
        //        ChessType oppcolor = 2 - color;
        //        int sqnum;
        //        int eval;
        //        int flipped;
        //        int moves, mobility;
        //        int best_value, best_index;
        //        Empties em, old_em;
        //        Empties[] move_ptr = new Empties[Constants.MaxEmpties];
        //        int holepar;
        //        int[] goodness = new int[Constants.MaxEmpties];
        //        //是否调用零窗口的标志
        //        bool fFoundPv = false;
        //        //计算搜索的结点数
        //        //nodes++;
        //        if (depth == 0)
        //        {
        //            nodes++;
        //            //return evalation.MidEval2(board, color, oppcolor, empties,EmHead);
        //            return QuiescenceSolve(board, alpha, beta, color, 2, empties, discdiff, prevmove);
        //        }

        //        int bound = (int)((pp * psigma + beta - pb) / pa);
        //        //调用零窗口
        //        if (FastestFirstMidSolve(board, bound - 1, bound, color, tryDepth, empties, discdiff,prevmove) >= bound)
        //            return beta;
        //        bound = (int)((-pp * psigma + alpha - pb) / pa);
        //        //调用零窗口
        //        if (FastestFirstMidSolve(board, bound, bound + 1, color, tryDepth, empties, discdiff,prevmove) <= bound)
        //            return alpha;

        //        moves = 0;
        //        for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
        //        {
        //            sqnum = em.Square;
        //            flipped = Board.DoFlips(board, sqnum, color, oppcolor);
        //            if (flipped > 0)
        //            {
        //                //下棋 
        //                board[sqnum] = color;
        //                old_em.Succ = em.Succ;
        //                //计算对方行动力（下棋后，计算对方行动力，少的优先）
        //                mobility = count_mobility(board, oppcolor);
        //                //恢复到原来的棋局
        //                Board.UndoFlips(board, flipped, oppcolor);
        //                old_em.Succ = em;
        //                board[sqnum] = ChessType.EMPTY;
        //                move_ptr[moves] = em;
        //                goodness[moves] = -mobility;
        //                moves++;
        //            }
        //        }

        //        if (moves != 0)
        //        {
        //            for (i = 0; i < moves; i++)
        //            {
        //                //优先选择
        //                best_value = goodness[i];
        //                best_index = i;
        //                for (j = i + 1; j < moves; j++)
        //                    if (goodness[j] > best_value)
        //                    {
        //                        best_value = goodness[j];
        //                        best_index = j;
        //                    }
        //                em = move_ptr[best_index];
        //                move_ptr[best_index] = move_ptr[i];
        //                goodness[best_index] = goodness[i];
        //                //下棋
        //                sqnum = em.Square;
        //                holepar = em.HoleId;
        //                j = Board.DoFlips(board, sqnum, color, oppcolor);
        //                board[sqnum] = color;

        //                em.Pred.Succ = em.Succ;
        //                if (em.Succ != null)
        //                    em.Succ.Pred = em.Pred;

        //                nodes++;
        //                //检测
        //                if (fFoundPv)
        //                {
        //                    //调用零窗口
        //                    eval = -MPCMidSolve(board, -alpha - 1, -alpha, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
        //                    if ((eval > alpha) && (eval < beta))
        //                    {
        //                        eval = -MPCMidSolve(board, -beta, -eval, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
        //                        //eval = -FastestFirstMidSolve(board, -beta, -alpha, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
        //                    }
        //                }
        //                else
        //                {
        //                    eval = -MPCMidSolve(board, -beta, -alpha, oppcolor, depth - 1, empties - 1, -discdiff - 2 * j - 1, sqnum);
        //                }
        //                //恢复到上一步
        //                Board.UndoFlips(board, j, oppcolor);
        //                board[sqnum] = ChessType.EMPTY;
        //                em.Pred.Succ = em;
        //                if (em.Succ != null)
        //                    em.Succ.Pred = em;

        //                if (eval > score)
        //                {
        //                    score = eval;
        //                    //如果当前结点已经搜索完毕,就更新位置
        //                    if (depth == searchDepth)
        //                    {
        //                        bestMove = sqnum;
        //                        StaticMethod.UpdateMessage(score, nodes);
        //                        StaticMethod.UpdateThinkingMove(sqnum);
        //                    }
        //                    if (eval > alpha)
        //                    {
        //                        if (eval >= beta)
        //                        {
        //                            //剪枝
        //                            return score;
        //                        }
        //                        alpha = eval;
        //                        fFoundPv = true;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (prevmove == 0)//游戏结束
        //            {
        //                if (discdiff > 0)//自己赢
        //                    return Constants.HighestScore;
        //                if (discdiff < 0)//对方赢
        //                    return -Constants.HighestScore;
        //                return 0;//平手
        //            }
        //            else  /* I pass: */
        //                score = -MPCMidSolve(board, -beta, -alpha, oppcolor, depth, empties, -discdiff, 0);
        //            //如果当前结点已经搜索完毕,就更新位置
        //            if (depth == searchDepth)
        //            {
        //                bestMove = MVPASS;
        //            }
        //        }
        //        return score;
        //    }
        //}

        /// <summary>
        /// 中局搜索
        /// </summary>
        /// <param name="board"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="color"></param>
        /// <param name="empties"></param>
        /// <param name="discdiff"></param>
        /// <param name="prevmove"></param>
        /// <returns></returns>
        public double Solve(ChessType[] board, int alpha, int beta, ChessType color, int empties, int discdiff, int prevmove)
        {
            nodes = 0;
            searchDepth = Config.Instance.MidDepth;
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
