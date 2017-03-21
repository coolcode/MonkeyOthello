using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Engines
{
    public class PurningWindow
    {
        public int Alpha { get; set; }
        public int Beta { get; set; }

        public PurningWindow(int alpha, int beta)
        {
            Alpha = alpha;
            Beta = beta;
        }
    }

    public class MonkeyEngine : BaseEngine
    {
        //protected const int highScore = 40; Constants.HighestScore;

        protected SearchResult searchResult = new SearchResult();

        public IEvaluation Evaluation { get; set; } = new StateEvaluation();

        public IEvaluation EndGameEvaluation { get; set; } = new EndGameEvaluation();

        public PurningWindow Window { get; set; } = new PurningWindow(-Constants.HighestScore - 1, Constants.HighestScore + 1);

        private static readonly int[] orderedSquares = new int[]
        {
            36,35,28,27,63,56,7,0,61,58,47,40,23,16,5,2,45,42,21,18,60,59,39,32,31,24,4,3,44,43,37,34,29,26,20,19,52,51,38,33,30,25,12,11,53,50,46,41,22,17,13,10,62,57,55,48,15,8,6,1,54,49,14,9
            //E5,D5,E4,D4,H8,A8,H1,A1,F8,C8,H6,A6,H3,A3,F1,C1,F6,C6,F3,C3,E8,D8,H5,A5,H4,A4,E1,D1,E6,D6,F5,C5,F4,C4,E3,D3,E7,D7,G5,B5,G4,B4,E2,D2,F7,C7,G6,B6,G3,B3,F2,C2,G8,B8,H7,A7,H2,A2,G1,B1,G7,B7,G2,B2
        };

        private Dictionary<int, int> squareDict = new Dictionary<int, int>(64);

        private readonly Dictionary<BitBoard, int>[] scoreCaches = new Dictionary<BitBoard, int>[22];

        private int hits = 0;

        private void PrepareSearch(BitBoard board)
        {
            hits = 0;
            squareDict = orderedSquares.Select((c, i) => new { K = c, V = i }).ToDictionary(kv => kv.K, kv => kv.V);

            for (var i = 0; i < scoreCaches.Length; i++)
            {
                scoreCaches[i] = new Dictionary<BitBoard, int>(1 << 19);
            }
        }

        public override SearchResult Search(BitBoard board, int depth)
        {
            PrepareSearch(board);

            searchResult = new SearchResult();

            if (depth > Constants.MaxEndGameDepth)
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

            var alpha = Window.Alpha;
            var beta = Window.Beta;

            if (depth >= Constants.MaxEndGameDepth - 2)
            {//make search window small
                alpha = -1;
                beta = 1;
            }

            var score = minimumScore;
            var foundPv = false;
            var index = 0;

            var orderedMoves = OrderMovesByMobility(moves, board);

            foreach (var pos in orderedMoves)
            {
                //move  
                var oppBoard = Rule.MoveSwitch(board, pos);

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
                            //pruning
                            break;
                        }
                        alpha = eval;
                        foundPv = true;
                    }
                }

                searchResult.TimeSpan = clock.Elapsed;
                searchResult.Process = (++index) / (double)(moves.Length + 1);
                UpdateProgress?.Invoke(searchResult);
            }

            clock.Stop();

            searchResult.TimeSpan = clock.Elapsed;

            var s = from q in scoreCaches.Select((e, i) => new { e.Count, Index = i })
                    where q.Count > 0
                    select new { q.Count, q.Index };

            var cacheInfo = string.Join(",", s.Select(c => $"({c.Index}:{c.Count})"));
            searchResult.Message += $" (hits: {hits}, cache info:{cacheInfo})";
            searchResult.Process = 1;

            return searchResult;
        }

        protected virtual int FastestFirstSolve(BitBoard board, int alpha, int beta, int depth, bool prevmove = true)
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

            //moves = moves.OrderBy(i => squareDict[i]).ToArray();

            var orderedMoves = OrderMovesByMobility(moves, board);

            foreach (var pos in orderedMoves)
            {
                var eval = 0;

                var oppBoard = Rule.MoveSwitch(board, pos);

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

        protected IEnumerable<int> OrderMovesBySquares(IEnumerable<int> moves)
        {
            return moves.OrderBy(i => squareDict[i]);
        }

        protected IEnumerable<int> OrderMovesByMobility(IEnumerable<int> moves, BitBoard board)
        {
            return moves.OrderBy(m => Rule.DiffMobility(Rule.MoveSwitch(board, m)));
        }

        protected int GetCachedScore(BitBoard board, int depth, Func<int> func)
        {
            var cache = scoreCaches[depth];

            int score;
            if (cache.TryGetValue(board, out score))
            {
                hits++;
            }
            else
            {
                score = func();
                cache[board] = score;
            }
            return score;
        }

    }
}
