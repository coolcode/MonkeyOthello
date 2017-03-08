/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // ��Ȩ���С� 
          // �����ߣ�Fengart
          // �ļ����� 
          // �ļ�����������
//----------------------------------------------------------------*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MonkeyOthello.Core;

namespace MonkeyOthello.Presentation
{
    /// <summary>
    /// ���̻�ͼ��
    /// </summary>
    public class DrawBoard
    {
        /// <summary>
        /// �岽
        /// </summary>
        [Serializable]
        public struct Move
        {
            /// <summary>
            /// ����λ��
            /// </summary>
            public int Square;

            /// <summary>
            /// ��ת��������Ŀ
            /// </summary>
            public int Flips;

            /// <summary>
            /// ��ǰ������ɫ
            /// </summary>
            public ChessType Color;
        }

        /// <summary>
        /// ����ͼƬ
        /// </summary>
        private Image blackStone, canputBlack;
        private Image whiteStone, canputWhite;
        private Image empty;
        private Image background;

        /// <summary>
        /// ���巭תΪ�����һ��ͼƬ
        /// </summary>
        private Image[] BlacktoWhite; 

        /// <summary>
        /// ����ͼƬ
        /// </summary>
        private Bitmap bmpBuffer;

        public Bitmap BmpBuffer
        {
            get { return bmpBuffer; }
        }
        /// <summary>
        /// ���̳���
        /// </summary>
        //private int boardLength;
        /// <summary>
        /// ���̱߿�
        /// </summary>
        private PointF boardBound=new PointF(24.4f,24);
        private SizeF chessboundSize = new SizeF(44.5f, 44.5f);
        /// <summary>
        /// ����ֱ��
        /// </summary>
        private Size chessSize=new Size(44,44);
        /// <summary>
        /// ��������
        /// </summary>
        private const int rowsNum = Constants.RowsNum;
        /// <summary>
        /// ���̷�����
        /// </summary>
        private const int count = Constants.RowsNum * Constants.RowsNum;

        private const int flipsNum=12;
        /// <summary>
        /// ��һ��������λ��
        /// </summary>
        private Move curMove;

        /// <summary>
        /// ��ǰ���巽����������
        /// </summary>
        private ChessType curColor;

        /// <summary>
        /// ��������
        /// </summary>
        private ChessType[] board;

        /// <summary>
        /// ���̵�GDI
        /// </summary>
        private Graphics GraphicsBuffer;

        /// <summary>
        /// �ܹ������λ��
        /// </summary>
        private List<int> canputSquareList;

        /// <summary>
        /// ��ǰ����ת������
        /// </summary>
        private List<int> flipList;

        /// <summary>
        /// �岽ջ
        /// </summary>
        private Stack<Move> moveStack;
        

        private int blackNum;

        private int whiteNum;

        private int empties;

        /// <summary>
        ///  ��ǰ���巽����������
        /// </summary>
        public ChessType CurColor
        {
            get { return curColor; }
            set { curColor = value; }
        }

        /// <summary>
        /// �岽ջ
        /// </summary>
        public Stack<Move> MoveStack
        {
            get { return moveStack; }
            set { moveStack = value; }
        }
        /// <summary>
        /// ������Ŀ
        /// </summary>
        public int BlackNum
        {
            get { return blackNum; }
        }

        /// <summary>
        /// ������Ŀ 
        /// </summary>
        public int WhiteNum
        {
            get { return whiteNum; }
        }

        /// <summary>
        /// ��λ��Ŀ
        /// </summary>
        public int Empties
        {
            get { return empties; }
        }

        /// <summary>
        /// ���̵Ĺ��캯��
        /// </summary>
        public DrawBoard()
        {
            initialResources();
            curColor = ChessType.BLACK;
            bmpBuffer = new Bitmap(400, 400);
            GraphicsBuffer = Graphics.FromImage(bmpBuffer);
        }

        /// <summary>
        /// �������̵����̽ṹ
        /// </summary>
        /// <param name="value">���̽ṹ</param>
        public void SetBoardStruct(ChessType[] value)
        {
            board = value;
            whiteNum = 0;
            blackNum = 0;
            empties = 0;
            curMove =new Move ();
            canputSquareList = new List<int>(Constants.MaxEmpties / 2);
            moveStack = new Stack<Move>(Constants.MaxEmpties - 4);
            for (int i = 10; i <= 80;i++ )
            {
                switch (board[i])
                {
                    case ChessType.BLACK:
                        blackNum++;
                        break;
                    case ChessType.WHITE:
                        whiteNum++;
                        break;
                    case ChessType.EMPTY:
                        empties++;
                        break;
                    default:
                        break;
                }
            }            
        }

        //��ʼ����Դ
        private void initialResources()
        {
            //����Դ�л�ȡ����,���ӵ�ͼƬ
            blackStone = global::MonkeyOthello.Properties.Resources.BlackStone;
            whiteStone = global::MonkeyOthello.Properties.Resources.WhiteStone;
            canputBlack = global::MonkeyOthello.Properties.Resources.CanputBlack;
            canputWhite = global::MonkeyOthello.Properties.Resources.CanputWhite;
            empty = global::MonkeyOthello.Properties.Resources.Empty;
            background = global::MonkeyOthello.Properties.Resources.background;
            BlacktoWhite = new Bitmap[12];
            BlacktoWhite[0] = blackStone;
            BlacktoWhite[1] = global::MonkeyOthello.Properties.Resources.Flip_1;
            BlacktoWhite[2] = global::MonkeyOthello.Properties.Resources.Flip_2;
            BlacktoWhite[3] = global::MonkeyOthello.Properties.Resources.Flip_3;
            BlacktoWhite[4] = global::MonkeyOthello.Properties.Resources.Flip_4;
            BlacktoWhite[5] = global::MonkeyOthello.Properties.Resources.Flip_5;
            BlacktoWhite[6] = global::MonkeyOthello.Properties.Resources.Flip_6;
            BlacktoWhite[7] = global::MonkeyOthello.Properties.Resources.Flip_7;
            BlacktoWhite[8] = global::MonkeyOthello.Properties.Resources.Flip_8;
            BlacktoWhite[9] = global::MonkeyOthello.Properties.Resources.Flip_9;
            BlacktoWhite[10] = global::MonkeyOthello.Properties.Resources.Flip_10;
            BlacktoWhite[11] = whiteStone;              
        }

        public void Paint(Graphics boardGraphics)
        {
            //PaintBuffer();
            boardGraphics.DrawImage(bmpBuffer, 0, 0);
        }

        /// <summary>
        /// ������,����,���µ�λ��,��һ���ĺۼ�
        /// </summary>
        public void PaintBuffer()
        {
            if (board == null) return;
            GraphicsBuffer.DrawImage(background,0,0,400,400);
            //������
            for (int i = 10; i <=80; i++)
            {
                if (board[i] == ChessType.BLACK)
                    GraphicsBuffer.DrawImage(blackStone, getStoneRectangle(i));
                else if (board[i] == ChessType.WHITE)
                    GraphicsBuffer.DrawImage(whiteStone, getStoneRectangle(i));
            }

            int sqnum;
            //�����µ�λ��
            for (int i = 0; i < canputSquareList.Count; i++)
            {
                sqnum = canputSquareList[i];
                if (curColor == ChessType.BLACK)
                    GraphicsBuffer.DrawImage(canputBlack, getStoneRectangle(sqnum));
                else
                    GraphicsBuffer.DrawImage(canputWhite, getStoneRectangle(sqnum));
            }

            //����һ���ĺۼ�
            if (curMove.Square!=0)
            {
                if (curMove.Color == ChessType.BLACK)
                {
                    GraphicsBuffer.DrawImage(blackStone, getStoneRectangle(curMove.Square));
                    GraphicsBuffer.DrawImage(canputBlack, getStoneRectangle(curMove.Square));
                }
                else
                {
                    GraphicsBuffer.DrawImage(whiteStone, getStoneRectangle(curMove.Square));
                    GraphicsBuffer.DrawImage(canputWhite, getStoneRectangle(curMove.Square));
                }
            }
        }


        /// <summary>
        /// ����תʱ�Ķ���
        /// </summary>
        public void PaintFlips(int index)
        {
            if (board == null || flipList==null || flipList.Count==0) return;
            RectangleF rect;      
            for (int i = 0; i < flipList.Count; i++)
            {
                rect=getStoneRectangle(flipList[i]);
                Utils.RefreshBoard(this, new Rectangle((int)rect.X, (int)rect.Y,(int) rect.Width, (int)rect.Height));
                
                GraphicsBuffer.DrawImage(background, rect, rect, GraphicsUnit.Pixel);
                GraphicsBuffer.DrawImage(BlacktoWhite[index], rect);
            }
            //StaticObject.RefreshBoard(this);
        }

        /// <summary>
        /// ��ȡ��ǰ������λ�ú����Ӵ�С
        /// </summary>
        /// <param name="index">λ������</param>
        /// <returns>λ�úʹ�С</returns>
        private RectangleF getStoneRectangle(int index)
        {
            int m, n;
            m = (index - 9) % 9 - 1;
            n = (index - 9) / 9;
            return new RectangleF(m * chessboundSize.Width + boardBound.X, n * chessboundSize.Height + boardBound.Y,
                chessSize.Width, chessSize.Height);
        }

        /// <summary>
        /// ת�����λ��Ϊ����λ��
        /// </summary>
        /// <param name="point">���λ��</param>
        /// <returns>����λ��</returns>
        public int PointToSquare(Point point)
        {
            int m, n;
            m =(int)((point.Y - boardBound.Y) / chessSize.Height);
            n =(int) ((point.X - boardBound.X) / chessSize.Width);
            if (m >= 0 && n >= 0 && m < rowsNum && n < rowsNum)
                return 9 * m + n + 10;
            return -1;
        }

        public bool CanPut(int sqnum)
        {
            return Board.AnyFlips(board, sqnum, curColor, (ChessType)(2 - curColor));
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sqnum"></param>
        /// <returns></returns>
        public bool DownStone(int sqnum)
        {
            int flipped;
            flipList = Board.DrawBoardFlips(board, sqnum, curColor, (ChessType)(2 - curColor));
            flipped = flipList.Count;
            if (flipped > 0)
            {
                curMove.Flips = flipped;
                curMove.Square = sqnum;
                curMove.Color = curColor;
                board[sqnum] = curColor;
                //�岽��ջ
                moveStack.Push(curMove);

                if (curColor == ChessType.BLACK)
                {
                    blackNum += (flipped + 1);
                    whiteNum -= flipped;
                }
                else
                {
                    whiteNum += (flipped + 1);
                    blackNum -= flipped;
                }
                empties--;

                curColor = 2 - curColor;
                SetCanpuSquare();
                PaintBuffer();
                /*
                if (Config.Instance.Animation)
                {
                    if (curColor == ChessType.BLACK)
                        for (int i = 0; i < flipped; i++)
                            GraphicsBuffer.DrawImage(blackStone, getStoneRectangle(flipList[i]));

                    else
                        for (int i = 0; i < flipped; i++)
                            GraphicsBuffer.DrawImage(whiteStone, getStoneRectangle(flipList[i]));
                }*/
                //StaticMethod.RefreshBoard(this);
                return true;
            }
            return false;
        }

        /// <summary>
        /// ��¼�ܹ������λ��
        /// </summary>
        public void SetCanpuSquare()
        {
            canputSquareList.Clear();
            for (int i = 10; i <= 80; i++)
            {
                if (board[i] == ChessType.EMPTY)
                {
                    if (Board.AnyFlips(board, i, curColor, (ChessType)(2 - curColor)))
                        canputSquareList.Add(i);
                }
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        public int Reback()
        {
            int num=0;
            if (moveStack.Count > 0)
            {
                do
                {
                    num++;
                    curMove = moveStack.Pop();
                    Board.UndoFlips(board, curMove.Flips, 2 - curMove.Color);
                    board[curMove.Square] = ChessType.EMPTY;
                    curColor = curMove.Color;

                    if (curColor == ChessType.BLACK)
                    {
                        blackNum -= (curMove.Flips + 1);
                        whiteNum += curMove.Flips;
                    }
                    else
                    {
                        whiteNum -= (curMove.Flips + 1);
                        blackNum += curMove.Flips;
                    }
                    empties++;
                    if (moveStack.Count != 0)
                        curMove = moveStack.Peek();
                    else
                        curMove.Square = 0;//��ʾ��Чλ��

                } while (curMove.Square != 0 && curMove.Color == curColor);
                //��¼�ܹ������λ��
                SetCanpuSquare();
                PaintBuffer();
            }
            return num;
        }

        /// <summary>
        /// ��Ϸ�Ƿ����(������˫��������µ����)
        /// </summary>
        /// <returns></returns>
        public bool IsGameOver()
        {
            if (empties == 0)
                return true;
            return (blackNum == 0 || whiteNum == 0);
        }

        /// <summary>
        /// �ܷ�����
        /// </summary>
        /// <returns></returns>
        public bool CanDown()
        {
            for (int i = 10; i <= 80; i++)
            {
                if (board[i] == ChessType.EMPTY)
                {
                    if (Board.AnyFlips(board, i, curColor, (ChessType)(2 - curColor)))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ������Ϸ�����Ľ��
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public string GameOverResult(GameMode mode)
        {
            int result;
            string msg = "";
            if (mode == GameMode.PvsP || mode== GameMode.CvsC)
            {
                result = blackNum - whiteNum;
                if (result == 0)
                    msg = "ƽ�֣������ѷָ��°���";
                if (result > 0)
                    msg = "����Ӯ!\n";
                else if (result < 0)
                    msg = "����Ӯ!\n";
                msg += ("\n���� " + blackNum + " : " + whiteNum + " ����");
            }
            else
            {
                result = (mode == GameMode.CvsP ? blackNum - whiteNum : whiteNum - blackNum);
                if (result == 0)
                    msg = "ƽ�֣������ѷָ��°�\n";
                if (result < 0)
                    msg = "��Ӯ��! ��ϲ��ϲ!\n";
                else if (result > 0)
                    msg = "����Ӯ! ����Ŭ����!\n";
                msg +=  (mode == GameMode.CvsP ? ("\n\t���� " + blackNum + " : " + whiteNum + " ����"):("\n\t���� " +whiteNum   + " : " + blackNum+ " ����"));
            }
            
            return msg;
        }

    }
}
