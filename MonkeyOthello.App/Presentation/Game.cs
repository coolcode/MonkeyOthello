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
    public delegate void UpdatePlay(PlayerType player, int square);
    public delegate void UpdateMessage(string message);

    public enum PlayerType
    {
        Human,
        Computer
    }

    public class Game
    {
        public Board Board { get; set; }
        public bool Busy { get; set; }
        private GameMode Mode { get; set; } = GameMode.HumanVsComputer;
        public UpdateResult UpdateResult;
        public UpdatePlay UpdatePlay;
        public UpdateMessage UpdateMessage;
        private PlayerType currentPlayer = PlayerType.Human;

        public Game(Board board)
        {
            this.Board = board;
        }

        public void NewGame()
        {
            currentPlayer = PlayerType.Human;
            Board.NewGame();
            Busy = false;
            if (Mode == GameMode.ComputerVsComputer)
            {
                ComputervsComputer();
            }
        }

        private void ComputervsComputer()
        {
            //do
            //{
            //   // PlayGame();
            //} while (!IsGameOver());

        }

        public bool IsGameOver()
        {
            return Board.IsGameOver();
        }

        public PlayerType TurnNext()
        {
            if (Board.CanMove())
            {
                currentPlayer = (currentPlayer == PlayerType.Human ? PlayerType.Computer : PlayerType.Human);
            }
            else
            {
                //switch back to last player
                Board.SwitchPlayer();
            }

            return currentPlayer;
        }

        public void ComputerPlay()
        {
            UpdateMessage?.Invoke("think...");
            Busy = true;

            var pilot = new Pilot();
            pilot.UpdateProgress = r => UpdateResult?.Invoke(r);
            var result = pilot.Search(Board.ToBitBoard(), 8);
            PlayerPlay(result.Move);
            UpdatePlay?.Invoke(PlayerType.Computer, result.Move);
            UpdateResult?.Invoke(result);

            Busy = false;
        }

        public bool HumanPlay(int square)
        {
            if (Busy)
            {
                return false;
            }

            if (!Board.ValidMove(square))
            {
                return false;
            }

            PlayerPlay(square);
            UpdatePlay?.Invoke(PlayerType.Human, square);

            return true;
        }

        private void PlayerPlay(int square)
        {
            var flips = Board.MakeMove(square);
        }

        public bool Undo()
        {
            return Board.Reback();
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
