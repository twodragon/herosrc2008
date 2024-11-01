using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    internal class DataDebugger
    {
        public static void S2CDebugData(byte[] data, string dataName)
        {
            if (ServerSettings.ENABLE_DEBUGS == false) return;

            Debug.Log("["+ dataName+ "]" + BitConverter.ToString(data), ConsoleColor.Magenta);
        }
    }
}
