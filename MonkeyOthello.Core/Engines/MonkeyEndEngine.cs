using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;

namespace MonkeyOthello.Engines
{
    public class MonkeyEndEngine : MonkeyEngine
    {
        public MonkeyEndEngine()
        {
            //Evaluation = new StateEvaluation();
        }

        protected override int FastestFirstSolve(BitBoard board, int alpha, int beta, int depth, bool prevmove = true)
        {
            searchResult.Nodes++;

            //game over
            if (board.IsFull)
            {
                return EndGameEvaluation.Eval(board);
            }

            //leaf node
            if (depth == 0)
            {
                return Evaluation.Eval(board);
            }

            var moves = Rule.FindMoves(board);

            if (moves.Length == 0)
            {
                if (!prevmove)
                {
                    //END
                    return EndGameEvaluation.Eval(board);
                }
                else
                {
                    var oppBoard = board.Switch();
                    //TODO: should it be +, not -?
                    return GetCachedScore(oppBoard, depth, () => -FastestFirstSolve(oppBoard, -beta, -alpha, depth, false));
                }
            }

            var score = minimumScore;
            var foundPv = false;
            
            var orderedMoves = OrderMovesByMobility(moves, board);

            foreach (var pos in orderedMoves)
            {
                var eval = 0;

                var oppBoard = Rule.MoveSwitch(board, pos);

                if (depth <= Constants.ParityDepth)
                {
                    //Parity Search
                    if (foundPv)
                    {
                        //zero window
                        eval = GetCachedScore(oppBoard, depth, () => -ParitySearch(oppBoard, -alpha - 1, -alpha, depth - 1));
                        if ((eval > alpha) && (eval < beta))
                        {
                            eval = -ParitySearch(oppBoard, -beta, -eval, depth - 1);
                        }
                    }
                    else
                    {
                        eval = GetCachedScore(oppBoard, depth, () => -ParitySearch(oppBoard, -beta, -alpha, depth - 1));
                    }
                }
                else
                {
                    if (foundPv)
                    {
                        //zero window
                        eval = GetCachedScore(oppBoard, depth, () => -FastestFirstSolve(oppBoard, -alpha - 1, -alpha, depth - 1));
                        if ((eval > alpha) && (eval < beta))
                        {
                            eval = -FastestFirstSolve(oppBoard, -beta, -eval, depth - 1);
                        }
                    }
                    else
                    {
                        eval = GetCachedScore(oppBoard, depth, () => -FastestFirstSolve(oppBoard, -beta, -alpha, depth - 1));
                    }
                }

                //reback?

                if (eval > score)
                {
                    score = eval;

                    if (eval > alpha)
                    {
                        if (eval >= beta)
                        {
                            //pruning
                            return score;
                        }

                        alpha = eval;
                        foundPv = true;
                    }
                }
            }

            return score;
        }


        private int ParitySearch(BitBoard board, int alpha, int beta, int depth, bool prevmove = true)
        {
            searchResult.Nodes++;

            //game over
            if (board.IsFull)
            {
                return EndGameEvaluation.Eval(board);
            }

            //leaf node
            if (depth == 0)
            {
                return Evaluation.Eval(board);
            }

            var moves = Rule.FindMoves(board);

            if (moves.Length == 0)
            {
                if (!prevmove)
                {
                    //END
                    return EndGameEvaluation.Eval(board);
                }
                else
                {
                    return -ParitySearch(board.Switch(), -beta, -alpha, depth, false);
                }
            }

            var score = minimumScore;
            var foundPv = false;

            //moves = moves.OrderBy(i => squareDict[i]).ToArray();

            var orderedMoves = OrderMovesBySquares(moves);

            foreach (var pos in orderedMoves)
            {
                var eval = 0;

                var oppBoard = Rule.MoveSwitch(board, pos);

                if (depth <= Constants.NoParityDepth)
                {
                    //Parity Search
                    if (foundPv)
                    {
                        //zero window
                        eval = -NoParitySearch(oppBoard, -alpha - 1, -alpha, depth - 1);
                        if ((eval > alpha) && (eval < beta))
                        {
                            eval = -NoParitySearch(oppBoard, -beta, -eval, depth - 1);
                        }
                    }
                    else
                    {
                        eval = -NoParitySearch(oppBoard, -beta, -alpha, depth - 1);
                    }
                }
                else
                {
                    if (foundPv)
                    {
                        //zero window
                        eval = -ParitySearch(oppBoard, -alpha - 1, -alpha, depth - 1);
                        if ((eval > alpha) && (eval < beta))
                        {
                            eval = -ParitySearch(oppBoard, -beta, -eval, depth - 1);
                        }
                    }
                    else
                    {
                        eval = -ParitySearch(oppBoard, -beta, -alpha, depth - 1);
                    }
                }

                //reback?

                if (eval > score)
                {
                    score = eval;

                    if (eval > alpha)
                    {
                        if (eval >= beta)
                        {
                            //pruning
                            return score;
                        }

                        alpha = eval;
                        foundPv = true;
                    }
                }
            }

            return score;
        }

        private int NoParitySearch(BitBoard board, int alpha, int beta, int empties, bool prevmove = true)
        {
            searchResult.Nodes++;
            var moves = Rule.FindMoves(board);

            if (moves.Length == 0)
            {
                if (!prevmove)
                {
                    //END
                    return EndGameEvaluation.Eval(board);
                }
                else
                {
                    return -NoParitySearch(board.Switch(), -beta, -alpha, empties, false);
                }
            }

            var score = minimumScore;
            var diffCount = board.DiffCount();

            foreach (var pos in moves)
            {
                var eval = 0;

                var flips = Rule.FindFlips(board, pos);
                var oppBoard = Rule.FlipSwitch(board, pos, flips);

                if (empties == 2)
                {
                    var ownFlipsCount = flips.CountBits();
                    //the last move of opponent player
                    var lastEmptySquare = oppBoard.EmptyPieces.Index();
                    if (lastEmptySquare < 0)
                    {
                        throw new Exception($"invalid square index:{lastEmptySquare}");
                    }

                    var oppFlipsCount = Rule.CountFlips(oppBoard, lastEmptySquare);

                    if (oppFlipsCount > 0)
                    {
                        //both done.
                        eval = diffCount + 2 * (ownFlipsCount - oppFlipsCount);
                    }
                    else
                    {
                        //opp pass
                        var ownLastFlipsCount = Rule.CountFlips(oppBoard, lastEmptySquare);

                        if (ownLastFlipsCount > 0)
                        {
                            eval = diffCount + 2 * (ownFlipsCount + ownLastFlipsCount) + 2;
                        }
                        else
                        {
                            //all pass
                            eval = diffCount + 2 * ownFlipsCount;
                            //TODO: eval==0?
                            if (eval >= 0)
                            {
                                eval += 2;
                            }
                        }
                    }
                }
                else
                {
                    //empties!=2
                    eval = -NoParitySearch(oppBoard, -beta, -alpha, empties - 1);
                }

                if (eval > score)
                {
                    score = eval;
                    if (eval > alpha)
                    {
                        if (eval >= beta)
                        {
                            return score;
                        }
                        alpha = eval;
                    }
                }
            }

            return score;
        }
        
    }
}
