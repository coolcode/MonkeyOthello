using MonkeyOthello.Core;
using MonkeyOthello.Engines;
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
        private Game game = null;
        private Options options = Options.Default;
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

            pvsPToolStripMenuItem.Visible = false;
            cvsCToolStripMenuItem.Visible = false;
            helpHToolStripMenuItem.Visible = false;
            optionOToolStripMenuItem.DropDownItems.Clear();
            optionOToolStripMenuItem.DropDownItems.Add(pvsCToolStripMenuItem);
            optionOToolStripMenuItem.DropDownItems.Add(cvsPToolStripMenuItem);

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
            pvsCToolStripMenuItem.Click += PvsCToolStripMenuItem_Click;
            cvsPToolStripMenuItem.Click += CvsPToolStripMenuItem_Click;
            this.Load += MainForm_Load;
        }

        private void PvsCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pvsCToolStripMenuItem.Checked = true;
            cvsPToolStripMenuItem.Checked = false;
            game.Mode = GameMode.HumanVsComputer;
            options.Mode = GameMode.HumanVsComputer;
            options.Save();
        }

        private void CvsPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pvsCToolStripMenuItem.Checked = false;
            cvsPToolStripMenuItem.Checked = true;
            game.Mode = GameMode.ComputerVsHuman;
            options.Mode = GameMode.ComputerVsHuman;
            options.Save();
        }

        private void MenuLevel_Click(object sender, EventArgs e)
        {
            var currentMenuItem = (ToolStripMenuItem)sender;
            var levelText = currentMenuItem.Name.Substring(4);
            GameLevel gameLevel;
            if (Enum.TryParse(levelText, out gameLevel))
            {
                game.Level = gameLevel;
                options.Level = gameLevel;
                options.Save();
            }

            menuEasy.Checked = false;
            menuMedium.Checked = false;
            menuHard.Checked = false;
            menuExpert.Checked = false;
            menuCrazy.Checked = false;
            currentMenuItem.Checked = true;
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
            //options
            options = Options.Load();
            switch (options.Level)
            {
                case GameLevel.Easy:
                    menuEasy.Checked = true;
                    break;
                case GameLevel.Medium:
                    menuMedium.Checked = true;
                    break;
                case GameLevel.Hard:
                    menuHard.Checked = true;
                    break;
                case GameLevel.Expert:
                    menuExpert.Checked = true;
                    break;
                case GameLevel.Crazy:
                    menuCrazy.Checked = true;
                    break;
                default:
                    break;
            }

            switch (options.Mode)
            {
                case GameMode.HumanVsComputer:
                    pvsCToolStripMenuItem.Checked = true;
                    break;
                case GameMode.ComputerVsHuman:
                    cvsPToolStripMenuItem.Checked = true;
                    break;
                default:
                    break;
            }

            //board
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
                    DisplaySearchResult(r);

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
                        ShowMessage(msg);
                    });
                };

            game.NewGame();
        }

        private void DisplaySearchResult(SearchResult r)
        {
            var speed = r.Nodes / r.TimeSpan.TotalSeconds;
            lblSquare.Text = r.Move.ToNotation();
            lblEval.Text = $"{r.Score}";
            lblEval.ForeColor = (r.Score < 0 ? Color.Red : Color.Blue);
            lblNodes.Text = FormatNumber(r.Nodes);
            lblSpendTime.Text = $"{r.TimeSpan.TotalSeconds:F1}s";
            lblSpeed.Text = (speed > (1ul << 31) | double.IsNaN(speed) ? "+∞" : FormatNumber(speed) + "/s");
            ShowMessage(r.Message);//$"{r.Process:p0} {string.Join(",", r.EvalList)}";

            statMain.Invalidate();
        }

        private string FormatNumber(double number)
        {
            if (number >= 1000000)
            {
                return $"{number / 1000000:F0} M";
            }

            if (number >= 1000)
            {
                return $"{number / 1000:F0} K";
            }

            return $"{number:F0}";
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
                            ShowMessageBox("Game over!");
                            return;
                        }

                        if (game.TurnNext() == PlayerType.Computer)
                        {
                            ComputerStartTask();
                        }
                        else
                        {
                            ShowMessageBox("Pass, your turn.");
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

        private void ComputerStartTask()
        {
            var thread = new Thread(new ThreadStart(ComputerPlay));
            thread.Priority = ThreadPriority.Normal;
            thread.IsBackground = true;
            thread.Start();
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
                ShowMessageBox("Game over!");
                return;
            }

            if (game.TurnNext() == PlayerType.Computer)
            {
                ShowMessageBox("Pass, computer's turn.");
                ComputerStartTask();
            }
        }

        private void ShowMessageBox(string msg, string title = null)
        {
            Safe(() =>
            {
                MessageBox.Show(this, msg, title ?? Text);
            });
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
                var result = MessageBox.Show("Start a new game?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                imageSave.Filter = "png|*.png|jpg|*.jpg";
                DialogResult result = imageSave.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string name = imageSave.FileName;
                    switch (imageSave.FilterIndex)
                    {
                        case 2:
                            boardPainter.Save(name, ImageFormat.Jpeg);
                            break;
                        default:
                            boardPainter.Save(name, ImageFormat.Png);
                            break;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Fail while saving image", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void NewGame()
        {
            game.NewGame();
            UpdateBoard();
            UpdatePiecesCount();
            DisplaySearchResult(new SearchResult());
            ShowMessage("New game");
            lsvMoves.Items.Clear();
            undoUToolStripMenuItem.Enabled = false;

            if (game.Mode == GameMode.ComputerVsHuman)
            {
                ComputerStartTask();
            }
        }

        private void ShowMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }
            lblMessage.ToolTipText = msg;
            if (msg.Length >= 59)
            {
                msg = msg.Substring(0, 56) + "...";
            }
            lblMessage.Text = msg;
            lblMessage.Invalidate();
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
