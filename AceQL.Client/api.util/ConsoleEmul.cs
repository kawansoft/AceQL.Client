using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AceQL.Client.api.util
{
    /// <summary>
    /// Console emulator as in Portable Class Library we don't have access to Console.
    /// </summary>
    internal class ConsoleEmul
    {
        private const string CONSOLE_EMUL = "CONSOLE_EMUL";

        internal static void WriteLine()
        {
            System.Diagnostics.Debug.WriteLine(CONSOLE_EMUL);
        }

        internal static void WriteLine(string s)
        {
            System.Diagnostics.Debug.WriteLine("CONSOLE_EMUL" + " " + s);
        }
    }
}
