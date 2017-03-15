/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // 版权所有。 
          // 开发者：Fengart
          // 文件名： 
          // 文件功能描述：
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;


namespace MonkeyOthello.Core
{
    /// <summary>
    /// 棋盘结构以及AI
    /// </summary>
    public static class Board
    {
        /// <summary>
        /// 稳定程度
        /// </summary>
        private enum Stability
        {
            /// <summary>
            /// 一点也不稳定
            /// </summary>
            NO,
            /// <summary>
            /// 可能稳定
            /// </summary>
            MEYBE,
            /// <summary>
            /// 绝对稳定
            /// </summary>
            YES,
        }

        /// <summary>
        /// 8个方向的增量,比如向左下移动一格,棋盘数组就要加8
        /// </summary>
        private static int[] dirinc ={ 1, -1, 8, -8, 9, -9, 10, -10, 0 };
        //                    (r, l ,bl, tr, b,  t, br,  tl, N)
        //                    (→,←,↙, ↗,↓, ↑, ↘,  ↖, .)


        /// <summary>
        /// 方向蒙版,用位指明某位置需要搜索哪几个方向,提高搜索速度
        /// 从低位到高位分别为Dir 0-7  (参见DoFlips函数)
        /// </summary>
        private static int[] dirmask = new int[91] 
        {
            0,0,0,0,0,0,0,0,0,
            0,81,81,87,87,87,87,22,22,
            0,81,81,87,87,87,87,22,22,
            0,121,121,255,255,255,255,182,182,
            0,121,121,255,255,255,255,182,182,
            0,121,121,255,255,255,255,182,182,
            0,121,121,255,255,255,255,182,182,
            0,41,41,171,171,171,171,162,162,
            0,41,41,171,171,171,171,162,162,
            0,0,0,0,0,0,0,0,0,0
        };


        /// <summary>
        /// 被翻转的棋子索引栈
        /// </summary>
        private static Stack<int> FlipStack = new Stack<int>();

        /// <summary>
        /// 初始化棋盘
        /// </summary>
        /// <param name="board"></param>
        public static void Initial(ChessType[] board)
        {
            for (int i = 0; i <=9; i++)
            {
                board[i] = ChessType.DUMMY;
            }
            for (int i = 10; i <= 80; i++)
            {
                board[i] = ChessType.EMPTY;
            }
            for (int i = 18; i <= 72; i+=9)
            {
                board[i] = ChessType.DUMMY;
            }
            for (int i = 81; i <= 90; i++)
            {
                board[i] = ChessType.DUMMY;
            }
        }

        /// <summary>
        /// （单方向翻转）
        /// 翻转棋子并把翻转的棋子索引压到FilpStack（被翻转的棋子索引栈）里
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="inc"></param>
        /// <param name="color"></param>
        /// <param name="oppcolor"></param>
        private static void DrctnlFlips(ChessType[] board, int boardIndex, int inc, ChessType color, ChessType oppcolor)
        {
            int pt = boardIndex + inc;
            if (board[pt] == oppcolor)
            {
                pt += inc;
                if (board[pt] == oppcolor)/*2*/
                {
                    pt += inc;
                    if (board[pt] == oppcolor) /*3*/
                    {
                        pt += inc;
                        if (board[pt] == oppcolor) /*4*/
                        {
                            pt += inc;
                            if (board[pt] == oppcolor) /*5*/
                            {
                                pt += inc;
                                if (board[pt] == oppcolor) /*6*/
                                {
                                    pt += inc;
                                }
                            }
                        }
                    }
                }
                if (board[pt] == color)
                {
                    pt -= inc;
                    do
                    {
                        board[pt] = color;
                        FlipStack.Push(pt);
                        pt -= inc;
                    } while (pt != boardIndex);
                }
            }
        }

        /// <summary>
        /// （所有方向翻转）
        /// 翻转棋子并把翻转的棋子索引压到FilpStack（被翻转的棋子索引栈）里
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="sqnum"></param>
        /// <param name="color"></param>
        /// <param name="oppcolor"></param>
        /// <returns>返回被翻转的棋子数</returns>
        public static int DoFlips(ChessType[] board, int sqnum, ChessType color, ChessType oppcolor)
        {
            int j = dirmask[sqnum];
            int sq = sqnum;
            int old_count = FlipStack.Count;
            if ((j & 128) !=0)
                DrctnlFlips(board,sq, dirinc[7], color, oppcolor);
            if ((j & 64)  !=0)
                DrctnlFlips(board, sq, dirinc[6], color, oppcolor);
            if ((j & 32)  !=0)
                DrctnlFlips(board, sq, dirinc[5], color, oppcolor);
            if ((j & 16)  !=0)
                DrctnlFlips(board, sq, dirinc[4], color, oppcolor);
            if ((j & 8)  !=0)
                DrctnlFlips(board, sq, dirinc[3], color, oppcolor);
            if ((j & 4)  !=0)
                DrctnlFlips(board, sq, dirinc[2], color, oppcolor);
            if ((j & 2)  !=0)
                DrctnlFlips(board, sq, dirinc[1], color, oppcolor);
            if ((j & 1)  !=0)
                DrctnlFlips(board, sq, dirinc[0], color, oppcolor);
            return FlipStack.Count-old_count;
        }

        /// <summary>
        /// （单方向）
        /// 计算被翻转的棋子数
        /// （在终局时，我们只是计算被翻转的棋子数，而不用更新棋盘）
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="inc"></param>
        /// <param name="color"></param>
        /// <param name="oppcolor"></param>
        private static int CtDrctnlFlips(ChessType[] board, int boardIndex, int inc, ChessType color, ChessType oppcolor)
        {
            int count;
            int pt = boardIndex + inc;
            if (board[pt] == oppcolor)
            {
                count = 1;
                pt += inc;
                if (board[pt] == oppcolor) /*2*/
                {
                    count = 2;
                    pt += inc;
                    if (board[pt] == oppcolor) /*3*/
                    {
                        count = 3;
                        pt += inc;
                        if (board[pt] == oppcolor)/*4*/
                        {
                            count = 4;
                            pt += inc;
                            if (board[pt] == oppcolor) /*5*/
                            {
                                count = 5;
                                pt += inc;
                                if (board[pt] == oppcolor)/*6*/
                                {
                                    count = 6;
                                    pt += inc;
                                }
                            }
                        }
                    }
                }
                if (board[pt] == color)
                {
                    return count;
                }
            }
            return 0;
        }

        /// <summary>
        /// （所有方向）
        /// 计算被翻转的棋子数
        /// （在终局时，我们只是计算被翻转的棋子数，而不用更新棋盘）
        /// </summary>
        /// <param name="board"></param>
        /// <param name="sqnum"></param>
        /// <param name="color"></param>
        /// <param name="oppcolor"></param>
        /// <returns>返回被翻转的棋子数</returns>
        public static int CountFlips(ChessType[] board, int sqnum, ChessType color, ChessType oppcolor)
        {
            int count = 0;
            int j = dirmask[sqnum];
            if ((j & 128)  !=0)
                count += CtDrctnlFlips(board, sqnum, dirinc[7], color, oppcolor);
            if ((j & 64)  !=0)
                count += CtDrctnlFlips(board, sqnum, dirinc[6], color, oppcolor);
            if ((j & 32)  !=0)
                count += CtDrctnlFlips(board, sqnum, dirinc[5], color, oppcolor);
            if ((j & 16)  !=0)
                count += CtDrctnlFlips(board, sqnum, dirinc[4], color, oppcolor);
            if ((j & 8)  !=0)
                count += CtDrctnlFlips(board, sqnum, dirinc[3], color, oppcolor);
            if ((j & 4)  !=0)
                count += CtDrctnlFlips(board, sqnum, dirinc[2], color, oppcolor);
            if ((j & 2)  !=0)
                count += CtDrctnlFlips(board, sqnum, dirinc[1], color, oppcolor);
            if ((j & 1)  !=0)
                count += CtDrctnlFlips(board, sqnum, dirinc[0], color, oppcolor);
            return count;
        }

        /// <summary>
        /// （单方向）
        /// 是否能下棋（有时候我们只需要知道该位置是否能下棋，而不求翻转的棋子数）
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="inc"></param>
        /// <param name="color"></param>
        /// <param name="oppcolor"></param>
        /// <returns></returns>
        private static  bool AnyDrctnlFlips(ChessType[] board, int boardIndex, int inc, ChessType color, ChessType oppcolor)
        {
            int pt = boardIndex + inc;
            if (board[pt] == oppcolor)
            {
                pt += inc;
                if (board[pt] == oppcolor) /*2*/
                {
                    pt += inc;
                    if (board[pt] == oppcolor) /*3*/
                    {
                        pt += inc;
                        if (board[pt] == oppcolor)/*4*/
                        {
                            pt += inc;
                            if (board[pt] == oppcolor) /*5*/
                            {
                                pt += inc;
                                if (board[pt] == oppcolor)/*6*/
                                {
                                    pt += inc;
                                }
                            }
                        }
                    }
                }
                return (board[pt] == color);
            }
            return false;
        }

        ///// <summary>
        ///// （所有方向）
        ///// 是否能下棋（有时候我们只需要知道该位置是否能下棋，而不求翻转的棋子数）
        ///// </summary>
        ///// <param name="board"></param>
        ///// <param name="sqnum"></param>
        ///// <param name="color"></param>
        ///// <param name="oppcolor"></param>
        ///// <returns></returns>
        //public static bool AnyFlips(ChessType[] board, int sqnum, ChessType color, ChessType oppcolor)
        //{
        //    bool canFilps = false;
        //    int j = dirmask[sqnum];
        //    if ((j & 128) != 0)
        //        canFilps |= AnyDrctnlFlips(board, sqnum, dirinc[7], color, oppcolor);
        //    if ((j & 64) != 0)
        //        canFilps |= AnyDrctnlFlips(board, sqnum, dirinc[6], color, oppcolor);
        //    if ((j & 32) != 0)
        //        canFilps |= AnyDrctnlFlips(board, sqnum, dirinc[5], color, oppcolor);
        //    if ((j & 16) != 0)
        //        canFilps |= AnyDrctnlFlips(board, sqnum, dirinc[4], color, oppcolor);
        //    if ((j & 8) != 0)
        //        canFilps |= AnyDrctnlFlips(board, sqnum, dirinc[3], color, oppcolor);
        //    if ((j & 4) != 0)
        //        canFilps |= AnyDrctnlFlips(board, sqnum, dirinc[2], color, oppcolor);
        //    if ((j & 2) != 0)
        //        canFilps |= AnyDrctnlFlips(board, sqnum, dirinc[1], color, oppcolor);
        //    if ((j & 1) != 0)
        //        canFilps |= AnyDrctnlFlips(board, sqnum, dirinc[0], color, oppcolor);
        //    return canFilps;
        //}

        /// <summary>
        /// （所有方向）
        /// 是否能下棋（有时候我们只需要知道该位置是否能下棋，而不求翻转的棋子数）
        /// </summary>
        /// <param name="board"></param>
        /// <param name="sqnum"></param>
        /// <param name="color"></param>
        /// <param name="oppcolor"></param>
        /// <returns></returns>
        public static bool AnyFlips(ChessType[] board, int sqnum, ChessType color, ChessType oppcolor)
        {
            int j = dirmask[sqnum];
            if ((j & 128) != 0 && AnyDrctnlFlips(board, sqnum, dirinc[7], color, oppcolor))
                return true;
            if ((j & 64) != 0 && AnyDrctnlFlips(board, sqnum, dirinc[6], color, oppcolor))
                return true;
            if ((j & 32) != 0 && AnyDrctnlFlips(board, sqnum, dirinc[5], color, oppcolor))
                return true;
            if ((j & 16) != 0 && AnyDrctnlFlips(board, sqnum, dirinc[4], color, oppcolor))
                return true;
            if ((j & 8) != 0 && AnyDrctnlFlips(board, sqnum, dirinc[3], color, oppcolor))
                return true;
            if ((j & 4) != 0 && AnyDrctnlFlips(board, sqnum, dirinc[2], color, oppcolor))
                return true;
            if ((j & 2) != 0 && AnyDrctnlFlips(board, sqnum, dirinc[1], color, oppcolor))
                return true;
            if ((j & 1) != 0 && AnyDrctnlFlips(board, sqnum, dirinc[0], color, oppcolor))
                return true;
            return false;
        }


        /// <summary>
        /// 恢复翻转的棋子，但不包括下棋的子
        /// </summary>
        /// <param name="FlipCount"></param>
        /// <param name="oppcol"></param>
        public static void UndoFlips(ChessType[] board, int FlipCount, ChessType oppcolor)
        {
            //判断FlipCount是否为奇数
            if ((FlipCount & 1) == 1)
            {
                FlipCount--;
                board[FlipStack.Pop()] = oppcolor;
            }
            while (FlipCount > 0)
            {
                FlipCount -= 2;
                board[FlipStack.Pop()] = oppcolor;
                board[FlipStack.Pop()] = oppcolor;
            }
        }


        /// <summary>
        /// 是否稳定
        /// </summary>
        /// <param name="board"></param>
        /// <param name="boardIndex"></param>
        /// <param name="inc"></param>
        /// <param name="color"></param>
        /// <returns>返回稳定的程度</returns>
        private static Stability DrctnlStab(ChessType[] board, int boardIndex, int inc, ChessType color)
        {
            int pt = boardIndex;
            ChessType curcol;
            do
            {
                pt += inc;
                curcol = board[pt];
                if (curcol == ChessType.DUMMY)
                    return Stability.YES;
            } while (curcol == color);
            if(board[pt]== ChessType.EMPTY)
                return Stability.NO;
            do
            {
                if (board[pt] == ChessType.DUMMY)
                    return Stability.MEYBE;
                pt += inc;
            } while (board[pt] != ChessType.EMPTY);
            return Stability.NO;
        }

        /// <summary>
        /// （所有方向）
        /// 是否稳定
        /// </summary>
        /// <param name="board"></param>
        /// <param name="sqnum">只能是非空格位置</param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool AnyStab(ChessType[] board, int sqnum, ChessType color)
        {
            Stability stab1;
            Stability stab2;
            
            int j = dirmask[sqnum];
            // "\"方向
            if (((j & 128) != 0) && ((j & 64) != 0))
            {
                stab1 = DrctnlStab(board, sqnum, dirinc[7], color);
                if (stab1 != Stability.YES)      
                {
                    stab2 = DrctnlStab(board, sqnum, dirinc[6], color);
                    if (stab2 != Stability.YES)                       
                    {
                        if (stab1 == Stability.NO || stab2 == Stability.NO)
                            return false;
                    }
                }
            }
            
            // "|"方向
            if (((j & 32) != 0) && ((j & 16) != 0))
            {
                stab1 = DrctnlStab(board, sqnum, dirinc[5], color);
                if (stab1 != Stability.YES)
                {
                    stab2 = DrctnlStab(board, sqnum, dirinc[4], color);
                    if (stab2 != Stability.YES)
                    {
                        if (stab1 == Stability.NO || stab2 == Stability.NO)
                            return false;
                    }
                }
            }
            
            // "/"方向
            if (((j & 8) != 0) && ((j & 4) != 0))
            {
                stab1 = DrctnlStab(board, sqnum, dirinc[3], color);
                if (stab1 != Stability.YES)
                {
                    stab2 = DrctnlStab(board, sqnum, dirinc[2], color);
                    if (stab2 != Stability.YES)
                    {
                        if (stab1 == Stability.NO || stab2 == Stability.NO)
                            return false;
                    }
                }
            }
           
            // "-"方向
            if (((j & 2) != 0) && ((j & 1) != 0))
            {
                stab1 = DrctnlStab(board, sqnum, dirinc[1], color);
                if (stab1 != Stability.YES)
                {
                    stab2 = DrctnlStab(board, sqnum, dirinc[0], color);
                    if (stab2 != Stability.YES)
                    {
                        if (stab1 == Stability.NO || stab2 == Stability.NO)
                            return false;
                    }
                }
            }
            
            return true;
        }

        /// <summary>
        /// 是否潜在行动力
        /// </summary>
        /// <param name="board"></param>
        /// <param name="sqnum">只能是空格位置</param>
        /// <param name="color"></param>
        /// <param name="oppcolor"></param>
        /// <returns></returns>
        public static bool AnyPotMobility(ChessType[] board, int sqnum, ChessType oppcolor)
        {
            if ((board[sqnum + 1] == oppcolor) ||
                   (board[sqnum - 1] == oppcolor) ||
                   (board[sqnum + 8] == oppcolor) ||
                   (board[sqnum - 8] == oppcolor) ||
                   (board[sqnum + 9] == oppcolor) ||
                   (board[sqnum - 9] == oppcolor) ||
                   (board[sqnum + 10] == oppcolor) ||
                   (board[sqnum - 10] == oppcolor))
                return true;
            return false;
        }

        /// <summary>
        /// （所有方向翻转）
        /// 翻转棋子并把翻转的棋子索引压到FilpStack（被翻转的棋子索引栈）里
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="sqnum"></param>
        /// <param name="color"></param>
        /// <param name="oppcolor"></param>
        /// <returns>返回被翻转的棋子数</returns>
        public static List<int> DrawBoardFlips(ChessType[] board, int sqnum, ChessType color, ChessType oppcolor)
        {
            int j = dirmask[sqnum];
            int sq = sqnum;
            int old_count = FlipStack.Count;
            int flips;
            if ((j & 128) != 0)
                DrctnlFlips(board, sq, dirinc[7], color, oppcolor);
            if ((j & 64) != 0)
                DrctnlFlips(board, sq, dirinc[6], color, oppcolor);
            if ((j & 32) != 0)
                DrctnlFlips(board, sq, dirinc[5], color, oppcolor);
            if ((j & 16) != 0)
                DrctnlFlips(board, sq, dirinc[4], color, oppcolor);
            if ((j & 8) != 0)
                DrctnlFlips(board, sq, dirinc[3], color, oppcolor);
            if ((j & 4) != 0)
                DrctnlFlips(board, sq, dirinc[2], color, oppcolor);
            if ((j & 2) != 0)
                DrctnlFlips(board, sq, dirinc[1], color, oppcolor);
            if ((j & 1) != 0)
                DrctnlFlips(board, sq, dirinc[0], color, oppcolor);
            flips=FlipStack.Count - old_count;
            List<int> FlipList = new List<int>(flips);
            Stack<int>.Enumerator en = FlipStack.GetEnumerator();
            for (int i = 0; i < flips; i++)
                if (en.MoveNext())
                    FlipList.Add(en.Current);
            return FlipList;
        }

    }
}
