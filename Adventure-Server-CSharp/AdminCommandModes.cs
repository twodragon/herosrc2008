using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public enum ADMIN_MODE
    {
        Normal = 0,
        SpeedMode = 1
    }

    public static class AdminCommandModes
    {
        public static bool GOD_MODE = false;

    }
}
