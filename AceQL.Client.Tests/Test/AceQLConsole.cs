using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.Tests
{
    public static class AceQLConsole
    {
        public static void WriteLine()
        {
            WriteLine("");
        }

        public static void WriteLine(String s)
        {
            Console.WriteLine(s);
        }
    }
}
