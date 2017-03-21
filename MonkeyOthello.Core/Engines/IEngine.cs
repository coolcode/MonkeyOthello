using MonkeyOthello.Core;
using System;
using System.Collections.Generic;

namespace MonkeyOthello.Engines
{
    public interface IEngine
    {
        string Name { get; }
        UpdateProgress UpdateProgress { get; set; }
        SearchResult Search(BitBoard board, int depth);
    }

    public delegate void UpdateProgress(SearchResult result);

    public class SearchResult
    {
        public int Move { get; set; }
        public int Score { get; set; }
        public string Message { get; set; } = string.Empty;
        public TimeSpan TimeSpan { get; set; }
        public int Nodes { get; set; }
        public List<EvalItem> EvalList { get; set; } = new List<EvalItem>();
        public double Process { get; set; } = 0.0;
        public bool IsTimeout { get; set; } = false;
        
        public SearchResult()
        {
            TimeSpan = TimeSpan.Zero;
        }

        public override string ToString()
        {
            var notation = Move.ToNotation();
            return string.Format("Best Move:{0} {6}, Score:{1:N}, Nodes:{2}, TimeSpan:{3:F1}s, Message:{4}, NPS:{5}",
                                 Move,
                                 Score,
                                 Nodes,
                                 TimeSpan.TotalSeconds,
                                 Message,
                                 Nodes / (TimeSpan.TotalSeconds + 0.000001),
                                 notation);
        }
    }

    public class EvalItem
    {
        public int Move { get; set; }
        public int Score { get; set; }

        public override string ToString()
        {
            return $"({Move}:{Score})";
        }
    }
}
