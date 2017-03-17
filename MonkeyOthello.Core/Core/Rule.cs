using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonkeyOthello.Core
{
    public static class Rule
    {
        private const ulong LeftMask = 0x7F7F7F7F7F7F7F7F;
        private const ulong RightMask = 0xFEFEFEFEFEFEFEFE;

        private static Func<ulong, ulong> Left = x => (x >> 1) & LeftMask;
        private static Func<ulong, ulong> Right = x => (x << 1) & RightMask;
        private static Func<ulong, ulong> Up = x => (x >> 8);
        private static Func<ulong, ulong> Down = x => (x << 8);
        private static Func<ulong, ulong> UpLeft = x => (x >> 9) & LeftMask;
        private static Func<ulong, ulong> UpRight = x => (x >> 7) & RightMask;
        private static Func<ulong, ulong> DownRight = x => (x << 9) & RightMask;
        private static Func<ulong, ulong> DownLeft = x => (x << 7) & LeftMask;
        /*
        private static readonly Dictionary<BitBoard, ulong> MovesCache = new Dictionary<BitBoard, ulong>(1 << 10);
        private static int hits = 0;

        public static void ClearCache()
        {
            hits = 0;
            MovesCache.Clear();
            GC.Collect();
        }

        public static string CacheInfo()
        {
            return $"hits:{hits}, cache items: {MovesCache.Count}";
        }*/

        public static int[] FindMoves(BitBoard board)
        {
            var bm = ValidPlays(board.PlayerPieces, board.OpponentPieces, board.EmptyPieces);

            var moves = bm.Indices();

            return moves.ToArray();
        }


        /// <summary>
        /// make move and switch board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="index"></param>
        /// <param name="oppBoard">a new board after moving and switching</param>
        /// <returns>valid move or not</returns>
        public static bool TryMoveSwitch(BitBoard board, int index, out BitBoard oppBoard)
        {
            var placement = 1UL << index;

            var flippedPieces = PlacePiece(placement, board.PlayerPieces, board.OpponentPieces);

            if (flippedPieces != 0)
            {
                var playerPieces = board.PlayerPieces | flippedPieces | placement;
                var opponentPieces = board.OpponentPieces ^ flippedPieces;

                //switch
                oppBoard = new BitBoard(opponentPieces, playerPieces);

                return true;
            }
            oppBoard = null;
            return false;
        }

        /// <summary>
        /// make move and switch board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="index"></param>
        /// <returns>a new board after moving and switching</returns>
        public static BitBoard MoveSwitch(BitBoard board, int index)
        {
            var placement = 1UL << index;

            var flippedPieces = PlacePiece(placement, board.PlayerPieces, board.OpponentPieces);

            var playerPieces = board.PlayerPieces | flippedPieces | placement;
            var opponentPieces = board.OpponentPieces ^ flippedPieces;

            //switch
            return new BitBoard(opponentPieces, playerPieces);
        }

        public static int CountFlips(BitBoard board, int index)
        {
            var placement = 1UL << index;

            var flippedPieces = PlacePiece(placement, board.PlayerPieces, board.OpponentPieces);

            var count = (flippedPieces == 0 ? 0 : flippedPieces.CountBits());

            return count;
        }

        public static ulong FindFlips(BitBoard board, int index)
        {
            var placement = 1UL << index;

            var flippedPieces = PlacePiece(placement, board.PlayerPieces, board.OpponentPieces);

            return flippedPieces;
        }

        public static BitBoard FlipSwitch(BitBoard board, int index, ulong flippedPieces)
        {
            var placement = 1UL << index;
            var playerPieces = board.PlayerPieces | flippedPieces | placement;
            var opponentPieces = board.OpponentPieces ^ flippedPieces;

            //switch
            return new BitBoard(opponentPieces, playerPieces);
        }

        public static int DiffMobility(BitBoard board)
        {
            var pm = ValidPlays(board.PlayerPieces, board.OpponentPieces, board.EmptyPieces);

            var om = ValidPlays(board.OpponentPieces, board.PlayerPieces, board.EmptyPieces);

            var moves = pm.CountBits() - om.CountBits();

            return moves;
        }


        public static bool CanMove(ulong playerPieces, ulong opponentPieces, ulong emptySquares)
        {
            return CanMoveOneDirection(Up, playerPieces, opponentPieces, emptySquares)
                   || CanMoveOneDirection(UpRight, playerPieces, opponentPieces, emptySquares)
                   || CanMoveOneDirection(Right, playerPieces, opponentPieces, emptySquares)
                   || CanMoveOneDirection(DownRight, playerPieces, opponentPieces, emptySquares)
                   || CanMoveOneDirection(Down, playerPieces, opponentPieces, emptySquares)
                   || CanMoveOneDirection(DownLeft, playerPieces, opponentPieces, emptySquares)
                   || CanMoveOneDirection(Left, playerPieces, opponentPieces, emptySquares)
                   || CanMoveOneDirection(UpLeft, playerPieces, opponentPieces, emptySquares);
        }

        public static ulong ValidPlays(ulong playerPieces, ulong opponentPieces, ulong emptySquares)
        {
            //var bb = new BitBoard(playerPieces, opponentPieces);
            //ulong v;
            //if (MovesCache.TryGetValue(bb, out v))
            //{
            //    hits++;
            //    return v;
            //}

            var v = ValidateOneDirection(Up, playerPieces, opponentPieces, emptySquares)
                   | ValidateOneDirection(UpRight, playerPieces, opponentPieces, emptySquares)
                   | ValidateOneDirection(Right, playerPieces, opponentPieces, emptySquares)
                   | ValidateOneDirection(DownRight, playerPieces, opponentPieces, emptySquares)
                   | ValidateOneDirection(Down, playerPieces, opponentPieces, emptySquares)
                   | ValidateOneDirection(DownLeft, playerPieces, opponentPieces, emptySquares)
                   | ValidateOneDirection(Left, playerPieces, opponentPieces, emptySquares)
                   | ValidateOneDirection(UpLeft, playerPieces, opponentPieces, emptySquares);

            //MovesCache.Add(bb, v);

            return v;
        }

        /*
        public static ulong ValidateOneDirection(Func<ulong, ulong> function, ulong playerPieces, ulong opponentPieces, ulong emptySquares)
        {
            var shift = function(playerPieces);
            var potential = shift & opponentPieces;
            ulong validPlays = 0;
            
            while (potential > 0)
            {
                potential = function(potential);
                validPlays |= potential & emptySquares;
                potential = potential & opponentPieces;
            }
            return validPlays;
        }*/

        public static ulong ValidateOneDirection(Func<ulong, ulong> function, ulong playerPieces, ulong opponentPieces, ulong emptySquares)
        {
            var shift = function(playerPieces);
            var potential = shift & opponentPieces;
            ulong validPlays = 0;

            if (potential == 0)
            {
                return validPlays;
            }

            potential = function(potential);
            validPlays |= potential & emptySquares;
            potential = potential & opponentPieces;

            if (potential == 0)
            {
                return validPlays;
            }

            potential = function(potential);
            validPlays |= potential & emptySquares;
            potential = potential & opponentPieces;

            if (potential == 0)
            {
                return validPlays;
            }

            potential = function(potential);
            validPlays |= potential & emptySquares;
            potential = potential & opponentPieces;

            if (potential == 0)
            {
                return validPlays;
            }

            potential = function(potential);
            validPlays |= potential & emptySquares;
            potential = potential & opponentPieces;

            if (potential == 0)
            {
                return validPlays;
            }

            potential = function(potential);
            validPlays |= potential & emptySquares;
            potential = potential & opponentPieces;

            if (potential == 0)
            {
                return validPlays;
            }

            potential = function(potential);
            validPlays |= potential & emptySquares;
            potential = potential & opponentPieces;

            if (potential == 0)
            {
                return validPlays;
            }

            potential = function(potential);
            validPlays |= potential & emptySquares;
            potential = potential & opponentPieces;

            if (potential == 0)
            {
                return validPlays;
            }

            potential = function(potential);
            validPlays |= potential & emptySquares;
            potential = potential & opponentPieces;

            return validPlays;
        }


        public static bool CanMoveOneDirection(Func<ulong, ulong> function, ulong playerPieces, ulong opponentPieces, ulong emptySquares)
        {
            var shift = function(playerPieces);
            var potential = shift & opponentPieces;

            while (potential != 0)
            {
                potential = function(potential);
                if ((potential & emptySquares) != 0)
                {
                    return true;
                }
                potential &= opponentPieces;
            }
            return false;
        }


        public static ulong PlacePiece(ulong placement, ulong playerPieces, ulong opponentPieces)
        {
            return PlaceOneDirection(Up, placement, playerPieces, opponentPieces)
                   | PlaceOneDirection(UpRight, placement, playerPieces, opponentPieces)
                   | PlaceOneDirection(Right, placement, playerPieces, opponentPieces)
                   | PlaceOneDirection(DownRight, placement, playerPieces, opponentPieces)
                   | PlaceOneDirection(Down, placement, playerPieces, opponentPieces)
                   | PlaceOneDirection(DownLeft, placement, playerPieces, opponentPieces)
                   | PlaceOneDirection(Left, placement, playerPieces, opponentPieces)
                   | PlaceOneDirection(UpLeft, placement, playerPieces, opponentPieces);
        }

        public static ulong PlaceOneDirection(Func<ulong, ulong> function, ulong placement, ulong playerPieces, ulong opponentPieces)
        {
            var potential = function(placement);
            ulong flippedPieces = 0;

            do
            {
                if ((potential & playerPieces) > 0)
                    return flippedPieces;

                potential &= opponentPieces;
                flippedPieces |= potential;
                potential = function(potential);
            }
            while (potential > 0);

            return 0;
        }

        public static ulong PotentialMobility(ulong playerPieces, ulong emptySquares)
        {
            return PotentialMobilityOneDirection(Up, playerPieces, emptySquares)
                   | PotentialMobilityOneDirection(UpRight, playerPieces, emptySquares)
                   | PotentialMobilityOneDirection(Right, playerPieces, emptySquares)
                   | PotentialMobilityOneDirection(DownRight, playerPieces, emptySquares)
                   | PotentialMobilityOneDirection(Down, playerPieces, emptySquares)
                   | PotentialMobilityOneDirection(DownLeft, playerPieces, emptySquares)
                   | PotentialMobilityOneDirection(Left, playerPieces, emptySquares)
                   | PotentialMobilityOneDirection(UpLeft, playerPieces, emptySquares);
        }

        public static ulong PotentialMobilityOneDirection(Func<ulong, ulong> function, ulong playerPieces, ulong emptySquares)
        {
            var shift = function(playerPieces);
            return shift & emptySquares;
        }


        private static ulong _stabilityRequirement = (new List<string> {
                                                     "a1", "a2", "b1",
                                                     "g1", "h1", "h2",
                                                     "a7", "a8", "b8",
                                                     "g8", "h8", "h7" }).ToBitBoard();

        // I haven't figured out how to do stability as yet.
        public static ulong StablePieces(ulong playerPieces, ulong opponentPieces)
        {
            // If no corners or edges adjacent to corners contain a piece, there can not be any stable pieces on the board
            // See: http://pressibus.org/ataxx/autre/minimax/node3.html
            if ((playerPieces & _stabilityRequirement) == 0UL)
                return 0UL;


            return 0UL;
        }
    }
}
