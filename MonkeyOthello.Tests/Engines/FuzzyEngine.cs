using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;
using MonkeyOthello.Engines;

namespace MonkeyOthello.Tests.Engines
{
    /// <summary>
    /// mix v2 engine and random engine
    /// </summary>
    public class FuzzyEngine : MonkeyV2Engine
    {
        private static readonly Random rand = new Random();

        /// <summary>
        /// 0.0-1.0 (V2-Random)
        /// </summary>
        public double RevolutionRate { get; set; } = 0.30;  //30% 

        public override SearchResult Search(BitBoard bb, int depth)
        {
            if (IsRevolution())
            {
                var moves = Rule.FindMoves(bb);
                var fuzzMove = moves[rand.Next(0, moves.Length)];

                return new SearchResult
                {
                    Move = fuzzMove,
                    Message = "fuzzy move",
                };
            }

            return base.Search(bb, depth);
        }

        protected virtual bool IsRevolution()
        {
            return rand.NextDouble() >= (1 - RevolutionRate);
        }
    }
}
