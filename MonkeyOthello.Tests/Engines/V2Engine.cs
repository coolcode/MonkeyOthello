using MonkeyOthello.Core;
using MonkeyOthello.Engines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Tests.Engines
{
    public abstract class V2Engine : BaseEngine
    {
        public override SearchResult Search(BitBoard bb, int depth)
        {
            var board = V3BoardToV2(bb);

            var r = Solve(board, 'w');

            return r;
        }

        public static int[] V3BoardToV2(BitBoard bb)
        {
            var board = new int[91];
            for (var j = 0; j < board.Length; j++)
            {
                board[j] = 3;// ChessType.DUMMY;
            }

            int x, y, k, empties = 0;
            for (var j = 0; j < 64; j++)
            {
                x = j & 7;
                y = (j >> 3) & 7;
                k = x + 10 + 9 * y;

                var w = bb.PlayerPieces & (1ul << j);
                var b = bb.OpponentPieces & (1ul << j);

                if (w > 0)
                {
                    board[k] = 0;//white
                }
                else if (b > 0)
                {
                    board[k] = 2;//black
                }
                else
                {
                    board[k] = 1;//empty
                    empties++;
                }
            }

            return board;
        }

        public static int V2SquareToV3(int square)
        {
            var m = (square - 9) % 9 - 1;
            var n = (square - 9) / 9;
             
            return  n * 8 + m;
        }

        protected abstract SearchResult Solve(int[] board, char color);
    }
}
