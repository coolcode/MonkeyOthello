using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;

namespace MonkeyOthello.Engines
{
    public class MonkeyOpeningEngine : MonkeyEngine
    {
        public MonkeyOpeningEngine()
        {
            Evaluation = new StateEvaluation();
            Window = new PurningWindow(-20, 20);
        }

        protected override int FastestFirstSolve(BitBoard board, int alpha, int beta, int depth, bool prevmove = true)
        {
            searchResult.Nodes++;

            //leaf node
            if (depth == 0)
            {
                return QuiescenceSolve(board, alpha, beta, 2, prevmove);
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

                if (foundPv)
                {
                    //zero window
                    eval = GetCachedScore(oppBoard, depth-1, () => -FastestFirstSolve(oppBoard, -alpha - 1, -alpha, depth - 1));
                    if ((eval > alpha) && (eval < beta))
                    {
                        eval = -FastestFirstSolve(oppBoard, -beta, -eval, depth - 1);
                    }
                }
                else
                {
                    eval = GetCachedScore(oppBoard, depth-1, () => -FastestFirstSolve(oppBoard, -beta, -alpha, depth - 1));
                }

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

        protected virtual int QuiescenceSolve(BitBoard board, int alpha, int beta, int depth, bool prevmove = true)
        {
            searchResult.Nodes++;
            var cacheIndex = depth + Constants.MaxOpeningDepth + 1;

            var currentScore = Evaluation.Eval(board);

            if (depth == 0 || currentScore >= beta)
            {
                return currentScore;
            }

            if (currentScore > alpha)
            {
                alpha = currentScore;
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
                    var childScore = GetCachedScore(oppBoard, cacheIndex, () => -QuiescenceSolve(oppBoard, -beta, -alpha, depth, false));

                    return BalanceScore(currentScore, childScore);
                }
            }

            var nextScore = minimumScore;
            var foundPv = false;

            var orderedMoves = OrderMovesByMobility(moves, board);

            foreach (var pos in orderedMoves)
            {
                var eval = 0;

                var oppBoard = Rule.MoveSwitch(board, pos);

                if (foundPv)
                {
                    //zero window
                    eval = GetCachedScore(oppBoard, cacheIndex - 1, () => -QuiescenceSolve(oppBoard, -alpha - 1, -alpha, depth - 1));
                    if ((eval > alpha) && (eval < beta))
                    {
                        eval = -QuiescenceSolve(oppBoard, -beta, -eval, depth - 1);
                    }
                }
                else
                {
                    eval = GetCachedScore(oppBoard, cacheIndex - 1, () => -QuiescenceSolve(oppBoard, -beta, -alpha, depth - 1));
                }

                if (eval > nextScore)
                {
                    nextScore = eval;

                    if (eval > alpha)
                    {
                        if (eval >= beta)
                        {
                            //pruning
                            return nextScore;
                        }

                        alpha = eval;
                        foundPv = true;
                    }
                }
            }

            return BalanceScore(currentScore, nextScore);
        }

        private static int BalanceScore(int s1, int s2)
        {
            return (s1 + 2 * s2) / 3;
        }
    }
}
