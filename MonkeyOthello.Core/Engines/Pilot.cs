using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;

namespace MonkeyOthello.Engines
{
    public class Pilot : BaseEngine
    {
        public override string Name
        {
            get
            {
                return "MonkeyOthelloV3";
            }
        }

        private static readonly IEngine openingBookEngine = new OpeningBookEngine();
        //private static readonly IEngine deepLearningEngine = new OpeningBookEngine();

        public override SearchResult Search(BitBoard board, int depth)
        {
            IEngine engine;
            var empties = board.EmptyPiecesCount();
            if (empties.InRange(55, 60))
            {
                engine = openingBookEngine;
                depth = 8;
            }
            else if (empties.InRange(40, 54))
            {
                engine = new MonkeyOpeningEngine();
                depth = 8;
            }
            else if (empties.InRange(19, 39))
            {
                engine = new MonkeyOpeningEngine();
                depth = 8;
            }
            else if (empties.InRange(0, 18))
            {
                engine = new MonkeyEndEngine();
                depth = empties;
            }
            else
            {
                throw new Exception($"invalid empties:{empties}");
            }

            engine.UpdateProgress = UpdateProgress;
            return engine.Search(board, depth);
        }


    }
}
