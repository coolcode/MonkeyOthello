using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Core
{
    public class Constants
    {
        public const int Line = 8;
        public const int StonesCount = Line * Line;
        public const int MaxEmptyNum = StonesCount - 4;
        public const int HighestScore = 64;
        public static readonly int MaxEndGameDepth = Math.Min((StonesCount - 4), 22);
        public const ulong FullBoard = ulong.MaxValue;

        public const int ParityDepth = 8;
        public const int NoParityDepth = 5;

        public const int MaxOpeningDepth = 10;
    }
}
