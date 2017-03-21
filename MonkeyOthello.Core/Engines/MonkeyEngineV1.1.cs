using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MonkeyOthello.Engines
{
    public class MonkeyEngine11 : BaseEngine
    {
        private const int highScore = Constants.HighestScore;

        private SearchResult searchResult = new SearchResult();

        public IEvaluation Evaluation { get; set; } = new SimpleEvaluation();

        public IEvaluation EndGameEvaluation { get; set; } = new EndGameEvaluation();

        private static readonly int[] orderedSquares = new int[]
        {
            36,35,28,27,63,56,7,0,61,58,47,40,23,16,5,2,45,42,21,18,60,59,39,32,31,24,4,3,44,43,37,34,29,26,20,19,52,51,38,33,30,25,12,11,53,50,46,41,22,17,13,10,62,57,55,48,15,8,6,1,54,49,14,9
            //E5,D5,E4,D4,H8,A8,H1,A1,F8,C8,H6,A6,H3,A3,F1,C1,F6,C6,F3,C3,E8,D8,H5,A5,H4,A4,E1,D1,E6,D6,F5,C5,F4,C4,E3,D3,E7,D7,G5,B5,G4,B4,E2,D2,F7,C7,G6,B6,G3,B3,F2,C2,G8,B8,H7,A7,H2,A2,G1,B1,G7,B7,G2,B2
        };

        private Link link = new Link();

        private Dictionary<int, int> squareDict = new Dictionary<int, int>(64);

        private void PrepareSearch(BitBoard board)
        {
            var squares = board.EmptyPieces.Indices().ToList();
            squareDict = orderedSquares.Select((c, i) => new { K = c, V = i }).ToDictionary(kv => kv.K, kv => kv.V);

            squares = squares.OrderBy(i => squareDict[i]).ToList();

            link = new Link(squares);
        }

        public override SearchResult Search(BitBoard board, int depth)
        {
            PrepareSearch(board);

            searchResult = new SearchResult();

            if (depth > Constants.MaxEndGameDepth)
            {
                searchResult.Message = "too depth...";
                return searchResult;
            }

            var clock = new Clock();
            clock.Start();

            var moves = Rule.FindMoves(board);

            if (moves.Length == 0)
            {
                moves = Rule.FindMoves(board);

                if (moves.Length == 0)
                {
                    //END
                    var endScore = EndGameEvaluation.Eval(board);

                    return new SearchResult() { Move = -1, Score = endScore };
                }
                else
                {
                    var result = Search(board.Switch(), depth);
                    result.Score = -result.Score;

                    return result;
                }
            }

            var alpha = -highScore - 1;
            var beta = highScore + 1;

            if (depth >= Constants.MaxEndGameDepth - 2)
            {//make search window small
                alpha = -1;
                beta = 1;
            }

            var score = minimumScore;
            var foundPv = false;

            var orderedMoves = FindOrderedMoves(board); //OrderMoves(moves, board);

            foreach (var m in orderedMoves)
            {
                //move  
                var pos = m.Index;
                var oppBoard = Rule.MoveSwitch(board, pos);
                m.Out();

                var eval = 0;
                //check
                if (foundPv)
                {
                    //zero window
                    eval = -FastestFirstSolve(oppBoard, -alpha - 1, -alpha, depth - 1);
                    if ((eval > alpha) && (eval < beta))
                    {
                        eval = -FastestFirstSolve(oppBoard, -beta, -eval, depth - 1);
                    }
                }
                else
                {
                    eval = -FastestFirstSolve(oppBoard, -beta, -alpha, depth - 1);
                }

                //reback? 
                m.In();

                searchResult.EvalList.Add(new EvalItem { Move = pos, Score = eval });

                searchResult.Message += string.Format("({0}:{1})", pos, eval);
                if (eval > score)
                {
                    score = eval;
                    //update move
                    searchResult.Move = pos;
                    searchResult.Score = score;

                    if (eval > alpha)
                    {
                        if (eval >= beta)
                        {
                            //purning
                            break;
                        }
                        alpha = eval;
                        foundPv = true;
                    }
                }
            }

            clock.Stop();

            searchResult.TimeSpan = clock.Elapsed;

            return searchResult;
        }

        private int FastestFirstSolve(BitBoard board, int alpha, int beta, int depth, bool prevmove = true)
        {
            searchResult.Nodes++;

            //game over
            if (board.IsFull)
            {
                return EndGameEvaluation.Eval(board);
            }

            //leaf node
            if (depth == 0)
            {
                return Evaluation.Eval(board);
            }

            //var moves = Rule.FindMoves(board);

            //if (moves.Length == 0)
            //{
            //    if (!prevmove)
            //    {
            //        //END
            //        return EndGameEvaluation.Eval(board);
            //    }
            //    else
            //    {
            //        return -FastestFirstSolve(board.Switch(), -beta, -alpha, depth, false);
            //    }
            //}

            var score = minimumScore;
            var foundPv = false;
            var hasMoves = false;
            //var orderedMoves = OrderMoves(moves, board); 

            var orderedMoves = FindOrderedMoves(board);

            foreach (var m in orderedMoves)
            {
                var pos = m.Index;
                var oppBoard = Rule.FlipSwitch(board, pos, m.Flips);
                var eval = 0;

                hasMoves = true;
                m.Out();

                if (depth <= Constants.ParityDepth)
                {
                    //Parity Search
                    if (foundPv)
                    {
                        //zero window
                        eval = -ParitySearch(oppBoard, -alpha - 1, -alpha, depth - 1);
                        if ((eval > alpha) && (eval < beta))
                        {
                            eval = -ParitySearch(oppBoard, -beta, -eval, depth - 1);
                        }
                    }
                    else
                    {
                        eval = -ParitySearch(oppBoard, -beta, -alpha, depth - 1);
                    }
                }
                else
                {
                    if (foundPv)
                    {
                        //zero window
                        eval = -FastestFirstSolve(oppBoard, -alpha - 1, -alpha, depth - 1);
                        if ((eval > alpha) && (eval < beta))
                        {
                            eval = -FastestFirstSolve(oppBoard, -beta, -eval, depth - 1);
                        }
                    }
                    else
                    {
                        eval = -FastestFirstSolve(oppBoard, -beta, -alpha, depth - 1);
                    }
                }

                //reback
                m.In();

                if (eval > score)
                {
                    score = eval;

                    if (eval > alpha)
                    {
                        if (eval >= beta)
                        {
                            //purning
                            return score;
                        }

                        alpha = eval;
                        foundPv = true;
                    }
                }
            }

            if (!hasMoves)
            {
                if (!prevmove)
                {
                    //END
                    return EndGameEvaluation.Eval(board);
                }
                else
                {
                    return -FastestFirstSolve(board.Switch(), -beta, -alpha, depth, false);
                }
            }

            return score;
        }


        private int ParitySearch(BitBoard board, int alpha, int beta, int depth, bool prevmove = true)
        {
            searchResult.Nodes++;

            //game over
            if (board.IsFull)
            {
                return EndGameEvaluation.Eval(board);
            }

            //leaf node
            if (depth == 0)
            {
                return Evaluation.Eval(board);
            }

            var moves = FindMoves(board).ToArray();// Rule.FindMoves(board);

            if (moves.Length == 0)
            {
                if (!prevmove)
                {
                    //END
                    return EndGameEvaluation.Eval(board);
                }
                else
                {
                    return -ParitySearch(board.Switch(), -beta, -alpha, depth, false);
                }
            }

            var score = minimumScore;
            var foundPv = false;

            foreach (var m in moves)
            {
                var eval = 0;
                var pos = m.Index;
                var oppBoard = Rule.FlipSwitch(board, pos, m.Flips);
                //remove node
                m.Out();

                if (depth <= Constants.NoParityDepth)
                {
                    //Parity Search
                    if (foundPv)
                    {
                        //zero window
                        eval = -NoParitySearch(oppBoard, -alpha - 1, -alpha, depth - 1);
                        if ((eval > alpha) && (eval < beta))
                        {
                            eval = -NoParitySearch(oppBoard, -beta, -eval, depth - 1);
                        }
                    }
                    else
                    {
                        eval = -NoParitySearch(oppBoard, -beta, -alpha, depth - 1);
                    }
                }
                else
                {
                    if (foundPv)
                    {
                        //zero window
                        eval = -ParitySearch(oppBoard, -alpha - 1, -alpha, depth - 1);
                        if ((eval > alpha) && (eval < beta))
                        {
                            eval = -ParitySearch(oppBoard, -beta, -eval, depth - 1);
                        }
                    }
                    else
                    {
                        eval = -ParitySearch(oppBoard, -beta, -alpha, depth - 1);
                    }
                }

                //reback
                m.In();

                if (eval > score)
                {
                    score = eval;

                    if (eval > alpha)
                    {
                        if (eval >= beta)
                        {
                            //purning
                            return score;
                        }

                        alpha = eval;
                        foundPv = true;
                    }
                }
            }

            return score;
        }

        private int NoParitySearch(BitBoard board, int alpha, int beta, int empties, bool prevmove = true)
        {
            searchResult.Nodes++;
            var moves = Rule.FindMoves(board);

            if (moves.Length == 0)
            {
                if (!prevmove)
                {
                    //END
                    return EndGameEvaluation.Eval(board);
                }
                else
                {
                    return -NoParitySearch(board.Switch(), -beta, -alpha, empties, false);
                }
            }

            var score = minimumScore;
            var diffCount = board.DiffCount();

            foreach (var pos in moves)
            {
                var eval = 0;

                var flips = Rule.FindFlips(board, pos);
                var oppBoard = Rule.FlipSwitch(board, pos, flips);

                if (empties == 2)
                {
                    var ownFlipsCount = flips.CountBits();
                    //the last move of opponent player
                    var lastEmptySquare = oppBoard.EmptyPieces.Index();
                    if (lastEmptySquare < 0)
                    {
                        throw new Exception($"invalid square index:{lastEmptySquare}");
                    }

                    var oppFlipsCount = Rule.CountFlips(oppBoard, lastEmptySquare);

                    if (oppFlipsCount > 0)
                    {
                        //both done.
                        eval = diffCount + 2 * (ownFlipsCount - oppFlipsCount);
                    }
                    else
                    {
                        //opp pass
                        var ownLastFlipsCount = Rule.CountFlips(oppBoard, lastEmptySquare);

                        if (ownLastFlipsCount > 0)
                        {
                            eval = diffCount + 2 * (ownFlipsCount + ownLastFlipsCount) + 2;
                        }
                        else
                        {
                            //all pass
                            eval = diffCount + 2 * ownFlipsCount;
                            //TODO: eval==0?
                            if (eval >= 0)
                            {
                                eval += 2;
                            }
                        }
                    }
                }
                else
                {
                    //empties!=2
                    eval = -NoParitySearch(oppBoard, -beta, -alpha, empties - 1);
                }

                if (eval > score)
                {
                    score = eval;
                    if (eval > alpha)
                    {
                        if (eval >= beta)
                        {
                            return score;
                        }
                        alpha = eval;
                    }
                }
            }

            return score;
        }

        private IEnumerable<int> OrderMoves(IEnumerable<int> moves, BitBoard board)
        {
            return moves.OrderBy(m => Rule.DiffMobility(Rule.MoveSwitch(board, m)));
        }

        private IEnumerable<Move> FindMoves(BitBoard board)
        {
            var moves = from node in link
                        let flips = Rule.FindFlips(board, node.Value)
                        where flips != 0
                        select new Move { Flips = flips, Node = node };

            //check move
            //foreach (var m in moves)
            //{
            //    if (((1UL << m.Index) & board.EmptyPieces) == 0)
            //    {
            //        throw new Exception($"invalid move: {m}");
            //    }
            //}

            return moves;
        }

        private IEnumerable<Move> FindOrderedMoves(BitBoard board)
        {
            var moves = FindMoves(board);

            var orderedMoves = moves.OrderBy(m => Rule.DiffMobility(Rule.MoveSwitch(board, m.Index)));

            return orderedMoves;
        }

        class Move
        {
            public int Index { get { return Node.Value; } }
            //public int FlipsCount { get { return Flips.CountBits(); } }
            public ulong Flips { get; set; }

            public LinkNode Node { get; set; }

            public void Out()
            {
                Node.Pre.Next = Node.Next;
                if (Node.Next != null)
                {
                    Node.Next.Pre = Node.Pre;
                }
            }

            public void In()
            {
                Node.Pre.Next = Node;
                if (Node.Next != null)
                {
                    Node.Next.Pre = Node;
                }
            }

            public override string ToString()
            {
                return $"{Index}";
            }
        }

        class Link : IEnumerable<LinkNode>
        {
            public LinkNode Head { get; private set; }
            public List<LinkNode> Origin { get; private set; } = new List<LinkNode>();

            public Link()
            {

            }

            public Link(IEnumerable<int> items)
            {
                Head = new LinkNode { Value = -1 };
                var pre = Head;
                foreach (var item in items)
                {
                    var current = new LinkNode { Value = item, Pre = pre };
                    pre.Next = current;
                    pre = current;
                }

                Origin.AddRange(this.ToArray());
            }

            public IEnumerator<LinkNode> GetEnumerator()
            {
                for (var current = Head.Next; current != null; current = current.Next)
                {
                    yield return current;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public void Do(Action<int> action)
            {
                for (LinkNode pre = Head, current = pre.Next; current != null; pre = current, current = current.Next)
                {
                    pre.Next = current.Next;
                    action(current.Value);
                    pre.Next = current;
                }
            }

            public void Add(LinkNode current)
            {
                var pre = current.Pre;
                var next = current.Next;
                if (pre == null)
                {
                    Head = current;
                }
                else
                {
                    pre.Next = current;
                }

                if (next == null)
                {
                    //  Last = current;
                }
                else
                {
                    next.Pre = current;
                }
            }

            public void Remove(LinkNode current)
            {
                var pre = current.Pre;

                if (pre == null)
                {
                    //first node
                    Head = current.Next;
                }
                else
                {
                    pre.Next = current.Next;
                }
            }


            public override string ToString()
            {
                return $"{string.Join(",", this)} (origin:{string.Join(",", Origin)})";
            }
        }

        class LinkNode
        {
            public LinkNode Pre { get; set; }
            public LinkNode Next { get; set; }
            public int Value { get; set; }

            public override string ToString()
            {
                return Value.ToString();
            }
        }
    }
}
