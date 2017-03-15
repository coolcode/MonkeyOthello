using MonkeyOthello.Core;
using System;
using System.Collections.Generic;

namespace MonkeyOthello.Engines
{
    public interface IEngine
    {
        string Name { get; }
        SearchResult Search(BitBoard board, int depth);
    }

    public class SearchResult
    {
        public int Move { get; set; }
        public int Score { get; set; }
        public string Message { get; set; } = string.Empty;
        public TimeSpan TimeSpan { get; set; }
        public int Nodes { get; set; }
        public List<EvalItem> EvalList { get; set; } = new List<EvalItem>();

        public SearchResult()
        {
        }

        public override string ToString()
        {
            return string.Format("Best Move:{0}, Score:{1:N}, Nodes:{2}, TimeSpan:{3}, Message:{4}, NPS:{5}",
                                 Move,
                                 Score,
                                 Nodes,
                                 TimeSpan,
                                 Message,
                                 Nodes / (TimeSpan.TotalSeconds + 0.000001));
        }
    }

    public class EvalItem
    {
        public int Move { get; set; }
        public int Score { get; set; }
    }
}
