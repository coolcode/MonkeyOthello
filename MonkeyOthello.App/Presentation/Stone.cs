using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonkeyOthello.Presentation
{
    public class Stone
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Index { get { return X + Y * Constants.Line; } }
        public StoneType Type { get; set; }

        public Stone(int index, StoneType type)
            :this(index % Constants.Line, index / Constants.Line, type)
        { 
        }

        public Stone(int x, int y, StoneType type)
        {
            this.X = x;
            this.Y = y;
            this.Type = type;
        }


        public bool IsEmpty
        {
            get
            {
                return this.Type == StoneType.Empty;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", X, Y);
        }
    }
}