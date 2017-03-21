using MonkeyOthello.Engines;
using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using MonkeyOthello.Engines.X;

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

    public enum GameLevel
    {
        Easy,
        Medium,
        Hard,
        Expert,
        Crazy
    }

    public class Game
    {
        public IEngine Engine { get; set; } = new EdaxEngine(); // Pilot;
        public Board Board { get; set; }
        public bool Busy { get; set; }
        public GameMode Mode { get; set; } = GameMode.HumanVsComputer;
        public GameLevel Level { get; set; } = GameLevel.Expert;

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

            Engine.UpdateProgress = r => UpdateResult?.Invoke(r);

            var gameLevelMap = new Dictionary<GameLevel, int>
            {
                { GameLevel.Easy, 6 },
                { GameLevel.Medium, 8 },
                { GameLevel.Hard, 10 },
                { GameLevel.Expert, 12 },
                { GameLevel.Crazy, 14 },
            };

            var depth = gameLevelMap[Level];
            var result = Engine.Search(Board.ToBitBoard(), 8);
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

    }
}
