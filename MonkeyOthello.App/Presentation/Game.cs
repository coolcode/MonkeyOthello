using MonkeyOthello.Engines;
using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using MonkeyOthello.Engines.X;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

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
        public IEngine BackgroundEngine { get; set; } = new EdaxEngine { Timeout = 15 }; // Pilot;
        public Board Board { get; set; }
        public bool Busy { get; set; }
        public GameMode Mode { get; set; } = GameMode.HumanVsComputer;
        public GameLevel Level { get; set; } = GameLevel.Hard;

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
            if (Mode == GameMode.ComputerVsHuman)
            {
                currentPlayer = PlayerType.Computer;
            }
            else if (Mode == GameMode.ComputerVsComputer)
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

        private CancellationTokenSource backgroundSearchTokenSource = new CancellationTokenSource();
        private Task backgroundSearchTask = null;

        private ConcurrentDictionary<int, MoveMapItem> moveMaps = new ConcurrentDictionary<int, MoveMapItem>();

        class MoveMapItem
        {
            public int Move { get; set; }
            public int Eval { get; set; }

            public override string ToString()
            {
                return $"{Move.ToNotation()},{Eval}";
            }
        }


        public void ComputerPlay()
        {
            if (backgroundSearchTask != null && backgroundSearchTask.Status == TaskStatus.Running)
            {
                UpdateMessage?.Invoke("stop thinking...");
                backgroundSearchTokenSource.Cancel();
            }

            Busy = true;
            UpdateMessage?.Invoke("searching...");

            Engine.UpdateProgress = r => UpdateResult?.Invoke(r);

            var bb = Board.ToBitBoard();
            var depth = GetSearchDepth();
            SearchResult result = null;

            if (Board.LastMove != null)
            {
                if (moveMaps.ContainsKey(Board.LastMove.Value))
                {
                    var item = moveMaps[Board.LastMove.Value];
                    if (Board.ValidMove(item.Move))
                    {
                        result = new SearchResult
                        {
                            Move = item.Move,
                            Score = item.Eval,
                            Process = 1,
                            Message = "BackgroundSearch",
                        };
                    }
                }
            }

            if (result == null)
            {
                result = Engine.Search(bb, depth);

                if (result.IsTimeout)
                {
                    //timeout, random move
                    var moves = Board.FindMoves();
                    result.Move = moves[new Random().Next(0, moves.Length)];
                }
            }

            PlayerPlay(result.Move);
            UpdatePlay?.Invoke(PlayerType.Computer, result.Move);
            UpdateResult?.Invoke(result);

            // if (bb.EmptyPiecesCount() > EdaxEngine.WinLoseDepth)
            if (!Board.CanMove())
            {
                moveMaps.Clear();
            }
            else
            {
                if (backgroundSearchTask != null)
                {
                    var i = 0;
                    while (i++ < 100)
                    {
                        if (backgroundSearchTask.Status != TaskStatus.Running)
                        {
                            break;
                        }

                        UpdateMessage?.Invoke($"wait times: {i++}...");
                        Thread.Sleep(100);
                    }
                    UpdateMessage?.Invoke("thinking was stoped.");
                }

                backgroundSearchTokenSource = new CancellationTokenSource();
                backgroundSearchTask = BackgroundSearch(backgroundSearchTokenSource.Token);
            }

            Busy = false;

        }

        private Task BackgroundSearch(CancellationToken token)
        {
            Action search = () =>
            {
                UpdateMessage?.Invoke("⌛thinking...");

                var currentBoard = Board.ToBitBoard();
                var moves = Rule.FindMoves(currentBoard);
                var bestScore = -Constants.HighestScore - 1;
                var bestMove = -1;
                //var moveEvalMap = new Dictionary<int, int>();
                moveMaps.Clear();
                var i = 0;
                foreach (var move in moves)
                {
                    i++;
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    var oppboard = Rule.MoveSwitch(currentBoard, move);

                    var own = false;
                    if (!Rule.CanMove(oppboard))
                    {
                        oppboard = oppboard.Switch();
                        own = true;
                    }

                    var sr = BackgroundEngine.Search(oppboard, GetSearchDepth());

                    if (sr.IsTimeout)
                    {
                        sr.Move = -1;
                        sr.Score = -Constants.HighestScore;
                    }

                    var eval = own ? sr.Score : -sr.Score;//opp's score
                    if (eval > bestScore)
                    {
                        bestScore = eval;
                        bestMove = move;
                    }
                    moveMaps[move] = new MoveMapItem { Move = sr.Move, Eval = -eval };
                    var nextMovesMessage = ToMessage(moveMaps, bestMove, bestScore);
                    //moveEvalMap[move] = eval;
                    //var moveEvalResult = string.Join(",", moveEvalMap.Select(kv => $"({kv.Key.ToNotation()}:{kv.Value})"));
                    UpdateMessage?.Invoke($"[{i}/{moves.Length}] {nextMovesMessage}");
                    //Console.WriteLine($"move:{move}, score:{eval}");
                }

                if (moveMaps.Count == 0)
                {
                    if (token.IsCancellationRequested)
                    {
                        UpdateMessage?.Invoke($"canceled.");
                    }
                    else
                    {
                        UpdateMessage?.Invoke($"no moves.");
                    }
                }
                else
                {
                    var nextMovesMessage = ToMessage(moveMaps, bestMove, bestScore);
                    UpdateMessage?.Invoke(nextMovesMessage);
                }
            };

            return Task.Run(search, token);
        }

        private string ToMessage(IDictionary<int, MoveMapItem> map, int bestMove, int bestScore)
        {
            var moveEvalResult = string.Join(" | ", map.Select(kv => $"{kv.Key.ToNotation()},{kv.Value}"));

            return $"[{bestMove.ToNotation()},{bestScore}], [{moveEvalResult}]";
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

        private int GetSearchDepth()
        {
            Dictionary<GameLevel, int> gameLevelMap = new Dictionary<GameLevel, int>
            {
                { GameLevel.Easy, 6 },
                { GameLevel.Medium, 12 },
                { GameLevel.Hard, 16 },
                { GameLevel.Expert, 24 }, // VS. WZebra: 0-0 
                { GameLevel.Crazy, 26 },
            };

            return gameLevelMap[Level];
        }

        private void PlayerPlay(int square)
        {
            var flips = Board.MakeMove(square);
        }

        public bool Undo()
        {
            moveMaps.Clear();
            return Board.Reback();
        }

    }
}
