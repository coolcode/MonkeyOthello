using MonkeyOthello.Core;
using MonkeyOthello.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonkeyOthello
{
    public partial class MainForm : Form
    {
        private Random rand = new Random();
        private Board board = new Board();
        private BoardPainter boardPainter;
        private Point currentLoaction = new Point();
        private Game game = new Game();
        private GameMode gameMode = GameMode.HumanVsComputer;
        private BufferBoard bufferBoard;

        public MainForm()
        {
            InitializeComponent();

            ResizeRedraw = true;

            bufferBoard = new BufferBoard();
            bufferBoard.Size = new Size(400, 400);
            bufferBoard.Location = new Point(1, 50);
            bufferBoard.MouseDown += new MouseEventHandler(OthelloBoard_MouseDown);
            Controls.Add(bufferBoard);

            boardPainter = new BoardPainter(board, bufferBoard);
            this.Load += MainForm_Load;

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            game.NewGame();
        }

        private void OthelloBoard_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left && gameMode != GameMode.ComputerVsComputer)
                    if (game.HumanPlay(e.Location))
                    {
                        undoUToolStripMenuItem.Enabled = true;

                        var thread = new Thread(new ThreadStart(PlayGame));
                        thread.Priority = ThreadPriority.Normal;
                        thread.Start();
                    }
                    else
                    {
                        ShowMessage("Invalid move!");
                    }
            }
            catch
            {
            }
        }

        private void PlayGame()
        {
            UpdatePiecesCount();
            pnl_OthelloBoard.Cursor = Cursors.WaitCursor;
            BeforeThink();
            game.PlayGame();
            pnl_OthelloBoard.Cursor = Cursors.Hand;
            FinishThink();
        }

        private void BeforeThink()
        {
            ;
            for (int i = 0; i < mns_Main.Items.Count; i++)
                mns_Main.Items[i].Enabled = false;

        }

        private void FinishThink()
        {
            for (int i = 0; i < mns_Main.Items.Count; i++)
                mns_Main.Items[i].Enabled = true;
        }

        private void newNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!game.IsGameOver())
            {
                DialogResult result = MessageBox.Show("Start a new game?", "New Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                    return;
            }
            NewGame();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }


        private void NewGame()
        {
            lsv_Moves.Items.Clear();
            undoUToolStripMenuItem.Enabled = false;
            lbl_Nodes.Text = "0";
            lbl_Eval.Text = "0";
            lbl_Speed.Text = "  ";
            lbl_SpendTime.Text = "  ";
            lbl_Square.Text = "  ";
            ShowMessage("balabala...");
            game.NewGame();
            UpdatePiecesCount();
        }

        private void ShowMessage(string msg)
        {
            lbl_Message.Text = msg;
        }

        private void UpdateMessage(int square, double score, int nodes, double time, double speed)
        {
            SafeUpdateUI(() =>
            {
                lbl_Square.Text = square.ToNotation();
                lbl_Eval.Text = string.Format("{0:F2}", score);
                lbl_Nodes.Text = string.Format("{0}", (nodes > 1000 ? Math.Round(nodes / 1000.0) + " K" : nodes.ToString()));
                lbl_SpendTime.Text = string.Format("{0:F2}秒", time);
                lbl_Speed.Text = string.Format("{0}", (speed < 10000000 ? ((int)(speed / 1000) + " kn/s") : "+∞"));
            });

            UpdatePiecesCount();
        }

        private void UpdatePiecesCount()
        {
            SafeUpdateUI(() =>
            {
                lbl_blackNum.Text = board.Count(StoneType.Black).ToString();
                lbl_whiteNum.Text = board.Count(StoneType.White).ToString();
                lbl_Empties.Text = board.Count(StoneType.Empty).ToString();
                lbl_blackNum.Refresh();
                lbl_whiteNum.Refresh();
            });
        }

        private void SafeUpdateUI(MethodInvoker updateUI)
        {
            if (InvokeRequired)
            {
                this.Invoke(updateUI);
            }
            else
            {
                updateUI();
            }
        }
    }
}
