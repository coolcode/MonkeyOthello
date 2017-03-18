using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Presentation
{
    public enum StoneType
    {
        Empty,
        Black,
        White, 
    }

    static class StoneTypeExtensions
    {
        public static StoneType Opp(this StoneType s)
        {
            return s == StoneType.Black ? StoneType.White : StoneType.Black;
        }
    }
}
