using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Learning
{
    static class Extensions
    {
        public static string Repeat(this string s, int count)
        {
            return string.Join("", Enumerable.Repeat(s, count));
        }
    }
}
