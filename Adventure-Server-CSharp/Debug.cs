using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    internal class Debug
    {
        
        public static void Log(string text, ConsoleColor clr = ConsoleColor.Gray)
        {
            if (ServerSettings.ENABLE_DEBUGS == false) return;

            Console.ForegroundColor = clr;
            Console.WriteLine(text, Console.ForegroundColor);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
