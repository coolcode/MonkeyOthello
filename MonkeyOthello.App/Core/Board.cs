/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // ��Ȩ���С� 
          // �����ߣ�Fengart
          // �ļ����� 
          // �ļ�����������
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;


namespace MonkeyOthello.Core
{
    /// <summary>
    /// ���̽ṹ�Լ�AI
    /// </summary>
    public static class Board
    {
        /// <summary>
        /// �ȶ��̶�
        /// </summary>
        private enum Stability
        {
            /// <summary>
            /// һ��Ҳ���ȶ�
            /// </summary>
            NO,
            /// <summary>
            /// �����ȶ�
            /// </summary>
            MEYBE,
            /// <summary>
            /// �����ȶ�
            /// </summary>
            YES,
        }

        /// <summary>
        /// 8�����������,�����������ƶ�һ��,���������Ҫ��8
        /// </summary>
        private static int[] dirinc ={ 1, -1, 8, -8, 9, -9, 10, -10, 0 };
        //                    (r, l ,bl, tr, b,  t, br,  tl, N)
        //                    (��,��,�L, �J,��, ��, �K,  �I, .)


        /// <summary>
        /// �����ɰ�,��λָ��ĳλ����Ҫ�����ļ�������,��������ٶ�
        /// �ӵ�λ����λ�ֱ�ΪDir 0-7  (�μ�DoFlips����)
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
        /// ����ת����������ջ
        /// </summary>
        private static Stack<int> FlipStack = new Stack<int>();

        /// <summary>
        /// ��ʼ������
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
        /// ��������ת��
        /// ��ת���Ӳ��ѷ�ת����������ѹ��FilpStack������ת����������ջ����
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
        /// �����з���ת��
        /// ��ת���Ӳ��ѷ�ת����������ѹ��FilpStack������ת����������ջ����
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="sqnum"></param>
        /// <param name="color"></param>
        /// <param name="oppcolor"></param>
        /// <returns>���ر���ת��������</returns>
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
        /// ��������
        /// ���㱻��ת��������
        /// �����վ�ʱ������ֻ�Ǽ��㱻��ת���������������ø������̣�
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
        /// �����з���
        /// ���㱻��ת��������
        /// �����վ�ʱ������ֻ�Ǽ��㱻��ת���������������ø������̣�
        /// </summary>
        /// <param name="board"></param>
        /// <param name="sqnum"></param>
        /// <param name="color"></param>
        /// <param name="oppcolor"></param>
        /// <returns>���ر���ת��������</returns>
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
        /// ��������
        /// �Ƿ������壨��ʱ������ֻ��Ҫ֪����λ���Ƿ������壬������ת����������
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
        ///// �����з���
        ///// �Ƿ������壨��ʱ������ֻ��Ҫ֪����λ���Ƿ������壬������ת����������
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
        /// �����з���
        /// �Ƿ������壨��ʱ������ֻ��Ҫ֪����λ���Ƿ������壬������ת����������
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
        /// �ָ���ת�����ӣ����������������
        /// </summary>
        /// <param name="FlipCount"></param>
        /// <param name="oppcol"></param>
        public static void UndoFlips(ChessType[] board, int FlipCount, ChessType oppcolor)
        {
            //�ж�FlipCount�Ƿ�Ϊ����
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
        /// �Ƿ��ȶ�
        /// </summary>
        /// <param name="board"></param>
        /// <param name="boardIndex"></param>
        /// <param name="inc"></param>
        /// <param name="color"></param>
        /// <returns>�����ȶ��ĳ̶�</returns>
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
        /// �����з���
        /// �Ƿ��ȶ�
        /// </summary>
        /// <param name="board"></param>
        /// <param name="sqnum">ֻ���Ƿǿո�λ��</param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool AnyStab(ChessType[] board, int sqnum, ChessType color)
        {
            Stability stab1;
            Stability stab2;
            
            int j = dirmask[sqnum];
            // "\"����
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
            
            // "|"����
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
            
            // "/"����
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
           
            // "-"����
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
        /// �Ƿ�Ǳ���ж���
        /// </summary>
        /// <param name="board"></param>
        /// <param name="sqnum">ֻ���ǿո�λ��</param>
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
        /// �����з���ת��
        /// ��ת���Ӳ��ѷ�ת����������ѹ��FilpStack������ת����������ջ����
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <param name="sqnum"></param>
        /// <param name="color"></param>
        /// <param name="oppcolor"></param>
        /// <returns>���ر���ת��������</returns>
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
