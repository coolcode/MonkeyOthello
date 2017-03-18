using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Presentation
{
    public class Config
    {
        public static readonly Config Instance = new Config();

        public int MidDepth { get; set; } = 6;
        public int EndDepth { get; set; } = 20;
        public int WldDepth { get; set; } = 22;

    }

    public delegate void UpdateMessageDelegate(string msg);
}
