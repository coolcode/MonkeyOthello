/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // 版权所有。 
          // 开发者：Fengart
          // 文件名： 
          // 文件功能描述：
//----------------------------------------------------------------*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MonkeyOthello.Core;

namespace MonkeyOthello.Presentation
{
    /// <summary>
    /// 棋盘画图类
    /// </summary>
    public class DrawBoard
    {
        /// <summary>
        /// 棋步
        /// </summary>
        [Serializable]
        public struct Move
        {
            /// <summary>
            /// 下棋位置
            /// </summary>
            public int Square;

            /// <summary>
            /// 翻转的棋子数目
            /// </summary>
            public int Flips;

            /// <summary>
            /// 当前步的颜色
            /// </summary>
            public ChessType Color;
        }

        /// <summary>
        /// 棋子图片
        /// </summary>
        private Image blackStone, canputBlack;
        private Image whiteStone, canputWhite;
        private Image empty;
        private Image background;

        /// <summary>
        /// 黑棋翻转为白棋的一组图片
        /// </summary>
        private Image[] BlacktoWhite; 

        /// <summary>
        /// 缓冲图片
        /// </summary>
        private Bitmap bmpBuffer;

        public Bitmap BmpBuffer
        {
            get { return bmpBuffer; }
        }
        /// <summary>
        /// 棋盘长度
        /// </summary>
        //private int boardLength;
        /// <summary>
        /// 棋盘边框
        /// </summary>
        private PointF boardBound=new PointF(24.4f,24);
        private SizeF chessboundSize = new SizeF(44.5f, 44.5f);
        /// <summary>
        /// 棋子直径
        /// </summary>
        private Size chessSize=new Size(44,44);
        /// <summary>
        /// 棋盘行数
        /// </summary>
        private const int rowsNum = Constants.RowsNum;
        /// <summary>
        /// 棋盘方格数
        /// </summary>
        private const int count = Constants.RowsNum * Constants.RowsNum;

        private const int flipsNum=12;
        /// <summary>
        /// 上一步的下棋位置
        /// </summary>
        private Move curMove;

        /// <summary>
        /// 当前下棋方的棋子类形
        /// </summary>
        private ChessType curColor;

        /// <summary>
        /// 虚拟棋盘
        /// </summary>
        private ChessType[] board;

        /// <summary>
        /// 棋盘的GDI
        /// </summary>
        private Graphics GraphicsBuffer;

        /// <summary>
        /// 能够下棋的位置
        /// </summary>
        private List<int> canputSquareList;

        /// <summary>
        /// 当前被翻转的棋子
        /// </summary>
        private List<int> flipList;

        /// <summary>
        /// 棋步栈
        /// </summary>
        private Stack<Move> moveStack;
        

        private int blackNum;

        private int whiteNum;

        private int empties;

        /// <summary>
        ///  当前下棋方的棋子类形
        /// </summary>
        public ChessType CurColor
        {
            get { return curColor; }
            set { curColor = value; }
        }

        /// <summary>
        /// 棋步栈
        /// </summary>
        public Stack<Move> MoveStack
        {
            get { return moveStack; }
            set { moveStack = value; }
        }
        /// <summary>
        /// 黑棋数目
        /// </summary>
        public int BlackNum
        {
            get { return blackNum; }
        }

        /// <summary>
        /// 白棋数目 
        /// </summary>
        public int WhiteNum
        {
            get { return whiteNum; }
        }

        /// <summary>
        /// 空位数目
        /// </summary>
        public int Empties
        {
            get { return empties; }
        }

        /// <summary>
        /// 棋盘的构造函数
        /// </summary>
        public DrawBoard()
        {
            initialResources();
            curColor = ChessType.BLACK;
            bmpBuffer = new Bitmap(400, 400);
            GraphicsBuffer = Graphics.FromImage(bmpBuffer);
        }

        /// <summary>
        /// 设置棋盘的棋盘结构
        /// </summary>
        /// <param name="value">棋盘结构</param>
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

        //初始化资源
        private void initialResources()
        {
            //从资源中获取黑子,白子等图片
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
        /// 画棋盘,棋子,能下的位置,上一步的痕迹
        /// </summary>
        public void PaintBuffer()
        {
            if (board == null) return;
            GraphicsBuffer.DrawImage(background,0,0,400,400);
            //画棋子
            for (int i = 10; i <=80; i++)
            {
                if (board[i] == ChessType.BLACK)
                    GraphicsBuffer.DrawImage(blackStone, getStoneRectangle(i));
                else if (board[i] == ChessType.WHITE)
                    GraphicsBuffer.DrawImage(whiteStone, getStoneRectangle(i));
            }

            int sqnum;
            //画能下的位置
            for (int i = 0; i < canputSquareList.Count; i++)
            {
                sqnum = canputSquareList[i];
                if (curColor == ChessType.BLACK)
                    GraphicsBuffer.DrawImage(canputBlack, getStoneRectangle(sqnum));
                else
                    GraphicsBuffer.DrawImage(canputWhite, getStoneRectangle(sqnum));
            }

            //画上一步的痕迹
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
        /// 画翻转时的动画
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
        /// 获取当前索引的位置和棋子大小
        /// </summary>
        /// <param name="index">位置索引</param>
        /// <returns>位置和大小</returns>
        private RectangleF getStoneRectangle(int index)
        {
            int m, n;
            m = (index - 9) % 9 - 1;
            n = (index - 9) / 9;
            return new RectangleF(m * chessboundSize.Width + boardBound.X, n * chessboundSize.Height + boardBound.Y,
                chessSize.Width, chessSize.Height);
        }

        /// <summary>
        /// 转换鼠标位置为棋盘位置
        /// </summary>
        /// <param name="point">鼠标位置</param>
        /// <returns>棋盘位置</returns>
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
        /// 下棋
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
                //棋步进栈
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
        /// 记录能够下棋的位置
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
        /// 悔棋
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
                        curMove.Square = 0;//表示无效位置

                } while (curMove.Square != 0 && curMove.Color == curColor);
                //记录能够下棋的位置
                SetCanpuSquare();
                PaintBuffer();
            }
            return num;
        }

        /// <summary>
        /// 游戏是否结束(不考虑双方无棋可下的情况)
        /// </summary>
        /// <returns></returns>
        public bool IsGameOver()
        {
            if (empties == 0)
                return true;
            return (blackNum == 0 || whiteNum == 0);
        }

        /// <summary>
        /// 能否下棋
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
        /// 返回游戏结束的结果
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
                    msg = "平手！看来难分高下啊！";
                if (result > 0)
                    msg = "黑子赢!\n";
                else if (result < 0)
                    msg = "白子赢!\n";
                msg += ("\n黑子 " + blackNum + " : " + whiteNum + " 白子");
            }
            else
            {
                result = (mode == GameMode.CvsP ? blackNum - whiteNum : whiteNum - blackNum);
                if (result == 0)
                    msg = "平手！看来难分高下啊\n";
                if (result < 0)
                    msg = "你赢了! 恭喜恭喜!\n";
                else if (result > 0)
                    msg = "猴子赢! 继续努力吧!\n";
                msg +=  (mode == GameMode.CvsP ? ("\n\t黑子 " + blackNum + " : " + whiteNum + " 白子"):("\n\t白子 " +whiteNum   + " : " + blackNum+ " 黑子"));
            }
            
            return msg;
        }

    }
}
