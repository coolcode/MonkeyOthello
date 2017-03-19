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

        public int PlayerPlayerPiecesCount()
        {
            return PlayerPieces.CountBits();
        }

        public int OpponentPiecesPiecesCount()
        {
            return OpponentPieces.CountBits();
        }

        public int DiffCount()
        {
            return PlayerPlayerPiecesCount() - OpponentPiecesPiecesCount();
        }

        /// <summary>
        /// winner got all empty pieces when game is over
        /// </summary>
        /// <returns></returns>
        public int EndDiffCount()
        {
            var ownCount = PlayerPlayerPiecesCount();
            var oppCount = OpponentPiecesPiecesCount();
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
    }
}