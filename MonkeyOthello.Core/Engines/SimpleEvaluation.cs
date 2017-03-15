using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;

namespace MonkeyOthello.Engines
{
    public class SimpleEvaluation : IEvaluation
    {
        public int Eval(BitBoard board)
        {
            return board.Diff();
        }
    }
}
