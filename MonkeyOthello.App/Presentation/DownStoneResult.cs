using System;

namespace MonkeyOthello.Presentation
{
    public class DownStoneResult
    {
        private int downedSeat;

        public int DownedSeat
        {
            get { return downedSeat; }
            set { downedSeat = value; }
        }
        private int score;

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        public override string ToString()
        {
            return downedSeat.ToString()+"("+score+")";
        }
    }
}
