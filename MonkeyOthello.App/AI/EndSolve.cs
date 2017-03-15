/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // ��Ȩ���С� 
          // �����ߣ�Fengart
          // �ļ�����EndSolve.cs
          // �ļ������������վ���������
//----------------------------------------------------------------*/

using MonkeyOthello.Core;
using System;

namespace MonkeyOthello.AI
{
    /// <summary>
    /// �վ���������
    /// </summary>
    class EndSolve:BaseSolve
    {

        /*�������û���������������34���ӣ�������27���ӡ���ʱ�����ּƷַ������ձ��ļƷַ��ǣ�ʣ��Ŀո�ȫ����ʤ����
         * ��ô����ʤ34+3-27=10���ӡ�ŷ�޵ļƷַ��ǣ�ʣ��Ŀո�˫������һ�룬��ô����ʤ(34+1.5)-(27+1.5)=7���ӡ�*/
        /// <summary>
        /// ʤ�ߵÿ�ģʽ(�������ձ��ļƷַ���)
        /// </summary>
        private const bool WINNER_GETS_EMPTIES = true;

        private const int highestScore = Constants.HighestScore;


        /// <summary>
        /// ��ż��
        /// </summary>
        private uint RegionParity;

        /// <summary>
        /// ����������õ�����λ��
        /// </summary>
        public int BestMove
        {
            get { return bestMove; }
        }

        /// <summary>
        /// �����Ľ����
        /// </summary>
        public int Nodes
        {
            get { return nodes; }
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        public EndSolve():base()
        {
        }

        /// <summary>
        /// ����ż����
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
            //���������Ľ����
            nodes++;
            for (old_em = EmHead, em = old_em.Succ; em != null; old_em = em, em = em.Succ)
            {
                sqnum = em.Square;
                j = Board.DoFlips(board, sqnum, color, oppcolor);
                if (j > 0)
                {
                    //���� 
                    board[sqnum] = color;
                    old_em.Succ = em.Succ;
                    if (empties == 2)
                    {
                        //��ת
                        j1 = Board.CountFlips(board, EmHead.Succ.Square, oppcolor, color);
                        if (j1 > 0)//����Ȼ��Է���
                        {
                            eval = discdiff + 2 * (j - j1);
                        }
                        else //�Է�Pass
                        {
                            j1 = Board.CountFlips(board, EmHead.Succ.Square, color, oppcolor);
                            eval = discdiff + 2 * j;
                            if (j1 > 0)//��Pass
                            {
                                eval += 2 * (j1 + 1);
                            }
                            else
                            {
                                //˫����������£���Ϸ����
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
                    /*�Ʒ�*/
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
                            { /* ��֦ */
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
                { /* ��Ϸ����: */
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
        /// ��ż����
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
            //���������Ľ����
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
                        //����
                        board[sqnum] = color;
                        //������ż
                        RegionParity ^= (uint)holepar;
                        //ɾ����λ
                        old_em.Succ = em.Succ;
                        if (empties <= 1 + Constants.Use_Parity)
                        {
                            eval = -NoParEndSolve(board, -beta, -alpha, oppcolor, empties - 1, -discdiff - 2 * j - 1, sqnum);

                        }
                        else
                        {
                            eval = -ParEndSolve(board, -beta, -alpha, oppcolor, empties - 1, -discdiff - 2 * j - 1, sqnum);
                        }
                        //�ָ���ԭ�������
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
                { /* ��Ϸ����: */
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
        /// �����������
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
                //���������Ľ����
                nodes++;
                sqnum = em.Square;
                flipped = Board.DoFlips(board, sqnum, color, oppcolor);
                if (flipped > 0)
                {
                    //����
                    board[sqnum] = color;
                    old_em.Succ = em.Succ;
                    //����Է��ж���
                    mobility = count_mobility(board, oppcolor);
                    //�ָ���ԭ�������
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
        /// �վ�����
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
            /* ��ʼ��������ż,���ո�Ϊ����,��λΪ1,����Ϊ0 ;*/
            /* ��һ�θ�ĳλ��ֵ����λ��1���ڶ��ξ�=0�ˣ�����������=1��ż����=0 ;*/
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
