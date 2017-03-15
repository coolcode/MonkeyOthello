using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Engines
{
    public class RandomEngine : BaseEngine
    {
        private Random rand = new Random();

        private string name;
        public override string Name
        {
            get
            {
                return name ?? base.Name;
            }
        }

        public RandomEngine()
        {

        }

        public RandomEngine(string name)
        {
            this.name = name;
        }


        public override SearchResult Search(BitBoard board, int depth)
        {
            var moves = Rule.FindMoves(board) ;
            var randMoveIndex = rand.Next(0, moves.Length);

            return new SearchResult
            {
                Move = moves[randMoveIndex],
                Score = 0,
                Message = "random move"
            };
        }
    }
}
