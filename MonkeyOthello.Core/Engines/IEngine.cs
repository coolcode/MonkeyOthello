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
        public int Score { get; set; } = 0;
        public string Message { get; set; } = string.Empty;
        public TimeSpan TimeSpan { get; set; }
        public ulong Nodes { get; set; } = 0;
        public List<EvalItem> EvalList { get; set; } = new List<EvalItem>();
        public double Process { get; set; } = 0.0;
        public bool IsTimeout { get; set; } = false;
        public double Reliability { get; set; } = 1.0;

        public SearchResult()
        {
            TimeSpan = TimeSpan.Zero;
        }

        public override string ToString()
        {
            var notation = Move.ToNotation();
            var speed = Nodes / (TimeSpan.TotalSeconds + 0.000001);

            return $"Best Move:{Move} {notation}, Score:{Score:N}, Nodes:{Nodes:##,#}, TimeSpan:{TimeSpan.TotalSeconds:F1}s, NPS:{speed:##,#}, Message:{Message}, Reliability%: {Reliability:p0}";
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
