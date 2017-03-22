using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Colosseum
{
    public static class Extensions
    {
        public static void PK<T>(this IEnumerable<T> list, Action<T, T> action) where T : class
        {
            foreach (T t1 in list)
                foreach (T t2 in list)
                    if (t1 != t2)
                        action(t1, t2);
        }
    }
}
