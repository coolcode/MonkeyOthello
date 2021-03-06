﻿using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonkeyOthello.Presentation
{
    public class Board : IEnumerable<Stone>, ICloneable
    {
        class BoardHistItem
        {
            public BitBoard BitBoard { get; set; }
            public StoneType Color { get; set; }
            public int Pos { get; set; }
        }

        private Stone[] stones = new Stone[Constants.StonesCount];
        private Stack<BoardHistItem> movesHistory = new Stack<BoardHistItem>();

        public StoneType Color { get; set; } = StoneType.Black;
        public int Empties { get { return stones.Count(c => c.Type == StoneType.Empty); } }
        public StoneType? LastColor { get; set; }
        public int? LastMove { get; set; }
        public int Steps { get { return movesHistory.Count; } }

        public Board()
        {
            NewGame();
        }

        public Board(string boardText)
            : this(boardText.Select(c => (c == '●' || c == '1' ? StoneType.Black : (c == '○' || c == '2' ? StoneType.White : StoneType.Empty))))
        {
        }

        public Board(IEnumerable<StoneType> board)
            : this()
        {
            stones = board.Select((c, i) => new Stone(i, c)).ToArray();
        }


        public Board(BitBoard bitboard, StoneType color)
            : this()
        {
            Color = color;
            FillStones(bitboard, color);
        }

        public Stone this[int index]
        {
            get
            {
                return stones[index];
            }
            set
            {
                stones[index] = value;
            }
        }

        public Stone this[int row, int col]
        {
            get
            {
                return stones[row * Constants.Line + col];
            }
            set
            {
                stones[row * Constants.Line + col] = value;
            }
        }

        public void NewGame()
        {
            Color = StoneType.Black;
            LastColor = null;
            LastMove = null;
            movesHistory.Clear();
            for (int i = 0; i < Constants.StonesCount; i++)
            {
                stones[i] = new Stone(i, StoneType.Empty);
            }

            var flipsStartIndex = (Constants.Line / 2 - 1) * (Constants.Line + 1);

            stones[flipsStartIndex].Type = StoneType.White;
            stones[flipsStartIndex + 1].Type = StoneType.Black;
            stones[flipsStartIndex + Constants.Line].Type = StoneType.Black;
            stones[flipsStartIndex + Constants.Line + 1].Type = StoneType.White;
        }

        #region IEnumerable<Stone> Members

        public IEnumerator<Stone> GetEnumerator()
        {
            return this.stones.ToList().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return Copy();
        }

        public Board Copy()
        {
            return new Board(this.Select(c => c.Type));
        }

        #endregion

        public int Count(StoneType color)
        {
            return this.Where(c => c.Type == color).Count();
        }

        public override string ToString()
        {
            var conv = new Dictionary<StoneType, string>();
            conv.Add(StoneType.Empty, "□");
            conv.Add(StoneType.Black, "●");
            conv.Add(StoneType.White, "○");
            var sb = new StringBuilder();
            this.ToList().ForEach(c =>
            {
                sb.Append(conv[c.Type]);
                if (c.X == Constants.Line - 1)
                {
                    sb.Append(Environment.NewLine);
                }
            });

            return sb.ToString();
        }

        public string ToFlatString()
        {
            return ToString().Replace(Environment.NewLine, "");
        }

        public int[] MakeMove(int pos)
        {
            LastColor = Color;
            LastMove = pos;

            var bb = ToBitBoard();
            movesHistory.Push(new BoardHistItem { BitBoard = bb, Color = Color, Pos = pos });

            int[] flips;
            var oppBoard = Rule.MoveSwitch(bb, pos, out flips);

            SwitchPlayer();
            FillStones(oppBoard, Color);

            return flips;
        }

        public int[] FindMoves()
        {
            var bb = ToBitBoard();
            return Rule.FindMoves(bb);
        }

        public bool Reback()
        {
            if (movesHistory.Count == 0)
            {
                return false;
            }

            var item = movesHistory.Pop();
            Color = item.Color;
            FillStones(item.BitBoard, Color);
            if (movesHistory.Count > 0)
            {
                item = movesHistory.Peek();
                LastMove = item.Pos;
                LastColor = item.Color;
            }
            else
            {
                LastMove = null;
                LastColor = null;
            }

            return true;
        }

        public BitBoard ToBitBoard()
        {
            var black = 0UL;
            var white = 0UL;

            for (var i = 0; i < Constants.StonesCount; i++)
            {
                var stone = stones[i];
                if (stone.Type == StoneType.Black)
                {
                    black |= 1UL << i;
                }
                else if (stone.Type == StoneType.White)
                {
                    white |= 1UL << i;
                }
            }

            return Color == StoneType.Black ? new BitBoard(black, white) : new BitBoard(white, black);
        }

        public void FillStones(BitBoard board, StoneType color)
        {
            for (var i = 0; i < Constants.StonesCount; i++)
            {
                stones[i].Type = StoneType.Empty;
            }

            Action<ulong, StoneType> FillStonesByColor = (b, c) =>
            {
                for (var i = 0; i < Constants.StonesCount; i++)
                {
                    if ((b & (1UL << i)) != 0)
                    {
                        stones[i].Type = c;
                    }
                }
            };

            FillStonesByColor(board.PlayerPieces, color);

            FillStonesByColor(board.OpponentPieces, color.Opp());
        }

        public void SwitchPlayer()
        {
            Color = Color.Opp();
        }

        public bool ValidMove(int square)
        {
            return Rule.FindMoves(ToBitBoard()).Contains(square);
        }

        public bool CanMove()
        {
            return Rule.CanMove(ToBitBoard());
        }

        public bool IsGameOver()
        {
            var bb = ToBitBoard();

            var over = (Empties == 0
                || (!Rule.CanMove(bb) && !Rule.CanMove(bb.Switch()))
                );

            return over;
        }
    }
}
