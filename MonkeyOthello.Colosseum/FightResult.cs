using MonkeyOthello.Core;
using MonkeyOthello.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Colosseum
{
    public class FightResult
    {
        public FightResult()
        {

        }

        public FightResult(BitBoard board, IEngine[] engines, int turn)
        {
            var diff = board.PlayerPiecesCount() - board.OpponentPiecesCount();

            var winnerIndex = diff > 0 ? turn : 1 - turn;

            WinnerName = engines[winnerIndex].Name;
            LoserName = engines[1 - winnerIndex].Name;
            WinnerStoneType = winnerIndex == 0 ? "Black" : "White";
            Score = Math.Abs(diff);
        }

        public string WinnerName { get; set; }
        public string LoserName { get; set; }
        public string WinnerStoneType { get; set; }
        public int Score { get; set; }
        public TimeSpan TimeSpan { get; set; }

        public override string ToString()
        {
            return string.Format("Winner:{0},{1} Loser:{2}, Score:{3}, TimeSpan:{4}",
                                 WinnerName,
                                 WinnerStoneType,
                                 LoserName,
                                 Score,
                                 TimeSpan);
        }
    }
}
