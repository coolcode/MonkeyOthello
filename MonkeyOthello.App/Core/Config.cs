using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Core
{
    public class Config
    {
        public readonly static Config Instance = new Config();

        public int MidDepth { get; set; } = 6;
        public int EndDepth { get; set; } = 20;
        public int WldDepth { get; set; } = 22;

        public OpenType OpenType { get; set; } = OpenType.WBBW;

        public ComputerAI MonkeyAI { get; set; } = ComputerAI.NORMAL;

        public GameMode GameMode { get; set; } = GameMode.PvsC;
    }

    public delegate void UpdateMessageDelegate(string msg);
}
 