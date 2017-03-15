using MonkeyOthello.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonkeyOthello.Engines
{
    public abstract class BaseEngine : IEngine
    {

        public virtual string Name
        {
            get { return this.GetType().Name; }
        }

        public abstract SearchResult Search(BitBoard board, int depth);

    }
}
