using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Engines
{
    public class Node
    {
        //[JsonIgnore]
        public Node Parent { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
        public int[] Values { get; set; } = new int[2];
        public int Visits { get; set; } = 0;
        public int Action { get; set; } = 0;
        public int Player { get; set; } = 0;
        public int Depth { get; set; } = 0;
        public double Score { get; set; } = 0.0;

        private double ucb1 = 0d;
        private const double C = 1.0d;

        public double UCB1
        {
            get
            {
                if (Visits == 0)
                {
                    return 0d;
                }

                if (Parent == null)
                {
                    ucb1 = Values[Player] / (double)Visits;
                }
                else
                {
                    var i = Player;
                    //find root
                    var current = Parent;
                    //while (current.parent != null)
                    //{
                    //    current = current.parent;
                    //}

                    ucb1 = (Values[i] / (double)Visits) + C * Math.Sqrt((2.0 * Math.Log(current.Visits)) / Visits);
                }
                return ucb1;
            }
            set
            {
                ucb1 = value;
            }
        }

        public string SeqText
        {
            get
            {
                var posStack = new Stack<int>();
                var current = this;
                do
                {
                    posStack.Push(current.Action);
                    current = current.Parent;
                } while (current != null);

                var text = string.Join("->", posStack);

                return text;
            }
        }

        public double WinRate
        {
            get { return Values[Player] / (double)Visits; }
        }

        public BitBoard Bits { get; set; } 

        public Node(Node parent, int action, int player, int depth)
        {
            this.Parent = parent;
            this.Action = action;
            this.Player = player;
            this.Depth = depth;
        }

        public void Add(Node item)
        {
            Children.Add(item);
        }

        public Node FindBestNode()
        {
            var bestWinRate = double.MinValue;
            Node bestNode = null;
            foreach (var node in Children)
            {
                if (node.WinRate > bestWinRate)
                {
                    bestWinRate = node.WinRate;
                    bestNode = node;
                }
            }

            return bestNode;
        }

        /*
        //public static Node Load(string path)
        //{
        //    var text = File.ReadAllText(path, Encoding.UTF8);
        //    var node = JsonConvert.DeserializeObject<Node>(text);
        //    Visit(node, (current, child) => child.parent = current);

        //    return node;
        //}

        public Node Search(int b)
        {
            return Search(this, b);
        }

        private Node Search(Node current, int b)
        {
            if (current.Bits == b)
            {
                return current;
            }

            foreach (var child in current.Children)
            {
                var node = Search(child, b);

                if (node != null)
                {
                    return node;
                }
            }

            return null;
        }

        public void Visit(Action<Node, Node> action)
        {
            Visit(this, action);
        }

        private static void Visit(Node current, Action<Node, Node> action)
        {
            foreach (var child in current.Children)
            {
                action(current, child);
                Visit(child, action);
            }
        }

        //public void Save(string path)
        //{
        //    var text = JsonConvert.SerializeObject(this);
        //    File.WriteAllText(path, text, Encoding.UTF8);
        //}
        */

        public override string ToString()
        {
            if (Parent == null)
            {
                return "Root Node";
            }

            return $"{SeqText} p{Player}'s move: {Action} Vi/Va[b]: {Visits}/{Values[0]} Vi/Va[w]: {Visits}/{Values[1]} ucb1: {UCB1.ToString("f4")}, win%: {WinRate.ToString("p2")}, score: {Score}, depth: {Depth} [{Bits?.Draw()}] ";
        }

        public string DrawTree()
        {
            return DrawTree("", true);
        }

        private string DrawTree(string indent, bool last)
        {
            var sb = new StringBuilder();

            sb.Append(indent);
            if (last)
            {
                sb.Append("\\-");
                indent += "  ";
            }
            else
            {
                sb.Append("|-");
                indent += "| ";
            }
            sb.AppendLine(ToString());

            for (int i = 0; i < Children.Count; i++)
            {
                var childText = Children[i].DrawTree(indent, i == Children.Count - 1);
                sb.Append(childText);
            }

            return sb.ToString();
        }
    }
}
