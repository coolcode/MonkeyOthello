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
        private Game game;
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

            game = new Game(board);
            boardPainter = new BoardPainter(board, bufferBoard);
            game.UpdatePlay = (x, g) =>
             {
                 Safe(() =>
                 {
                     bufferBoard.Invalidate();
                     bufferBoard.Update();
                     bufferBoard.Refresh();
                 });
             };

            game.UpdateResult = r =>
            {
                Safe(() =>
                {
                    var speed = r.Nodes / r.TimeSpan.TotalSeconds;
                    lblSquare.Text = r.Move.ToNotation();
                    lblEval.Text = string.Format("{0:F2}", r.Score);
                    lblNodes.Text = string.Format("{0}", (r.Nodes > 1000 ? Math.Round(r.Nodes / 1000.0) + " K" : r.Nodes.ToString()));
                    lblSpendTime.Text = string.Format("{0:F1}s", r.TimeSpan.TotalSeconds);
                    lblSpeed.Text = string.Format("{0}", (speed < 10000000 ? ((int)(speed / 1000) + " kn/s") : "+∞"));
                    lblMessage.Text = $"{string.Join(",", r.EvalList)}";
                    statMain.Invalidate();
                });
            };

            game.UpdateMessage = msg =>
            {
                lblMessage.Text = $"{msg}";
                lblMessage.Invalidate();
            };

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
                {
                    var square = boardPainter.PointToSquare(e.Location);
                    if (square != null && game.HumanPlay(square.Value))
                    {
                        undoUToolStripMenuItem.Enabled = true;

                        if (game.IsGameOver())
                        {
                            MessageBox.Show("Game over!");
                            return;
                        }

                        if (game.TurnNext() == PlayerType.Computer)
                        {
                            var thread = new Thread(new ThreadStart(ComputerPlay));
                            thread.Priority = ThreadPriority.Normal;
                            thread.Start();
                        }
                        else
                        {
                            MessageBox.Show("Pass, your turn.");
                        }
                    }
                    else
                    {
                        ShowMessage("Invalid move!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        private void ComputerPlay()
        {
            Safe(UpdatePiecesCount, BeforeThink);
            pnlOthelloBoard.Cursor = Cursors.WaitCursor;
            game.ComputerPlay();
            pnlOthelloBoard.Cursor = Cursors.Hand;
            Safe(pnlOthelloBoard.Refresh, UpdatePiecesCount, FinishThink);

            if (game.IsGameOver())
            {
                MessageBox.Show("Game over!");
                return;
            }

            if (game.TurnNext() == PlayerType.Computer)
            {
                MessageBox.Show("Pass, computer's turn.");
                var thread = new Thread(new ThreadStart(ComputerPlay));
                thread.Priority = ThreadPriority.Normal;
                thread.Start();
            }
        }

        private void BeforeThink()
        {
            for (int i = 0; i < menuMain.Items.Count; i++)
                menuMain.Items[i].Enabled = false;
        }

        private void FinishThink()
        {
            for (int i = 0; i < menuMain.Items.Count; i++)
                menuMain.Items[i].Enabled = true;
        }

        private void newNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!game.IsGameOver())
            {
                var result = MessageBox.Show("Start a new game?", "New Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            lsvMoves.Items.Clear();
            undoUToolStripMenuItem.Enabled = false;
            lblNodes.Text = "0";
            lblEval.Text = "0";
            lblSpeed.Text = "  ";
            lblSpendTime.Text = "  ";
            lblSquare.Text = "  ";
            ShowMessage("New game");
            game.NewGame();
            pnlOthelloBoard.Refresh();
            UpdatePiecesCount();
        }

        private void ShowMessage(string msg)
        {
            lblMessage.Text = msg;
        }

        private void UpdateMessage(int square, double score, int nodes, double time, double speed)
        {
            lblSquare.Text = square.ToNotation();
            lblEval.Text = string.Format("{0:F2}", score);
            lblNodes.Text = string.Format("{0}", (nodes > 1000 ? Math.Round(nodes / 1000.0) + " K" : nodes.ToString()));
            lblSpendTime.Text = string.Format("{0:F2}s", time);
            lblSpeed.Text = string.Format("{0}", (speed < 10000000 ? ((int)(speed / 1000) + " kn/s") : "+∞"));
        }

        private void UpdatePiecesCount()
        {
            lblblackNum.Text = board.Count(StoneType.Black).ToString();
            lblwhiteNum.Text = board.Count(StoneType.White).ToString();
            lblEmpties.Text = board.Count(StoneType.Empty).ToString();
            lblblackNum.Refresh();
            lblwhiteNum.Refresh();
        }

        private void Safe(params MethodInvoker[] updateUIMethods)
        {
            if (InvokeRequired)
            {
                foreach (var updateUI in updateUIMethods)
                {
                    Invoke(updateUI);
                }
            }
            else
            {
                foreach (var updateUI in updateUIMethods)
                {
                    updateUI();
                }
            }
        }

        private void aboutAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/coolcode/monkeyothello");
        }

        private void undoUToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
