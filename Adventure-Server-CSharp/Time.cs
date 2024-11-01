using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public class Time
    {

        public static int time
        {
            get
            {
                return Environment.TickCount;
            }
        }

    }
}
