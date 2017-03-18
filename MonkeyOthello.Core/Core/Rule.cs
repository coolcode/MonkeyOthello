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

        public static BitBoard MoveSwitch(BitBoard board, int index, out int[] flips)
        {
            var flipsPieces = FindFlips(board, index);
            flips = flipsPieces.Indices().ToArray();

            return FlipSwitch(board, index, flipsPieces);
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

        public static int Mobility(BitBoard board)
        {
            var bm = ValidPlays(board.PlayerPieces, board.OpponentPieces, board.EmptyPieces);

            return bm.CountBits();
        }

        public static int PotentialMobility(BitBoard board)
        {
            var pm = PotentialMobility(board.PlayerPieces, board.OpponentPieces);

            return pm.CountBits();
        }

        public static bool CanMove(BitBoard board)
        {
            return CanMove(board.PlayerPieces, board.OpponentPieces, board.EmptyPieces);
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

        private static ulong PotentialMobilityOneDirection(Func<ulong, ulong> function, ulong playerPieces, ulong emptySquares)
        {
            var shift = function(playerPieces);
            return shift & emptySquares;
        }


        private readonly static ulong centerMask = (new[] { "d4", "d5", "e4", "e5" }).ToBitBoard();

        private readonly static ulong edgeMask = (new[] { "a1", "h1", "a8", "h8" }).ToBitBoard();

        private readonly static ulong unstableMask = (new[] { "a2", "a7", "h2", "h7",
                                                              "b1", "g1", "b8", "g8"
                                                        }).ToBitBoard();

        private readonly static ulong bit_line_a = (new[] { "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8" }).ToBitBoard();
        private readonly static ulong bit_line_h = (new[] { "h1", "h2", "h3", "h4", "h5", "h6", "h7", "h8" }).ToBitBoard();
        private readonly static ulong bit_line_1 = (new[] { "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1" }).ToBitBoard();
        private readonly static ulong bit_line_8 = (new[] { "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8" }).ToBitBoard();

        // edge 
        private readonly static ulong bit_a1 = "a1".ToBitBoard();
        private readonly static ulong bit_h1 = "h1".ToBitBoard();
        private readonly static ulong bit_a8 = "a8".ToBitBoard();
        private readonly static ulong bit_h8 = "h8".ToBitBoard();

        // unstable
        private readonly static ulong bit_a2 = "a2".ToBitBoard();
        private readonly static ulong bit_a7 = "a7".ToBitBoard();
        private readonly static ulong bit_h2 = "h2".ToBitBoard();
        private readonly static ulong bit_h7 = "h7".ToBitBoard();
        private readonly static ulong bit_b1 = "b1".ToBitBoard();
        private readonly static ulong bit_g1 = "g1".ToBitBoard();
        private readonly static ulong bit_b8 = "b8".ToBitBoard();
        private readonly static ulong bit_g8 = "g8".ToBitBoard();

        public static int Unstable(BitBoard board)
        {
            var allPieces = board.AllPieces;
            var playerPieces = board.PlayerPieces;
            var count = 0;
            if (IsUnstable(playerPieces, allPieces, bit_a2, bit_line_a))
            {
                count++;
            }

            if (IsUnstable(playerPieces, allPieces, bit_a7, bit_line_a))
            {
                count++;
            }

            if (IsUnstable(playerPieces, allPieces, bit_h2, bit_line_h))
            {
                count++;
            }

            if (IsUnstable(playerPieces, allPieces, bit_h7, bit_line_h))
            {
                count++;
            }

            if (IsUnstable(playerPieces, allPieces, bit_b1, bit_line_1))
            {
                count++;
            }

            if (IsUnstable(playerPieces, allPieces, bit_g1, bit_line_1))
            {
                count++;
            }

            if (IsUnstable(playerPieces, allPieces, bit_b8, bit_line_8))
            {
                count++;
            }

            if (IsUnstable(playerPieces, allPieces, bit_g8, bit_line_8))
            {
                count++;
            }

            return count;
        }

        private static bool IsUnstable(ulong playerPieces, ulong allPieces, ulong unstableSquare, ulong lineSquares)
        {
            return ((playerPieces & unstableSquare) != 0) && ((allPieces & lineSquares) != lineSquares);
        }

        public static int Edge(BitBoard board)
        {
            return EdgePieces(board).CountBits();
        }

        public static ulong EdgePieces(BitBoard board)
        {
            return (board.PlayerPieces & edgeMask);
        }

        public static int CenterPiecesCount(BitBoard board)
        {
            return (board.PlayerPieces & centerMask).CountBits();
        }

        /*
       public static ulong StableOneDirection(Func<ulong, ulong> function, ulong playerPieces, ulong opponentPieces, ulong emptySquares)
       {
           var shift = function(playerPieces);
           var allPieces = playerPieces | opponentPieces;
           var potential = shift & allPieces;

           while (potential != 0)
           {
               potential = function(potential);
               if ((potential & emptySquares) != 0)
               {
                   return true;
               }
               potential &= allPieces;
           }
           return false;
       }*/

        /*
    private readonly static ulong stabilityMask = (new List<string> {
                                                 "a1", "a2", "b1",
                                                 "g1", "h1", "h2",
                                                 "a7", "a8", "b8",
                                                 "g8", "h8", "h7" }).ToBitBoard();*/

    }
}
