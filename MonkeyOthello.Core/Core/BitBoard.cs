using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonkeyOthello.Core
{
    public class BitBoard
    {
        public ulong PlayerPieces { get; set; }
        public ulong OpponentPieces { get; set; }

        public ulong AllPieces
        {
            get { return PlayerPieces | OpponentPieces; }
        }

        public ulong EmptyPieces
        {
            get { return AllPieces ^ Constants.FullBoard; }
        }

        public bool IsFull
        {
            get { return AllPieces == Constants.FullBoard; }
        }

        public BitBoard(ulong playerPieces, ulong opponentPieces)
        {
            PlayerPieces = playerPieces;
            OpponentPieces = opponentPieces;
        }

        public static BitBoard NewGame()
        {
            return new BitBoard(1UL << 28 | 1UL << 35, 1UL << 27 | 1UL << 36);
        }

        public BitBoard Switch()
        {
            return new BitBoard(OpponentPieces, PlayerPieces);
        }

        public BitBoard Copy()
        {
            return new BitBoard(PlayerPieces, OpponentPieces);
        }

        public int EmptyPiecesCount()
        {
            return EmptyPieces.CountBits();
        }

        public int PlayerPiecesCount()
        {
            return PlayerPieces.CountBits();
        }

        public int OpponentPiecesCount()
        {
            return OpponentPieces.CountBits();
        }

        public int DiffCount()
        {
            return PlayerPiecesCount() - OpponentPiecesCount();
        }

        /// <summary>
        /// winner got all empty pieces when game is over
        /// </summary>
        /// <returns></returns>
        public int EndDiffCount()
        {
            var ownCount = PlayerPiecesCount();
            var oppCount = OpponentPiecesCount();
            var emptyCount = Constants.StonesCount - (ownCount + oppCount);
            var diffCount = ownCount - oppCount;

            if (diffCount > 0)
            {
                return diffCount + emptyCount;
            }
            else if (diffCount < 0)
            {
                return diffCount - emptyCount;
            }

            return 0;
        }

        public override bool Equals(object obj)
        {
            var comparedGameState = (BitBoard)obj;
            return (PlayerPieces == comparedGameState.PlayerPieces) && (OpponentPieces == comparedGameState.OpponentPieces);
        }

        public bool IsGameOver()
        {
            return IsFull || (!Rule.CanMove(this) && !Rule.CanMove(Switch()));
        }

        public override int GetHashCode()
        {
            // return PlayerPieces.GetHashCode() | OpponentPieces.GetHashCode();//same speed
            return PlayerPieces.GetHashCode() ^ (int)(OpponentPieces >> 33);
        }

        public override string ToString()
        {
            return Draw(multiLine: true);
        }

        public string Draw(string color)
        {
            if (color.ToLower() == "black" || color.ToLower() == "b")
            {
                return Draw(ownSymbol: "X", oppSymbol: "O", multiLine: true);
            }
            else
            {
                return Draw(ownSymbol: "O", oppSymbol: "X", multiLine: true);
            }
        }

        public string Draw(string ownSymbol = "w",
            string oppSymbol = "b",
            string emptySymbol = ".",
            bool multiLine = false)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Constants.StonesCount; i++)
            {
                if (i % Constants.Line == 0 && multiLine)
                {
                    sb.AppendLine();
                }

                var pos = 1UL << i;
                if ((PlayerPieces & pos) > 0)
                {
                    sb.Append(ownSymbol);
                }
                else if ((OpponentPieces & pos) > 0)
                {
                    sb.Append(oppSymbol);
                }
                else
                {
                    sb.Append(emptySymbol);
                }
            }

            if (multiLine)
            {
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static BitBoard Parse(string text, bool playerTakeBlack=false)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException("text");
            }

            if (text.Length != 64)
            {
                throw new ArgumentOutOfRangeException("the length of text must be 64");
            }

            var w = 0ul;
            var b = 0ul;
            for (var i = 0; i < 64; i++)
            {
                var p = text[i];
                var x = 1ul << i;
                switch (p)
                {
                    case 'w':
                    case 'W':
                    case 'o':
                    case 'O':
                    case '¡ð':
                        w |= x;
                        break;
                    case 'b':
                    case 'B':
                    case 'x':
                    case 'X':
                    case '¡ñ':
                        b |= x;
                        break;
                    case '.':
                    case '-':
                    case '¡õ':
                    case ' ':
                    default:
                        //do nothing, just to tell you guys which empty symbols are
                        break;
                }
            }

            return playerTakeBlack ? new BitBoard(b, w) : new BitBoard(w, b);
        }
    }
}