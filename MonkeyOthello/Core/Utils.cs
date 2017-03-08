using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyOthello.Core
{
    public class Utils
    {
        public static string SquareToString(int square)
        {
            int m = (square - 9) / 9;
            int n = (square - 9) % 9 - 1;
            int int_m = m + 1;
            char str_n = Convert.ToChar(n + 65);
            return str_n.ToString() + int_m.ToString();
        }

    }
}
