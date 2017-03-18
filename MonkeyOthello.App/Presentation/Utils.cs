using System;
using System.Drawing;

namespace MonkeyOthello.Presentation
{
    static class Utils
    {
        public static Point ToPoint(this int index)
        {
            var x = index % 8;
            var y = index / 8;
            return new Point(x, y);
        }
        
    }

    /*
    static class  Utils
    {
        /// <summary>
        /// ������ʾ����������ı�ǩ
        /// </summary>
        public static System.Windows.Forms.ToolStripStatusLabel lbl_Nodes;

        /// <summary>
        /// ������ʾ��ֵ�ı�ǩ
        /// </summary>
        public static System.Windows.Forms.ToolStripStatusLabel lbl_Eval;

        /// <summary>
        /// ������ʾ��ǰ�Ϻ�λ�õı�ǩ
        /// </summary>
        public static System.Windows.Forms.ToolStripStatusLabel lbl_GoodMove;

        public static System.Windows.Forms.ToolStripStatusLabel lbl_SpendTime;
        public static System.Windows.Forms.ToolStripStatusLabel lbl_Speed;
        public static System.Windows.Forms.ToolStripStatusLabel lbl_Message;
        public static System.Windows.Forms.ToolStripStatusLabel lbl_Empties;
        public static System.Windows.Forms.Label lbl_BlackNum;
        public static System.Windows.Forms.Label lbl_WhiteNum;


        public static System.Windows.Forms.Panel pnl_OthelloBoard;


        public static void UpdateMessage(double score,int nodes,int square)
        {
            Utils.lbl_Eval.Text = string.Format("{0:F2}", score/100);
            Utils.lbl_Nodes.Text = string.Format("{0}", (nodes > 1000 ? nodes / 1000 + " K" : nodes.ToString()));
            Utils.lbl_Message.Text = "˼��λ�ã�" + SquareToString(square) + "...";
            
        }

        public static void UpdateMessage(int square, double score, int nodes, double time, double speed, DrawBoard drawBoard)
        {
            lbl_GoodMove.Text = SquareToString(square);
            lbl_Eval.Text = string.Format("{0:F2}", score);
            lbl_Nodes.Text = string.Format("{0}", (nodes > 1000 ? Math.Round(nodes / 1000.0) + " K" : nodes.ToString()));
            lbl_SpendTime.Text = string.Format("{0:F2}��", time);
            lbl_Speed.Text = string.Format("{0}", (speed < Constants.MaxSpeed ? ((int)(speed / 1000) + " kn/s") : "+��"));
            
            lbl_BlackNum.Text = drawBoard.BlackNum.ToString();
            lbl_WhiteNum.Text = drawBoard.WhiteNum.ToString();
            lbl_Empties.Text = drawBoard.Empties.ToString();
            lbl_BlackNum.Refresh();
            lbl_WhiteNum.Refresh();
        }

        public static void AllRefreshBoard(DrawBoard drawBoard)
        {
            drawBoard.PaintBuffer();
           // RefreshBoard(drawBoard);
            othelloBoard.Refresh();
        }

        public static void RefreshBoard(DrawBoard drawBoard)
        {
            //othelloBoard.BackgroundImage = drawBoard.BmpBuffer;
            othelloBoard.Refresh();
            //pnl_OthelloBoard.BackgroundImage = drawBoard.BmpBuffer;
            //pnl_OthelloBoard.Invalidate(new System.Drawing.Rectangle(22, 22, 360, 360));


            lbl_BlackNum.Text = drawBoard.BlackNum.ToString();
            lbl_WhiteNum.Text = drawBoard.WhiteNum.ToString();
            lbl_Empties.Text = drawBoard.Empties.ToString();
            lbl_BlackNum.Refresh();
            lbl_WhiteNum.Refresh();
        }

        public static void RefreshBoard(DrawBoard drawBoard,System.Drawing.Rectangle rect)
        {
            //pnl_OthelloBoard.BackgroundImage = drawBoard.BmpBuffer;
            //pnl_OthelloBoard.Invalidate(rect);
            //othelloBoard.BackgroundImage = drawBoard.BmpBuffer;
            othelloBoard.Invalidate(rect);
        }

        public static void Flips(DrawBoard drawBoard, int speed)
        {
            int index;
            if (speed == 0)
                for (int i = 1; i < 12; i += 2)
                {
                    index = (drawBoard.CurColor == ChessType.BLACK ? i : 11 - i);
                    drawBoard.PaintFlips(index);
                    System.Threading.Thread.Sleep(100);
                }
            else
                for (int i = 0; i < 12; i++)
                {
                    index = (drawBoard.CurColor == ChessType.BLACK ? i : 11 - i);
                    drawBoard.PaintFlips(index);
                    System.Threading.Thread.Sleep(speed  * 80);
                }
        }

        /// <summary>
        /// ���µ���˼����λ��
        /// </summary>
        /// <param name="square"></param>
        public static void UpdateThinkingMove(int square)
        {
            UpdateMessage("˼��λ�ã�" + SquareToString(square) + "...");
        }

        //private static  delegate void SetTextDelegate(System.Windows.Forms.ToolStripStatusLabel label, string text);
        //private static void SetTexthelp(System.Windows.Forms.ToolStripStatusLabel label, string text)
        //{
        //    label.Tag = text;
        //}


        public static void UpdateMessage(string msg)
        {
            lbl_Message.Text = msg;
            lbl_Message.Invalidate();
        }

        //public static OthelloBoard othelloBoard;
    }
    */
}
