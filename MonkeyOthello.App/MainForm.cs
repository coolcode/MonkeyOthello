using MonkeyOthello.Core;
using MonkeyOthello.Presentation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
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
        private UserControl bufferBoard = new DoubleBufferBoard();

        class DoubleBufferBoard : UserControl
        {
            public DoubleBufferBoard()
            {
                SetStyle(ControlStyles.UserPaint, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.DoubleBuffer, true);
            }
        }

        public MainForm()
        {
            InitializeComponent();

            ResizeRedraw = true;

            bufferBoard.Size = new Size(400, 400);
            bufferBoard.Location = new Point(6, 30);
            bufferBoard.MouseDown += bufferBoard_MouseDown;
            bufferBoard.MouseMove += bufferBoard_MouseMove;
            Controls.Add(bufferBoard);

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            picMonkey.MouseEnter += picMonkey_MouseEnter;
            picMonkey.MouseLeave += picMonkey_MouseLeave;
            picMonkey.MouseDown += picMonkey_MouseDown;
            picMonkey.MouseMove += picMonkey_MouseMove;
            menuEasy.Click += MenuLevel_Click;
            menuMedium.Click += MenuLevel_Click;
            menuHard.Click += MenuLevel_Click;
            menuExpert.Click += MenuLevel_Click;
            menuCrazy.Click += MenuLevel_Click;

            this.Load += MainForm_Load;
        }

        private void MenuLevel_Click(object sender, EventArgs e)
        {
            var levelText = ((ToolStripMenuItem)sender).Name.Substring(4);
            GameLevel gameLevel;
            if (Enum.TryParse(levelText, out gameLevel))
            {
                game.Level = gameLevel;
            }

            if (!game.Busy)
            {
            }
        }

        private void AddMoveItem(int move, int? score = null)
        {
            var item = new ListViewItem(new[] {
                board.Steps.ToString(),
                (board.LastColor  ==  StoneType.White ? "W" : "B"),
                move.ToNotation(), score?.ToString()
            });

            if (score != null)
            {
                item.BackColor = Color.LightGray;
            }

            lsvMoves.Items.Add(item);
            lsvMoves.Invalidate();
        }

        private void RemoveMoveItem()
        {
            lsvMoves.Items.RemoveAt(lsvMoves.Items.Count - 1);
            lsvMoves.Invalidate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            boardPainter = new BoardPainter(board, bufferBoard);
            game = new Game(board);
            game.UpdatePlay = (player, square) =>
            {
                Safe(() =>
                {
                    UpdateBoard();
                    if (player == PlayerType.Human)
                    {
                        AddMoveItem(square);
                    }
                });
            };

            game.UpdateResult = r =>
            {
                Safe(() =>
                {
                    var speed = r.Nodes / r.TimeSpan.TotalSeconds;
                    lblSquare.Text = r.Move.ToNotation();
                    lblEval.Text = string.Format("{0}", r.Score);
                    lblNodes.Text = string.Format("{0}", (r.Nodes > 1000 ? Math.Round(r.Nodes / 1000.0) + " K" : r.Nodes.ToString()));
                    lblSpendTime.Text = string.Format("{0:F1}s", r.TimeSpan.TotalSeconds);
                    lblSpeed.Text = string.Format("{0}", (speed < 10000000 ? ((int)(speed / 1000) + " kn/s") : "+∞"));
                    lblMessage.Text = $"{r.Message}"; //$"{r.Process:p0} {string.Join(",", r.EvalList)}";
                    if (lblMessage.Text.Length >= 49)
                    {
                        lblMessage.Text = lblMessage.Text.Substring(0, 46) + "...";
                    }
                    lblMessage.Invalidate();
                    statMain.Invalidate();

                    if (r.Process == 1)
                    {
                        AddMoveItem(r.Move, r.Score);
                    }
                });
            };

            game.UpdateMessage = msg =>
            {
                Safe(() =>
                {
                    lblMessage.Text = $"{msg}";
                    lblMessage.Invalidate();
                });
            };

            game.NewGame();
        }

        private void bufferBoard_MouseDown(object sender, MouseEventArgs e)
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
                        // ShowMessage("Invalid move!");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        private void bufferBoard_MouseMove(object sender, MouseEventArgs e)
        {
            if (game.Busy)
            {
                return;
            }

            var square = boardPainter.PointToSquare(e.Location);
            if (square != null && board.ValidMove(square.Value))
            {
                bufferBoard.Cursor = Cursors.Hand;
            }
            else
            {
                bufferBoard.Cursor = Cursors.Default;
            }
        }

        private void ComputerPlay()
        {
            Safe(UpdatePiecesCount, BeforeThink);
            game.ComputerPlay();
            Safe(UpdateBoard, UpdatePiecesCount, FinishThink);

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
            {
                menuMain.Items[i].Enabled = false;
            }
            bufferBoard.Cursor = Cursors.WaitCursor;
        }

        private void FinishThink()
        {
            for (int i = 0; i < menuMain.Items.Count; i++)
            {
                menuMain.Items[i].Enabled = true;
            }
            bufferBoard.Cursor = Cursors.Default;
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
            try
            {
                var imageSave = new SaveFileDialog();
                imageSave.Title = "Image Name";
                imageSave.Filter = "jpg|*.jpg|bmp|*.bmp|gif|*.gif";
                DialogResult result = imageSave.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string name = imageSave.FileName;
                    switch (imageSave.FilterIndex)
                    {
                        case 2:
                            boardPainter.Save(name, ImageFormat.Bmp);
                            break;
                        case 3:
                            boardPainter.Save(name, ImageFormat.Gif);
                            break;
                        default:
                            boardPainter.Save(name, ImageFormat.Jpeg);
                            break;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Fail while saving image");
            }
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
            UpdateBoard();
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
            if (!game.Undo())
            {
                return;
            }

            Safe(() =>
            {
                RemoveMoveItem();
                UpdatePiecesCount();
                UpdateBoard();
            });
        }

        private void UpdateBoard()
        {
            bufferBoard.Invalidate();
            bufferBoard.Update();
            bufferBoard.Refresh();
        }

        private void exitEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region Monkey Icon Events

        private Point currentPoint;

        private void picMonkey_MouseEnter(object sender, EventArgs e)
        {
            var pic = sender as PictureBox;
            if (pic != null)
            {
                pic.Left -= pic.Width / 10;
                pic.Top -= pic.Height / 10;
                pic.Width = 6 * pic.Width / 5;
                pic.Height = 6 * pic.Height / 5;
            }
        }

        private void picMonkey_MouseLeave(object sender, EventArgs e)
        {
            var pic = sender as PictureBox;
            if (pic != null)
            {
                pic.Width = 5 * pic.Width / 6;
                pic.Height = 5 * pic.Height / 6;
                pic.Left += pic.Width / 10;
                pic.Top += pic.Height / 10;
            }
        }

        private void picMonkey_MouseDown(object sender, MouseEventArgs e)
        {
            currentPoint = e.Location;
        }

        private void picMonkey_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var newPoint = e.Location;
                this.Left += (newPoint.X - currentPoint.X);
                this.Top += (newPoint.Y - currentPoint.Y);
            }
        }


        #endregion
    }
}
