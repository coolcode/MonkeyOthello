using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using MonkeyOthello.Core;

namespace MonkeyOthello.Presentation
{
    public class BoardPainter
    {
        private BufferBoard bufferBoard;
        private Image blackStone, blackHint;
        private Image whiteStone, whiteHint;
        private Image empty;
        private Image background;

        private PointF boardBound = new PointF(24.4f, 24);
        private SizeF chessboundSize = new SizeF(44.5f, 44.5f);
        private Size chessSize = new Size(44, 44);
        private const int rowsNum = Constants.Line;
        private const int count = Constants.StonesCount;
        private const int flipsNum = 12;
        private Board board = new Board();
        private Graphics painter;
        private List<int> hintMoves = new List<int>();
        private List<int> flips = new List<int>();

        public StoneType Color { get { return board.Color; } }

        public int BlackPieces { get; private set; }
        public int WhitePieces { get; private set; }
        public int Empties { get; private set; }

        public Bitmap Buffer
        {
            get; private set;
        } = new Bitmap(400, 400);

        public BoardPainter(Board board, BufferBoard bufferBoard)
        {
            InitialResources();
            InitBoard(board);
            this.bufferBoard = bufferBoard;
            bufferBoard.Paint += BufferBoard_Paint;
            painter = Graphics.FromImage(Buffer);
        }

        private void BufferBoard_Paint(object sender, PaintEventArgs e)
        {
           // Paint();
        }

        private void InitBoard(Board b)
        {
            board = b;
            BlackPieces = 0;
            WhitePieces = 0;
            Empties = 0;
            foreach (var s in board)
            {
                switch (s.Type)
                {
                    case StoneType.Black:
                        BlackPieces++;
                        break;
                    case StoneType.White:
                        WhitePieces++;
                        break;
                    case StoneType.Empty:
                        Empties++;
                        break;
                    default:
                        break;
                }
            }
        }

        private void InitialResources()
        {
            blackStone = global::MonkeyOthello.Properties.Resources.BlackStone;
            whiteStone = global::MonkeyOthello.Properties.Resources.WhiteStone;
            blackHint = global::MonkeyOthello.Properties.Resources.CanputBlack;
            whiteHint = global::MonkeyOthello.Properties.Resources.CanputWhite;
            empty = global::MonkeyOthello.Properties.Resources.Empty;
            background = global::MonkeyOthello.Properties.Resources.background;
            //BlacktoWhite = new Bitmap[12];
            //BlacktoWhite[0] = blackStone;
            //BlacktoWhite[1] = global::MonkeyOthello.Properties.Resources.Flip_1;
            //BlacktoWhite[2] = global::MonkeyOthello.Properties.Resources.Flip_2;
            //BlacktoWhite[3] = global::MonkeyOthello.Properties.Resources.Flip_3;
            //BlacktoWhite[4] = global::MonkeyOthello.Properties.Resources.Flip_4;
            //BlacktoWhite[5] = global::MonkeyOthello.Properties.Resources.Flip_5;
            //BlacktoWhite[6] = global::MonkeyOthello.Properties.Resources.Flip_6;
            //BlacktoWhite[7] = global::MonkeyOthello.Properties.Resources.Flip_7;
            //BlacktoWhite[8] = global::MonkeyOthello.Properties.Resources.Flip_8;
            //BlacktoWhite[9] = global::MonkeyOthello.Properties.Resources.Flip_9;
            //BlacktoWhite[10] = global::MonkeyOthello.Properties.Resources.Flip_10;
            //BlacktoWhite[11] = whiteStone;
        }

        public void Paint(Graphics boardGraphics)
        {
            boardGraphics.DrawImage(Buffer, 0, 0);
        }

        public void Paint()
        {
            painter.DrawImage(background, 0, 0, 400, 400);
            //draw chess
            foreach (var item in board)
            {
                if (item.Type == StoneType.Black)
                    painter.DrawImage(blackStone, SquareToRectangle(item.Index));
                else if (item.Type == StoneType.White)
                    painter.DrawImage(whiteStone, SquareToRectangle(item.Index));
            }

            //draw hints
            var hintMoves = board.FindMoves();
            for (int i = 0; i < hintMoves.Length; i++)
            {
                var sqnum = hintMoves[i];
                if (Color == StoneType.Black)
                    painter.DrawImage(blackHint, SquareToRectangle(sqnum));
                else
                    painter.DrawImage(whiteHint, SquareToRectangle(sqnum));
            }

            //draw current move 
            if (board.LastMove!=null)
            {
                var lastMove = board.LastMove.Value;
                if (board.LastColor == StoneType.Black)
                {
                    painter.DrawImage(blackStone, SquareToRectangle(lastMove));
                    painter.DrawImage(blackHint, SquareToRectangle(lastMove));
                }
                else if (board.LastColor == StoneType.White)
                {
                    painter.DrawImage(whiteStone, SquareToRectangle(lastMove));
                    painter.DrawImage(whiteHint, SquareToRectangle(lastMove));
                }
            }
            bufferBoard.BackgroundImage = Buffer;
        }

        private RectangleF SquareToRectangle(int index)
        {
            var m = index % 8;
            var n = index / 8;

            return new RectangleF(
                m * chessboundSize.Width + boardBound.X,
                n * chessboundSize.Height + boardBound.Y,
                chessSize.Width,
                chessSize.Height);
        }

        public int? PointToSquare(Point point)
        {
            var m = (int)((point.Y - boardBound.Y) / chessSize.Height);
            var n = (int)((point.X - boardBound.X) / chessSize.Width);
            if (m >= 0 && n >= 0 && m < rowsNum && n < rowsNum)
            {
                return 8 * m + n;
            }

            return null;
        }

    }
}
