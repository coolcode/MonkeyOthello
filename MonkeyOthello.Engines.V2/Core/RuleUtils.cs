/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;


namespace MonkeyOthello.Engines.V2
{
    public static class RuleUtils
    {
        private enum Stability
        {
            NO,
            MEYBE,
            YES,
        }        
        
        private static int[] dirinc ={ 1, -1, 8, -8, 9, -9, 10, -10, 0 };
        //                    (r, l ,bl, tr, b,  t, br,  tl, N)
        //                    (¡ú,¡û,¨L, ¨J,¡ý, ¡ü, ¨K,  ¨I, .)


        /// <summary>
        /// direct mask, ref:DoFlips
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
         
        private static Stack<int> FlipStack = new Stack<int>();
         
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
         
        public static void UndoFlips(ChessType[] board, int FlipCount, ChessType oppcolor)
        { 
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
         
        public static bool AnyStab(ChessType[] board, int sqnum, ChessType color)
        {
            Stability stab1;
            Stability stab2;
            
            int j = dirmask[sqnum];
            // "\" 
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
            
            // "|" 
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
            
            // "/" 
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
           
            // "-" 
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
