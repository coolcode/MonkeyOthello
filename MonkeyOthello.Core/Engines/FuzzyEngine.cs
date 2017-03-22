using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;

namespace MonkeyOthello.Engines
{
    /// <summary>
    /// mix reliable engine and adventure engine
    /// </summary>
    public class FuzzyEngine : BaseEngine
    {
        private static readonly Random rand = new Random();

        public IEngine Reliable { get; set; } = new Pilot();

        public IEngine Adventure { get; set; } = new RandomEngine();

        public override UpdateProgress UpdateProgress
        {
            get
            {
                return base.UpdateProgress;
            }

            set
            {
                base.UpdateProgress = value;
                Reliable.UpdateProgress = value;
                Adventure.UpdateProgress = value;
            }
        }

        public FuzzyEngine(IEngine reliable, IEngine adventure)
        {
            Reliable = reliable;
            Adventure = adventure;
        }

        /// <summary>
        /// 0.0-1.0 (reliable-adventure)
        /// </summary>
        public double RevolutionRate { get; set; } = 0.30;  //30% 

        public override SearchResult Search(BitBoard bb, int depth)
        {
            return IsRevolution() ? Adventure.Search(bb, depth) : Reliable.Search(bb, depth);
        }

        protected virtual bool IsRevolution()
        {
            return rand.NextDouble() >= (1 - RevolutionRate);
        }
    }
}
