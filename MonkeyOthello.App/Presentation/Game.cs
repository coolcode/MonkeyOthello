using MonkeyOthello.Engines;
using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace MonkeyOthello.Presentation
{
    public delegate void UpdateResult(SearchResult result);

    public class Game
    {
        public Board Board { get; set; }
        //public BoardPainter Painter { get; set; }
        public bool Busy { get; set; }
        private GameMode Mode { get; set; }
        public UpdateResult UpdateResult;
        private bool turnToComputerPlayer = false;

        public Game()
        {
        }

        public void NewGame()
        {
            Board.NewGame();
            Busy = false;
            if (Mode == GameMode.ComputerVsComputer)
            {
                ComputervsComputer();
            }
        }

        private void ComputervsComputer()
        {
            do
            {
                PlayGame();
            } while (!IsGameOver());

        }

        public bool PlayGame()
        {
            return false;
            //if (turnToComputerPlayer)
            //{
            //    ComputerPlay();
            //    if (turn == TurnTo.YOU)
            //    {
            //        MessageBox.Show("电脑无棋可下,\n该你再下一步！");
            //        Utils.AllRefreshBoard(drawBoard);
            //        return;
            //    }
            //    else if (turn == TurnTo.ME)
            //    {
            //        ComputerDown();
            //    }
            //    //当前棋手下
            //    Utils.UpdateMessage("该你下棋了!");
            //}
            //if (turn == TurnTo.GAMEOVER)
            //{
            //    gameIsOver = true;
            //    drawBoard.Paint();
            //    Utils.RefreshBoard(drawBoard);
            //    Utils.UpdateMessage("游戏结束!");
            //    //StaticMethod.UpdateMessage(
            //    MessageBox.Show("游戏结束!结果是:" + drawBoard.GameOverResult((GameMode)Config.Instance.GameMode), "游戏结束!");
            //}
        }


        public bool IsGameOver()
        {
            return Board.IsGameOver();
        }

        private void ComputerPlay()
        {
            //int square = VaildMove;
            //double speed;
            //Utils.UpdateMessage("think...");
            //Busy = true;
            //var monkey = new MonkeyOpeningEngine();
            //var result = monkey.Search(Board.ToBitBoard(), 6);
            //square = result.Move;
             
            //AddMoveItem(square, monkey.BestScore);
            //speed = (monkey.Time > 0 && monkey.Nodes > 0 ? monkey.Nodes / monkey.Time : Constants.MaxSpeed);
            //Utils.UpdateMessage(square, monkey.BestScore, monkey.Nodes, monkey.Time, speed, drawBoard);

            //playerUpdateBoard(square);
            //Busy = false;
        }

        public bool HumanPlay(Point point)
        {
            if (Busy)
            {
                return false;
            }

            /*
            int sqnum = Board.PointToSquare(point);
            if (sqnum == -1)
            {
                return false;
            }
            else
            {
                if (board[sqnum] == StoneType.EMPTY)
                {
                    if (!drawBoard.ValidMove(sqnum))
                        return false;
                }
                else
                    return false;
            }

            //moveList.Add(sqnum);
            //AddMoveItem(sqnum);

            lock (this)
            {
                var t = new Thread(playerUpdateBoard);
                t.Start(sqnum);
            }*/
            return true;
        }

        protected void OnUpdateResult(SearchResult result)
        {
            UpdateResult?.Invoke(result);
        }
        
        /*
        public bool StepUp()
        {
            int rebackNum = 0;
            if (gameMode != GameMode.PvsP)
            {
                if (moveList.Count >= 2)
                {
                    gameIsOver = false;
                    rebackNum += drawBoard.Reback();
                    rebackNum += drawBoard.Reback();
                    for (int i = 1; i <= rebackNum; i++)
                    {
                        moveList.RemoveAt(moveList.Count - 1);
                        RemoveMoveItem();

                    }
                    drawBoard.Paint();
                    Utils.RefreshBoard(drawBoard);
                }
                if (moveList.Count < 2)
                {
                    return false;
                }
            }
            else
            {
                if (moveList.Count >= 1)
                {
                    gameIsOver = false;
                    rebackNum = drawBoard.Reback();
                    for (int i = 1; i <= rebackNum; i++)
                    {
                        moveList.RemoveAt(moveList.Count - 1);
                        RemoveMoveItem();
                        drawBoard.Paint();
                        Utils.RefreshBoard(drawBoard);
                    }
                }
                if (moveList.Count < 1)
                {
                    return false;
                }
            }
            return true;
        }*/

        /*
        private delegate void ListViewAddEvent(object Item);

        private void ListViewAddItem(object Item)
        {
            movesListView.Items.Add((ListViewItem)Item);
        }

        private void UpdateListView(System.Windows.Forms.ListViewItem Item)
        {
            movesListView.Invoke(new ListViewAddEvent(ListViewAddItem), new object[] { Item });
        }

        private delegate void ListViewRemoveEvent();

        private void ListViewRemoveItem()
        {
            movesListView.Items.RemoveAt(movesListView.Items.Count - 1);
        }

        private void AddMoveItem(int square, double? eval = null)
        {
            var lastColor = (Board.Color == StoneType.White ? "W" : "B");
            var Item = new ListViewItem(new string[] {
                moveList.Count.ToString(),
                lastColor,
                square.ToNotation(),
                eval==null? string.Empty: string.Format("{0}", Math.Round(eval.Value)) });
            UpdateListView(Item);
        }

        private void RemoveMoveItem()
        {
            movesListView.Invoke(new ListViewRemoveEvent(ListViewRemoveItem), null);
        }
        */
    }
}
