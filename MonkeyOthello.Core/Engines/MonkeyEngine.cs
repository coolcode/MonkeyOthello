using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Engines
{
    public class MonkeyEngine : BaseEngine
    {
        private const int highScore = Constants.HighestScore;

        private SearchResult searchResult = new SearchResult();

        public IEvaluation Evaluation { get; set; } = new SimpleEvaluation();

        public IEvaluation EndGameEvaluation { get; set; } = new EndGameEvaluation();

        private static readonly int[] orderedSquares = new int[]
        {
            36,35,28,27,63,56,7,0,61,58,47,40,23,16,5,2,45,42,21,18,60,59,39,32,31,24,4,3,44,43,37,34,29,26,20,19,52,51,38,33,30,25,12,11,53,50,46,41,22,17,13,10,62,57,55,48,15,8,6,1,54,49,14,9
            //E5,D5,E4,D4,H8,A8,H1,A1,F8,C8,H6,A6,H3,A3,F1,C1,F6,C6,F3,C3,E8,D8,H5,A5,H4,A4,E1,D1,E6,D6,F5,C5,F4,C4,E3,D3,E7,D7,G5,B5,G4,B4,E2,D2,F7,C7,G6,B6,G3,B3,F2,C2,G8,B8,H7,A7,H2,A2,G1,B1,G7,B7,G2,B2
        };

        private Queue<int> squareQueue = new Queue<int>();

        private Dictionary<int, int> squareDict = new Dictionary<int, int>(64);

        private void PrepareSearch(BitBoard board)
        {
            squareQueue.Clear();
            var squares = board.EmptyPieces.Indices().ToList();
            squareDict = orderedSquares.Select((c, i) => new { K = c, V = i }).ToDictionary(kv => kv.K, kv => kv.V);

            squares = squares.OrderBy(i => squareDict[i]).ToList();

            squares.ForEach(squareQueue.Enqueue);
        }

        public override SearchResult Search(BitBoard board, int depth)
        {
            PrepareSearch(board);

            searchResult = new SearchResult();

            if (depth > 14)
            {
                searchResult.Message = "too depth...";
                return searchResult;
            }

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
            var foundPv = false;

            //moves = moves.OrderBy(i => squareDict[i]).ToArray();
            //for (var i = 0; i < moves.Length; i++)
            //{
            //    var oppBoard = Rule.MoveSwitch(board, moves[i]);
            //    var diffmob = Rule.DiffMobility(oppBoard);

            //}
            var orderedMoves = OrderMoves(moves, board);

            foreach (var pos in orderedMoves)
            {
                //move  
                var oppBoard = Rule.MoveSwitch(board, pos);

                //searchResult.Nodes++;

                var eval = 0;
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

                //moves = moves.OrderBy(i => squareDict[i]).ToArray();

                var orderedMoves = OrderMoves(moves, board);

                foreach (var pos in orderedMoves)
                {
                    var eval = 0; 

                    var oppBoard = Rule.MoveSwitch(board, pos);

                    //searchResult.Nodes++;
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


        private IEnumerable<int> OrderMoves(IEnumerable<int> moves, BitBoard board)
        {
            //moves = moves.OrderBy(i => squareDict[i]).ToArray();
            return moves.OrderBy(m => Rule.DiffMobility(Rule.MoveSwitch(board, m)));
        }
    }
}
