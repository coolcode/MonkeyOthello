using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Engines
{
    /// <summary>
    /// Monte Carlo tree search
    /// <see cref="https://en.wikipedia.org/wiki/Monte_Carlo_tree_search"/>
    /// </summary>
    public class MCTSEngine : BaseEngine
    {
        private static readonly Random rand = new Random();

        public override SearchResult Search(BitBoard board, int depth)
        {
            var color = 0;//first player

            var searchResult = new SearchResult();

            var clock = new Clock();
            clock.Start();

            var bm = GetBestMove(board, color);

            clock.Stop();
            var movesMsg = string.Join("\n", bm.Parent.Children.Select(c => $"{c.Action} : {c.WinRate.ToString("p2")}"));
            searchResult.Move = bm.Action;
            searchResult.Score = 0;
            searchResult.Message = $"mcts move: \n{movesMsg} \n";
            searchResult.TimeSpan = clock.Elapsed;

            return searchResult;
        }

        public Node GetBestMove(BitBoard b, int color)
        {
            var board = b.Copy();
            Node root = new Node(null, 0, color.Opp(), 0);

            root.Bits = board;

            for (int iteration = 0; iteration < 100; iteration++)
            {
                Node current = Selection(root, color);
                int value = Rollout(current, color);
                Update(current, value);
            }

            //root.Save($"{DateTime.Now.ToString("yyyy-MM-dd")}.json");// DrawTree();

            var bestNode = root.FindBestNode();

            return bestNode; //BestChildUCB(root, 0).action;
        }

        //#1. Select a node if 1: we have more valid feasible moves or 2: it is terminal 
        public Node Selection(Node root, int startColor)
        {
            var color = startColor;
            var current = root;
            var board = current.Bits;
            while (!board.IsGameOver())
            {
                var moves = Rule.FindMoves(board).ToList();

                if (moves.Count == 0)
                {
                    board = board.Switch();
                    color = color.Opp();
                    continue;
                }

                if (moves.Count > current.Children.Count)
                {
                    return Expand(current, color);
                }

                var bestChild = BestChildUCB(current, startColor); /*1.44*/
                if (bestChild == null)
                {
                    return bestChild;
                }

                current = bestChild;
                board = current.Bits;
                color = current.Player.Opp();
            }

            return current;
        }

        //#1.  
        public Node BestChildUCB(Node current, int startColor)
        {
            var maxUCB1 = current.Children.Max(c => c.UCB1);
            /* var childColor = current.children[0].player;
             var maxUCB1 = childColor == startColor ? current.children.Max(c => c.ucb1) : current.children.Min(c => c.ucb1);
             */
            return current.Children.FirstOrDefault(c => c.UCB1 == maxUCB1);
            /*
            Node bestChild = null;
            double best = double.NegativeInfinity;

            foreach (Node child in current.children)
            {
                double UCB1 = ((double)child.value / (double)child.visits) + C * Math.Sqrt((2.0 * Math.Log((double)current.visits)) / (double)child.visits);
                //child.ucb1 = UCB1;

                if (UCB1 > best)
                {
                    bestChild = child;
                    best = UCB1;
                }
            }

            return bestChild;*/
        }

        //#2. Expand a node by creating a new move and returning the node
        private Node Expand(Node current, int color)
        {
            var board = current.Bits;
            var moves = Rule.FindMoves(board).ToList();

            for (int i = 0; i < moves.Count; i++)
            {
                var move = moves[i];

                if (current.Children.Exists(a => a.Action == move))
                    continue;

                var opp = color.Opp();

                Node node = new Node(current, move, color, current.Depth + 1);
                current.Children.Add(node);

                //Do the move in the game and save it to the child node
                var oppBoard = Rule.MoveSwitch(board, move);

                node.Bits = oppBoard;

                return node;
            }

            throw new Exception("Error");
        }

        //#3. Roll-out. Simulate a game with a given policy and return the value
        public int Rollout(Node current, int startColor)
        {
            var board = current.Bits;
            var opp = current.Player.Opp();

            /*
            var empties = board.EmptyPieces.CountBits();
            if (empties <= 18)
            {
                var endGameEngine = new MonkeyEngine();
                var result = endGameEngine.Search(board, empties);
                var score = -result.Score;
                current.Score = score;

                return (score >= 0) ? 1 : 0;
            }*/ 

            startColor = current.Player;
            //If this move is terminal and the opponent wins, this means we have previously made a move where the opponent can always find a move to win.. not good
            if (board.IsGameOver())
            {
                if (board.DiffCount() >= 0 )
                {
                    return 1;
                }

                return 0;
            }

            var player = opp; //current.player.Opp();

            //Do the policy until a winner is found for the first (change?) node added
            while (!board.IsGameOver())
            {
                //Random
                var moves = Rule.FindMoves(board).ToList();

                if (moves.Count == 0)
                {
                    board = board.Switch();
                    player = player.Opp();
                    continue;
                }
                var move = moves[rand.Next(0, moves.Count)];
                board = Rule.MoveSwitch(board, move);
                player = player.Opp();
                
                /*empties = board.EmptyPieces.CountBits();
                if (empties <= 18)
                {
                    var endGameEngine = new MonkeyEngine();
                    var result = endGameEngine.Search(board, empties); 

                    return (result.Score >= 0 &&  player==startColor) ? 1 : 0;
                }*/

            }

            if (board.DiffCount() >= 0 && player== startColor)
            {
                return 1;
            }

            return 0;

        }

        //#4. Update
        public void Update(Node current, int value)
        {
            var color = (value >0) ? current.Player : current.Player.Opp();
            do
            {
                current.Visits++;
                current.Values[color] += 1;
                current = current.Parent;
            }
            while (current != null);
        }


    }
}
