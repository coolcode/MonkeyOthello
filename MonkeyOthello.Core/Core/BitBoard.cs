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

        /*public void Switch()
        {
            var t = PlayerPieces;
            PlayerPieces = OpponentPieces;
            OpponentPieces = t;
        }*/

        public BitBoard Switch()
        {
            return new BitBoard(OpponentPieces, PlayerPieces);
        }

        public BitBoard Copy()
        {
            return new BitBoard(PlayerPieces, OpponentPieces);
        }

        public int Diff()
        {
            return PlayerPieces.CountBits() - OpponentPieces.CountBits();
        }

        public override string ToString()
        {
            return Draw(multiLine: true);
        }

        public string Draw(bool multiLine = false)
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
                    sb.Append("X");
                }
                else if ((OpponentPieces & pos) > 0)
                {
                    sb.Append("O");
                }
                else
                {
                    sb.Append("_");
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