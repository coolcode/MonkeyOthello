using MonkeyOthello.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;
using System.IO;
using Accord.Neuro.Networks;
using Accord.Neuro;

namespace MonkeyOthello.Learning
{
    public class DeepLearningEngine : MonkeyEngine
    {
        public DeepLearningEngine()
        {
            Evaluation = new DeepLearningEvaluation();
            Window = new PurningWindow(-1, 1);
        }

        public override SearchResult Search(BitBoard board, int depth)
        { 
            var empties = board.EmptyPiecesCount();
            if (empties.InRange(0, 20))
            {
                var engine = new MonkeyEndEngine();
                return engine.Search(board, empties);
            }
            else
            {
                return base.Search(board, depth);
            }
        }
    }

    public class DeepLearningEvaluation : IEvaluation
    {
        private static readonly string networkPath = Path.Combine(Environment.CurrentDirectory, @"Tools\networks\");
        private static string networkFile = "all-deeplearning-19.net";
        private static Network network;

        static DeepLearningEvaluation()
        {
            networkFile = Path.Combine(networkPath, "all-deeplearning-19.net");
            network = DeepBeliefNetwork.Load(networkFile);
        }

        public int Eval(BitBoard board)
        {

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
