using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Google.Protobuf.WellKnownTypes.Field.Types;
using static System.Net.WebRequestMethods;

namespace Adventure_Server_CSharp.PacketTypes
{

    public class SSellItemMessage
    {
        // AA 55 16 00 58 02 0A 00 62 BE A1 00 0D 00 62 F6 F3 05 00 00 00 00 20 1C 00 00 55 AA
        // AA-55-16-00-58-02-0A-00-8F-54-9D-00-0C-00-88-6C-00-00-00-00-00-00-20-1C-00-00-55-AA
        public uint itemId;
        public ushort itemSlot;
        public ulong gold;

        public byte[] getValue()
        {
            //                            AA-55-16-00-58-02-0A-00-                       8F-54-9D-00-                                     0C-00-                                                     88-6C-00-00-00-00-00-00-                20-1C-00-00-55-AA
            byte[] x = Functions.FromHex("AA 55 16 00 58 02 0A 00 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + BitConverter.ToString(BitConverter.GetBytes(itemSlot)) + BitConverter.ToString(BitConverter.GetBytes(gold)) + " 20 1C 00 00 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }


    public class SItemDestroy
    {
        // AA 55 0B 00 59 02 0A 00 01 2B D2 BD 00 0B 00 55 AA

        public uint itemId;
        public ushort itemSlot;

        public byte[] getValue()
        {
            byte[] x = Functions.FromHex("AA 55 0B 00 59 02 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + BitConverter.ToString(BitConverter.GetBytes(itemSlot)) + " 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }


    public class SItemUsage
    {
        // AA 55 0C 00 59 04 0A 00 62 BE A1 00 0D 00 00 00 55 AA
        // AA 55 0C 00 00 00 0A 00 62 BE A1 00 0D 00 00 00 55 AA

        public uint itemId;
        public ushort itemSlot;
        public ushort itemCount;

        public byte[] getValue()
        {

            //AA-55-0C-00-59-04-0A-00-62-BE-A1-00-0B-00-00-00-55-AA

            //                            AA-55-0C-00-59-04-0A-00-                 62-BE-A1-00-                                            0B-00-                                                             00-00-                              55-AA
            //                            AA 55 0C 00 59 04 0A 00                  62 BE A1 00                                             0D 00                                                              00 00                               55 AA
            byte[] x = Functions.FromHex("AA 55 0C 00 59 04 0A 00 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + BitConverter.ToString(BitConverter.GetBytes(itemSlot)) + BitConverter.ToString(BitConverter.GetBytes(itemCount)) + " 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }


    public class SItemDrop
    {
        // AA 55 0B 00 59 02 0A 00 01 20 2C 0E 01 0B 00 55 AA

        public uint itemId;
        public ushort itemSlot;

        public byte[] getValue()
        {
            byte[] x = Functions.FromHex("AA 55 0B 00 59 02 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + BitConverter.ToString(BitConverter.GetBytes(itemSlot)) + " 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }

    public class SMobItemDrop
    {
        // AA-55-42-00-67-02-BB-80-10-11-A4-38-D2-9D-76-F1-1B-F4-74-32-16-57-AD-37-B0-50-0A-10-10-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-10-00-00-00-00-00-00-00-07-A0-30-00-05-5A
        // AA 55 42 00 67 02 0E 04  01 01 00 00 D6 42 D7 6F 11 BF 00 00 75 43 7E C7 C9 01 00 A1 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 02 00 00 00 55 AA 


        // AA 55 42 00 67 02 SESS2X 01 01   POSX-4X   D7 6F 11 BF    POSY4X   ITEMID4X    00 A1 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 7A 03 00 00 55 AA 
        // AA-55-42-00-67-02- BB-80-10-11-A4-38-D2-9D-76-F1-1B-F4-74-32-16-57-AD-37-B0-50-0A-10-10-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-10-00-00-00-00-00-00-00-07-A0-30-00-05-5A


        public ushort sessionId;
        public ushort claimerId;
        public uint itemId;
        public float posX = 0, posY = 0;
        public string itemNameColor = "A2";

        public byte upgradeLevel = 1;

        public byte[] getValue()
        {



            //AA 55 42 00 67 02 SESS2X 01 01 POSX-4X     D7 6F 11 BF    POSY4X     ITEMID4X       00 A1 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 7A 03 00 00 55 AA 
            //AA-55-42-00-67-02-0B-B8- 01-01-2F-43-17-CD-D7-6F-11-BF- 49-43-97-9A-  7A-D3-7B-05-  00-A1-01-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-01-00-00-00-00-00-00-00-00-7A-03-00-00-55-AA

            //                            AA-55-42-00-67-02        -0B-BF-	                 01-01-           A6-C9-A6-43                                              -D7-6F-11-BF     -00-00-4B-43-                                                                              D2-CA-9B-00               -00-                   A2-01-00-00-00-               01-                  00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-01-00-00-55-AA

            byte[] x = Functions.FromHex("AA 55 30 00 67 02 " + sessionId.ToString("X4") + " 01 01 " + BitConverter.ToString(Functions.GetFloatLittleEndian(posX)) + " D7 6F 11 BF " + BitConverter.ToString(Functions.GetFloatLittleEndian(posY)) + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 " + itemNameColor + " 01 00 00 00 " + upgradeLevel.ToString("X2") + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " + claimerId.ToString("X4") + "00 00 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);

            Debug.Log("session id: " + sessionId + "\nitemId:" + itemId + "\n pos: " + posX + ", " + posY, ConsoleColor.Cyan);
            Debug.Log("HEX posx: " + BitConverter.ToString(Functions.GetFloatLittleEndian(posX)) + ", Y:" + BitConverter.ToString(Functions.GetFloatLittleEndian(posY)));
            return x;
        }
    }

    public class SItemDrag
    {
        // AA 55 0C 00 59 03 0A 00 91 5A F6 05 0B 00 0C 00 55 AA 
        // AA 55 0C 00 59 03 0A 00 91 5A F6 05 0C 00 0D 00 55 AA

        public uint itemId;
        public ushort itemSlot, itemToSlot;

        public byte[] getValue()
        {
            byte[] x = Functions.FromHex("AA 55 0C 00 59 03 0A 00 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + BitConverter.ToString(BitConverter.GetBytes(itemSlot)) + BitConverter.ToString(BitConverter.GetBytes(itemToSlot)) + " 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            Console.WriteLine("ITEM DRAG PACKET:\n" + BitConverter.ToString(x));

            return x;
        }
    }

    public class SItemUpgrade
    {
        // AA 55 0C 00 59 03 0A 00 91 5A F6 05 0B 00 0C 00 55 AA 
        // AA 55 0C 00 59 03 0A 00 91 5A F6 05 0C 00 0D 00 55 AA
        // AA 55 31 00 54 02 A1 0F 01 D1 CA 9B 00 00 00 00 00 slotid 00 60 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA

        public uint itemId; //BitConverter.ToString(BitConverter.GetBytes(itemId))
        public byte itemSlot; //BitConverter.ToString(BitConverter.GetBytes(itemSlot))
        public byte isSucceed;
        public byte[] getValue()
        {
            //             54 02 A1 0F 01 D1 CA 9B 00 00 00 00 00 slotid 00 60 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA
            string hex = "";

            if (isSucceed == 1)
             hex = ("54 02 A1 0F " + isSucceed.ToString("X2") + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 00 00 00 " + itemSlot.ToString("X2") + " 00 60 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            else
             hex = ("54 02 A2 0F " + isSucceed.ToString("X2") + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 00 00 00 " + itemSlot.ToString("X2") + " 00 60 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");

            // AA 55 00 00 73 02 00  55 AA
            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);

            DataDebugger.S2CDebugData(x, this.GetType().Name);

            return x;
        }
    }

    public class SRemoveDroppedLootItem
    {
        public uint itemPseudoId;

        public byte[] getValue()
        {
            byte[] x = Functions.FromHex("AA 55 04 00 67 04 " + itemPseudoId.ToString("X4") + " 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);

            Debug.Log("REMOVING LOOT ITEM FROM GROUND... ps_id: " + itemPseudoId, ConsoleColor.Cyan);

            return x;
        }
    }

    public class SItemSwitchAckMessage
    {
        // AA 55 15 00 59 07 0A 00 01 D2 CA 9B 00 0D 00 03 00 72 51 9D 00 03 00 0D 00 55 AA 

        public uint fromItemId, toItemId;
        public ushort inventorySlot, characterSlot;

        public byte[] getValue()
        {
            byte[] x = Functions.FromHex("AA 55 15 00 59 07 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(toItemId)) + BitConverter.ToString(BitConverter.GetBytes(inventorySlot)) + BitConverter.ToString(BitConverter.GetBytes(characterSlot)) + BitConverter.ToString(BitConverter.GetBytes(fromItemId)) + BitConverter.ToString(BitConverter.GetBytes(characterSlot)) + BitConverter.ToString(BitConverter.GetBytes(inventorySlot)) + " 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }

    public class SInventoryInfo
    {
        // AA 55 3C 00 58 01 0A 00 D1 CA 9B 00 00 A1 01 00 11 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 84 00 00 00 00 00 00 00 20 1C 00 00 55 AA
        // AA-55-3C-00-58-01-0A-00-00-00-00-00-00-A1-88-6C-00-00-01-88-88-01-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-84-00-00-00-00-00-00-20-1C-00-00-55-AA
        public ushort itemSlot, itemCount = 1;
        public uint itemId;
        public ulong gold;

        public byte[] getValue(TcpClient client = null)
        {
            //                            AA-55-3C-00-58-01-0A-00-                8F-54-9D-00-                                 00-A1-                01-00-                                                   0B-00-                                          00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-05-84-00-00-00-00-00-00-20-1C-00-00-55-AA
            //                            AA-55-3C-00-58-01-0A-00-                00-00-00-00-                                 00-A1-                88-6C-                                                   00-00-                                          01-88-88-01-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-84-00-00-00-00-00-00-20-1C-00-00-55-AA
            byte[] x = Functions.FromHex("AA 55 3C 00 58 01 0A 00 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A1 " + BitConverter.ToString(BitConverter.GetBytes(itemCount)) + BitConverter.ToString(BitConverter.GetBytes(itemSlot)) + BitConverter.ToString(BitConverter.GetBytes(gold)) + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 20 1C 00 00 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            string pkt = BitConverter.ToString(x).Replace("-", " ");

            pkt = pkt.Substring(0, (pkt.LastIndexOf("55 AA") + "55 AA".Length));

            //Debug.Log("item count: " + BitConverter.ToString(BitConverter.GetBytes(itemCount)));
            //Debug.Log("item slot: " + BitConverter.ToString(BitConverter.GetBytes(itemSlot)));

            // Console.WriteLine("SEPERATE PACKET\n" + pkt);

            return x;
        }
    }

    public class SChangeMap
    {
        public int mapID;
        public int posX, posY;
        public byte[] getValue()
        {
            ////                          AA 55 05 00 28 F5 03 00 00 55 AA B4 54 42 00 00 00 00 00 00 C8 41 00 00 24 42 00 00 00 00 06 00 00 A0 A0 41 00 55 AA

            Debug.Log("---------- SCHANGE MAP CALLED ----------", ConsoleColor.Red);
            string hex = ("73 " + mapID.ToString("X2") + BitConverter.ToString(BitConverter.GetBytes(posX)) + BitConverter.ToString(BitConverter.GetBytes(posY)) + "00 00 7A 44 ");
            // AA 55 00 00 73 02 00  55 AA
            // AA 55 0E 00 73 01 00 01 00 00 01 01 00 00 00 00 01 55 AA
            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);

            // end pvp AA 55 02 00 2B 01 55 AA
            // change minimap AA 55 07 00 01 B9 0A 00 00 01 00 55 AA
            // teleport safezone AA 55 09 00 26 55 AA
            // unknown AA 55 03 00 A6 00 00 55 AA
            // unknown AA 55 02 00 AD 01 55 AA

            DataDebugger.S2CDebugData(x, this.GetType().Name);

            return x;
        }
    }

    public class SInventoryItemCombine
    {
        // 0c deki 1 adet sağlık iksirini 0d deki 6 adet sağlık iksiriyle item  birleştirme
        // AA 55 10 00 59 06 0A 00 62 BE A1 00 0C 00 00 00 0D 00 07 00 55 AA

        public ushort fromItemSlot, toItemSlot, totalItemCount;
        public uint itemId;


        public byte[] getValue()
        {
            byte[] x = Functions.FromHex("AA 55 10 00 59 06 0A 00 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + BitConverter.ToString(BitConverter.GetBytes(fromItemSlot)) + " 00 00 " + BitConverter.ToString(BitConverter.GetBytes(toItemSlot)) + BitConverter.ToString(BitConverter.GetBytes(totalItemCount)) + " 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }


    public class SInventoryItemSeperate
    {
        // 7li sağlık iksirini 0D'den 0C ye 1 adet olarak item ayırma
        // AA 55 5C 00 59 09 0A 00 62 BE A1 00 00 A1 06 00 0D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 62 BE A1 00 00 A1 01 00 0C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA 

        // AA 55 5C 00 59 09 0A 00 62 BE A1 00 00 A1 06 00 0D 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 62 BE A1 00 00 A1 01 00 0C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA

        public ushort seperateTo, seperateFrom, remainingItemCount, seperateItemCount;
        public uint itemId;


        public byte[] getValue()
        {
            byte[] x = Functions.FromHex("AA 55 5C 00 59 09 0A 00 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A1 " + BitConverter.ToString(BitConverter.GetBytes(remainingItemCount)) + BitConverter.ToString(BitConverter.GetBytes(seperateFrom)) + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A1 " + BitConverter.ToString(BitConverter.GetBytes(seperateItemCount)) + BitConverter.ToString(BitConverter.GetBytes(seperateTo)) + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            string pkt = BitConverter.ToString(x).Replace("-", " ");

            pkt = pkt.Substring(0, (pkt.LastIndexOf("55 AA") + "55 AA".Length));

            //Console.WriteLine("SEPERATE PACKET:\n" + pkt);

            return x;
        }
    }



    public class SMobAttackAnimation
    {
        // AA 55 0C 00 41 01 6C  4E 00 02 01 01 01 00 00 00 55 AA
        // AA 55 0C 00 41 01 "mob sess id" "user ses id" 01 01 01 00 00 00 55 AA
        // AA 55 0B 00 41 01 5C 02 06 6E 4E 00 01 01 00 55 AA 
        
        public ushort user_sessionid, mob_sessionid;

        public byte[] getValue()
        {

            byte[] x = Functions.FromHex("AA 55 0C 00 41 01 " + mob_sessionid.ToString("X4") + user_sessionid.ToString("X4") + " 01 01 01 00 00 00 55 AA");
            // DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;

        }
    }


    public class SDropGold
    {
        // AA 55 0D 00 59 01 0A 00 02 63 00 00 00 00 00 00 00 55 AA 

        public ulong gold;

        public byte[] getValue()
        {

            byte[] x = Functions.FromHex("AA 55 0D 00 59 01 0A 00 02 " + BitConverter.ToString(BitConverter.GetBytes(gold)) + " 55 AA ");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;

        }
    }

    public class SCreateItemWithUpgradeLevel
    {
        // AA 55 33 00 59 01 0A 00 01 " + itemidx4 + " 00 A2 01 00 " + slotidx2 + " 00 60 60 00 00 00 00 00 00 00 00 21 11 55 AA
        // AA 55 33 00 59 01 0A 00 01 D1-CA-9B-00 00 A2 01 00 0C 00 60 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA

        public byte itemSlot;
        public uint itemId;
        public int upgradeLevel = 1;
        public byte upgradeStone = 96;

        public byte[] getValue()
        {
            string hex = "";

            if (upgradeLevel == 1)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");

            if (upgradeLevel == 2)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");

            if (upgradeLevel == 3)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00");

            if (upgradeLevel == 4)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00 00 00 00 00 00 00 00 00 00");

            if (upgradeLevel == 5)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00 00 00 00 00 00 00 00 00");

            if (upgradeLevel == 6)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00 00 00 00 00 00 00 00");

            if (upgradeLevel == 7)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00 00 00 00 00 00 00");

            if (upgradeLevel == 8)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00 00 00 00 00 00");

            if (upgradeLevel == 9)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00 00 00 00 00");

            if (upgradeLevel == 10)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00 00 00 00");

            if (upgradeLevel == 11)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00 00 00");

            if (upgradeLevel == 12)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00 00");

            if (upgradeLevel == 13)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00 00");

            if (upgradeLevel == 14)
                hex = ("59 01 0A 00 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00 A2 01 00 " + itemSlot.ToString("X2") + " 00 " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " " + upgradeStone.ToString("X2") + " 00 00 00");


            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);

            DataDebugger.S2CDebugData(x, this.GetType().Name);

            return x;

        }
    }

    public class SMobWalkMoveAckMessage
    {
        // "AA 55 22 00 22 A4 02 01 " + pos + pos + pos + pos + pos + pos + " 66 66 C6 40 00 00 55 AA"

        // AA 55 21 00 33 6C 4E 01 6E 86 E8 42 48 48 6B 43 59 FB 7C BF 00 00 EA 42 00 00 75 43 D2 EF 2E BE 00 00 80 3F 00 55 AA

        public ushort mob_session_id;
        public Vector3 position1 = null, position2 = null;

        public byte walk_state = 1;

        public byte[] getValue()
        {
            if (position1.Equals(null) || position2.Equals(null))
                return Functions.FromHex("00");

            byte[] x = Functions.FromHex("AA 55 21 00 33 " + mob_session_id.ToString("X4") + walk_state.ToString("X2") + position1.ToByteString() + position2.ToByteString() + " 00 00 00 00 00 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;

        }
    }

    public class SMoveAckMessage
    {
        // "AA 55 22 00 22 A4 02 01 " + pos + pos + pos + pos + pos + pos + " 66 66 C6 40 00 00 55 AA"

        // AA 55 22 00 22 D5 00 01 2D 7B 7D 42 66 61 56 43 24 69 72 BF 00 00 7C 42 00 00 5B 43 00 00 80 BF 66 66 C6 40 00 00 55 AA 
        //

        // 0xAA, 0x55, 0x22, 0x00, 0x22, 0x01, 0x00, 0x00, 0x00, 0x00, 0xC8, 0xB0, 0xFE, 0xBE, 0x00, 0x00, 0x55, 0xAA

        public float x1 = 0, y1 = 0, z1 = 0, x2 = 0, y2 = 0, z2 = 0;
        public ushort uid;
        public ushort runMode;
        public Vector3 position1 = null, position2 = null;
        public float runSpeed;
        public byte runType;
        public enum MOVE_TYPE
        {
            WALK = 8705,
            RUN = 8706,
            FLY = 9732
        }

        // ushort len = Convert.ToUInt16(38);
        public byte[] getValue()
        {
            //Debug.Log("MOVE!", ConsoleColor.Red);

            byte[] x;

            // new
            // x = Functions.FromHex("AA 55 " + uid.ToString("X4") + BitConverter.ToString(BitConverter.GetBytes(runMode)) + BitConverter.ToString(BitConverter.GetBytes(x1)) + BitConverter.ToString(BitConverter.GetBytes(y1)) + "00 00 80 BF" + BitConverter.ToString(BitConverter.GetBytes(x2)) + BitConverter.ToString(BitConverter.GetBytes(y2)) + "9F 6F 80 BF" + BitConverter.ToString(Functions.GetMidBigEndianFloat(BitConverter.GetBytes(runSpeed))) + " 00 00 00 55 AA");

           // Debug.Log("x: " + BitConverter.ToString(BitConverter.GetBytes(x2)) + ", y: " + BitConverter.ToString(BitConverter.GetBytes(y2)), ConsoleColor.DarkGreen);

            if (position1 == null || position2 == null)
                x = Functions.FromHex("AA 55 22 00 22 " + uid.ToString("X4") + runType.ToString("X2") + BitConverter.ToString(BitConverter.GetBytes(x1)) + BitConverter.ToString(BitConverter.GetBytes(y1)) + BitConverter.ToString(BitConverter.GetBytes(z1)) + BitConverter.ToString(BitConverter.GetBytes(x2)) + BitConverter.ToString(BitConverter.GetBytes(y2)) + BitConverter.ToString(BitConverter.GetBytes(z2)) + BitConverter.ToString(Functions.GetMidBigEndianFloat(BitConverter.GetBytes(runSpeed))) + " 00 00 55 AA");
            else // target move using by mobs
                x = Functions.FromHex("AA 55 22 00 22 " + uid.ToString("X4") + " 01 " + position1.ToByteString() + position2.ToByteString() + " 66 66 " + BitConverter.ToString(Functions.GetMidBigEndianFloat(BitConverter.GetBytes(runSpeed))) + " 55 AA");

            //Debug.Log("Run Speed " + BitConverter.ToString(Functions.GetMidBigEndianFloat(BitConverter.GetBytes(runSpeed))));

            return x;

        }
    }

    public class SIngameRemoteCharacterInfoAckMessage
    {

        public ushort uid;
        public string nickname;
        public byte evolution;
        public byte race;
        public byte level;
        public byte isActivated = 3; // 03 activated , 04 not activated
        public long battlePower = 0;
        public long honorPoint = 0;

        public float posX = 0f, posY = 0f, posZ = 0f;

        public byte[] getValue()
        {
            //AA 55 E4 00 21 01 "+uid.ToString("X4")+ " C0 9A 01 00 04 01 00 00 00 " + nickname.Length.ToString("X2") + BitConverter.ToString(Encoding.ASCII.GetBytes(nickname)) + " 01 00 00 00 "class id 1 byte" 02 "scale 1 byte" 00 83 0E 00 00 00 2D 7B " 6 tane pos mid-big endian(7D 42 66 61 56 43 07 9C 78 BF 2D 7B 7D 42 66 61 56 43 07 9C 78 BF 00 00) " 00 00 FF FF 10 27 00 00 00 00 00 00 D2 00 00 00 00 00 00 00 00 03 F3 03 00 00 00 05 00 00 00 00 10 27 00 00 35 FF FF FF FF 01 00 00 00 "chaos-order" 00 00 00 00 64 FF FF FF FF 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 55 AA
            //ushort len = Convert.ToUInt16(0xAD + nickname.Length);
            // "AA 55 " + Functions.ReverseBytes16(len).ToString("X4") + " 17 " + Functions.ReverseBytes16(Convert.ToUInt16(len - 5)).ToString("X4") + Convert.ToByte(0x79 + ilk).ToString("X2") + " 0D " + ((nickname.Length < 10) ? Convert.ToByte(0x1A + ilk).ToString("X2") : (0x1F).ToString("X2")) + Convert.ToByte(0x77 + ilk).ToString("X2") + " 0D 08 " + uid.ToString("X4") + " 65 2F E7 " + nickname.Length.ToString("X4") + BitConverter.ToString(Encoding.ASCII.GetBytes(nickname)) + evolution.ToString("X2") + " 02 00 " + currentMap.ToString("X2") + " " + BitConverter.ToString(BitConverter.GetBytes(posX)) + BitConverter.ToString(BitConverter.GetBytes(posY)) + " 00 20 00 20 0C E0 " + honor + "00 01" + " A0 01 " + " 00 06 20 01 E0 03 00 00 04 E0 03 0C 60 00 00 64 60 05 06 00 " + faction + " 10 0E 00 00 48 20 07 00 D2 20 03 00 1E 20 03 00 30 20 03 60 00 01 03 01 20 00 60 09 60 00 00 " + race.ToString("X2") + " E0 0A 43 E0 35 00 0A 04 00 00 00 00 00 00 A1 01 00 0B E0 1A 48 03 00 00 00 00 40 2B 00 0C E0 1A 2B 00 00 A0 2B 00 0D E0 1A 2B 02 00 00 00 60 83 21 06 E0 FF 00 E0 FF 00 E0 FF 00 E0 FF 00 E0 F0 00 00 03 E0 F0 F9 E0 37 00 E1 FF 39 E0 6A 00 00 02 20 73 E1 0E 7E 02 03 00 03 E0 0E 19 E0 18 00 01 05 05 E0 18 22 E0 FF 00 E0 FF 00 E0 FF 00 E0 A8 00 01 00 00 55 AA");


            //AA 55 BE 00 21 01 + 2byte psid + F6 6F 03 00 + 1 byte activation + 01 00 00 00 + 1byte Name Length      + Name +       01 + evolutionClass1byte + 02 00 +  4byte battle point +       00 9D E2 + posx4byte + posy4byte + 80 BF 9D E2 + posx4byte + posy4byte + 00 00 00 80 BF 00 00 00 00 + nametag 4byte + 00 00 00 00 00 00 58 00 00 00 1E 00 00 00 D2 00 00 00 30 00 00 00 00 00 00 00 00 03 F3 03 00 00 00 03 00 00 00 00 10 27 00 00 35 FF FF FF FF 01 00 00 00 02 00 00 00 00 64 FF FF FF FF 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 55 AA

            //best AA 55 BE 00 21 01         34 00            F6 6F 03 00              03                    01 00 00 00             05                                  61 62 63 64 72                                    01           35                     02 00                  14 0E 00 00                  00 9D E2 61 42                                 AC 43 5D 43                       00 00 80 BF            9D E2 61 42                               AC 43 5D 43              00 00 00 00 00 80 BF 00 00 00 00 FF FF 10 27 00 00 00 00 00 00 58 00 00 00 1E 00 00 00 D2 00 00 00 30 00 00 00 00 00 00 00 00 03 F3 03 00 00 00 03 00 00 00 00 10 27 00 00 35 FF FF FF FF 01 00 00 00 02 00 00 00 00 64 FF FF FF FF 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 55 AA

            //AA-55-BD-00- 21-01-       00-02-            F6-6F-03-00         -03-                       00-00-00-00-           05-                                72-65-61-6B-73-                                     01-        35-                      02-00-           00-00-00-00-                           00-           43-A9-00-00-                        43-4A-00-00-                   80-BF-9D-E2-43-A9-00-00-43-4A-00-00-00-00-00-00-00-80-BF-00-00-00-00-00-00-00-00-00-00-00-00-00-00-58-00-00-00-1E-00-00-00-D2-00-00-00-30-00-00-00-00-00-00-00-00-03-F3-03-00-00-00-03-00-00-00-00-10-27-00-00-35-FF-FF-FF-FF-01-00-00-00-02-00-00-00-00-64-FF-FF-FF-FF-05-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-01-00-00-00-00-00-00-00-00-00-00-00-02-00-00-00-00-00-00-00-00-00-00-00-09-00-00-00-00-00-00-00-00-00-00-00-0A-00-00-00-00-00-00-00-00-00-00-00-55-AA

            //     AA 55 BE 00 21 01         34 00            F6 6F 03 00              03                    01 00 00 00             05                                  61 62 63 64 73                                    01           35                     02 00                  14 0E 00 00                      00                         00 00 42 C8                                 00 00 42 C8                       00 00 80 BF            00 00 42 C8                               00 00 42 C8              00 00 00 00 00 80 BF 00 00 00 00 FF FF 10 27 00 00 00 00 00 00 58 00 00 00 1E 00 00 00 D2 00 00 00 30 00 00 00 00 00 00 00 00 03 F3 03 00 00 00 03 00 00 00 00 10 27 00 00 35 FF FF FF FF 01 00 00 00 02 00 00 00 00 64 FF FF FF FF 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 55 AA


            // pos 101,101  abcdr
            // AA 55 BE 00 21 01 63 03 F6 6F 03 00 04 01 00 00 00 05 61 62 63 64 72 01 00 00 00 35 02 00 00 20 1C 00 00 00 1F D2 CA 42 9C D0 C9 42 00 00 00 00 1F D2 CA 42 9C D0 C9 42 00 00 00 00 00 00 00 00 FF FF 10 27 00 00 00 00 00 00 79 00 00 00 1E 00 00 00 D2 00 00 00 30 00 00 00 14 05 00 00 00 03 F3 03 00 00 00 02 00 00 00 00 10 27 00 00 35 FF FF FF FF 01 00 00 00 02 00 00 00 00 64 FF FF FF FF 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 55 AA 

            // AA 55 BE 00 21 01       63 03              F6 6F 03 00           04                       01 00 00 00           05                             61 62 63 64 72                                           01                35                02 00           00 20 1C 00                             00 00                          1F D2 CA 42                      9C D0 C9 42                                        00 00 00 00                  1F D2 CA 42                                            9C D0 C9 42                             00 00 00 00 00 00 00 00 FF FF 10 27 00 00 00 00 00 00 79 00 00 00 1E 00 00 00 D2 00 00 00 30 00 00 00 14 05 00 00 00 03 F3 03 00 00 00 02 00 00 00 00 10 27 00 00 35 FF FF FF FF 01 00 00 00 02 00 00 00 00 64 FF FF FF FF 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 55 AA 
            // AA-55-BE-00-21-01-      00-01-             F6-6F-03-00-          03-                      00-00-00-00-          05-                            72-65-61-6B-74-                                          01-               35-               02-00-          00-00-00-00-                            00-00-                         00-00-CC-42-00-00-CA-42-00-00-00-00-00-00-CC-42-00-00-CA-42-00-00-00-00-00-80-BF-00-00-00-00-00-00-00-00-00-00-00-00-00-00-58-00-00-00-1E-00-00-00-D2-00-00-00-30-00-00-00-00-00-00-00-00-03-F3-03-00-00-00-03-00-00-00-00-10-27-00-00-35-FF-FF-FF-FF-01-00-00-00-02-00-00-00-00-64-FF-FF-FF-FF-05-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-01-00-00-00-00-00-00-00-00-00-00-00-02-00-00-00-00-00-00-00-00-00-00-00-09-00-00-00-00-00-00-00-00-00-00-00-0A-00-00-00-00-00-00-00-00-00-00-00-55-AA
            // AA 55 BE 00 21 01       34 00              F6 6F 03 00           03                       01 00 00 00           05                             61 62 63 64 72                                           01                35                02 00           14 0E 00 00                             00                     1F D2 CA 42                                            9C D0 C9 42                             00 00 00 00                 1F D2 CA 42                                        9C D0 C9 42                                  00 00 00 00 00 80 BF 00 00 00 00 FF FF 10 27 00 00 00 00 00 00 58 00 00 00 1E 00 00 00 D2 00 00 00 30 00 00 00 00 00 00 00 00 03 F3 03 00 00 00 03 00 00 00 00 10 27 00 00 35 FF FF FF FF 01 00 00 00 02 00 00 00 00 64 FF FF FF FF 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 55 AA


            string hex = ("21 01" + uid.ToString("X4") + "F6 6F 03 00 " + isActivated.ToString("X2") + " 00 00 00 00 " + nickname.Length.ToString("X2") + BitConverter.ToString(Encoding.ASCII.GetBytes(nickname)) + " 01 " + evolution.ToString("X2") + " 02 00 " + LongExtension.Reverse((ulong)battlePower) + " 00 " + BitConverter.ToString(BitConverter.GetBytes(posX)) + BitConverter.ToString(BitConverter.GetBytes(posY)) + " 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(posX)) + BitConverter.ToString(BitConverter.GetBytes(posY)) + " 00 00 00 00 00 80 BF 00 00 00 00 " + LongExtension.Reverse((ulong)honorPoint) + " 00 00 00 00 00 00 58 00 00 00 1E 00 00 00 D2 00 00 00 30 00 00 00 00 00 00 00 00 03 F3 03 00 00 00 03 00 00 00 00 10 27 00 00 35 FF FF FF FF 01 00 00 00 02 00 00 00 00 64 FF FF FF FF 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00");

            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            Debug.Log("POS X: " + BitConverter.ToString(BitConverter.GetBytes(posX)) + ", POS Y:" + BitConverter.ToString(BitConverter.GetBytes(posY)), ConsoleColor.Yellow);

            byte[] x = Functions.FromHex(hexString);

            DataDebugger.S2CDebugData(x, this.GetType().Name);

            return x;
        }
    }

    public class STeleportMessage
    {
        public float posX, posY;

        // AA 55 09 00 24 64 00 00 00 64 00 00 00 55 AA
        public byte[] getValue(float px = -1, float py = -1)
        {
            byte[] x;

            if (px == -1 || py == -1)
            {
                x = Functions.FromHex("AA 55 09 00 24 " + BitConverter.ToString(BitConverter.GetBytes(posX)) + BitConverter.ToString(BitConverter.GetBytes(posY)) + " 55 AA");
            }
            else
            {
                x = Functions.FromHex("AA 55 09 00 24 " + BitConverter.ToString(BitConverter.GetBytes(px)) + BitConverter.ToString(BitConverter.GetBytes(py)) + " 55 AA");
            }
            
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }

    public class CCharacterOutfitInfoAckMessage
    {
        // AA 55 4F 00 59 05 0A 00 "2bytes uid" 03 06 "4bytes helmet id" 00 00 00 00 00 00 00 00 "4bytes mask" 01 00 00 00 00 00 00 00 "4bytes armor" 02 00 00 00 00 00 00 00 "4bytes weapon" 03 00 A1 00 00 00 00 "4bytes shoes" 09 00 00 00 00 00 00 00 "4bytes pet" 0A 00 00 00 00 00 00 00 55 AA 
        // 
        public ushort uid;
        public uint weaponId;
        public uint armorId;
        public uint shoesId;
        public uint helmetId;
        public uint maskId;
        public uint petId;

        public byte wepUpgradeLv;
        public byte wep2UpgradeLv;
        public byte helmetUpgradeLv;
        public byte shoesUpgradeLv;
        public byte armorUpgradeLv;
        public byte maskUpgradeLv;
        // unknown bug cause hide item views
        public byte[] getValue()
        {

            /*
            string hex = ("59 05 0A 00 " + uid.ToString("X4") + " 03 06 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(helmetId)) + " 00 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(maskId)) + "01 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(armorId)) + " 02 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(weaponId)) + " 03 00 A1 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(shoesId)) + " 09 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(petId)) + " 0A 00 00 00 00 00 00 00");
            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";
            byte[] x = Functions.FromHex(hexString);
            Debug.Log("Weapon ID: " + BitConverter.ToString(BitConverter.GetBytes(weaponId)), ConsoleColor.Cyan);
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            */

            // AA-55-4F-00-59-05-0A-00-00-02-03-06-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-01-00-00-00-00-00-00-00-00-00-00-00-02-00-00-00-00-00-00-00-43-AB-E6-05-03-00-A1-00-00-00-00-00-00-00-00-09-00-00-00-00-00-00-00-00-00-00-00-0A-00-00-00-00-00-00-00-55-AA
            // AA-55-4F-00-59-05-0A-00-00-02-03-06-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-01-00-00-00-00-00-00-00-00-00-00-00-02-00-00-00-00-00-00-00-D1-CA-9B-00-03-00-A1-00-00-00-00-00-00-00-00-09-00-00-00-00-00-00-00-00-00-00-00-0A-00-00-00-00-00-00-00-55-AA
            // AA-55-53-00-59-05-0A-00-00-02-03-06-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-01-00-00-00-00-00-00-00-00-00-00-00-02-00-00-00-00-00-00-00-00-00-00-00-4A-26-E8-05-03-00-A1-00-00-00-00-00-00-00-00-09-00-00-00-00-00-00-00-00-00-00-00-0A-00-00-00-00-00-00-00-55-AA
            // AA-55-53-00-59-05-0A-00-00-02-03-06-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-01-00-00-00-00-00-00-00-00-00-00-00-02-00-00-00-00-00-00-00-00-00-00-00-88-13-00-00-03-00-A1-00-00-00-00-00-00-00-00-09-00-00-00-00-00-00-00-00-00-00-00-0A-00-00-00-00-00-00-00-55-AA
            /* string hex = ("59 05 0A 00 " + uid.ToString("X4") + " 03 06 " + BitConverter.ToString(BitConverter.GetBytes(helmetId)) + " 00 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(maskId)) + "01 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(armorId)) + " 02 00 00 00 00 00 00 00 00 00 00 00" + BitConverter.ToString(BitConverter.GetBytes(weaponId)) + " 03 00 A1 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(shoesId)) + " 09 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(petId)) + " 0A 00 00 00 00 00 00 00");

             string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

             byte[] x = Functions.FromHex(hexString);
            */
            //                            AA-55-53-00-59-05-0A-00          00-02-            03-06-                          00-00-00-00-                         00-00             -A2-                                 05-00-00-00-00-00-00-00-00-01-00-A2-05-00-00-00-00-00-00-00-00-02-00-A2-05-00-00-00-00-00-D1-CA-9B-00-03-00-A2-05-00-00-00-00-22-4B-BC-00-09-00-A2-05-00-00-00-00-00-00-00-00-0A-00-00-00-00-00-00-00-55-AA

            //                            AA-55-53-00-59-05-0A-00               -00-02-      03-06-            00-00-00-00                                       -00-00-     00-       00-                            00-00-00-00-            00-00-00-00                                    -01-00-    00-           00-                      00-00-00-00-             00-00-00-00                                     -02-00-    00-          00-                       00-00-00-00-00-00-00-00-            D1-CA-9B-00-                                      03-00-    A2      -00-                         00 00-00-00-                   22-4B-BC-00-                               0c-00-    00-       00                          -00-00-00-00-            00-00-00-00-                                    0A-00-00-00-00-00-00-00-55-AA
            //                            AA-55-53-00-59-05-0A-00-               00-02-      03-06-            E1-3D-B9-00-                                       00-00-     A2-       05-                            00-00-00-00-            81-C4-BA-00-                                    01-00-    A2-      05-                           00-00-00-00-             C2-D1-BD-00-                                     02-00-    A2-          05-                       00-00-00-00-00-00-00-00            -D1-CA-9B-00-                                      03-00-    A2-      05-                         00-00-00-00- -XX XX-                   22-4B-BC-00-                               09-00-    A2-       05-                          00-00-00-00-            00-00-00-00-0A-00-00-00-00-00-00-00-55-AA

            //                            AA-55-53-00-59-05-0A-00-               00-02-      03-06-            E1-3D-B9-00-                                       00-00-     A2-       05-                            00-00-00-00-            81-C4-BA-00-                                    01-00-    A2-      05-                           00-00-00-00-             C2-D1-BD-00-                                     02-00-    A2-          05-                       00-00-00-00-00-00-00-00            -D1-CA-9B-00-                                      03-00-    A2-      05-                         00-00-00-00-                   22-4B-BC-00-                               09-00-    A2-       05-                          00-00-00-00-            00-00-00-00-                                    0A-00-00-00-00-00-00-00-55-AA
            // AA-55-53-00 59-05-0A-00-               00-02-      03-06-            E1-3D-B9-00-                                       00-00-     A2-       05-                            00-00-00-00-            81-C4-BA-00-                                    01-00-    A2-      05-                           00-00-00-00-             C2-D1-BD-00-                                     02-00-    A2-          05-                       00-00-00-00-00-00-00-00            -D1-CA-9B-00-                                      03-00-    A2-      05-                         00-00-                  22-4B-BC-00-                               09-00-    A2-       05-                          00-00-00-00-            00-00-00-00                                     -0A-00-00-00-00-00-00-00-55-AA

            // AA-55-52-00-59-05-0A-00                -00-02-     03-06-            E4-3D-B9-00                                        -00-00-    A2-      00-                             00-00-00-00-              84-C4-BA-00-                                   01-00-A2-         00-                           00-00-00-00-             C4-D1-BD-00-                                     02-00-    A2-            00-                    00-00-03-00-A2-00-00-00-23-4B-BC-00-09-00-A2-00-00-00-00-00-00-00-00-00-0A-00-00-00-00-00-00-00-55-AA
            // AA-55-52-00-59-05-0A-00-                00-02-     03-06-            E4-3D-B9-00-                                       00-00-     A2-          00-                                    84-C4-BA-00-                                    01-00-   A2-           00-                       00-00-00-00-        00-00-00-00-                                          02-00-    A2-            00                     -00-00-00-00-00-00-00-00-      55-E5-A1-00-03-00-A2-00-00-00-23-4B-BC-00-09-00-A2-00-00-00-00-00-00-00-00-00-0A-00-00-00-00-00-00-00-55-AA
            // AA-55-48-00-59-05-0A-00-                00-02-     03-06-            E4-3D-B9-00-                                       00-00-     A2-          00                                     84-C4-BA-00-                                    01-00-    A2-     00-                            00-00-00-00 00-00-00-00-           ARMOR4X-                                          02-00-     A2-    00-                    00-00-00-00-55-E5-A1-00-03-00-A2-00-23-4B-BC-00-09-00-A2-00-00-00-00-00-00-00-00-00-0A-00-00-00-00-00-00-00-55-AA
            string hex = ("59 05 0A 00 " + uid.ToString("X4") + " 03 06 " + BitConverter.ToString(BitConverter.GetBytes(helmetId)) + " 00 00 " + "A2" + helmetUpgradeLv.ToString("X2") + "" + BitConverter.ToString(BitConverter.GetBytes(maskId)) + "01 00 "+ "A2" + maskUpgradeLv.ToString("X2") + " 00 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(armorId)) + " 02 00 "+ "A2" + armorUpgradeLv.ToString("X2") +"" + BitConverter.ToString(BitConverter.GetBytes(weaponId)) + " 03 " + (weaponId == 0 ? "01" : "00") + " A2" + wepUpgradeLv.ToString("X2") + "" + BitConverter.ToString(BitConverter.GetBytes(shoesId)) + " 09 00 "+ "A2" + shoesUpgradeLv.ToString("X2") +" 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(petId)) + " 0A 00 00 00 00 00 00 00");
           
            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);

            Debug.Log("Weapon ID: " + BitConverter.ToString(BitConverter.GetBytes(weaponId)), ConsoleColor.Cyan);
            DataDebugger.S2CDebugData(x, this.GetType().Name);

            return x;
        }
    }


    public class SMeditationState
    {
        // AA 55 05 00 82 05 E6 01 01 55 AA 

        public byte trueorfalse;
        public ushort uid;

        public byte[] getValue()
        {
            byte[] x = Functions.FromHex("AA 55 05 00 82 05 " + uid.ToString("X4") + trueorfalse.ToString("X2") + " 55 AA ");

            DataDebugger.S2CDebugData(x, this.GetType().Name);

            return x;
        }
    }

    public class SHealthInfoAckMessage
    {
        // AA 55 28 00 16 "uid 2  byte" + "uid 2 byte" D2 00 00 00 1E 00 00 00 "1 byte attack?" 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 10 27 00 00 55 AA 

        public int health, mana;
        public ushort uid;
        public byte debuffId = 0;
        public byte[] getValue(bool isMob = false)
        {

            // AA-55-24-00-16-        00-02-              00-02-                            C9-FF-30-00-01-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-55-AA

            //AA 55 24 00 16          00 02               00 02                             94 FF 62 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 10 27 00 00 55 AA
            //AA-55-28-00-16-        00-02-                00-02-                       B5-00-00-00-                                    00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-55-AA

            if(!isMob)
                debuffId = 0; // 1 cause felch!

            if (health <= 0) health = 0;
            if (mana <= 0) mana = 0;

            // AA-55-24-00-16-         00-02-            00-00-                       9F-FC-30-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-55-AA
            string hex = ("16 " + uid.ToString("X4") + uid.ToString("X4") + Functions.ReverseBytes16((ushort)health).ToString("X4") + Functions.ReverseBytes16((ushort)mana).ToString("X4") + debuffId.ToString("X2") + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");

            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);

            // Console.WriteLine("HEALTH PACKET:\nDATA: " + BitConverter.ToString(x));
            // DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }

    public class SReadCombatArtBook
    {
        public byte skillSlot;
        public uint itemId; 

        public byte[] getValue()
        {
            //AA 55 07 00 81 01 00 A5 AA F5 00 55 AA 
            //AA 55 07 00 81 01 03 A5 AA F5 00 00 55 AA 
            //AA-55-07-00-81-01-01-A1-AA-F5-00-55-AA
            //AA 55 00 00 82 01 00 45 31 F7 00 00 55 AA
            string hex = ("81 01 " + skillSlot.ToString("X2") + BitConverter.ToString(BitConverter.GetBytes(itemId)) + "00");

            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);

            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }

    public class SReadPassiveArtBook
    {
        public uint itemId;
        public byte skillSlot;

        public byte[] getValue()
        {
            //45-31-F7-00 passive book
            // AA 55 00 00 82 01 03 45 31 F7 00 00 55 AA
            // AA 55 00 00 82 01 04 45 31 F7 00 00 55 AA
            // AA 55 00 00 82 01 05 45 31 F7 00 00 55 AA
            // AA 55 00 00 82 01 06 45 31 F7 00 00 55 AA
            // AA 55 00 00 82 01 07 45 31 F7 00 00 55 AA
            // AA 55 00 00 82 01 08 45 31 F7 00 00 55 AA
            // AA 55 00 00 82 01 09 45 31 F7 00 00 55 AA

            string hex = ("82 01" + skillSlot.ToString("X2") + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 00");

            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);

            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }

    public class SUpgradeSkillBook
    {
        public byte skillBookSlot;
        public byte skillBookLevel;

        public byte[] getValue()
        {
            // AA 55 0B 00 81 02 0A 00 02 05 00 00 00 00 01 55 AA

            string hex = ("81 02 0A 00 " + skillBookSlot.ToString("X2") + skillBookLevel.ToString("X2") + " 00 00 00 00 01");

            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);

            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }

    public class SUpgradePassiveSkillBook
    {
        public byte skillBookSlot;
        public byte skillBookLevel;

        public byte[] getValue()
        {
            // AA 55 0B 00 82 02 0A 00 00 02 00 00 00 00 01 55 AA

            string hex = ("82 02 0A 00 " + skillBookSlot.ToString("X2") + skillBookLevel.ToString("X2") + " 00 00 00 00 01");

            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);

            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }


    public class SAttackDefendModeAckMessage
    {
        // AA 55 04 00 43 79 01 01 55 AA  // SERVER 79 01 session id

        public byte attackMode;
        public ushort uid;
        public byte[] getValue()
        {
            byte[] x = Functions.FromHex("AA 55 04 00 43 " + uid.ToString("X4") + attackMode.ToString("X2") + " 55 AA ");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }
   
    public class SIngameCharacterInfoAckMessage
    {

        // "AA 55 B9 00 17 B4 00 23 0D 1F 21 0D 08 4C 00 AC 99 01 00 " + len(nickname) + nickname + class id (34 monk, 35 savasci erkek) + " 02 00 01 " + pos+ pos + pos " 05 01 00 00 00 10 27 20 04 20 00 00 0C A0 01 00 06 20 01 E0 03 00 00 04 E0 03 0C 60 00 00 64 60 05 06 00 00 57 0F 00 00 D2 20 07 04 8E 03 00 00 1E 20 07 00 30 20 03 60 00 01 03 01 20 00 60 09 60 00 00 "order-chaos" E0 0A 43 E0 35 00 0A 02 00 B3 07 0B 01 00 A1 01 00 0B E0 1A 48 02 C0 75 06 60 2B 00 0C E0 1A 2B E0 FF 00 E0 FF 00 E0 FF 00 E0 FF 00 E0 CF 00 25 C2 E0 FF 00 E0 26 00 E1 FF 39 E0 76 00 E1 FF 86 E0 FF 00 E0 FF 00 E0 FF 00 E0 0D 00 01 00 00 55 AA"

        // "AA 55 B9 00 17 (packetlen-5) 00 ( DE 62 )   (5A - msg.len)

        public string nickname;
        public byte evolution;
        public uint currentMap;
        public byte race;
        public ushort uid;
        public float posX, posY;
        public ushort str, dex, intel, wind, water, fire, atkspeed;

        public byte[] getValue()
        {
            // WEAPON BUG
            // teleport bug
            byte[] x = new byte[0];
            ushort len = Convert.ToUInt16(210 + nickname.Length);
            int ilk = nickname.Length - 4;
            string honor = "00 00"; // 0B efendi
            string faction = "01"; // 01 zhu 02 shao
            string weaponStartSlot = "A7"; // A7 = 1, A8 = 2


            Debug.Log("[SIngameCharacterInfoAckMessage] Current Map:" + currentMap, ConsoleColor.Cyan);

            // x = Functions.FromHex("AA 55 " + Functions.ReverseBytes16(len).ToString("X4") + " 17 " + Functions.ReverseBytes16(Convert.ToUInt16(len - 5)).ToString("X4") + Convert.ToByte(0x79 + ilk).ToString("X2") + " 0D " + ((nickname.Length < 10) ? Convert.ToByte(0x1A + ilk).ToString("X2") : (0x1F).ToString("X2")) + Convert.ToByte(0x77 + ilk).ToString("X2") + " 0D 08 " + uid.ToString("X4") + " 65 2F E7 00 " + nickname.Length.ToString("X2") + BitConverter.ToString(Encoding.ASCII.GetBytes(nickname)) + evolution.ToString("X2") + " 02 00 " + level.ToString("X2") + " " + BitConverter.ToString(BitConverter.GetBytes(posX)) + BitConverter.ToString(BitConverter.GetBytes(posY)) + " 00 20 00 20 0C E0 00 00 00 0C A0 01 00 06 20 01 E0 03 00 00 04 E0 03 0C 60 00 00 64 60 05 06 00 00 10 0E 00 00 48 20 07 00 D2 20 03 00 1E 20 03 00 30 20 03 60 00 01 03 01 20 00 60 09 60 00 00 " + race.ToString("X2") + " E0 0A 43 E0 35 00 0A 04 00 00 00 00 00 00 A1 01 00 0B E0 1A 48 03 00 00 00 00 40 2B 00 0C E0 1A 2B 00 00 A0 2B 00 0D E0 1A 2B 02 00 00 00 60 83 21 06 E0 FF 00 E0 FF 00 E0 FF 00 E0 FF 00 E0 F0 00 00 03 E0 F0 F9 E0 37 00 E1 FF 39 E0 6A 00 00 02 20 73 E1 0E 7E 02 03 00 03 E0 0E 19 E0 18 00 01 05 05 E0 18 22 E0 FF 00 E0 FF 00 E0 FF 00 E0 A8 00 01 00 00 55 AA");
            //                     AA-55              -D8-00                                 -17-                D3-00-                                                          7B-                                    0D-                      1C-                                                                                      79-                               0D-08-          00-02-           65-2F-E7-        00-06-                                72-65-61-72-65-61-                                         35-                 02-00-        05-                                00-00-C8-42-00-00-C8-42-00-20-00-20-0C-E0-00-00-00-01-A0-03-00-06-20-03-E0-03-00-00-04-E0-03-0C-60-00-00-64-60-05-06-00-01-10-0E-00-00-48-20-07-00-D2-20-03-00-1E-20-03-00-30-20-03-60-00-01-03-01-20-00-60-09-60-00-00-01-E0-0A-43-E0-35-00-0A-04-00-00-00-00-00-00-A1-01-00-0B-E0-1A-48-03-00-00-00-00-40-2B-00-0C-E0-1A-2B-00-00-A0-2B-00-0D-E0-1A-2B-02-00-00-00-60-83-21-06-E0-FF-00-E0-FF-00-E0-FF-00-E0-FF-00-E0-F0-00-00-03-E0-F0-F9-E0-37-00-E1-FF-39-E0-6A-00-00-02-20-73-E1-0E-7E-02-03-00-03-E0-0E-19-E0-18-00-01-05-05-E0-18-22-E0-FF-00-E0-FF-00-E0-FF-00-E0-A8-00-01-00-00-55-AA
            x = Functions.FromHex("AA 55 " + Functions.ReverseBytes16(len).ToString("X4") + " 17 " + Functions.ReverseBytes16(Convert.ToUInt16(len - 5)).ToString("X4") + Convert.ToByte(0x79 + ilk).ToString("X2") + " 0D " + ((nickname.Length < 10) ? Convert.ToByte(0x1A + ilk).ToString("X2") : (0x1F).ToString("X2")) + Convert.ToByte(0x77 + ilk).ToString("X2") + " 0D 08 " + uid.ToString("X4") + " 65 2F E7 " + nickname.Length.ToString("X4") + BitConverter.ToString(Encoding.ASCII.GetBytes(nickname)) + evolution.ToString("X2") + " 02 00 " + currentMap.ToString("X2") + " " + BitConverter.ToString(BitConverter.GetBytes(posX)) + BitConverter.ToString(BitConverter.GetBytes(posY)) + " 00 20 00 20 0C E0 " + honor + "00 01" + " A0 01 " + " 00 06 20 01 E0 03 00 00 04 E0 03 0C 60 00 00 64 60 05 06 00 " + faction + " 10 0E 00 00 48 20 07 00 D2 20 03 00 1E 20 03 00 30 20 03 60 00 01 03 01 20 00 60 09 60 00 00 " + race.ToString("X2") + " E0 0A 43 E0 35 00 0A 04 00 00 00 00 00 00 A1 01 00 0B E0 1A 48 03 00 00 00 00 40 2B 00 0C E0 1A 2B 00 00 A0 2B 00 0D E0 1A 2B 02 00 00 00 60 83 21 06 E0 FF 00 E0 FF 00 E0 FF 00 E0 FF 00 E0 F0 00 00 03 E0 F0 F9 E0 37 00 E1 FF 39 E0 6A 00 00 02 20 73 E1 0E 7E 02 03 00 03 E0 0E 19 E0 18 00 01 05 05 E0 18 22 E0 FF 00 E0 FF 00 E0 FF 00 E0 A8 00 01 00 00 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }

    public class SMobInfo
    {


        //AA-55-4F-00-31-01-6B-78-A5-9C-00-00-01-FF-FF-FF-FF-14-41-73-73-61-00-01-00-00-00-00-00-00-00-00-01-00-00-00-00-00-00-01-00-00-00-01-00-00-00-01-00-00-00-00-00-00-01-01-00-00-86-42-00-00-51-43-00-00-00-00-00-00-86-42-00-00-51-43-00-00-00-00-00-00-00-00-FF-FF-00-28-0A-00-00-00-00-00-64-00-55-AA

        //                  SESS  A59C 00 00: WOLF PUP                                            
        public ushort session_id;
        public ushort skin;
        public byte level;
        public uint curHp;
        public uint maxHp;
        public float x1 = 0, y1 = 0, z1 = 0, x2 = 0, y2 = 0, z2 = 0;
        public byte scale;

        public byte[] getValue()
        {

            // AA-55-5C-00-31-01-        6B-78-                                A5-9C-                                   00-00-            04-             FF-FF-FF-FF-14-41-73-73-61-     4D-01-00-00-   00-00-00-00-00-      2B-02-00-00-     00-00-00-AA-AA-AA-AA-                                                00-01-00-00-AA-AA-AA-AA-                                           00-00-00-00-01-01-00-00-82-42-00-00-59-43-00-00-00-00-00-00-82-42-00-00-59-43-00-00-00-00-00-00-00-00-FF-FF-00-00-01-00-00-00-00-00-C8-00-55-AA
            // AA-55-5C-00-31-01-40-D7-A6-9C-00-00-02-FF-FF-FF-FF-14-41-73-73-61-73-73-69-2E-00-00-00-00-00-2E-00-00-00-00-00-00-59-00-00-00-00-01-00-00-59-00-00-00-00-00-00-00-01-01-00-80-A2-43-00-00-73-43-00-00-00-00-00-80-A2-43-00-00-73-43-00-00-00-00-00-00-00-00-FF-FF-00-00-00-00-00-00-00-00-64-00-55-AA
            string hex = ("31 01 " + session_id.ToString("X4") + BitConverter.ToString(BitConverter.GetBytes(skin)) + " 00 00 " + level.ToString("X2") + "FF FF FF FF 14 41 73 73 61 " + "73 73 69 2E" + "00 00 00 00 00 " + "2E 00 00 00" + " 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(curHp)) + " 00 01 00 00 " + BitConverter.ToString(BitConverter.GetBytes(maxHp)) + " 00 00 00 00 01 01" + BitConverter.ToString(BitConverter.GetBytes(x1)) + BitConverter.ToString(BitConverter.GetBytes(y1)) + BitConverter.ToString(BitConverter.GetBytes(z1)) + BitConverter.ToString(BitConverter.GetBytes(x2)) + BitConverter.ToString(BitConverter.GetBytes(y2)) + BitConverter.ToString(BitConverter.GetBytes(z2)) + " 00 00 00 00 FF FF 00 00 00 00 00 00 00 00 " + scale.ToString("X2") + " 00");
            //string hex = ("31 01 " + session_id.ToString("X4") + BitConverter.ToString(BitConverter.GetBytes(skin)) + " 00 00 " + level.ToString("X2") + "FF FF FF FF 14 41 73 73 61 " + "73 73 69 2E" + "00 00 00 00 00 " + "2E 00 00 00" + " 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(curHp)) + " 00 01 00 00 " + BitConverter.ToString(BitConverter.GetBytes(maxHp)) + " 00 00 00 00 01 01" + BitConverter.ToString(BitConverter.GetBytes(x1)) + BitConverter.ToString(BitConverter.GetBytes(y1)) + BitConverter.ToString(BitConverter.GetBytes(z1)) + BitConverter.ToString(BitConverter.GetBytes(x2)) + BitConverter.ToString(BitConverter.GetBytes(y2)) + BitConverter.ToString(BitConverter.GetBytes(z2)) + " 00 00 00 00 FF FF 00 00 00 00 00 00 00 00 " + scale.ToString("X2") + " 00");

            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            //Debug.Log("hexString: " + hexString, ConsoleColor.Red);

            byte[] x = Functions.FromHex(hexString);

           // Debug.Log("curhp data: " + BitConverter.ToString(BitConverter.GetBytes(curHp)) + ", maxhp data: " + BitConverter.ToString(BitConverter.GetBytes(maxHp)));
            //00-01-00-00
            //DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }

    }

    public class SPetInfo
    {


        //AA-55-4F-00-31-01-6B-78-A5-9C-00-00-01-FF-FF-FF-FF-14-41-73-73-61-00-01-00-00-00-00-00-00-00-00-01-00-00-00-00-00-00-01-00-00-00-01-00-00-00-01-00-00-00-00-00-00-01-01-00-00-86-42-00-00-51-43-00-00-00-00-00-00-86-42-00-00-51-43-00-00-00-00-00-00-00-00-FF-FF-00-28-0A-00-00-00-00-00-64-00-55-AA

        //                  SESS  A59C 00 00: WOLF PUP                                            
        public ushort session_id;
        public ushort skin;
        public byte level;
        public uint curHp;
        public uint maxHp;
        public float x1 = 0, y1 = 0, z1 = 0, x2 = 0, y2 = 0, z2 = 0;
        public byte scale;

        public byte[] getValue()
        {

            // AA-55-5C-00-31-01-        6B-78-                                A5-9C-                                   00-00-            04-             FF-FF-FF-FF-14-41-73-73-61-     4D-01-00-00-   00-00-00-00-00-      2B-02-00-00-     00-00-00-AA-AA-AA-AA-                                                00-01-00-00-AA-AA-AA-AA-                                           00-00-00-00-01-01-00-00-82-42-00-00-59-43-00-00-00-00-00-00-82-42-00-00-59-43-00-00-00-00-00-00-00-00-FF-FF-00-00-01-00-00-00-00-00-C8-00-55-AA
            // AA-55-5C-00-31-01-          40-D8-                               A6-9C-                                  00-00-       02                  -FF-FF-FF-FF-01-00-73-73-61-73-73-69-2E-00-00-00-00-00-2E-00-00-00-00-00-00-59-00-00-00-00-01-00-00-59-00-00-00-00-00-00-00-01-01-00-80-A2-43-00-00-73-43-00-00-00-00-00-80-A2-43-00-00-73-43-00-00-00-00-00-00-00-00-FF-FF-00-00-00-00-00-00-00-00-64-00-55-AA
  //kopek npc AA 55 5F 00 31 01        33 4E                       A5 9C                                               00 00         01                   00 00 00 00 14 41 73 73 61      73 73 69 6E     20 54 72 61 69      6E 65 72 20      42 69 6E 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 01 00 00 6C 42 00 00 5F 43 00 00 40 41 00 00 6C 42 00 00 5F 43 00 00 40 41 00 00 00 00 FF FF 00 00 00 00 00 00 00 00 64 00 55 AA
            string hex = ("31 01 " + session_id.ToString("X4") + BitConverter.ToString(BitConverter.GetBytes(skin)) + " 00 00 " + level.ToString("X2") + "01 00 00 00 14 41 73 73 61 " + "73 73 69 2E" + "00 00 00 00 00 " + "2E 00 00 00" + " 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(curHp)) + " 00 01 00 00 " + BitConverter.ToString(BitConverter.GetBytes(maxHp)) + " 00 00 00 00 01 01" + BitConverter.ToString(BitConverter.GetBytes(x1)) + BitConverter.ToString(BitConverter.GetBytes(y1)) + BitConverter.ToString(BitConverter.GetBytes(z1)) + BitConverter.ToString(BitConverter.GetBytes(x2)) + BitConverter.ToString(BitConverter.GetBytes(y2)) + BitConverter.ToString(BitConverter.GetBytes(z2)) + " 00 00 00 00 FF FF 00 E8 03 00 00 00 00 00 " + scale.ToString("X2") + " 00");

            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            //Debug.Log("hexString: " + hexString, ConsoleColor.Red);

            byte[] x = Functions.FromHex(hexString);

            // Debug.Log("curhp data: " + BitConverter.ToString(BitConverter.GetBytes(curHp)) + ", maxhp data: " + BitConverter.ToString(BitConverter.GetBytes(maxHp)));
            //00-01-00-00
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }

    }

    public class SGetPlayerStats
    {
        // AA 55 E4 00 14 09 00 00 00 05 00 00 00 "statpointhak 2 byte" "naturepointhak 2 byte" 00 00 00 00 00 00 "8 byte exp" 20 1C 00 00 00 "8 byte max exp" " 2 byte str + 2 byte str " "2 byte dex + 2 byte dex" "2 byte int + 2 byte int" "2 byte wind + 2 byte wind" "2 byte water + 2 byte water" "2 byte fire + 2 byte fire" 00 66 66 C6 40 66 66 C6 40 "attack speed 2 byte" 00 00 00 40 "4 byte max health" "4 byte max mana"  "attack 4 byte" "0000 + 4byte def * 3 + 0000"  "8 byte arts atk" "4 byte arts def" "2 byte accuracy" "2 byte dodge" 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 19 00 00 00 00 00 00 00 00 FF FF 00 30 30 31 2D 30 31 2D 30 31 20 30 30 3A 30 30 3A 30 30 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 64 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 80 3F 00 00 80 3F "2 byte critical" 55 AA

        public byte level;
        public ushort naturepoints;
        public float curexp, maxexp; // ulong?
        public ushort str, dex, intel, wind, water, fire, atkspeed;
        //public ushort exStr, exDex, exIntel, exWind, exWater, exFire;
        public uint maxhp, maxmana;
        public ushort attackpoint;
        public uint def;
        public uint artsatk;
        public uint artsdef;
        //public uint exArtsatk, exArtsdef;
        public ushort acc, dodge, critical;
        //public ushort exAcc, exDodge, exCritical;
        public byte race;
        public ushort statpoints;

        public ulong gold;
        public ushort skillPoints = 0; //need db edit
        public ushort duelState = 500;
        public float runSpeed = 5.5f; //DD-40 OK // BitConverter.ToString(Functions.GetMidBigEndianFloat(BitConverter.GetBytes(runSpeed)) // 54. yere


        // STATS - TIK TIK IŞINLANMAYI FIXLEYEN YER BURASI
        public byte[] getValue()
        {
            // string hex = ("14 " + level.ToString("X2") + " 00 00 00 05 00 00 00 " + Functions.ReverseBytes16(statpoints).ToString("X4") + Functions.ReverseBytes16(naturepoints).ToString("X4") + " 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(curexp))  + BitConverter.ToString(BitConverter.GetBytes(curexp)) + " 20 1C 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(maxexp)) + BitConverter.ToString(BitConverter.GetBytes(maxexp)) + Functions.ReverseBytes16(str).ToString("X4") + Functions.ReverseBytes16(str).ToString("X4") + Functions.ReverseBytes16(dex).ToString("X4") + Functions.ReverseBytes16(dex).ToString("X4") + Functions.ReverseBytes16(intel).ToString("X4") + Functions.ReverseBytes16(intel).ToString("X4") + Functions.ReverseBytes16(wind).ToString("X4") + Functions.ReverseBytes16(wind).ToString("X4") + Functions.ReverseBytes16(water).ToString("X4") + Functions.ReverseBytes16(water).ToString("X4") + Functions.ReverseBytes16(fire).ToString("X4") + Functions.ReverseBytes16(fire).ToString("X4") + " 00 66 66 C6 40 66 66 C6 40 " + Functions.ReverseBytes16(atkspeed).ToString("X4") + " 00 00 00 40 " + Functions.ReverseBytes32(maxhp).ToString("X8") + Functions.ReverseBytes32(maxmana).ToString("X8") + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint - (attackpoint / 3)))) + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint + (attackpoint / 3)))) + " 00 00 00 00 " + Functions.ReverseBytes32(def * 3).ToString("X8") + " 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes((artsatk - (artsatk / 5)))) + BitConverter.ToString(BitConverter.GetBytes((artsatk + (artsatk / 5)))) + Functions.ReverseBytes32(artsdef).ToString("X8") + Functions.ReverseBytes16(acc).ToString("X4") + Functions.ReverseBytes16(dodge).ToString("X4") + " 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 19 00 00 00 00 00 00 00 00 FF FF 00 30 30 31 2D 30 31 2D 30 31 20 30 30 3A 30 30 3A 30 30 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 64 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 80 3F 00 00 80 3F " + Functions.ReverseBytes16(critical).ToString("X4"));
            // string hex = (" 14 " + level.ToString("X2") + " " + Functions.ReverseBytes16(statpoints).ToString("X4") + Functions.ReverseBytes16(naturepoints).ToString("X4") + " 00 00 00 00" + BitConverter.ToString(BitConverter.GetBytes(curexp)) + BitConverter.ToString(BitConverter.GetBytes(curexp)) + " 20 1C 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(maxexp)) + BitConverter.ToString(BitConverter.GetBytes(maxexp)) + Functions.ReverseBytes16(str).ToString("X4") + Functions.ReverseBytes16(str).ToString("X4") + Functions.ReverseBytes16(dex).ToString("X4") + Functions.ReverseBytes16(dex).ToString("X4") + Functions.ReverseBytes16(intel).ToString("X4") + Functions.ReverseBytes16(intel).ToString("X4") + Functions.ReverseBytes16(wind).ToString("X4") + Functions.ReverseBytes16(wind).ToString("X4") + Functions.ReverseBytes16(water).ToString("X4") + Functions.ReverseBytes16(water).ToString("X4") + Functions.ReverseBytes16(fire).ToString("X4") + Functions.ReverseBytes16(fire).ToString("X4") + " 00 " + Functions.ReverseBytes16(atkspeed).ToString("X4") + " 00 00 00 40 " + Functions.ReverseBytes32(maxhp).ToString("X8") + Functions.ReverseBytes32(maxmana).ToString("X8") + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint - (attackpoint / 3)))) + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint + (attackpoint / 3)))) + " 00 00 00 00 " + Functions.ReverseBytes32(def * 3).ToString("X8") + " 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes((artsatk - (artsatk / 5)))) + BitConverter.ToString(BitConverter.GetBytes((artsatk + (artsatk / 5)))) + Functions.ReverseBytes32(artsdef).ToString("X8") + Functions.ReverseBytes16(acc).ToString("X4") + Functions.ReverseBytes16(dodge).ToString("X4") + " 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 19 00 00 00 00 00 00 00 00 FF FF 00 30 30 31 2D 30 31 2D 30 31 20 30 30 3A 30 30 3A 30 30 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 64 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 80 3F 00 00 80 3F " + Functions.ReverseBytes16(critical).ToString("X4"));
            // string hex = (" 14 " + level.ToString("X2") + " 00 00 00 05 00 00 " + Functions.ReverseBytes16(statpoints).ToString("X4") + Functions.ReverseBytes16(naturepoints).ToString("X4") + " 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(curexp)) + BitConverter.ToString(BitConverter.GetBytes(curexp)) + " 20 1C 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(maxexp)) + BitConverter.ToString(BitConverter.GetBytes(maxexp)) + Functions.ReverseBytes16(str).ToString("X4") + Functions.ReverseBytes16(str).ToString("X4") + Functions.ReverseBytes16(dex).ToString("X4") + Functions.ReverseBytes16(dex).ToString("X4") + Functions.ReverseBytes16(intel).ToString("X4") + Functions.ReverseBytes16(intel).ToString("X4") + Functions.ReverseBytes16(wind).ToString("X4") + Functions.ReverseBytes16(wind).ToString("X4") + Functions.ReverseBytes16(water).ToString("X4") + Functions.ReverseBytes16(water).ToString("X4") + Functions.ReverseBytes16(fire).ToString("X4") + Functions.ReverseBytes16(fire).ToString("X4") + " 00 66 66 C6 40 66 66 C6 40 " + Functions.ReverseBytes16(atkspeed).ToString("X4") + " 00 00 00 40 " + Functions.ReverseBytes32(maxhp).ToString("X8") + Functions.ReverseBytes32(maxmana).ToString("X8") + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint - (attackpoint / 3)))) + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint + (attackpoint / 3)))) + " 00 00 00 00 " + Functions.ReverseBytes32(def * 3).ToString("X8") + " 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes((artsatk - (artsatk / 5)))) + BitConverter.ToString(BitConverter.GetBytes((artsatk + (artsatk / 5)))) + Functions.ReverseBytes32(artsdef).ToString("X8") + Functions.ReverseBytes16(acc).ToString("X4") + Functions.ReverseBytes16(dodge).ToString("X4") + " 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 19 00 00 00 00 00 00 00 00 FF FF 00 30 30 31 2D 30 31 2D 30 31 20 30 30 3A 30 30 3A 30 30 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 64 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 80 3F 00 00 80 3F " + Functions.ReverseBytes16(critical).ToString("X4"));
            //Functions.ReverseBytes64((ulong)maxexp).ToString("#8")
            // DD-40 fixed data         AA-55-CD-00-14-05-00-F4-01-00-04-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-64-0C-00-0C-00-06-00-00-00-00-00-00-00-00-00-A0-DD-40-00-00-00-00-06-00-00-00-00-A0-D2-00-30-00-08-00-10-00-00-00-24-00-00-00-00-00-00-00-0A-00-00-00-0E-00-00-00-18-00-00-00-0B-00-0C-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-19-00-00-00-00-00-00-00-00-FF-FF-00-30-30-31-2D-30-31-2D-30-31-20-30-30-3A-30-30-3A-30-30-20-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-03-00-00-00-00-64-00-00-00-00-00-00-00-00-00-64-00-00-00-00-00-80-3F-00-00-80-3F-00-00-55-AA

            // Debug.Log("----------START HEX------------", ConsoleColor.Red);
            //             14-              02-00-                                                   F4-01-                                00-                    04-00-                                 00-00-                    00-00-                                   00-00-00-00-00-00-00-00-00-0A-00-00-00-00-00-00-00-00-00-00-00-00-64-0C-00-0C-00-06-00-00-00-00-00-00-00-00-00-A0-B0-40-00-00-00-00-00-00-00-00-00-00-D2-00-30-00-08-00-10-00-00-00-24-00-00-00-00-00-00-00-0A-00-00-00-0E-00-00-00-18-00-00-00-0B-00-0C-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-19-00-00-00-00-00-00-00-00-FF-FF-00-30-30-31-2D-30-31-2D-30-31-20-30-30-3A-30-30-3A-30-30-20-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-03-00-00-00-00-64-00-00-00-00-00-00-00-00-00-64-00-00-00-00-00-80-3F-00-00-80-3F-00-00-55-AA

            // AA-55-CD-00- 14-               02-00-                                              F4-01-                                   00-               08-00-                                      00-00-                               00-00-                        00-00-00-00-00-00-           41-A0-00-00-                      00-00-00-00-00-00-00-00-00-      42-C8-00-00-                                  0C-00-                                          0C-00-                                             06-00-                                            00-00-                                       00-00-                                              00-00-                                    00-00-A0-              B0-40-                                                                          00-00-       00-00-      00-00-00-00-00-00-40-16-20-00-80-01-00-00-00-02-40-00-00-00-00-00-00-00-A0-00-00-00-E0-00-00-01-80-00-00-00-B0-00-C0-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-01-90-00-00-00-00-00-00-00-0F-FF-F0-03-03-03-12-D3-03-12-D3-03-12-03-03-03-A3-03-03-A3-03-02-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-30-00-00-00-06-40-00-00-00-00-00-00-00-00-06-40-00-00-00-00-08-03-F0-00-08-03-F0-00-05-5A

            // AA 55 E4 00  14             0A 00                                             00 00                                         01        00 00 00 05 00                                      00 00 04 00 00 00 00 00 39 B2                                      00 00 00 00 00 00      BB 0C 00 00 00 1C 06 01                 00 00 00 00 00 24 00 42 00 0C 00 2A                                                       00 12 00 30 00 00 00 00 00 00 00 00 00 00 00 00 00 00 66 66 C6 40 66 66 C6 40 F3 03 00 00 30 41 75 03 00 00 DB 00 00 00 76 00 99 00 32 00 00 00 32 00 00 00 32 00 00 00 B6 00 00 00 EF 00 00 00 8F 00 00 00 3E 00 2A 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 32 00 00 00 8F 00 00 00 00 00 00 00 00 FF FF 00 30 30 31 2D 30 31 2D 30 31 20 30 30 3A 30 30 3A 30 30 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 64 00 04 00 00 00 08 00 00 00 64 00 00 00 00 00 80 3F 00 00 80 3F 05 00 55 AA

            // AA-55-CD-00- 14-            1E-00-                                            F4-01-                                        00                07-00-                                     05-05-                                                             00-00-                                        00-00-00-00-0A-0A-0A-0A-0A-14-00-00-00-00-00-00-00-00-00-00-00-00-64-0D-00-0C-00-24-00-00-00-00-00-00-00-00-00-A0-B0-40-00-00-00-00-00-00-00-00-00-00-0E-01-BC-00-06-01-0A-02-00-00-24-00-00-00-00-00-00-00-24-00-00-00-34-00-00-00-54-00-00-00-D4-00-0C-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-19-00-00-00-00-00-00-00-00-FF-FF-00-30-30-31-2D-30-31-2D-30-31-20-30-30-3A-30-30-3A-30-30-20-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-03-00-00-00-00-64-00-00-00-00-00-00-00-00-00-64-00-00-00-00-00-80-3F-00-00-80-3F-00-00-55-AA
            string hex = (" 14 " + Functions.ReverseBytes16(level).ToString("X4") + Functions.ReverseBytes16(duelState).ToString("X4") + " 00 " + Functions.ReverseBytes16(statpoints).ToString("X4") + Functions.ReverseBytes16(skillPoints).ToString("X4") + Functions.ReverseBytes16(naturepoints).ToString("X4") + " 00 00 00 00 00 00 " + LongExtension.Reverse((ulong)curexp) + " 00 00 00 00 00 00 00 00 00  " + LongExtension.Reverse((ulong)maxexp) + " " + Functions.ReverseBytes16(str).ToString("X4") + Functions.ReverseBytes16(dex).ToString("X4") + Functions.ReverseBytes16(intel).ToString("X4") + Functions.ReverseBytes16(wind).ToString("X4") + Functions.ReverseBytes16(water).ToString("X4") + Functions.ReverseBytes16(fire).ToString("X4") + " 00 00 00 " + BitConverter.ToString(Functions.GetMidBigEndianFloat(BitConverter.GetBytes(runSpeed))) + BitConverter.ToString(Functions.GetMidBigEndianFloat(BitConverter.GetBytes(runSpeed * 2))) + " " + " 00 00 00 00  " + Functions.ReverseBytes16((ushort)maxhp).ToString("X4") + Functions.ReverseBytes16((ushort)maxmana).ToString("X4") + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint - (attackpoint / 3)))) + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint + (attackpoint / 3)))) + " 00 00 " + Functions.ReverseBytes32(def * 3).ToString("X8") + " 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes((artsatk - (artsatk / 5)))) + BitConverter.ToString(BitConverter.GetBytes((artsatk + (artsatk / 5)))) + Functions.ReverseBytes32(artsdef).ToString("X8") + Functions.ReverseBytes16(acc).ToString("X4") + Functions.ReverseBytes16(dodge).ToString("X4") + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 19 00 00 00 00 00 00 00 00 FF FF 00 30 30 31 2D 30 31 2D 30 31 20 30 30 3A 30 30 3A 30 30 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 64 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 80 3F 00 00 80 3F " + Functions.ReverseBytes16(critical).ToString("X4"));
            
            // Debug.Log("hex: " + hex.Replace("-", ""), ConsoleColor.Cyan);

            // Debug.Log("RUN SPEED: " + BitConverter.ToString(Functions.GetMidBigEndianFloat(BitConverter.GetBytes(runSpeed))), ConsoleColor.Yellow);
            //Debug.Log("14-05-00-F4-01-00-04-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-64-0C-00-0C-00-06-00-00-64-0C-00-00-00-00-00-00-DD-40-00-00-00-00-06-00-00-00-00-A0-D2-00-30-00-08-BA-10-BA-BA-BA-24-BA-BA-BA-BA-BA-BA-BA-0A-BA-BA-BA-0E-BA-BA-BA-18-BA-BA-BA-0B-BA-0C-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-19-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-00-30-30-31-2D-30-31-2D-30-31-20-30-30-3A-30-30-3A-30-30-20-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-BA-03-BA-BA-BA-BA-64-BA-BA-BA-BA-BA-BA-BA-BA-BA-64-BA-BA-BA-BA-BA-80-3F-BA-BA-80-3F-BA-BA", ConsoleColor.Yellow);

            //string hex = (" 14 " + Functions.ReverseBytes16(level).ToString("X4") + " " + Functions.ReverseBytes16(duelState).ToString("X4") + " 00 " + Functions.ReverseBytes16(statpoints).ToString("X4") + "00 00" + " 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(curexp)) + BitConverter.ToString(BitConverter.GetBytes(curexp)) + " 20 1C 00 00 00 " + " 00 00 00 00 00 00 00 00 " + " 05 05 05 05 05 05 05 05 " + "00 05" + "00 05" + "00 05" + "00 05" + "00 05" + "00 05" + "00 05" + "00 05" + "00 05" + "00 05" + "00 05" + "00 05" + "55 AA"); //+ " 00 66 66 C6 40 66 66 C6 40 " + Functions.ReverseBytes16(atkspeed).ToString("X4") + " 00 00 00 40 " + Functions.ReverseBytes32(maxhp).ToString("X8") + Functions.ReverseBytes32(maxmana).ToString("X8") + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint - (attackpoint / 3)))) + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint + (attackpoint / 3)))) + " 00 00 00 00 " + Functions.ReverseBytes32(def * 3).ToString("X8") + " 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes((artsatk - (artsatk / 5)))) + BitConverter.ToString(BitConverter.GetBytes((artsatk + (artsatk / 5)))) + Functions.ReverseBytes32(artsdef).ToString("X8") + Functions.ReverseBytes16(acc).ToString("X4") + Functions.ReverseBytes16(dodge).ToString("X4") + " 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 19 00 00 00 00 00 00 00 00 FF FF 00 30 30 31 2D 30 31 2D 30 31 20 30 30 3A 30 30 3A 30 30 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 64 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 80 3F 00 00 80 3F " + Functions.ReverseBytes16(critical).ToString("X4") + " 55 AA");
            //AA-55-CD-00-14-05-00-F4-01-00-04-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-64-0C-00-0C-00-06-00-00-00-00-00-00-00-00-00-A0-00-00-00-00-00-00-06-00-00-00-00-A0-D2-00-30-00-08-00-10-00-00-00-24-00-00-00-00-00-00-00-0A-00-00-00-0E-00-00-00-18-00-00-00-0B-00-0C-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-19-00-00-00-00-00-00-00-00-FF-FF-00-30-30-31-2D-30-31-2D-30-31-20-30-30-3A-30-30-3A-30-30-20-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-03-00-00-00-00-64-00-00-00-00-00-00-00-00-00-64-00-00-00-00-00-80-3F-00-00-80-3F-00-00-55-AA
            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";
            
            byte[] x = Functions.FromHex(hexString);

            //Console.WriteLine("hexString: " + BitConverter.ToString(x));

            DataDebugger.S2CDebugData(x, this.GetType().Name);

            //Debug.Log("___________PLAYER INIT DATA_________", ConsoleColor.White);
            //Debug.Log("RUN SPEED: " + Functions.ReverseBytes16(runSpeed).ToString("X4"), ConsoleColor.White);
           // Debug.Log("EXP LONG: " + LongExtension.Reverse((ulong)curexp), ConsoleColor.White);
            //Debug.Log("EXP LONG: " + BitConverter.ToString(BitConverter.GetBytes(maxexp)), ConsoleColor.White);
           // Debug.Log("____________________________", ConsoleColor.White);

            //AA 55 6B 03 01 02 0A 00 02 02 01 00 AC 99 01 00 05 72 65 61 6B 73 35 02 01 01 00 00 01 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A1 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A1 01 00 00 00 00 00 00 00 00 06 00 00 00 0A 00 00 00 00 00 00 00 A1 01 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A1 01 00 09 00 00 00 00 00 00 13 00 00 00 D9 18 00 04 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 3B 2F E7 00 09 79 61 65 7A 61 72 72 72 72 35 02 01 00 00 00 01 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 3C 2F E7 00 0E 74 68 65 64 61 72 6B 73 6F 75 6C 31 32 33 35 02 01 00 00 00 01 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA
            
            /*
            Console.WriteLine("HEX LENGTH: " + DataLengthCalculator.CalculateHexDataLength(hex));
            Console.WriteLine("HEX LENGTH: " + DataLengthCalculator.CalculateHexDataLength(hex));
            Console.WriteLine("HEX LENGTH: " + DataLengthCalculator.CalculateHexDataLength(hex));
            Console.WriteLine("HEX LENGTH: " + DataLengthCalculator.CalculateHexDataLength(hex));
            Console.WriteLine("HEX LENGTH: " + DataLengthCalculator.CalculateHexDataLength(hex));
            */
            //byte[] x = Functions.FromHex("AA 55 " + Functions.ReverseBytes16(len).ToString("X4") + " 14 " + level.ToString("X2") + " " + Functions.ReverseBytes16(statpoints).ToString("X4") + Functions.ReverseBytes16(naturepoints).ToString("X4") + " 00 00 00 00" + BitConverter.ToString(BitConverter.GetBytes(curexp)) + BitConverter.ToString(BitConverter.GetBytes(curexp)) + " 20 1C 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(maxexp)) + BitConverter.ToString(BitConverter.GetBytes(maxexp)) + Functions.ReverseBytes16(str).ToString("X4") + Functions.ReverseBytes16(str).ToString("X4") + Functions.ReverseBytes16(dex).ToString("X4") + Functions.ReverseBytes16(dex).ToString("X4") + Functions.ReverseBytes16(intel).ToString("X4") + Functions.ReverseBytes16(intel).ToString("X4") + Functions.ReverseBytes16(wind).ToString("X4") + Functions.ReverseBytes16(wind).ToString("X4") + Functions.ReverseBytes16(water).ToString("X4") + Functions.ReverseBytes16(water).ToString("X4") + Functions.ReverseBytes16(fire).ToString("X4") + Functions.ReverseBytes16(fire).ToString("X4") + " 00 " + Functions.ReverseBytes16(atkspeed).ToString("X4") + " 00 00 00 40 " + Functions.ReverseBytes32(maxhp).ToString("X8") + Functions.ReverseBytes32(maxmana).ToString("X8") + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint - (attackpoint / 3)))) + BitConverter.ToString(BitConverter.GetBytes(Convert.ToUInt16(attackpoint + (attackpoint / 3)))) + " 00 00 00 00 " + Functions.ReverseBytes32(def * 3).ToString("X8") + " 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes((artsatk - (artsatk / 5)))) + BitConverter.ToString(BitConverter.GetBytes((artsatk + (artsatk / 5)))) + Functions.ReverseBytes32(artsdef).ToString("X8") + Functions.ReverseBytes16(acc).ToString("X4") + Functions.ReverseBytes16(dodge).ToString("X4") + " 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 19 00 00 00 00 00 00 00 00 FF FF 00 30 30 31 2D 30 31 2D 30 31 20 30 30 3A 30 30 3A 30 30 20 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 64 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 80 3F 00 00 80 3F " + Functions.ReverseBytes16(critical).ToString("X4") + " 55 AA");

            //Console.WriteLine("CHARACTER:\n" + BitConverter.ToString(x));

            return x;
        }

    }


    
    public class SNPCInfo
    {
        // 36 4E => NPC Type
        // 33 4E => session id
        // F4 75 D2 43 || 00 00 BC 42 => posX, posY
        // AA 55 5F 00 31 01 33 4E 36 4E 00 00 01 00 00 00 00 00 00 00 14 41 73 73 61 73 73 69 6E 20 54 72 61 69 6E 65 72 20 42 69 6E 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 01 F4 75 D2 43 00 00 BC 42 00 00 40 41 F4 75 D2 43 00 00 BC 42 00 00 40 41 00 00 00 00 FF FF 00 00 00 00 00 00 00 00 64 00 55 AA
        // assasin trainer bing calisiyor

        // X: 00 00 6C 42  Y: 00 00 5F 43
        // AA 55 5F 00 31 01 33 4E 2D 4E 00 00 01 00 00 00 00 00 00 00 14 41 73 73 61 73 73 69 6E 20 54 72 61 69 6E 65 72 20 42 69 6E 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 01 00 00 6C 42 00 00 5F 43 00 00 40 41 00 00 6C 42 00 00 5F 43 00 00 40 41 00 00 00 00 FF FF 00 00 00 00 00 00 00 00 64 00 55 AA


        // AA 55 5F 00 31 01 33 4E 2D 4E 00 00 01 00 00 00 00 14 41 73 73 61 73 73 69 6E 20 54 72 61 69 6E 65 72 20 42 69 6E 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 01 00 00 6C 42 00 00 5F 43 00 00 40 41 00 00 6C 42 00 00 5F 43 00 00 40 41 00 00 00 00 FF FF 00 00 00 00 00 00 00 00 64 00 55 AA
        // TAVERN CLERK SOHAE WORKING DATA

      
        string nickname = "HERO NPC";
        byte[] nicknameByte;
        public ushort nicknameLength;
        public uint npctype;
        public uint npcLevel = 10;
        public uint npcHP = 100;
        public float posX, posY;
        public byte pseudoNPCId;
        public uint mapId;


        public byte[] getValue()
        {
            nicknameLength = Convert.ToUInt16(nickname.Length);
            nicknameByte = Encoding.ASCII.GetBytes(nickname);
           // string hex = " 00 ";
            //             31 01              33                                                    2D 4E                                        00 00 01 00 00 00 00        14 41 73 73 61 73 73 69 6E 20 54 72 61 69 6E 65 72 20 42 69 6E      01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 01                00 00 6C 42                                    00 00 5F 43                                      00 00 40 41            00 00 6C 42                                         00 00 5F 43                                      00 00 40 41 00 00 00 00 FF FF 00 00 00 00 00 00 00 00 64 00 55 AA
            string hex = ("31 01" + pseudoNPCId.ToString("X2") + "4E" + Functions.ReverseBytes16((ushort)npctype).ToString("X4") + "00 00 01 00 00 00 00" + "14 41 73 73 61 73 73 69 6E 20 54 72 61 69 6E 65 72 20 42 69 6E" + " 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 01" + BitConverter.ToString(BitConverter.GetBytes(posX)) + BitConverter.ToString(BitConverter.GetBytes(posY)) + " 00 00 40 41 " + BitConverter.ToString(BitConverter.GetBytes(posX)) + BitConverter.ToString(BitConverter.GetBytes(posY)) + " 00 00 40 41 00 00 00 00 FF FF 00 00 00 00 00 00 00 00 64 00");
            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";
            byte[] x = Functions.FromHex(hexString);

          // byte[] x = Functions.FromHex("AA 55 5F 00  55 AA");


            //                                        (uint64(npcPos.PseudoID), 2, true), 6)            
           // DataDebugger.S2CDebugData(x, this.GetType().Name);
            //Console.WriteLine("PACKET:\n" + BitConverter.ToString(x));

            return x;
        }
    }

    public class SRegisterAckMessage
    {
        // AA 55 12 00 01 03 0A 00 00 EB 9A 01 00 08 59 6F 73 68 69 73 61 6E 55 AA     mavi
        // AA 55 13 00 01 03 0A 00 00 ED 9A 01 00 09 6B 6F 6E 73 66 69 72 72 6D 55 AA  mavi
        // AA 55 14 00 01 03 0A 00 00 EF 9A 01 00 0A 6B 6F 6E 73 74 72 69 6B 78 7A 55 AA  kırmızı

        public string nickname;

        public byte[] getValue()
        {
            ushort len = Convert.ToUInt16(10 + nickname.Length);

            byte[] x = Functions.FromHex("AA 55 " + BitConverter.ToString(BitConverter.GetBytes(len)) + " 01 03 0A 00 00 EB 9A 01 00 " + nickname.Length.ToString("X2") + BitConverter.ToString(Encoding.ASCII.GetBytes(nickname)) + " 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }

    }

    public class SAddPartyMember
    {
        //AA 55 2A 00 52 04 0A 00 00 UID4X 00 00 NAMELENGTH2X NAME HP4X MAXHP4X 00 00 00 00 00 00 00 00 00 MANA4X MAXMANA4X LEVEL2X CLASS2X 00 00 00 00 00 00 00 55 AA
        public string nickname;
        public uint memberUID;
        public int curHP, maxHP;
        public int curMP, maxMP;
        public ushort _level;
        public ushort _class;

        public byte[] getValue()
        {
            var hex = ("52 04 0A 00 00 "+ memberUID.ToString("X4") + " 00 00 " + nickname.Length.ToString("X2") + BitConverter.ToString(Encoding.ASCII.GetBytes(nickname)) + Functions.ReverseBytes16((ushort)curHP).ToString("X4") + Functions.ReverseBytes16((ushort)maxHP).ToString("X4") + " 00 00 00 00 00 00 00 00 00 " + Functions.ReverseBytes16((ushort)curMP).ToString("X4") + Functions.ReverseBytes16((ushort)maxMP).ToString("X4") + _level.ToString("X2") + _class.ToString("X2") + " 00 00 00 00 00 00 00");
            
            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);



            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }

    }

    public class SSendPartyInvite
    {
        //AA 55 0D 00 52 01 0A 00 09 05 50 4C 41 59 45 52 11 55 AA
       // public uint targetPlayerUID;
        public string nickname;

        public byte[] getValue()
        {
            // AA 55   52 01             0A 00                       09       05                                   50 4C 41 59 45 52                                  11 55 AA
            // AA 55   52 01             0A 00                       09       05                                   50 4C 41 59 45 52 11 55 AA



            //AA 55 0D 00 52 01         0A 00                          09                    05                        50 4C 41 59 45 52                                   11 55 AA

            //AA-55-10-00-52-01-         00-02-                         09-          09-                                6B-61-72-61-65-6A-64-65-72-                            11-55-AA

            //AA-55-0B- 52-01-            0A-00-                             05-                                       62-61-63-74-79-55-AA
            var hex = ("52 01 0A " + "00" + nickname.Length.ToString("X2") + BitConverter.ToString(Encoding.ASCII.GetBytes(nickname)));

            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);



            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }

    }

    public class SRemoveCharacterQuitMessage
    {
        // AA 55 05 00 21 02 D5 00 00 55 AA

        public ushort uid;

        public byte[] getValue()
        {
            // Debug.Log("CHARACTER QUIT GAME!", ConsoleColor.Red);

            byte[] x = Functions.FromHex("AA 55 05 00 21 02 " + BitConverter.ToString(BitConverter.GetBytes(uid)) + " 00 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }


    
    public class SCharacterInfoAckMessage //CHARACTER_LIST DRAGON
    {
        // AA 55 39 02 01 02 0A 00 01 02 "+how many character+" " + character-slot + " AC 99 01 00 " + len(nickname) + nickname + evolution-id + " 02 "+ level +" 00 00 00 "+ map +" 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA
        
        public string nickname = "";
        public ushort level;
        public uint currentMap;
        public byte evolution;

        public byte charactercount = 1;

        public uint helmetId;
        public uint swordId;
        public uint armorId;
        public uint footId;

        const string VERSION = "02"; // 02
        const string HAIR_STYLE = "01";

        public byte[] getValue()
        {
            //ushort len = Convert.ToUInt16(870 + nickname.Length + 0 - 587 - 17);
            ushort len = Convert.ToUInt16(870 + nickname.Length);

            byte[] x;

            if (charactercount > 0) // " 77 51 9D 00 A1 01 00 03 "
                x = Functions.FromHex("AA 55 " + Functions.ReverseBytes16(len).ToString("X4") + " 01 " + VERSION + " 0A 00 02 02 " + charactercount.ToString("X2") + " 00 AC 99 01 " + nickname.Length.ToString("X4") + BitConverter.ToString(Encoding.ASCII.GetBytes(nickname)) + evolution.ToString("X2") + " 02 " + level.ToString("X2") + currentMap.ToString("X2") + currentMap.ToString("X2") + " 00 00 " + HAIR_STYLE + " 00 00 00 00 00 00 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(helmetId)) + " A1 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(armorId)) + " A1 01 00 00 00 00 00 00 00 00 06 00 00 00 0A 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(swordId)) + " A1 01 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(footId)) + " A1 01 00 09 00 00 00 00 00 00 13 00 00 00 D9 18 00 04 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 3B 2F E7 00 09 79 61 65 7A 61 72 72 72 72 35 02 01 00 00 00 01 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 3C 2F E7 00 0E 74 68 65 64 61 72 6B 73 6F 75 6C 31 32 33 35 02 01 00 00 00 01 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA");
            else
                x = Functions.FromHex("AA 55 07 00 01 02 0A 00 00 00 00 55 AA"); // u haven't created any characters yet.
            //                         AA 55 07 00 01 B9 0A 00 00 01 00 55 AA

            string pkt = BitConverter.ToString(x).Replace("-", " ");

            pkt = pkt.Substring(0, (pkt.LastIndexOf("55 AA") + "55 AA".Length));

            //Console.WriteLine("[SCharacterInfoAckMessage]\n" + pkt);
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }


    public class SChatMessageAckMessage
    {
        // "AA 55 17 00 71 01 C2 01 " + len(nickname) + nickname + " "+ len(message) +" 00 " + message + " 55 AA"

        
        public string nickname;
        public string message;

        public byte[] getValue()
        {
            if (message == null) return Functions.FromHex("AA 55 00 00 55 AA");

            ushort len = Convert.ToUInt16(6 + nickname.Length + message.Length);

            // Console.WriteLine("CHARACTER INIT CALLED\nCHARACTER INIT CALLED\nCHARACTER INIT CALLED\n");
            byte[] x = Functions.FromHex("AA 55 " + Functions.ReverseBytes16(len).ToString("X4") + " 71 01 C2 01 " + nickname.Length.ToString("X2") + BitConverter.ToString(Encoding.ASCII.GetBytes(nickname)) + message.Length.ToString("X2") + BitConverter.ToString(Encoding.ASCII.GetBytes(message)) + " 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }


    public class SMessageBox
    {
        // AA 55 1E 00 00 01 00 len message 65 2E 55 AA

        public string message;

        public byte[] getValue()
        {
            ushort len = Convert.ToUInt16(6 + message.Length);

            byte[] x = Functions.FromHex("AA 55 " + Functions.ReverseBytes16(len).ToString("X4") + " 00 01 00 " + message.Length.ToString("X2") + BitConverter.ToString(Encoding.ASCII.GetBytes(message)) + " 65 2E 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);

            return x;
        }
    }


    public class SLoginAckMessage
    {
        static string sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        //"AA-55-55-00-00-01-01 " + len(nickname) + nickname + " 40-35464543454236364646433836463338443935323738364336443639364337394332444243323339444434453931423436373239443733413237464235374539-00-00-D6-22-46-06-55-AA"

        public string nickname;
        public string password;

        public byte[] getValue()
        {
            string pass = password;

            pass = BitConverter.ToString(UTF8Encoding.UTF8.GetBytes(pass));

            ushort len = Convert.ToUInt16(75 + nickname.Length);

            byte[] x = Functions.FromHex("AA-55-" + Functions.ReverseBytes16(len).ToString("X4") + "-00-01-01 " + nickname.Length.ToString("X2") + BitConverter.ToString(Encoding.ASCII.GetBytes(nickname)) + " 40-" + pass + "-00-00-D6-22-46-06-55-AA");

            DataDebugger.S2CDebugData(x, this.GetType().Name);

            return x;
        }

    }

    public class SGameServerAddressAckMessage
    {
        public string ip_address;
        public ushort port;

        public byte[] getValue()
        {
            ushort len = Convert.ToUInt16(8 + ip_address.Length);

            byte[] x = Functions.FromHex("AA 55 " + Functions.ReverseBytes16(len).ToString("X4") + " 00 05 01 " + ip_address.Length.ToString("X2") + BitConverter.ToString(Encoding.ASCII.GetBytes(ip_address)) + Functions.ReverseBytes16(port).ToString("X4") + " 00 00 55 AA");

            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }

    public class SStartGameAckMessage
    {
        public byte[] getValue()
        {
            byte[] x = Functions.FromHex("AA 55 04 00 01 05 0A 00 55 AA");
            DataDebugger.S2CDebugData(x, this.GetType().Name);
            return x;
        }
    }

    public class SCharacterAttack
    {
        //AA 55 0B 00 41 01 00 02 02 6C 4E 01 01 01 00 55 AA  me
        //AA 55 0B 00 41 01 B9 00 01 69 4E 00 01 00 00 55 AA  other plyr
        //AA 55 0B 00 41 01 5C 02 06 6E 4E 00 01 01 00 55 AA
        public ushort CharSession_id;
        public ushort MobSession_id;
        public byte[] getValue()
        {
            //byte[] x = Functions.FromHex("AA 55 0B 00 41 01 " + CharSession_id.ToString("X4") + " 02 " + MobSession_id.ToString("X4") + "01 01 01 00 55 AA");
            //                            AA 55 0B 00 41 01              5C 02                    06         6E 4E                       00 01 01 00 55 AA
            //byte[] x = Functions.FromHex("AA 55 0B 00 41 01 " + CharSession_id.ToString("X4") + " 01 " + MobSession_id.ToString("X4") + "00 01 00 00 55 AA");
            byte[] x =   Functions.FromHex("AA 55 0C 00 41 01 " + CharSession_id.ToString("X4") + MobSession_id.ToString("X4") + " 01 01 01 00 00 00 55 AA");
            //                              AA 55 0C 00 41 01          95 4F                             C5 00                     01 01 00 00 00 00 55 AA 

            DataDebugger.S2CDebugData(x, this.GetType().Name);

            return x;
        }

    }

    public class SRemoteSkillAttack
    {
        //                      mobuid    me
        //AA 55 0C 00 41 01     95 4F   C5 00       01 01 00 00 00 00 55 AA 

        //AA 55 1D 00 42 0A 00 C5 00 09 59 02 00 00 00 80 AA 43 C4 CC D0 43 B8 71 0F 3F 01 95 4F 01 95 4F 01 55 AA 
        //AA 55 1D 00 42 0A 00 2A 00 05 5A 02 00 00 00 80 B0 43 00 80 D7 43 BA E8 AF BE 01 93 4F 01 93 4F 01 55 AA
        //AA 55 1D 00 42 0A 00 02 00 01 7C 00 00 00 43 29 00 00 42 BE 00 00 00 00 00 00 55 00 55 00 55 AA

        //AA 55 1D 00 42 00 03 05 00 00 00 59 AE 3A 43 59 AE EB 42 BA E8 AF BE 01 01 F9 01 01 F9 01 55 AA works
        public ushort CharSession_id;
        public ushort MobSession_id;
        public byte skillID;
        public float posX, posY, posZ;
        public byte[] getValue()
        {

            //           42 0A 00         C5 00                        09       59                       02 00 00                                    00 80 AA 43                                     C4 CC D0 43                                       B8 71 0F 3F                     01                      95 4F           01                 95 4F                01 55 AA 
            //          42 0A            2A 00                        05       5A                       02 00 00                                    00 80 B0 43                                     00 80 D7 43                                       BA E8 AF BE                     01           93 4F                      01           93 4F                      01 55 AA

         // AA 55 1D 00 42        00 03                        05                        00 00 00             59 AE 3A 43                                      59 AE EB 42                                            BA E8 AF BE                                      01              01 F9                   01               01 F9                  01 55 AA
            var hex = ("42" + CharSession_id.ToString("X4") + skillID.ToString("X2") + " 00 00 00 " + BitConverter.ToString(BitConverter.GetBytes(posX)) + BitConverter.ToString(BitConverter.GetBytes(posY)) + BitConverter.ToString(BitConverter.GetBytes(posZ)) + " 01 " + MobSession_id.ToString("X4") + " 01 " + MobSession_id.ToString("X4") + " 01");
            
            string hexString = "AA 55 " + DataLengthCalculator.CalculateHexDataLength(hex) + " " + (hex).Replace("-", " ") + " 55 AA";

            byte[] x = Functions.FromHex(hexString);

            DataDebugger.S2CDebugData(x, this.GetType().Name);

            Debug.Log("CHAR SESS ID: " + "00" + CharSession_id.ToString("X2") + "\nUSED SKILL ID: " + skillID.ToString("X2") + "\nMOB SESS ID: " + MobSession_id.ToString("X4") + "\nXYZ: " + BitConverter.ToString(BitConverter.GetBytes(posX)) + "," + BitConverter.ToString(BitConverter.GetBytes(posY)) + "," + BitConverter.ToString(BitConverter.GetBytes(posZ)),ConsoleColor.Cyan);

            return x;
        }

    }
    public class SChannelListAckMessage
    {
        public string ServerName;
        public string ChannelName;
        public int HowMuchFull;

        public byte[] getValue()
        {

            ushort len = Convert.ToUInt16(7 + ServerName.Length + ChannelName.Length + 17);

            string codstr = "AA 55 " + Functions.ReverseBytes16(len).ToString("X4") + " 00 03 01 00 00 00 00 00 " + ServerName.Length.ToString("X2") + " " + BitConverter.ToString(Encoding.ASCII.GetBytes(ServerName)) + " 01 00 00 00 " + ChannelName.Length.ToString("X2") + " " + BitConverter.ToString(Encoding.ASCII.GetBytes(ChannelName)) + " " + HowMuchFull.ToString("X2") + " 00 FA 00 12 00 00 00 01 00 55 AA";
            byte[] x = Functions.FromHex(codstr);

            DataDebugger.S2CDebugData(x, this.GetType().Name);

            return x;
        }

    }


    public class SAddStatAckMessage
    {
        public byte HaveStatPoint;
        public short StrPoint;
        public int Attack;


        public SAddStatAckMessage()
        {
            /* 
              string strarr = BitConverter.ToString(array).Replace("-", " ");

              strarr = Functions.getBetween(strarr, "AA 55 ", " 55 AA");
              byte[] arr = Functions.FromHex(strarr);

              HaveStatPoint = arr[11];
              StrPoint = BitConverter.ToInt16(arr, 41);
              Attack = BitConverter.ToInt32(arr, 86);
              */

        }


        public byte[] getValue()
        {

            byte[] a1 = BitConverter.GetBytes(HaveStatPoint);
            Array.Reverse(a1);
            byte[] a2 = BitConverter.GetBytes(StrPoint);
            Array.Reverse(a2);
            byte[] a3 = BitConverter.GetBytes(Attack);
            Array.Reverse(a3);

            int b2 = StrPoint;
            int b3 = Attack;

            string codstr = "AA 55 E4 00 14 01 00 00 00 01 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 5B 0E 00 00 00 64 00 00 00 00 00 00 00 90 01 DC 00 0C 00 0C 00 06 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 66 66 C6 40 66 66 C6 40 F3 03 00 00 00 40 D2 00 00 00 30 00 00 00 0C 00 0C 00 0C 00 00 00 0C 00 00 00 0C 00 00 00 0C 00 00 00 0C 00 00 00 18 00 00 00 0B 00 0C 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0C 00 00 00 18 00 00 00 00 00 00 00 00 FF FF 00 30 30 31 2D 30 31 2D 30 31 20 30 30 3A 30 30 3A 30 30 20 30 30 30 30 30 30 30 30 30 30 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 64 00 00 00 00 00 00 00 00 00 64 00 00 00 00 00 80 3F 00 00 80 3F 00 00 55 AA";
            byte[] x = Functions.FromHex(codstr);

            // Console.WriteLine("B2 OLAN BU STRPOINT :: " + b2.ToString("X4"));
            // Console.WriteLine("B3 OLAN BU ATTACK :: " + b3.ToString("X8"));

            DataDebugger.S2CDebugData(x, this.GetType().Name);

            return x;
        }
    }



}
