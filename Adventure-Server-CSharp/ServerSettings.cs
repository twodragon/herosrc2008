using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public static class ServerSettings
    {
        public static byte createPseudoIndex = 0;
        public static byte createDropItemIndex = 0;

        public static bool ENABLE_MOBS = true;

        public static float DROP_RATE = 0.8f;

        public static bool ENABLE_DEBUGS = true;

        public static string LOCAL_IPV4 = "192.168.1.3";
    }
}
