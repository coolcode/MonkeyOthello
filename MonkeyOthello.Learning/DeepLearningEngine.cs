using Accord.Neuro;
using Accord.Neuro.Networks;
using MonkeyOthello.Core;
using MonkeyOthello.Engines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Learning
{
    public class DeepLearningEngine : MonkeyEngine
    {
        public DeepLearningEngine()
        {
            Evaluation = DeepLearningEvaluation.Instance;
            Window = new PurningWindow(-1, 1);
        }

        private static readonly IEngine openingBookEngine = new OpeningBookEngine();

        public IEngine Allied { get; set; }

        public override SearchResult Search(BitBoard board, int depth)
        {
            var empties = board.EmptyPiecesCount();

            if (empties.InRange(55, 60))
            {
                var engine = openingBookEngine;
                return engine.Search(board, 0);
            }
            //else if (empties.InRange(0, 20))
            //{
            //    var engine = new MonkeyEndEngine();
            //    return engine.Search(board, empties);
            //}

            if (empties.InRange(32, 38))
            {
                var emptiesDepthMap = new Dictionary<int, int>
                {
                    {26, 4 },
                    {27, 4 },
                    {28, 4 },
                    {29, 4 },
                    {30, 4 },
                    {31, 4 },
                    {32, 6 },
                    {33, 6 },
                    //{34, 8 },
                    //{35, 8 },
                };

                var newDepth = 0;

                if (!emptiesDepthMap.TryGetValue(empties, out newDepth))
                {
                    newDepth = 8;
                }

                var r1 = base.Search(board, newDepth);
                var r2 = Allied.Search(board, depth);
                r1.Message = $"{r2.Move} {r2.Move.ToNotation()}, {r2.Score}, {r1.Message}";

                return r1;
            }
            else
            {
                var r2 = Allied.Search(board, depth);
                return r2;
            }
            /*
            if (empties.InRange(0, 20))
            {
                var engine = new MonkeyEndEngine();
                return engine.Search(board, empties);
            }
            else
            {
                return base.Search(board, newDepth);
            }*/
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

            //moves = moves.OrderBy(i => squareDict[i]).ToArray();

            var orderedMoves = OrderMovesByMobility(moves, board);
            var breakBeta = false;
            var evlist = new List<int>();
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

                evlist.Add(eval);
                if (eval >= score)
                {
                    score = eval;

                    if (eval >= alpha)
                    {
                        if (eval >= beta)
                        {
                            if (breakBeta)
                            {
                                //pruning
                                return score;
                            }

                            breakBeta = true;
                        }

                        alpha = eval;
                        foundPv = true;
                    }
                }
            }

            if (evlist.Count(x => x > 0) >= 2)
            {
                score = 64;
            }
            else
            {
                score = (int)evlist.Average();
            }

            return score;
        }
    }

    public class DeepLearningEvaluation : IEvaluation
    {
        private static readonly string networkPath = Path.Combine(Environment.CurrentDirectory, @"networks\");

        private readonly Network[] networks = new Network[Constants.MaxEmptiesCount + 1];

        public static readonly IEvaluation Instance = new DeepLearningEvaluation();

        public DeepLearningEvaluation()
        {
            LoadNetworks();
        }

        public int Eval(BitBoard board)
        {
            var empties = board.EmptyPiecesCount();
            var network = FindNetwork(empties);
            if (network == null)
            {
                throw new Exception($"Cannot find network file in empties '{empties}'");
            }

            var inputs = ToInputValues(board);
            var outputValues = network.Compute(inputs);

            if (outputValues[0] >= 0.5)
            {
                return 64;
            }
            else
            {
                return -64;
            }
        }

        private void LoadNetworks()
        {
            for (var i = 0; i <= Constants.MaxEmptiesCount; i++)
            {
                var networkFile = Path.Combine(networkPath, "deeplearning-19.net");
                if (!File.Exists(networkFile))
                {
                    continue;
                }
                networks[i] = DeepBeliefNetwork.Load(networkFile);
            }
        }

        private Network FindNetwork(int empties)
        {
            return networks[empties];
        }

        private static double[] ToInputValues(BitBoard board)
        {
            return ToInputValues(board.PlayerPieces)
                .Concat(ToInputValues(board.OpponentPieces))
                .ToArray();
        }

        private static double[] ToInputValues(ulong bits)
        {
            var values = new double[64];
            for (var i = 0; i < values.Length; i++)
            {
                if ((bits & (1UL << i)) != 0)
                {
                    values[i] = 1.0;
                }
                else
                {
                    values[i] = 0.0;
                }
            }

            return values;
        }
    }
}
