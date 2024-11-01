using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public static class ClientMemory
    {
        public static float PlayerPosX = 0;
        public static float PlayerPosY = 0;

        public static void SetPlayerPosition(float x, float y)
        {
            PlayerPosX = x;
            PlayerPosY = y;
        }
    }
}
