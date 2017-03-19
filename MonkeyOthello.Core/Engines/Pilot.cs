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

        public override SearchResult Search(BitBoard board, int depth)
        {
            IEngine engine;
            var empties = board.EmptyPieces.CountBits();
            if (empties.InRange(40, 60))
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
                engine = new MonkeyEngine();
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
