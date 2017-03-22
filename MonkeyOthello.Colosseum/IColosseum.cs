using MonkeyOthello.Engines;
using System;
using System.Collections.Generic;

namespace MonkeyOthello.Colosseum
{
    public interface IColosseum
    {
        IEnumerable<IEngine> FindGladiators();
        void Fight(IEnumerable<IEngine> engines, int count = 1);
    }

}
