using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp.PacketTypes
{

    public class CItemUsage
    {
        // AA-55-09-00-59-04-45-31-F7-00-0D-00-00-55-AA

        public uint itemId;
        public ushort itemSlot;

        public CItemUsage(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array);
                byte[] arr = Functions.FromHex(strarr);

                itemId = BitConverter.ToUInt32(arr, 6);
                itemSlot = BitConverter.ToUInt16(arr, 10);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }


    public class CItemSwitchRequestMessage
    {
        // AA-55-13-00-59-07-01-71-51-9D-00-1D-00-03-00-76-51-9D-00-03-00-1D-00-55-AA-43-00-00-00-00-4D-01-66-66-C6-40-00-55-AA

        //  AA-55-13-00-59-07-01-C2-D1-BD-00-15-00-02-00-7E-26-E8-05-02-00-15-00-55-AA-42-00-00-00-00-5A-01-66-66-C6-40-00-55-AA // MASTER GHOST ARMOR 2, STUDDED LEATHER ARMOR 15

        public uint fromItemId, toItemId;
        public ushort inventorySlot, characterSlot;

        public CItemSwitchRequestMessage(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array);
                byte[] arr = Functions.FromHex(strarr);

                toItemId = BitConverter.ToUInt32(arr, 7);
                inventorySlot = BitConverter.ToUInt16(arr, 11);
                characterSlot = BitConverter.ToUInt16(arr, 13);
                fromItemId = BitConverter.ToUInt32(arr, 15);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }


    public class CItemSeperateInfoRequest
    {
        // AA-55-08-00-59-09-10-00-0F-00-02-00-55-AA

        public ushort fromWhere, toWhere, howMany;

        public CItemSeperateInfoRequest(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array);
                byte[] arr = Functions.FromHex(strarr);

                fromWhere = BitConverter.ToUInt16(arr, 6);
                toWhere = BitConverter.ToUInt16(arr, 8);
                howMany = BitConverter.ToUInt16(arr, 10);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }


    public class CItemCombineInfoRequest
    {
        // AA-55-06-00-59-06-0E-00-0D-00-55-AA

        public ushort fromWhere, toWhere;

        public CItemCombineInfoRequest(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array);
                byte[] arr = Functions.FromHex(strarr);

                fromWhere = BitConverter.ToUInt16(arr, 6);
                toWhere = BitConverter.ToUInt16(arr, 8);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CSellRequestMessage
    {
        // AA-55-0A-00-58-02-B6-5E-A0-00-01-00-13-00-55-AA
        // AA-55-0A-00-58-02-62-BE-A1-00-05-00-33-00-55-AA

        public uint itemId;
        public ushort inventorySlot;
        public ushort itemCount;

        public CSellRequestMessage(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array);
                byte[] arr = Functions.FromHex(strarr);

                itemId = BitConverter.ToUInt32(arr, 6);
                inventorySlot = BitConverter.ToUInt16(arr, 12);
                itemCount = BitConverter.ToUInt16(arr, 10);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CBuyRequestMessage
    {
        // AA-55-12-00-58-01-E2-E3-C7-00-01-00-6C-00-00-00-0B-00-29-4E-00-00-55-AA-EA-42-00-00-00-00-52-01-66-66-C6-40-00-55-AA // SILVER EARRING TRY BUY

        public ushort NPCId;
        public uint itemId;
        public ushort inventorySlot;
        public ushort itemCount;

        public CBuyRequestMessage(TcpClient client, byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array);
                byte[] arr = Functions.FromHex(strarr);

                itemId = BitConverter.ToUInt32(arr, 6);
                NPCId = BitConverter.ToUInt16(arr, 18);
                inventorySlot = (ushort)Utils.FindEmptyInventorySlot(client);
                itemCount = BitConverter.ToUInt16(arr, 10);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CChangeMap
    {
        // AA 55 05 00 28 F3 03 00 00 55 AA highlands
        // AA 55 05 00 28 F4 03 00 00 55 AA zehir
        // AA 55 05 00 28 F5 03 00 00 55 AA ruh

        public int PortalId;

        public CChangeMap(TcpClient client, byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array);
                byte[] arr = Functions.FromHex(strarr);

                PortalId = int.Parse($"{array[6]:X2}{array[5]:X2}", System.Globalization.NumberStyles.HexNumber);
                //MapId = BitConverter.ToInt32(arr, 4);
                Debug.Log("MapId (Hex): " + PortalId.ToString("X4"), ConsoleColor.DarkCyan);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CNPCClick
    {
        // 23-4E
        // AA-55-03-00-CF-23-4E-55-AA
        public ushort requestNPCId;

        public CNPCClick(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array);

                //Debug.Log("CLICKED NPC: " + strarr);
                byte[] arr = Functions.FromHex(strarr);

                requestNPCId = BitConverter.ToUInt16(arr, 5);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }


    public class CNPCClick2
    {
        // 21-4E
        // AA-55-08-00-57-01-21-4E-00-00-00-00-55-AA

        public ushort requestNPCId;

        public CNPCClick2(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array);
                //Debug.Log("CLICKED NPC: " + strarr);
                byte[] arr = Functions.FromHex(strarr);

                requestNPCId = BitConverter.ToUInt16(arr, 6);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CItemDrop
    {
        // AA-55-09-00-59-02-01-D9-CA-9B-00-pos-00-55-AA

        public uint itemId;
        public ushort itemSlot;

        public CItemDrop(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");
                byte[] arr = Functions.FromHex(strarr);

                itemId = BitConverter.ToUInt32(arr, 7);
                itemSlot = BitConverter.ToUInt16(arr, 11);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }


    public class CItemDrag
    {
        public uint itemId;
        public ushort itemSlot, itemToSlot;

        public CItemDrag(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");
                byte[] arr = Functions.FromHex(strarr);

                itemId = BitConverter.ToUInt32(arr, 6);
                itemSlot = BitConverter.ToUInt16(arr, 10);
                itemToSlot = BitConverter.ToUInt16(arr, 12);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CGetLootItemInfo
    {
        public uint itemPseudo;
        public CGetLootItemInfo(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");
                byte[] arr = Functions.FromHex(strarr);
                byte[] arr2 =
                {
                    arr[8],
                    arr[7]
                };

                itemPseudo = BitConverter.ToUInt16(arr2, 0);

                Debug.Log("DATA: " + arr);

                Debug.Log("\ntrying to set item pseudo: " + itemPseudo + "\n", ConsoleColor.Yellow);
                Debug.Log("\ntrying to set item pseudo: " + itemPseudo + "\n", ConsoleColor.Yellow);
                Debug.Log("\ntrying to set item pseudo: " + itemPseudo + "\n", ConsoleColor.Yellow);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CGetUpgradeInfo
    {
        public byte itemSlot;
        public byte upgradeMaterialSlot;
        //                  slot
        // AA 55 0B 00 54 02 0B 00 01 0C 00 00 00 00 00 55 AA
        public CGetUpgradeInfo(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");
                byte[] arr = Functions.FromHex(strarr);

                itemSlot = arr[6];
                upgradeMaterialSlot = arr[9];
                Debug.Log("\ntrying to get item slot: " + itemSlot + "\n", ConsoleColor.Yellow);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CItemDestroy
    {
        public uint itemId;
        public ushort itemSlot;

        public CItemDestroy(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");
                byte[] arr = Functions.FromHex(strarr);

                itemId = BitConverter.ToUInt32(arr, 6);
                itemSlot = BitConverter.ToUInt16(arr, 10);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CStatPointAddReqMessage
    {
        public byte whichstat; // 00 str, 01 dex, 02 int
        public ushort howmanystat;

        public CStatPointAddReqMessage(byte[] array, int statType)
        {
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");
                byte[] arr = Functions.FromHex(strarr);

                if (statType == 2)
                    whichstat = 0;

                if (statType == 1)
                    whichstat = 1;

                if (statType == 0)
                    whichstat = 2;

                howmanystat = 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CPartyRequestAnswer
    {
        public byte isAccepted;

        public CPartyRequestAnswer(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");
                byte[] arr = Functions.FromHex(strarr);

                isAccepted = arr[6];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CGetTargetPartyMemberInfo
    {
        public ushort uid;

        public CGetTargetPartyMemberInfo(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");
                byte[] arr = Functions.FromHex(strarr);

                byte[] arr2 =
{
                    arr[7],
                    arr[6]
                };

                uid = BitConverter.ToUInt16(arr2, 0);
                Debug.Log("uid: " + uid);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CAttackDefendMode
    {
        // AA 55 09 00 02 06 00 40 73 14 DD 77 17 55 AA  // CLIENT attack mode
        // AA 55 09 00 02 06 00 40 72 82 ED 70 60 55 AA  // CLIENT defend mode


        // AA 55 04 00 43 79 01 01 55 AA  // SERVER 79 01 session id

        public byte attackModeBool;

        public CAttackDefendMode(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");
                byte[] arr = Functions.FromHex(strarr);

                attackModeBool = arr[5];
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }

    public class CUpgradedSkillBookSlot
    {
        // AA 55 09 00 02 06 00 40 73 14 DD 77 17 55 AA  // CLIENT attack mode
        // AA 55 09 00 02 06 00 40 72 82 ED 70 60 55 AA  // CLIENT defend mode


        // AA 55 04 00 43 79 01 01 55 AA  // SERVER 79 01 session id

        public byte upgradedSkillBookSlot;

        public CUpgradedSkillBookSlot(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");
                byte[] arr = Functions.FromHex(strarr);

                upgradedSkillBookSlot = arr[6];

                Debug.Log("Upgraded skill slot: " + upgradedSkillBookSlot);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }

    public class CRedirectFromServer
    {
        public string nickname;

        public CRedirectFromServer(byte[] array)
        {
            //  AA-55-10-00-01-01-07-66-72-65-6B-6F-6E-73-01-30-35-87-01-00-55-AA
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");
                byte[] arr = Functions.FromHex(strarr);

                string str = Encoding.ASCII.GetString(arr);

                str = str.Substring(7, arr[6]);

                nickname = str;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }

    public class CMoveReqMessage
    {
        public float oldX, oldY, oldZ;
        public float newX, newY, newZ;
        public ushort runMode;
        public float playerSpeed;

        public enum MOVE_TYPE
        {
            WALK = 8705,
            RUN = 8706,
            FLY = 9732
        }

        public CMoveReqMessage(byte[] array)
        {
            //  AA-55-21-00-22-01- oldX - oldY - oldZ - newX - newY - newZ -AF-00-66-66-C6-40-00-55-AA-30-37-43-37-43-35-30-36-41-45-31-38-31-33-34-39-30-45-34-42-41-36-37-35-46-38-34-33-44-35-41-31-30-45-30-42-41-41-43-44-42-38-00-00-55-AA

            //  "AA 55 " + uid.ToString("X2") + Functions.ReverseBytes32((int)MOVE_TYPE.WALK).ToString("#4") + BitConverter.ToString(BitConverter.GetBytes(x1)) + BitConverter.ToString(BitConverter.GetBytes(y1)) + " " + BitConverter.ToString(BitConverter.GetBytes(x2)) + BitConverter.ToString(BitConverter.GetBytes(y2)) + "00 CX B0 FE BE 00 00 55 AA");
            // AA-55-21-00-22-01-62-3F-58-42-09-09-46-43-C2-39-5C-BF-00-00-6C-42-00-00-2F-43-AF-99-2E-BF-5B-01-66-66-C6-40-00-55-AA
            MOVE_TYPE MoveType = MOVE_TYPE.WALK;

            var moveIndex = BitConverter.ToString(BitConverter.GetBytes((int)MoveType));

            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");

                strarr = Functions.getBetween(strarr, "AA 55 ", " 55 AA");
                byte[] arr = Functions.FromHex(strarr);

                //uid = System.BitConverter.ToUInt16(arr, 0); //index 4
                runMode = System.BitConverter.ToUInt16(arr, 2); //index 6 
                oldX = System.BitConverter.ToSingle(arr, 4);
                oldY = System.BitConverter.ToSingle(arr, 8);
                oldZ = System.BitConverter.ToSingle(arr, 12);

                newX = System.BitConverter.ToSingle(arr, 16);
                newY = System.BitConverter.ToSingle(arr, 20);
                newZ = System.BitConverter.ToSingle(arr, 24);

                playerSpeed = System.BitConverter.ToSingle(arr, 24);

                ClientMemory.SetPlayerPosition(newX, newY);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }
    }

    public class CSlashChatMessage
    {
        public int length;

        //public string connection_id;
        public string message;

        public CSlashChatMessage(byte[] array)
        {
            // "AA-55-0A-00-71-07 " + length + message +" 55-AA-00-55-AA-AA " + connection_id + " 00-00-55-AA"
            try
            {
 string strarr = BitConverter.ToString(array).Replace("-", " ");

            strarr = Functions.getBetween(strarr, "AA 55 ", " 55 AA");
            byte[] arr = Functions.FromHex(strarr);


            length = BitConverter.ToInt16(arr, 0);

            message = Encoding.ASCII.GetString(arr, 5, arr[4]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
           

            //strarr = BitConverter.ToString(array).Replace("-", " ");


            //Console.WriteLine("STRARR: \n" + strarr);

            //arr = Functions.FromHex(strarr);

            //connection_id = Encoding.ASCII.GetString(arr, 0, arr.Length);
        }

    }

    public class CAttackInfo
    {
        public ushort mobId;
        public CAttackInfo(byte[] array, bool isNull = false)
        {

            //mobid
            //AA-55-08-00-41-03-01-6B-78-00-01-00-55-AA
            //AA 55 07 00 41 00 6B 78 00 01 00 55 AA
            
            //AA 55 0C 00 41 01 0D 02 01 00 00 00 55 AA
            //AA 55 0C 00 41 01 00 01 00 01 00 00 55 AA
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");

                strarr = strarr.Substring(0, (strarr.LastIndexOf("55 AA") + "55 AA".Length));

                Debug.Log("CAttackInfo : " + strarr,ConsoleColor.Green);

                byte[] arr = Functions.FromHex(strarr);
                byte[] arr2 =
                {
                    arr[7],
                    arr[6]
                };

                if(isNull)
                    arr2 = new byte[] { arr[8], arr[7] };

                mobId = BitConverter.ToUInt16(arr2,0);
            }
            catch (Exception ex) { Console.WriteLine("CAttackInfo: " + ex); }

        }
    }


    public class CSkillUseInfo
    {
        public byte skillId;
        public ushort mobId;
        public CSkillUseInfo(byte[] array)
        {

            //AA 55 16 00 42 0F 00 00 00 B0 44 22 43 9E C7 A4 42 AF 4A CD 3E 75 A1 01 75 A1 55 AA
            // 0f
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");

                strarr = strarr.Substring(0, (strarr.LastIndexOf("55 AA") + "55 AA".Length));

                Debug.Log("CSkillUseInfo : " + strarr, ConsoleColor.Green);

                byte[] arr = Functions.FromHex(strarr);
                byte[] arr2 =
                {
                    arr[22],
                    arr[21]
                };

                skillId = arr[5];
                mobId = BitConverter.ToUInt16(arr2, 0);
            }
            catch (Exception ex) { Console.WriteLine("Error 1003 CSkillUseInfo: " + ex); }

        }
    }
    public class CChatMessage
    {
        public int length;

        //public string connection_id;
        public string message;

        public CChatMessage(byte[] array)
        {
            // "AA-55-17-00-71-01 " + len(message)(1 byte) + " 00 " + message + " 55-AA-BF-25-01-66-66-C6-40-00-55-AA " + connection_id + " 00-00-55-AA"
            // "AA-55-17-00-71-01 " + len(message)(1 byte) + " 00 " + 64 72 54 64 72 54 + " 55-AA-BF-25-01-66-66-C6-40-00-55-AA " + connection_id + " 00-00-55-AA"
            // AA 55 09 00 71 01 + len(message) + message 55 AA 00 00 00 00 80 AE 43 00 80 D9 43 00 00 00 00 86 00 00 A0 B0 40 00 55 AA
            
            // AA 55 09 00 71 01 + 06 + 64 72 54 64 72 54 55 AA 00 00 00 00 80 AE 43 00 80 D9 43 00 00 00 00 86 00 00 A0 B0 40 00 55 AA
            try
            {
            string strarr = BitConverter.ToString(array).Replace("-", " ");

            strarr = Functions.getBetween(strarr, "AA 55 ", " 55 AA");
            byte[] arr = Functions.FromHex(strarr);


            length = BitConverter.ToInt16(arr, 0);

            message = Encoding.ASCII.GetString(arr, 5, arr[4]);

            Debug.Log("Text Message: " + message, ConsoleColor.Yellow);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
    

           
        }

    }


    public class CMeditationState
    {
        // AA-55-03-00-82-05-00-55
        // AA-55-03-00-82-05-01-55

        public byte trueorfalse;

        public CMeditationState(byte[] array)
        {
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");

                byte[] arr = Functions.FromHex(strarr);

                trueorfalse = arr[6];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CDeleteCharacterReqMessage
    {
        public string nickname;


        public CDeleteCharacterReqMessage(byte[] array)
        {
            //AA-55-16-00-01-03-00-07-66-72-65-6B-6F-6E-73-35-01-00-00-00-00-00-00-00-00-00-55-AA

            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");

                byte[] arr = Functions.FromHex(strarr);

                nickname = Encoding.ASCII.GetString(arr).Substring(8, arr[7]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class CRegisterReqMessage
    {
        public string nickname;
        public byte race;
        public byte evolution;

        public CRegisterReqMessage(byte[] array)
        {
            //AA-55-16-00-01-03-00-07-66-72-65-6B-6F-6E-73-35-01-00-00-00-00-00-00-00-00-00-55-AA

            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");

                strarr = Functions.getBetween(strarr, "AA 55 ", " 55 AA");
                byte[] arr = Functions.FromHex(strarr);

                nickname = Encoding.ASCII.GetString(arr).Substring(6, arr[5]);

                evolution = Convert.ToByte(arr[6 + arr[5]]);
                race = Convert.ToByte(arr[7 + arr[5]]);

                //Console.WriteLine("CREGISTERREQMESSAGE\nDATA: " + BitConverter.ToString(arr));
                //16-00-01-03-00-07-66-72-65-6B-6F-6E-73-35-02-00-00-00-00-00-00-00-00-00
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }


    public class CLoginReqMessage
    {
        public int length = 0;

        public int passwordLength = 0;

        public string username="", password="";

        public CLoginReqMessage(byte[] array)
        {
            // "AA 55 50 00 00 00 00 00 00 " + len(nickname) + nickname + " " + len(pass_sha256) + pass  + " 00 00 55 AA"
            try
            {
                string strarr = BitConverter.ToString(array).Replace("-", " ");

                strarr = Functions.getBetween(strarr, "AA 55 ", " 55 AA");
                byte[] arr = Functions.FromHex(strarr);

                Debug.Log("LOGIN PACKET: " + BitConverter.ToString(arr).Replace("-", " "));

                //byte[] arr = 23 00 00 00 08 7A 61 72 6D 65 6E 39 38 15 4C 46 48 53 55 57 30 49 59 52 41 4B 54 31 4D 56 31 56 37 4C 31 00 00 55 AA

                if (arr[2] != 56)
                {
                    Debug.Log("USERNAME TEST: " + username);

                    length = arr[4];

                    Debug.Log("LENGTH: " + length);

                    username = Encoding.ASCII.GetString(arr, 5, length);

                    Debug.Log("USERNAME: " + username);

                    // Log the array after the username part
                    Debug.Log("ARRAY AFTER USERNAME: " + BitConverter.ToString(arr, 5 + length));

                    passwordLength = arr[5 + length];
                    Debug.Log("PASSWORD LENGTH: " + passwordLength);

                    // Make sure to calculate the correct starting position for the password
                    int passwordStartIndex = 6 + length; // 5 for username, 1 for password length byte
                    password = Encoding.ASCII.GetString(arr, passwordStartIndex, passwordLength);
                    Debug.Log("PASSWORD: " + password);
                }
                else
                {
                    Debug.Log("USERNAME TEST2: " + username);
                    strarr = BitConverter.ToString(array).Replace("-", " ");
                    arr = Functions.FromHex(strarr);

                    if(arr[4] == 0)
                    {
                        username = "nul";
                        password = "nul";
                    }

                    length = arr[4];

                    username = Encoding.ASCII.GetString(arr, 17, arr[16]);

                    password = Encoding.ASCII.GetString(arr, 17 + arr[16] + 1, arr[17 + arr[16]]);

                }

                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            

        }

    }

    //public class CAddStatReqMessage
    //{


    //    public short length;

    //    public short packet_id;

    //    public short statnum;



    //    public CAddStatReqMessage(byte[] array)
    //    {
    //        try
    //        {
    //            string strarr = BitConverter.ToString(array).Replace("-", " ");

    //            strarr = Functions.getBetween(strarr, "AA 55 ", " 55 AA");
    //            byte[] arr = Functions.FromHex(strarr);



    //            length = BitConverter.ToInt16(arr, 0);
    //            packet_id = BitConverter.ToInt16(arr, 2);
    //            statnum = BitConverter.ToInt16(arr, 4);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
    //        }
    //    }

    //}


}
