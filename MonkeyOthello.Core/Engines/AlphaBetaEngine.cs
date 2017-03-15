using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Engines
{
    public class AlphaBetaEngine : BaseEngine
    {
        private const int highScore = Constants.HighestScore;

        private SearchResult searchResult = new SearchResult();

        public IEvaluation Evaluation = new SimpleEvaluation();

        public IEvaluation EndGameEvaluation = new EndGameEvaluation();

        public override SearchResult Search(BitBoard board, int depth)
        {
            searchResult = new SearchResult();

            var clock = new Clock();
            clock.Start();

            var moves = Rule.FindMoves(board);

            if (moves.Length == 0)
            {
                moves = Rule.FindMoves(board);

                if (moves.Length == 0)
                {
                    //END
                    var endScore = EndGameEvaluation.Eval(board);

                    return new SearchResult() { Move = -1, Score = endScore };
                }
                else
                {
                    var result = Search(board.Switch(), depth);
                    result.Score = -result.Score;

                    return result;
                }
            }

            var alpha = -highScore - 1;
            var beta = highScore + 1;
            var score = -highScore;
            var eval = 0;
            var foundPv = false;

            for (int i = 0; i < moves.Length; i++)
            {
                var pos = moves[i];
                //move  
                var oppBoard = Rule.MoveSwitch(board, pos);

                searchResult.Nodes++;
                //check
                if (foundPv)
                {
                    //zero window
                    eval = -FastestFirstSolve(oppBoard, -alpha - 1, -alpha, depth - 1);
                    if ((eval > alpha) && (eval < beta))
                    {
                        eval = -FastestFirstSolve(oppBoard, -beta, -eval, depth - 1);
                    }
                }
                else
                {
                    eval = -FastestFirstSolve(oppBoard, -beta, -alpha, depth - 1);
                }

                //reback? 

                searchResult.EvalList.Add(new EvalItem { Move = pos, Score = eval });

                searchResult.Message += string.Format("({0}:{1})", pos, eval);
                if (eval > score)
                {
                    score = eval;
                    //update move
                    searchResult.Move = pos;
                    searchResult.Score = score;

                    if (eval > alpha)
                    {
                        if (eval >= beta)
                        {
                            //cut branch
                            break;
                        }
                        alpha = eval;
                        foundPv = true;
                    }
                }
            }

            clock.Stop();

            searchResult.TimeSpan = clock.Elapsed;

            return searchResult;
        }

        private int FastestFirstSolve(BitBoard board, int alpha, int beta, int depth, bool prevmove = true)
        {
            lock (this)
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
                        return -FastestFirstSolve(board.Switch(), -beta, -alpha, depth, false);
                    }
                }

                var score = -highScore; 
                var foundPv = false;
                for (int i = 0; i < moves.Length; i++)
                {
                    var eval = 0;
                    var pos = moves[i];

                    var oppBoard = Rule.MoveSwitch(board, pos);

                    searchResult.Nodes++;
                    //check
                    if (foundPv)
                    {
                        //zero window
                        eval = -FastestFirstSolve(oppBoard, -alpha - 1, -alpha, depth - 1);
                        if ((eval > alpha) && (eval < beta))
                        {
                            eval = -FastestFirstSolve(oppBoard, -beta, -eval, depth - 1);
                        }
                    }
                    else
                    {
                        eval = -FastestFirstSolve(oppBoard, -beta, -alpha, depth - 1);
                    }

                    //reback?

                    if (eval > score)
                    {
                        score = eval;

                        if (eval > alpha)
                        {
                            if (eval >= beta)
                            {
                                //cut branch
                                return score;
                            }

                            alpha = eval;
                            foundPv = true;
                        }
                    }
                }

                return score;
            }
        }

    }
}
