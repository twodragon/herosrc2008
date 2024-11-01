using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public static class SkillBook
    {

        public static List<int> SkillBooks = new List<int>();

        public static List<int> GetCombatSkillBooksInfo()
        {
            SkillBooks.Clear();

            //warrior
            SkillBooks.Add(16100001);
            SkillBooks.Add(16100005);
            SkillBooks.Add(16100009);
            SkillBooks.Add(16100033);
            SkillBooks.Add(16100047);

            //rod
            SkillBooks.Add(16100002);
            SkillBooks.Add(16100006);
            SkillBooks.Add(16100010);
            SkillBooks.Add(16100034);
            SkillBooks.Add(16100048);

            //bow
            SkillBooks.Add(16100003);
            SkillBooks.Add(16100007);
            SkillBooks.Add(16100011);
            SkillBooks.Add(16100035);
            SkillBooks.Add(16100049);

            //punch
            SkillBooks.Add(16100004);
            SkillBooks.Add(16100008);
            SkillBooks.Add(16100012);
            SkillBooks.Add(16100036);
            SkillBooks.Add(16100050);

            //axe
            SkillBooks.Add(16100023);
            SkillBooks.Add(16100024);
            SkillBooks.Add(16100032);
            SkillBooks.Add(16100038);
            SkillBooks.Add(16100052);

            //firlat
            SkillBooks.Add(16100021);
            SkillBooks.Add(16100022);
            SkillBooks.Add(16100031);
            SkillBooks.Add(16100037);
            SkillBooks.Add(16100051);

            //ciftel
            SkillBooks.Add(16100053);
            SkillBooks.Add(16100054);
            SkillBooks.Add(16100055);
            SkillBooks.Add(16100059);
            SkillBooks.Add(16100061);

            //penta
            SkillBooks.Add(16100056);
            SkillBooks.Add(16100057);
            SkillBooks.Add(16100058);
            SkillBooks.Add(16100060);
            SkillBooks.Add(16100062);

            //job
            SkillBooks.Add(16110001);
            SkillBooks.Add(16110002);
            SkillBooks.Add(16110003);
            SkillBooks.Add(16110004);

            return SkillBooks;
        }

        public static List<int> GetPassiveSkillBooks()
        {
            SkillBooks.Clear();

            SkillBooks.Add(16200005);

            return SkillBooks;
        }

        public static byte GetSkillSlotId(int itemLevel, int isPassive = 0)
        {
            if (isPassive == 1)
                return 8;

            if (itemLevel <= 10)
                return 0;

            if (itemLevel > 10 && itemLevel <= 25)
                return 1;

            if (itemLevel > 25 && itemLevel <= 40)
                return 2;

            if (itemLevel > 40 && itemLevel <= 70)
                return 3;

            if (itemLevel > 70)
                return 4;

            return 99;
        }
    }
}
