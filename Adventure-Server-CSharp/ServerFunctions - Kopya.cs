using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;

namespace Adventure_Server_CSharp
{



    public class ServerFunctions
    {
        public static void SelectCHAR(TcpClient client, byte[] array)
        {

            try
            {
                NetworkStream stream = client.GetStream();

                byte[] msg = Functions.FromHex("AA 55 02 00 CC 00 55 AA");
                stream.Write(msg, 0, msg.Length);

                msg = Functions.FromHex("AA 55 28 00 16 B1 00 B1 00 D2 00 00 00 21 00 00 00 00 00 00 00 00 00 00 00 00 00 00 20 03 00 00 00 00 00 00 00 00 00 00 10 27 00 00 55 AA ");
                stream.Write(msg, 0, msg.Length);

                msg = Functions.FromHex("AA 55 03 00 09 09 00 55 AA");
                stream.Write(msg, 0, msg.Length);
            }
            catch
            {

            }

        }

        public static void HandleDummyPackets(TcpClient client, byte[] array)
        {
            try
            {
                Member member = Server.Singleton.GetMemberByConnection(client);

                member.getKickTime = DateTime.Now.Second;
            }
            catch (Exception e)
            {

            }
        }

        public static void HandleItemDrop(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CItemDrop cItemDrop = new PacketTypes.CItemDrop(array);

                PacketTypes.SItemDrop sItemDrop = new PacketTypes.SItemDrop();

                sItemDrop.itemId = cItemDrop.itemId;
                sItemDrop.itemSlot = cItemDrop.itemSlot;

                Item item = new Item();

                item.itemId = cItemDrop.itemId;
                item.itemSlot = cItemDrop.itemSlot;

                Member member = Server.Singleton.GetMemberByConnection(client);

                Character[] characters = member.m_Characters;

                if (Database.database.RemoveItem(item, characters[0], member))
                {

                    if (sItemDrop.itemSlot < 0xB && sItemDrop.itemSlot != 0x4)
                    {
                        // cikarildi
                        Character character = characters[0];

                        Item item_info = Database.database.GetItemInfo(sItemDrop.itemId);


                        character.extraAccuracy -= item_info.accuracy;
                        character.extraMaxHp -= item_info.maxHp;
                        character.extraArtsAtk -= Convert.ToUInt64((item_info.minArtsDamage + item_info.maxArtsDamage) / 2);
                        character.extraArtsDef -= item_info.artsDefense;
                        character.extraDodge -= item_info.dodgerate;
                        character.extraMaxMana -= item_info.maxChi;
                        character.extraAttackPoint -= Convert.ToUInt16((item_info.minDamage + item_info.maxDamage) / 2);
                        character.extraAttackPoint -= Convert.ToUInt16((item_info.statMinDamage + item_info.statMaxDamage) / 2);

                        character.extraDefense -= item_info.defense;
                        character.extraDefense -= item_info.statDefense;
                        character.extraDefense -= item_info.rateDefense * character.defense;

                        character.extraStr -= item_info.strength;
                        character.extraDex -= item_info.dexterity;
                        character.extraIntelligence -= item_info.intelligence;
                        //to do: paraly,conf


                        Database.database.SaveCharacterInfo(character, member);

                        character.SendOutfitInfoToEveryone();
                    }


                    Item daxitem = characters[0].m_Inventory.m_Items.Find(x => x.itemSlot == item.itemSlot);

                    if (daxitem != default(Item))
                    {
                        characters[0].m_ToRemoveFromDatabase.Add(daxitem);
                        characters[0].m_Inventory.m_Items.Remove(daxitem);

                        byte[] msg = sItemDrop.getValue();
                        stream.Write(msg, 0, msg.Length);

                        Character character = characters[0];

                        character.SendOutfitInfoToEveryone();
                    }

                }
                else
                {
                    sItemDrop.itemSlot = 0;
                    byte[] msg = sItemDrop.getValue();
                    stream.Write(msg, 0, msg.Length);
                }
            }
            catch (Exception e)
            {

            }

        }


        public static void HandleItemDrag(TcpClient client, byte[] array)
        {
            PacketTypes.CItemDrag cItemDrag = new PacketTypes.CItemDrag(array);

            PacketTypes.SItemDrag sItemDrag = new PacketTypes.SItemDrag();

            sItemDrag.itemId = cItemDrag.itemId;
            sItemDrag.itemSlot = cItemDrag.itemSlot;
            sItemDrag.itemToSlot = cItemDrag.itemToSlot;
            NetworkStream stream = client.GetStream();

            try
            {

                Member member = Server.Singleton.GetMemberByConnection(client);

                Character[] characters = member.m_Characters;

                Item item = new Item();

                item.itemId = cItemDrag.itemId;
                item.itemSlot = cItemDrag.itemSlot;

                if (Database.database.DragItem(item, cItemDrag.itemToSlot, characters[0], member))
                {
                    item.itemSlot = cItemDrag.itemToSlot;

                    Item daxitem = characters[0].m_Inventory.m_Items.Find(x => x.itemSlot == cItemDrag.itemSlot);
                    daxitem.itemSlot = item.itemSlot;

                    characters[0].m_ToUpdateInDatabase.Add(daxitem);

                    byte[] msg = sItemDrag.getValue();
                    stream.Write(msg, 0, msg.Length);

                    if (sItemDrag.itemToSlot < 0xB && (sItemDrag.itemSlot >= 0xB || sItemDrag.itemSlot == 0x4) && sItemDrag.itemToSlot != 0x4)
                    {
                        // giyildi
                        Character character = characters[0];

                        Item item_info = Database.database.GetItemInfo(sItemDrag.itemId);

                        character.extraAccuracy += item_info.accuracy;
                        character.extraMaxHp += item_info.maxHp;
                        character.extraArtsAtk += Convert.ToUInt64((item_info.minArtsDamage + item_info.maxArtsDamage) / 2);
                        character.extraArtsDef += item_info.artsDefense;
                        character.extraDodge += item_info.dodgerate;
                        character.extraMaxMana += item_info.maxChi;
                        character.extraAttackPoint += Convert.ToUInt16((item_info.minDamage + item_info.maxDamage) / 2);
                        character.extraAttackPoint += Convert.ToUInt16((item_info.statMinDamage + item_info.statMaxDamage) / 2);

                        character.extraDefense += item_info.defense;
                        character.extraDefense += item_info.statDefense;
                        character.extraDefense += item_info.rateDefense * character.defense;

                        character.extraStr += item_info.strength;
                        character.extraDex += item_info.dexterity;
                        character.extraIntelligence += item_info.intelligence;

                        //to do: paraly,conf
                    

                        Database.database.SaveCharacterInfo(character, member);

                        character.SendOutfitInfoToEveryone();
                        character.SendCharacterActivateMessageToClient();
                    }

                    if((sItemDrag.itemSlot < 0xB && (sItemDrag.itemToSlot >= 0xB || sItemDrag.itemToSlot == 0x4) && sItemDrag.itemSlot != 0x4))
                    {
                        // cikarildi
                        Character character = characters[0];

                        Item item_info = Database.database.GetItemInfo(sItemDrag.itemId);


                        character.extraAccuracy -= item_info.accuracy;
                        character.extraMaxHp -= item_info.maxHp;
                        character.extraArtsAtk -= Convert.ToUInt64((item_info.minArtsDamage + item_info.maxArtsDamage) / 2);
                        character.extraArtsDef -= item_info.artsDefense;
                        character.extraDodge -= item_info.dodgerate;
                        character.extraMaxMana -= item_info.maxChi;
                        character.extraAttackPoint -= Convert.ToUInt16((item_info.minDamage + item_info.maxDamage) / 2);
                        character.extraAttackPoint -= Convert.ToUInt16((item_info.statMinDamage + item_info.statMaxDamage) / 2);

                        character.extraDefense -= item_info.defense;
                        character.extraDefense -= item_info.statDefense;
                        character.extraDefense -= item_info.rateDefense * character.defense;

                        character.extraStr -= item_info.strength;
                        character.extraDex -= item_info.dexterity;
                        character.extraIntelligence -= item_info.intelligence;
                        //to do: paraly,conf


                        Database.database.SaveCharacterInfo(character, member);

                        character.SendOutfitInfoToEveryone();
                        character.SendCharacterActivateMessageToClient();
                    }

                }
                else
                {
                    sItemDrag.itemSlot = item.itemSlot;
                    sItemDrag.itemId = item.itemId;
                    sItemDrag.itemToSlot = item.itemSlot;
                    byte[] msg = sItemDrag.getValue();
                    stream.Write(msg, 0, msg.Length);
                }
            }
            catch (Exception e)
            {
                sItemDrag.itemSlot = 0;
                sItemDrag.itemId = 0;
                sItemDrag.itemToSlot = 0;
                byte[] msg = sItemDrag.getValue();
                stream.Write(msg, 0, msg.Length);

                Console.WriteLine("HandleItemDrag: " + e.Message);
            }

        }

        public static void HandleItemDestroy(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CItemDestroy cItemDestroy = new PacketTypes.CItemDestroy(array);

                PacketTypes.SItemDestroy sItemDestroy = new PacketTypes.SItemDestroy();

                sItemDestroy.itemId = cItemDestroy.itemId;
                sItemDestroy.itemSlot = cItemDestroy.itemSlot;

                Member member = Server.Singleton.GetMemberByConnection(client);

                Character[] characters = member.m_Characters;

                Item item = new Item();

                item.itemId = sItemDestroy.itemId;
                item.itemSlot = sItemDestroy.itemSlot;

                Item daxitem = characters[0].m_Inventory.m_Items.Find(x => x.itemSlot == item.itemSlot);


                if (Database.database.RemoveItem(item, characters[0], member))
                {
                    if (sItemDestroy.itemSlot < 0xB && sItemDestroy.itemSlot != 0x4)
                    {
                        // cikarildi
                        Character character = characters[0];

                        Item item_info = Database.database.GetItemInfo(sItemDestroy.itemId);


                        character.accuracy -= item_info.accuracy;
                        character.maxhp -= item_info.maxHp;
                        character.artsatk -= Convert.ToUInt64((item_info.minArtsDamage + item_info.maxArtsDamage) / 2);
                        character.artsdef -= item_info.artsDefense;
                        character.dodge -= item_info.dodgerate;
                        character.maxmana -= item_info.maxChi;
                        character.attackpoint -= Convert.ToUInt16((item_info.minDamage + item_info.maxDamage) / 2);
                        character.attackpoint -= Convert.ToUInt16((item_info.statMinDamage + item_info.statMaxDamage) / 2);
                        character.defense -= item_info.defense;
                        character.str -= item_info.strength;
                        character.dex -= item_info.dexterity;
                        character.intelligence -= item_info.intelligence;
                        character.defense -= item_info.rateDefense * character.defense;
                        //to do: paraly,conf


                        Database.database.SaveCharacterInfo(character, member);

                        character.SendOutfitInfoToEveryone();
                    }

                    if (daxitem != default(Item))
                    {
                        characters[0].m_ToRemoveFromDatabase.Add(daxitem);
                        characters[0].m_Inventory.m_Items.Remove(daxitem);

                        byte[] msg = sItemDestroy.getValue();
                        stream.Write(msg, 0, msg.Length);

                        Character character = characters[0];

                        character.SendOutfitInfoToEveryone();
                    }
                }
                else
                {
                    sItemDestroy.itemSlot = 0;
                    byte[] msg = sItemDestroy.getValue();
                    stream.Write(msg, 0, msg.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("HandleItemDestroy: " + e.Message);
            }

        }

        public static void NPCS(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                foreach (var npc in Server.Singleton.m_GameNPCs)
                {
                    PacketTypes.SNPCInfo sNPCInfo = new PacketTypes.SNPCInfo();

                    sNPCInfo.npctype = npc.type;
                    sNPCInfo.posX = npc.posX;
                    sNPCInfo.posY = npc.posY;

                    byte[] msg = sNPCInfo.getValue();
                    stream.Write(msg, 0, msg.Length);
                }
            }

            catch
            {

            }



        }


        public static void NPCWindowRequest(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                int requestNPCId = 0;



                if ((array[2] == 8 && array[3] == 0 && array[4] == 0x57))
                {
                    var click = new PacketTypes.CNPCClick2(array);
                    requestNPCId = click.requestNPCId;
                }
                else
                {
                    var click = new PacketTypes.CNPCClick(array);
                    requestNPCId = click.requestNPCId;
                }



                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.WeaponStoreHyun), 0))
                {
                    NPC npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.WeaponStoreHyun);

                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    float diffX = character.posX - npc.posX;
                    float diffY = character.posY - npc.posY;

                    double diff = Math.Sqrt(diffX * diffX + diffY * diffY);

                    if (diff < 15)
                    {
                        byte[] msg = Functions.FromHex("AA 55 07 00 57 03 01 01 00 00 00 55 AA");
                        stream.Write(msg, 0, msg.Length); //npc click window
                    }

                }



                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.BankerSunny), 0))
                {
                    NPC npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.BankerSunny);

                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    float diffX = character.posX - npc.posX;
                    float diffY = character.posY - npc.posY;

                    double diff = Math.Sqrt(diffX * diffX + diffY * diffY);

                    if (diff < 15)
                    {
                        byte[] msg = Functions.FromHex("AA 55 07 00 57 03 01 03 00 00 00 55 AA");
                        stream.Write(msg, 0, msg.Length); //npc click window
                    }

                }


                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.BookManagerSeo), 0))
                {
                    NPC npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.BookManagerSeo);

                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    float diffX = character.posX - npc.posX;
                    float diffY = character.posY - npc.posY;

                    double diff = Math.Sqrt(diffX * diffX + diffY * diffY);

                    if (diff < 15)
                    {
                        byte[] msg = Functions.FromHex("AA 55 07 00 57 03 01 07 00 00 00 55 AA");
                        stream.Write(msg, 0, msg.Length); //npc click window
                    }

                }

                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.CastlePhysician), 0))
                {
                    NPC npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.CastlePhysician);

                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    float diffX = character.posX - npc.posX;
                    float diffY = character.posY - npc.posY;

                    double diff = Math.Sqrt(diffX * diffX + diffY * diffY);

                    if (diff < 15)
                    {
                        byte[] msg = Functions.FromHex("AA 55 07 00 57 03 01 09 00 00 00 55 AA");
                        stream.Write(msg, 0, msg.Length); //npc click window
                    }

                }


                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.LinenShopHwang), 0))
                {
                    NPC npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.LinenShopHwang);

                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    float diffX = character.posX - npc.posX;
                    float diffY = character.posY - npc.posY;

                    double diff = Math.Sqrt(diffX * diffX + diffY * diffY);

                    if (diff < 15)
                    {
                        byte[] msg = Functions.FromHex("AA 55 07 00 57 03 01 02 00 00 00 55 AA");
                        stream.Write(msg, 0, msg.Length); //npc click window
                    }

                }


                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.OrnamentStoreWang), 0))
                {
                    NPC npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.OrnamentStoreWang);

                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    float diffX = character.posX - npc.posX;
                    float diffY = character.posY - npc.posY;

                    double diff = Math.Sqrt(diffX * diffX + diffY * diffY);

                    if (diff < 15)
                    {
                        byte[] msg = Functions.FromHex("AA 55 07 00 57 03 01 08 00 00 00 55 AA");
                        stream.Write(msg, 0, msg.Length); //npc click window
                    }

                }


                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.StableMA), 0))
                {
                    NPC npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.StableMA);

                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    float diffX = character.posX - npc.posX;
                    float diffY = character.posY - npc.posY;

                    double diff = Math.Sqrt(diffX * diffX + diffY * diffY);

                    if (diff < 15)
                    {
                        byte[] msg = Functions.FromHex("AA 55 07 00 57 03 01 0A 00 00 00 55 AA");
                        stream.Write(msg, 0, msg.Length); //npc click window
                    }

                }

                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.TavernClerkSohae), 0))
                {
                    NPC npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkSohae);

                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    float diffX = character.posX - npc.posX;
                    float diffY = character.posY - npc.posY;

                    double diff = Math.Sqrt(diffX * diffX + diffY * diffY);

                    if (diff < 15)
                    {
                        byte[] msg = Functions.FromHex("AA 55 07 00 57 03 01 06 00 00 00 55 AA");
                        stream.Write(msg, 0, msg.Length); //npc click window
                    }

                }



                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.WarriorTrainer), 0))
                {


                }


                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.PhysicianTrainer), 0))
                {


                }


                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.LordSagun), 0))
                {

                }

                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.AssasinTrainer), 0))
                {


                }

                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.BlacksmithJang), 0))
                {


                }


                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.ChiCraftman), 0))
                {


                }

                if (requestNPCId == BitConverter.ToUInt16(BitConverter.GetBytes((int)NPCIds.HunterTrainer), 0))
                {

                }
            }
            catch
            {

            }
        }


        public static void NPCShopRequest(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                byte[] msg = Functions.FromHex("AA 55 07 00 57 03 01 01 00 00 00 55 AA");
                stream.Write(msg, 0, msg.Length); //SHOP WINDOW
            }
            catch
            {

            }


        }

        public static void HandleAutoLoginRequest(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CLoginReqMessage reqMessage = new PacketTypes.CLoginReqMessage(array);


                Member member = Database.database.GetMemberFromUsername(reqMessage.username);


                if (member.password.ToUpper() == reqMessage.password.ToUpper().Trim() && member.user_id > 0)
                {
                    PacketTypes.SLoginAckMessage ackMessage = new PacketTypes.SLoginAckMessage();

                    ackMessage.nickname = member.username;
                    ackMessage.password = member.password;

                    byte[] msg = ackMessage.getValue();
                    stream.Write(msg, 0, msg.Length); // login accept

                }

            }
            catch
            {

            }


        }



        public static void HandleLoginRequest(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CLoginReqMessage reqMessage = new PacketTypes.CLoginReqMessage(array);


                Member member = Database.database.GetMemberFromUsername(reqMessage.username);


                if (member.password.ToUpper() == reqMessage.password.ToUpper().Trim() && member.user_id > 0)
                {
                    PacketTypes.SLoginAckMessage ackMessage = new PacketTypes.SLoginAckMessage();

                    ackMessage.nickname = member.username;
                    ackMessage.password = member.password;

                    byte[] msg = ackMessage.getValue();
                    stream.Write(msg, 0, msg.Length); // login accept

                }
                else
                {
                    PacketTypes.SMessageBox messageBox = new PacketTypes.SMessageBox();
                    messageBox.message = "Hatali kullanici adi ya da sifre.";

                    byte[] msg = messageBox.getValue(); // ERROR CODE
                    stream.Write(msg, 0, msg.Length);
                }
            }
            catch
            {

            }

        }

        public static void HandleRedirectPlayer(TcpClient client, byte[] array)
        {
            try
            {
                PacketTypes.CRedirectFromServer redirectFromServer = new PacketTypes.CRedirectFromServer(array);
                string username = redirectFromServer.nickname;

                Member member = Database.database.GetMemberFromUsername(username);
                List<Character> characters = Database.database.GetCharactersOfMember(member);
                if(characters != null)
                {
                    member.m_Characters = characters.ToArray();
                    member.m_Connection = client;

                    for (int i = 0; i < member.m_Characters.Length; i++)
                    {
                        if (member.m_Characters[i] != null)
                            member.m_Characters[i].m_Connection = client;
                    }

                    member.getKickTime = DateTime.Now.Second;

                    Server.Singleton.m_InGameUsers.Add(member);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("HandleRedirectPlayer: " + e.Message);
            }

        }

        public static void HandleRequestServerList(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.SChannelListAckMessage ackMessage = new PacketTypes.SChannelListAckMessage();

                ackMessage.ServerName = "Hero";
                ackMessage.ChannelName = "Spirit - 1";

                ackMessage.HowMuchFull = Server.Singleton.m_InGameUsers.Count * 5 + 50;

                byte[] msg = ackMessage.getValue();
                stream.Write(msg, 0, msg.Length);
            }
            catch
            {

            }

        }

        public static void HandleRequestJoinChannel(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                string ip_addr = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

                IPAddress address = IPAddress.Parse(ip_addr);

                PacketTypes.SGameServerAddressAckMessage ackMessage = new PacketTypes.SGameServerAddressAckMessage();

                ackMessage.ip_address = Program.SERVER_IP;
                ackMessage.port = Program.SERVER_PORT;

                if (Functions.IsPrivateIPAddress(ip_addr))
                {
                    ackMessage.ip_address = "127.0.0.1";
                }


                byte[] msg = ackMessage.getValue();
                stream.Write(msg, 0, msg.Length); // ip:port
            }
            catch
            {

            }
        }

        public static void HandleStartGameRequest(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.SStartGameAckMessage ackMessage = new PacketTypes.SStartGameAckMessage();

                byte[] msg = ackMessage.getValue();
                stream.Write(msg, 0, msg.Length);
            }
            catch
            {

            }
        }

        public static void HandleItemCombine(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CItemCombineInfoRequest cItemCombineInfo = new PacketTypes.CItemCombineInfoRequest(array);

                PacketTypes.SInventoryItemCombine sInventoryItemCombine = new PacketTypes.SInventoryItemCombine();

                sInventoryItemCombine.fromItemSlot = cItemCombineInfo.fromWhere;
                sInventoryItemCombine.toItemSlot = cItemCombineInfo.toWhere;

                Member member = Server.Singleton.GetMemberByConnection(client);

                Character character = member.m_Characters[0];

                Item itemFrom = character.m_Inventory.m_Items.Find(x => x.itemSlot == sInventoryItemCombine.fromItemSlot);
                Item itemTo = character.m_Inventory.m_Items.Find(x => x.itemSlot == sInventoryItemCombine.toItemSlot);


                if (itemFrom.itemId == itemTo.itemId)
                {
                    ushort totalCount = Convert.ToUInt16(itemFrom.count + itemTo.count);

                    sInventoryItemCombine.itemId = itemTo.itemId;
                    sInventoryItemCombine.totalItemCount = totalCount;


                    Item newItem = new Item();

                    newItem.itemId = itemTo.itemId;
                    newItem.itemSlot = itemTo.itemSlot;
                    newItem.count = totalCount;

                    if (Database.database.RemoveItem(itemFrom, character, member) && Database.database.RemoveItem(itemTo, character, member))
                    {
                        character.m_Inventory.m_Items.Remove(itemFrom);
                        character.m_Inventory.m_Items.Remove(itemTo);

                        if (Database.database.AddItem(newItem, character, member))
                        {
                            character.m_Inventory.m_Items.Add(newItem);


                            byte[] msg = sInventoryItemCombine.getValue();
                            stream.Write(msg, 0, msg.Length);
                        }
                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine("HandleItemCombine: " + e.Message);
            }
        }

        public static void HandleItemSeperate(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CItemSeperateInfoRequest cItemSeperateInfo = new PacketTypes.CItemSeperateInfoRequest(array);

                PacketTypes.SInventoryItemSeperate sInventoryItemSeperate = new PacketTypes.SInventoryItemSeperate();

                sInventoryItemSeperate.seperateFrom = cItemSeperateInfo.fromWhere;
                sInventoryItemSeperate.seperateTo = cItemSeperateInfo.toWhere;
                sInventoryItemSeperate.seperateItemCount = cItemSeperateInfo.howMany;

                Member member = Server.Singleton.GetMemberByConnection(client);

                Character character = member.m_Characters[0];

                Item itemFrom = character.m_Inventory.m_Items.Find(x => x.itemSlot == sInventoryItemSeperate.seperateFrom);

                ushort leftCount = Convert.ToUInt16(itemFrom.count - cItemSeperateInfo.howMany);

                sInventoryItemSeperate.remainingItemCount = leftCount;
                sInventoryItemSeperate.itemId = itemFrom.itemId;

                Item newItem = new Item();
                Item newItem2 = new Item();

                newItem.itemId = itemFrom.itemId;
                newItem.itemSlot = itemFrom.itemSlot;
                newItem.count = leftCount;

                newItem2.itemId = itemFrom.itemId;
                newItem2.itemSlot = cItemSeperateInfo.toWhere;
                newItem2.count = cItemSeperateInfo.howMany;

                if (Database.database.RemoveItem(itemFrom, character, member))
                {
                    character.m_Inventory.m_Items.Remove(itemFrom);
                    if (Database.database.AddItem(newItem, character, member) && Database.database.AddItem(newItem2, character, member))
                    {
                        character.m_Inventory.m_Items.Add(newItem);
                        character.m_Inventory.m_Items.Add(newItem2);

                        byte[] msg = sInventoryItemSeperate.getValue();
                        stream.Write(msg, 0, msg.Length);
                    }

                }




            }
            catch (Exception e)
            {
                Console.WriteLine("HandleItemSeperate: " + e.Message);
            }
        }

        public static void HandleSwitchItemRequest(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CItemSwitchRequestMessage cItemSwitchRequestMessage = new PacketTypes.CItemSwitchRequestMessage(array);

                Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                Item toItem = character.m_Inventory.m_Items.Find(x => x.itemSlot == cItemSwitchRequestMessage.inventorySlot);
                Item fromItem = character.m_Inventory.m_Items.Find(x => x.itemSlot == cItemSwitchRequestMessage.characterSlot);


                if (toItem != default(Item) && fromItem != default(Item))
                {


                    if (Database.database.DragItem(fromItem, cItemSwitchRequestMessage.inventorySlot, character, character.member))
                    {
                        fromItem.itemSlot = cItemSwitchRequestMessage.inventorySlot;


                        if (Database.database.DragItem(toItem, cItemSwitchRequestMessage.characterSlot, character, character.member))
                        {
                            toItem.itemSlot = cItemSwitchRequestMessage.characterSlot;

                            PacketTypes.SItemSwitchAckMessage ackMessage = new PacketTypes.SItemSwitchAckMessage();

                            ackMessage.characterSlot = cItemSwitchRequestMessage.characterSlot;
                            ackMessage.inventorySlot = cItemSwitchRequestMessage.inventorySlot;
                            ackMessage.toItemId = cItemSwitchRequestMessage.toItemId;
                            ackMessage.fromItemId = cItemSwitchRequestMessage.fromItemId;

                            byte[] msg = ackMessage.getValue();
                            stream.Write(msg, 0, msg.Length);

                            //PacketTypes.SItemDestroy sItemDestroy = new PacketTypes.SItemDestroy();
                            //sItemDestroy.itemId = ackMessage.toItemId;
                            //sItemDestroy.itemSlot = ackMessage.characterSlot;

                            //msg = sItemDestroy.getValue();
                            //stream.Write(msg, 0, msg.Length);

                            //PacketTypes.SInventoryInfo sInventoryInfo = new PacketTypes.SInventoryInfo();

                            //sInventoryInfo.gold = character.gold;
                            //sInventoryInfo.itemSlot = ackMessage.characterSlot;
                            //sInventoryInfo.itemId = ackMessage.toItemId;

                            //msg = sInventoryInfo.getValue();
                            //stream.Write(msg, 0, msg.Length);

                            character.SendOutfitInfoToEveryone();


                            if(ackMessage.characterSlot < 0xB && ackMessage.inventorySlot >= 0xB && ackMessage.characterSlot != 0x4)
                            {
                                Item item_info = Database.database.GetItemInfo(ackMessage.fromItemId);

                                character.extraAccuracy -= item_info.accuracy;
                                character.extraMaxHp -= item_info.maxHp;
                                character.extraArtsAtk -= Convert.ToUInt64((item_info.minArtsDamage + item_info.maxArtsDamage) / 2);
                                character.extraArtsDef -= item_info.artsDefense;
                                character.extraDodge -= item_info.dodgerate;
                                character.extraMaxMana -= item_info.maxChi;
                                character.extraAttackPoint -= Convert.ToUInt16((item_info.minDamage + item_info.maxDamage) / 2);
                                character.extraAttackPoint -= Convert.ToUInt16((item_info.statMinDamage + item_info.statMaxDamage) / 2);

                                character.extraDefense -= item_info.defense;
                                character.extraDefense -= item_info.statDefense;
                                character.extraDefense -= item_info.rateDefense * character.defense;

                                character.extraStr -= item_info.strength;
                                character.extraDex -= item_info.dexterity;
                                character.extraIntelligence -= item_info.intelligence;

                                // giyildi

                                item_info = Database.database.GetItemInfo(ackMessage.toItemId);

                                character.extraAccuracy += item_info.accuracy;
                                character.extraMaxHp += item_info.maxHp;
                                character.extraArtsAtk += Convert.ToUInt64((item_info.minArtsDamage + item_info.maxArtsDamage) / 2);
                                character.extraArtsDef += item_info.artsDefense;
                                character.extraDodge += item_info.dodgerate;
                                character.extraMaxMana += item_info.maxChi;
                                character.extraAttackPoint += Convert.ToUInt16((item_info.minDamage + item_info.maxDamage) / 2);
                                character.extraAttackPoint += Convert.ToUInt16((item_info.statMinDamage + item_info.statMaxDamage) / 2);

                                character.extraDefense += item_info.defense;
                                character.extraDefense += item_info.statDefense;
                                character.extraDefense += item_info.rateDefense * character.defense;

                                character.extraStr += item_info.strength;
                                character.extraDex += item_info.dexterity;
                                character.extraIntelligence += item_info.intelligence;
                                //to do: paraly,conf


                                Database.database.SaveCharacterInfo(character, character.member);

                                character.SendOutfitInfoToEveryone();
                                character.SendCharacterActivateMessageToClient();
                            }

                            if(ackMessage.characterSlot >= 0xB && ackMessage.inventorySlot < 0xB && ackMessage.inventorySlot != 0x4)
                            {
                                // cikarildi

                                Item item_info = Database.database.GetItemInfo(ackMessage.fromItemId);

                                character.extraAccuracy -= item_info.accuracy;
                                character.extraMaxHp -= item_info.maxHp;
                                character.extraArtsAtk -= Convert.ToUInt64((item_info.minArtsDamage + item_info.maxArtsDamage) / 2);
                                character.extraArtsDef -= item_info.artsDefense;
                                character.extraDodge -= item_info.dodgerate;
                                character.extraMaxMana -= item_info.maxChi;
                                character.extraAttackPoint -= Convert.ToUInt16((item_info.minDamage + item_info.maxDamage) / 2);
                                character.extraAttackPoint -= Convert.ToUInt16((item_info.statMinDamage + item_info.statMaxDamage) / 2);

                                character.extraDefense -= item_info.defense;
                                character.extraDefense -= item_info.statDefense;
                                character.extraDefense -= item_info.rateDefense * character.defense;

                                character.extraStr -= item_info.strength;
                                character.extraDex -= item_info.dexterity;
                                character.extraIntelligence -= item_info.intelligence;
                                //to do: paraly,conf

                                item_info = Database.database.GetItemInfo(ackMessage.toItemId);

                                character.extraAccuracy += item_info.accuracy;
                                character.extraMaxHp += item_info.maxHp;
                                character.extraArtsAtk += Convert.ToUInt64((item_info.minArtsDamage + item_info.maxArtsDamage) / 2);
                                character.extraArtsDef += item_info.artsDefense;
                                character.extraDodge += item_info.dodgerate;
                                character.extraMaxMana += item_info.maxChi;
                                character.extraAttackPoint += Convert.ToUInt16((item_info.minDamage + item_info.maxDamage) / 2);
                                character.extraAttackPoint += Convert.ToUInt16((item_info.statMinDamage + item_info.statMaxDamage) / 2);

                                character.extraDefense += item_info.defense;
                                character.extraDefense += item_info.statDefense;
                                character.extraDefense += item_info.rateDefense * character.defense;

                                character.extraStr += item_info.strength;
                                character.extraDex += item_info.dexterity;
                                character.extraIntelligence += item_info.intelligence;
                                //to do: paraly,conf


                                Database.database.SaveCharacterInfo(character, character.member);

                                character.SendOutfitInfoToEveryone();
                                character.SendCharacterActivateMessageToClient();
                            }
                        }


                    }

                }


            }
            catch
            {

            }
        }

        public static void HandleSwitchWeaponRequest(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                Member member = Server.Singleton.GetMemberByConnection(client);

                if (member.m_Characters.Length > 0)
                {
                    Character character = member.m_Characters[0];

                    Item item1 = character.m_Inventory.m_Items.Find(x => x.itemSlot == 3);
                    Item item2 = character.m_Inventory.m_Items.Find(x => x.itemSlot == 4);

                    if (item1 != default(Item) && item2 != default(Item))
                    {
                        character.SwitchItems(item1, item2);
                    }
                }
            }

            catch
            {

            }

        }

        public static void HandleRequestCharacterInfo(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                // AA 55 39 02 01 02 0A 00 01 02 "+how many character+" " + character-slot + " AC 99 01 00 " + len(nickname) + nickname + " 35 02 "+ level +" 00 00 00 "+ map +" 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA

                Member member = Server.Singleton.GetMemberByConnection(client);

                member.m_Characters = Database.database.GetCharactersOfMember(member).ToArray();

                if (member.m_Characters != null && member.m_Characters.Length > 0 && member.m_Characters[0].nickname != null)
                {
                    Character character = member.m_Characters[0];
                    PacketTypes.SCharacterInfoAckMessage ackMessage = new PacketTypes.SCharacterInfoAckMessage();
                    ackMessage.nickname = character.nickname;
                    ackMessage.level = character.level;
                    ackMessage.evolution = character.evolution;
                    ackMessage.currentMap = character.currentMap;

                    if (character.m_Inventory.m_Items.Find(x => x.itemSlot == 2) != default(Item))
                        ackMessage.armorId = character.m_Inventory.m_Items.Find(x => x.itemSlot == 2).itemId;

                    if (character.m_Inventory.m_Items.Find(x => x.itemSlot == 3) != default(Item))
                        ackMessage.swordId = character.m_Inventory.m_Items.Find(x => x.itemSlot == 3).itemId;

                    if (character.m_Inventory.m_Items.Find(x => x.itemSlot == 0) != default(Item))
                        ackMessage.helmetId = character.m_Inventory.m_Items.Find(x => x.itemSlot == 0).itemId;

                    if (character.m_Inventory.m_Items.Find(x => x.itemSlot == 9) != default(Item))
                        ackMessage.footId = character.m_Inventory.m_Items.Find(x => x.itemSlot == 9).itemId;

                    //Console.WriteLine(characters[0] + " | Message: " + ackMessage.nickname + " : " + ackMessage.level + " : " + ackMessage.evolution + " : " + ackMessage.currentMap);

                    byte[] msg = ackMessage.getValue();//Functions.FromHex("AA 55 21 01 01 02 0A 00 01 02 01 00 0F 47 01 00 08 44 72 61 63 48 79 70 65 35 02 0A 00 00 00 02 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA");
                    stream.Write(msg, 0, msg.Length);
                }
                else
                {
                    PacketTypes.SCharacterInfoAckMessage ackMessage = new PacketTypes.SCharacterInfoAckMessage();
                    ackMessage.charactercount = 0;
                    byte[] msg = ackMessage.getValue();//Functions.FromHex("AA 55 21 01 01 02 0A 00 01 02 01 00 0F 47 01 00 08 44 72 61 63 48 79 70 65 35 02 0A 00 00 00 02 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA");
                    stream.Write(msg, 0, msg.Length);
                }
            }

            catch(Exception e)
            {
                Console.WriteLine("HandleRequestCharacterInfo: " + e.Message);

                NetworkStream stream = client.GetStream();

                PacketTypes.SCharacterInfoAckMessage ackMessage = new PacketTypes.SCharacterInfoAckMessage();
                ackMessage.charactercount = 0;
                byte[] msg = ackMessage.getValue();//Functions.FromHex("AA 55 21 01 01 02 0A 00 01 02 01 00 0F 47 01 00 08 44 72 61 63 48 79 70 65 35 02 0A 00 00 00 02 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA");
                stream.Write(msg, 0, msg.Length);
            }

        }

        public static void HandleMoveRequest(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CMoveReqMessage reqMessage = new PacketTypes.CMoveReqMessage(array);

                PacketTypes.SMoveAckMessage ackMessage = new PacketTypes.SMoveAckMessage();

                ackMessage.uid = Convert.ToUInt16(Server.Singleton.GetMemberByConnection(client).user_id);
                ackMessage.x1 = reqMessage.oldX;
                ackMessage.x2 = reqMessage.newX;
                ackMessage.y1 = reqMessage.oldY;
                ackMessage.y2 = reqMessage.newY;
                ackMessage.z1 = reqMessage.oldZ;
                ackMessage.z2 = reqMessage.newZ;

                Member member = Server.Singleton.GetMemberByConnection(client);

                //Console.WriteLine("Member id: " + member.user_id);

                if (member.m_Characters.Length > 0)
                {

                    //if (Database.database.SaveCharacterPosition(ackMessage.x1, ackMessage.y1, member, ))
                    {
                        member.m_Characters[0].posX = ackMessage.x1;
                        member.m_Characters[0].posY = ackMessage.y1;

                        byte[] msg = ackMessage.getValue();

                        Server.Singleton.SendAll(msg);
                    }

                }
            }
            catch (Exception e)
            {
                Server.Singleton.HandlePlayerQuitErrors();
                //Server.Singleton.m_InGameUsers.Remove(Server.Singleton.GetMemberByConnection(client));
            }


        }

        public static void HandleItemSell(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CSellRequestMessage cSellRequestMessage = new PacketTypes.CSellRequestMessage(array);

                PacketTypes.SSellItemMessage sSellItemMessage = new PacketTypes.SSellItemMessage();

                Item item = Database.database.GetItemInfo(cSellRequestMessage.itemId);

                Member member = Server.Singleton.GetMemberByConnection(client);

                Character character = member.m_Characters[0];

                Item toRemoveItem = character.m_Inventory.m_Items.Find(x => x.itemSlot == cSellRequestMessage.inventorySlot);

                ushort itemCount = toRemoveItem.count;


                if(Database.database.RemoveItem(toRemoveItem, character, member))
                {
                    character.m_Inventory.m_Items.Remove(toRemoveItem);



                    if (Database.database.SetGold(character.m_Gold + item.itemReturnPrice * itemCount, character,member))
                    {
                        character.m_Gold += item.itemReturnPrice * itemCount;

                        sSellItemMessage.gold = character.m_Gold;
                        sSellItemMessage.itemId = item.itemId;
                        sSellItemMessage.itemSlot = cSellRequestMessage.inventorySlot;

                        byte[] msg = sSellItemMessage.getValue();
                        stream.Write(msg, 0, msg.Length);
                    }
                 
                }

            }
            catch(Exception e)
            {
                Console.WriteLine("HandleItemSell: " + e.Message);
            }

        }


        public static void HandleBuyItem(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CBuyRequestMessage cBuyRequestMessage = new PacketTypes.CBuyRequestMessage(array);

                Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                ulong itemPrice = Database.database.GetItemInfo(cBuyRequestMessage.itemId).itemPrice; // TO DO

                cBuyRequestMessage.itemCount = Math.Min(cBuyRequestMessage.itemCount, Convert.ToUInt16(9999));
                cBuyRequestMessage.itemCount = Math.Max(cBuyRequestMessage.itemCount, Convert.ToUInt16(1));

                if (character.m_Gold >= itemPrice * cBuyRequestMessage.itemCount)
                {
                    Item item = new Item();

                    item.count = cBuyRequestMessage.itemCount;
                    item.itemId = cBuyRequestMessage.itemId;
                    item.itemSlot = cBuyRequestMessage.inventorySlot;

                    //character.m_Inventory.m_Items.Add(item);
                    //PacketTypes.SInventoryInfo sInventoryInfo = new PacketTypes.SInventoryInfo();

                    //sInventoryInfo.gold = character.gold;
                    //sInventoryInfo.itemId = item.itemId;
                    //sInventoryInfo.itemSlot = item.itemSlot;

                    //byte[] msg = sInventoryInfo.getValue();
                    //stream.Write(msg, 0, msg.Length);

                    character.m_Gold -= itemPrice * cBuyRequestMessage.itemCount;

                    if (Database.database.SetGold(character.m_Gold, character, character.member))
                    {
                        bool success = Database.database.AddItem(item, character, character.member);

                        if (success)
                        {
                            character.m_Inventory.m_Items.Add(item);
                            PacketTypes.SInventoryInfo sInventoryInfo = new PacketTypes.SInventoryInfo();

                            sInventoryInfo.gold = character.m_Gold;
                            sInventoryInfo.itemId = item.itemId;
                            sInventoryInfo.itemSlot = item.itemSlot;
                            sInventoryInfo.itemCount = cBuyRequestMessage.itemCount;

                            byte[] msg = sInventoryInfo.getValue();
                            stream.Write(msg, 0, msg.Length);
                        }

                        if (!success)
                        {
                            character.m_Gold += itemPrice * cBuyRequestMessage.itemCount;
                            Database.database.SetGold(character.m_Gold, character, character.member);

                            PacketTypes.SInventoryInfo sInventoryInfo = new PacketTypes.SInventoryInfo();
                            sInventoryInfo.gold = character.m_Gold;

                            byte[] msg = sInventoryInfo.getValue();
                            stream.Write(msg, 0, msg.Length);
                        }
                    }
                    else
                    {
                        character.m_Gold += itemPrice * cBuyRequestMessage.itemCount;

                        PacketTypes.SInventoryInfo sInventoryInfo = new PacketTypes.SInventoryInfo();
                        sInventoryInfo.gold = character.m_Gold;

                        byte[] msg = sInventoryInfo.getValue();
                        stream.Write(msg, 0, msg.Length);
                    }
                }
                else
                {
                    PacketTypes.SInventoryInfo sInventoryInfo = new PacketTypes.SInventoryInfo();
                    sInventoryInfo.gold = character.m_Gold;

                    byte[] msg = sInventoryInfo.getValue();
                    stream.Write(msg, 0, msg.Length);
                }
            }
            catch
            {

            }


        }


        public static void HandleItemUsage(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CItemUsage cItemUsage = new PacketTypes.CItemUsage(array);

                Member member = Server.Singleton.GetMemberByConnection(client);

                Character character = member.m_Characters[0];

                Item item = character.m_Inventory.m_Items.Find(x => x.itemSlot == cItemUsage.itemSlot);

                if (item != default(Item))
                {
                    Item itemInfo = Database.database.GetItemInfo(item.itemId);

                    switch(itemInfo.itemType)
                    {
                        case (ushort)ItemTypes.ITEMTYPE_CONSUMABLEITEM:
                            {
                                if (item.count > 0)
                                {
                                    character.curhp += itemInfo.healthPotMiktar;
                                    if (character.curhp > character.maxhp + character.extraMaxHp)
                                        character.curhp = character.maxhp + character.extraMaxHp;

                                    character.curmana += itemInfo.manaPotMiktar;
                                    if (character.curmana > character.maxmana + character.extraMaxMana)
                                        character.curmana = character.maxmana + character.extraMaxMana;

                                    PacketTypes.SHealthInfoAckMessage sHealthInfo = new PacketTypes.SHealthInfoAckMessage();
                                    sHealthInfo.health = character.curhp;
                                    sHealthInfo.mana = character.curmana;
                                    sHealthInfo.uid = Convert.ToUInt16(character.session_id);

                                    byte[] msg = sHealthInfo.getValue();
                                    stream.Write(msg, 0, msg.Length);
                                }

                                goto default;
                            }


                        default:
                            {
                                Item newItemToAdd = new Item(item);
                                newItemToAdd.count--;

                                PacketTypes.SItemUsage sItemUsage = new PacketTypes.SItemUsage();
                                sItemUsage.itemId = newItemToAdd.itemId;
                                sItemUsage.itemSlot = newItemToAdd.itemSlot;

                                if (Database.database.RemoveItem(item, character, member))
                                {
                                    character.m_Inventory.m_Items.Remove(item);

                                    Database.database.AddItem(newItemToAdd, character, member);
                                    character.m_Inventory.m_Items.Add(newItemToAdd);

                                    sItemUsage.itemCount = newItemToAdd.count;

                                    byte[] msg = sItemUsage.getValue();
                                    stream.Write(msg, 0, msg.Length);

                                }
                                else
                                {
                                    sItemUsage.itemCount = item.count;

                                    byte[] msg = sItemUsage.getValue();
                                    stream.Write(msg, 0, msg.Length);
                                }

                                break;
                            }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("HandleItemUsage: " + e);
            }
        }

        public static void HandleChatSlashMessage(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CSlashChatMessage chatMessage = new PacketTypes.CSlashChatMessage(array);

                if (chatMessage.message.Contains("/home") || chatMessage.message.Contains("/ home"))
                {
                    PacketTypes.STeleportMessage sTeleportMessage = new PacketTypes.STeleportMessage();

                    sTeleportMessage.posX = 337;
                    sTeleportMessage.posY = 201;

                    byte[] msg = sTeleportMessage.getValue();
                    stream.Write(msg, 0, msg.Length);

                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    character.posX = 337;
                    character.posY = 201;

                    character.SendEveryCharacterInfoToEveryone();
                }

                if (chatMessage.message.Contains("/createmob") || chatMessage.message.Contains("/ createmob"))
                {
                    PacketTypes.SMobInfo mobinfo = new PacketTypes.SMobInfo();
                    mobinfo.session_id = 27512;
                    mobinfo.level = 53;
                    mobinfo.maxHp = 70000;
                    mobinfo.curHp = 54544;
                    mobinfo.scale = 200;
                    mobinfo.skin = 40111;
                    mobinfo.x1 = 132;
                    mobinfo.y1 = 236;
                    mobinfo.z1 = 0;
                    mobinfo.x2 = 132;
                    mobinfo.y2 = 236;
                    mobinfo.z2 = 0;
                    byte[] toSend = mobinfo.getValue();
                    stream.Write(toSend, 0, toSend.Length);
                }

                if (chatMessage.message.Contains("/givegold") || chatMessage.message.Contains("/ givegold"))
                {
                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];
                    PacketTypes.SInventoryInfo sInventoryInfo = new PacketTypes.SInventoryInfo();

                    string msg = chatMessage.message.Replace(" ", "");

                    msg = msg.Substring(9, msg.Length - 9);

                    ulong toAddMoney = 0;
                    if (ulong.TryParse(msg, out toAddMoney))
                    {
                        character.m_Gold += toAddMoney;
                        if (Database.database.SetGold(character.m_Gold, character, character.member))
                        {
                            sInventoryInfo.gold = character.m_Gold;

                            byte[] x = sInventoryInfo.getValue();
                            stream.Write(x, 0, x.Length);
                        }

                    }

                }


                if (chatMessage.message.Contains("/setlevel") || chatMessage.message.Contains("/ setlevel"))
                {
                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];
                    PacketTypes.SInventoryInfo sInventoryInfo = new PacketTypes.SInventoryInfo();

                    string msg = chatMessage.message.Replace(" ", "");

                    msg = msg.Substring(9, msg.Length - 9);

                    byte newLevel = 1;
                    if (byte.TryParse(msg, out newLevel))
                    {
                        character.level = newLevel;
                        if (Database.database.SaveCharacterInfo(character, character.member))
                        {
                            character.SendCharacterActivateMessageToClient();
                        }

                    }

                }

                if (chatMessage.message.Contains("/giveitem") || chatMessage.message.Contains("/ giveitem"))
                {
                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];
                    PacketTypes.SInventoryInfo sInventoryInfo = new PacketTypes.SInventoryInfo();
                    sInventoryInfo.gold = character.m_Gold;
                    sInventoryInfo.itemSlot = 99;
                    for (ushort i = 11; i < 60; i++)
                    {
                        if (character.m_Inventory.m_Items.Find(x => x.itemSlot == i) == default(Item))
                        {
                            sInventoryInfo.itemSlot = i;
                            break;
                        }
                    }

                    if (sInventoryInfo.itemSlot == 99)
                        return;


                    string msg = chatMessage.message.Replace(" ", "");

                    msg = msg.Substring(9, msg.Length - 9);

                    uint itemId = 0;
                    if (uint.TryParse(msg, out itemId))
                    {
                        Item item = new Item();
                        item.itemId = itemId;
                        item.count = 1;
                        item.itemSlot = sInventoryInfo.itemSlot;




                        //if(character.m_Inventory.m_Items.Find(x => x.itemSlot == 0xB) == default(Item))
                        //{
                        //    character.m_Inventory.m_Items.Add(item);

                        //    sInventoryInfo.itemId = itemId;

                        //    byte[] x = sInventoryInfo.getValue();
                        //    stream.Write(x, 0, x.Length);
                        //}




                        if (Database.database.AddItem(item, character, character.member))
                        {
                            character.m_Inventory.m_Items.Add(item);

                            sInventoryInfo.itemId = itemId;

                            byte[] x = sInventoryInfo.getValue();
                            stream.Write(x, 0, x.Length);
                        }

                    }


                }
            }
            catch
            {

            }
        }



        public static void HandleStatPointAdd(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                Member member = Server.Singleton.GetMemberByConnection(client);

                //Console.WriteLine("Member id: " + member.user_id);


                if (member.user_id > 0 && member.m_Characters.Length > 0)
                {

                    PacketTypes.CStatPointAddReqMessage cAddStatReq = new PacketTypes.CStatPointAddReqMessage(array);

                    Character character_info = member.m_Characters[0];


                    if (character_info.statpoints >= cAddStatReq.howmanystat || character_info.naturepoints >= cAddStatReq.howmanystat)
                    {
                        if (cAddStatReq.whichstat == 0)
                        {
                            member.m_Characters[0].str += cAddStatReq.howmanystat;
                            member.m_Characters[0].maxhp += Convert.ToUInt32(10 * cAddStatReq.howmanystat);
                            member.m_Characters[0].attackpoint += Convert.ToUInt16(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].artsatk += Convert.ToUInt64(2 * cAddStatReq.howmanystat);
                            member.m_Characters[0].accuracy += Convert.ToUInt16(1 * cAddStatReq.howmanystat);
                            
                        }

                        else if (cAddStatReq.whichstat == 1)
                        {
                            member.m_Characters[0].dex += cAddStatReq.howmanystat;
                            member.m_Characters[0].defense += Convert.ToUInt32(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].artsdef += Convert.ToUInt32(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].dodge += Convert.ToUInt16(1 * cAddStatReq.howmanystat);
                        }

                        else if (cAddStatReq.whichstat == 2)
                        {
                            member.m_Characters[0].intelligence += cAddStatReq.howmanystat;
                            member.m_Characters[0].maxmana += Convert.ToUInt32(3 * cAddStatReq.howmanystat);
                            member.m_Characters[0].artsatk += Convert.ToUInt64(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].artsdef += Convert.ToUInt32(2 * cAddStatReq.howmanystat);
                        }
                        else if (cAddStatReq.whichstat == 3)
                        {
                            member.m_Characters[0].wind += cAddStatReq.howmanystat;
                            member.m_Characters[0].attackpoint += Convert.ToUInt16(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].maxhp += Convert.ToUInt32(10 * cAddStatReq.howmanystat);
                        }
                        else if (cAddStatReq.whichstat == 4)
                        {
                            member.m_Characters[0].water += cAddStatReq.howmanystat;
                            member.m_Characters[0].defense += Convert.ToUInt32(1 *cAddStatReq.howmanystat);
                            member.m_Characters[0].artsdef += Convert.ToUInt32(1 * cAddStatReq.howmanystat);
                        }
                        else if (cAddStatReq.whichstat == 5)
                        {
                            member.m_Characters[0].fire += cAddStatReq.howmanystat;
                            member.m_Characters[0].artsatk += Convert.ToUInt64(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].maxmana += Convert.ToUInt32(3 * cAddStatReq.howmanystat);
                        }

                        if (cAddStatReq.whichstat >= 0 && cAddStatReq.whichstat <= 2)
                        {
                            member.m_Characters[0].statpoints -= cAddStatReq.howmanystat;
                        }
                        if (cAddStatReq.whichstat >= 3 && cAddStatReq.whichstat <= 5)
                        {
                            member.m_Characters[0].naturepoints -= cAddStatReq.howmanystat;
                        }


                     
                    }



                    if(Database.database.SaveCharacterInfo(member.m_Characters[0], Server.Singleton.GetMemberByConnection(client)))
                    {
                        character_info.SendCharacterActivateMessageToClient();
                    }

                }
            }
            catch
            {

            }
        }

        public static void HandleChatMessage(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CChatMessage chatMessage = new PacketTypes.CChatMessage(array);

                PacketTypes.SChatMessageAckMessage ackMessage = new PacketTypes.SChatMessageAckMessage();

                Member member = Server.Singleton.GetMemberByConnection(client);


                if (member.m_Characters.Length > 0)
                {
                    ackMessage.nickname = member.m_Characters[0].nickname;
                    ackMessage.message = chatMessage.message;

                    byte[] msg = ackMessage.getValue();

                    Server.Singleton.SendAll(msg);
                }
            }
            catch { }
        }

        public static void HandleCreateCharacter(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CRegisterReqMessage cRegisterReq = new PacketTypes.CRegisterReqMessage(array);

                Member member = Server.Singleton.GetMemberByConnection(client);

                if (member.password != "nul" && cRegisterReq.race > 0)
                {
                    if (Database.database.CreateCharacter(cRegisterReq, member))
                    {
                        PacketTypes.SRegisterAckMessage sRegisterAck = new PacketTypes.SRegisterAckMessage();

                        sRegisterAck.nickname = cRegisterReq.nickname.Trim();

                        byte[] msg = sRegisterAck.getValue();
                        stream.Write(msg, 0, msg.Length);
                    }
                    else
                    {
                        //PacketTypes.SMessageBox sMessageBox = new PacketTypes.SMessageBox();
                        //sMessageBox.message = "Lutfen gereksinimleri karsiladiginizdan emin olun. (A-Z, a-z, 4-9, max 1 karakter)";

                        //byte[] msg = sMessageBox.getValue();
                        //stream.Write(msg, 0, msg.Length);
                    }
                }
            }
            catch { }
        }

        public static void HandleAttackInfo(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.SCharacterAttack sCharacterAttack = new PacketTypes.SCharacterAttack();
                PacketTypes.CAttackInfo cCharacterAttack = new PacketTypes.CAttackInfo(array);
                sCharacterAttack.CharSession_id = (ushort)Server.Singleton.GetMemberByConnection(client).m_Characters[0].session_id;
                sCharacterAttack.MobSession_id = cCharacterAttack.mobId;
                Server.Singleton.SendAll(sCharacterAttack.getValue());
                Console.WriteLine("MOB sesid:" + sCharacterAttack.MobSession_id + "char ses id:" + sCharacterAttack.CharSession_id);
            }
            catch (Exception ex) { Console.WriteLine(ex);  }

        }

                public static void HandleAttackDefendModeInfo(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CAttackDefendMode cAttackDefendMode = new PacketTypes.CAttackDefendMode(array);

                //Console.WriteLine("Attack MODE BOOL: " + cAttackDefendMode.attackModeBool);

                PacketTypes.SAttackDefendModeAckMessage sAttackDefendMode = new PacketTypes.SAttackDefendModeAckMessage();

                sAttackDefendMode.attackMode = cAttackDefendMode.attackModeBool;
                sAttackDefendMode.uid = Convert.ToUInt16(Server.Singleton.GetMemberByConnection(client).user_id);

                byte[] msg = sAttackDefendMode.getValue();
                Server.Singleton.SendAll(msg);
            }
            catch { }
        }

        public static void HandleMeditationInfo(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CMeditationState cMeditationState = new PacketTypes.CMeditationState(array);

                PacketTypes.SMeditationState sMeditationState = new PacketTypes.SMeditationState();

                sMeditationState.trueorfalse = cMeditationState.trueorfalse;
                sMeditationState.uid = Convert.ToUInt16(Server.Singleton.GetMemberByConnection(client).user_id);

                byte[] msg = sMeditationState.getValue();
                Server.Singleton.SendAll(msg);
            }
            catch { }
        }

        public static void HandleDeleteCharacter(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CDeleteCharacterReqMessage reqMessage = new PacketTypes.CDeleteCharacterReqMessage(array);

                Member member = Server.Singleton.GetMemberByConnection(client);

                if (Database.database.DeleteCharacter(reqMessage, member))
                {
                    ServerFunctions.HandleRequestCharacterInfo(client, array);
                }
            }
            catch
            {
            }
        }


        public static void HandleSelectServer(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                byte[] msg = Functions.FromHex("AA 55 03 00 09 08 00 55 AA");
                stream.Write(msg, 0, msg.Length);

                Member member = Server.Singleton.GetMemberByConnection(client);

                Database.database.SaveCharacterInfo(member.m_Characters[0], member);
                PacketTypes.SRemoveCharacterQuitMessage quitMessage = new PacketTypes.SRemoveCharacterQuitMessage();
                quitMessage.uid = Convert.ToUInt16(member.user_id);
                //member.m_Connection.Close();
                Server.Singleton.m_InGameUsers.Remove(member);

                Server.Singleton.SendAll(quitMessage.getValue());
            }
            catch { }
        }
        

        

        

      
        public static void HandleLoadingScreenInfos(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                // *** WARNING THIS IS IMPORTANT *** POSITION + MAP + CHARACTER INFOS

                Member member = Server.Singleton.GetMemberByConnection(client);

                //Console.WriteLine("Member id: " + member.user_id);
                //Console.WriteLine("Characters length: " + member.m_Characters.Length);

                if (member.m_Characters.Length > 0)
                {
                    Character character_info = member.m_Characters[0];

                    character_info.m_Connection = client;

                    /// SIngameCharacterInfoAckMessage
                    PacketTypes.SIngameCharacterInfoAckMessage ackMessage = new PacketTypes.SIngameCharacterInfoAckMessage();
                    {

                        ackMessage.nickname = character_info.nickname;
                        ackMessage.evolution = character_info.evolution;
                        ackMessage.level = Convert.ToByte(character_info.level);
                        ackMessage.posX = (int)character_info.posX;
                        ackMessage.posY = (int)character_info.posY;
                        ackMessage.uid = Convert.ToUInt16(member.user_id);
                        ackMessage.race = character_info.m_Race;

                    }


                    // - CHARACTER ACTIVATED
                    //PacketTypes.SCharacterActivateInfoAckMessage info = new PacketTypes.SCharacterActivateInfoAckMessage();
                    //{
                    //    info.level = character_info.level;
                    //    info.curexp = character_info.curexp;
                    //    info.maxexp = character_info.maxexp;
                    //    info.naturepoints = character_info.naturepoints;
                    //    info.statpoints = character_info.statpoints;
                    //    info.intel = character_info.intelligence;
                    //    info.str = character_info.str;
                    //    info.dex = character_info.dex;
                    //    info.water = character_info.water;
                    //    info.wind = character_info.wind;
                    //    info.fire = character_info.fire;
                    //    info.acc = character_info.accuracy;
                    //    info.artsatk = character_info.artsatk;
                    //    info.artsdef = character_info.artsdef;
                    //    info.def = character_info.defense;
                    //    info.atkspeed = character_info.atkspeed;
                    //    info.attackpoint = character_info.attackpoint;
                    //    info.critical = character_info.critical;
                    //    info.dodge = character_info.dodge;
                    //    info.race = character_info.m_Race;
                    //    info.maxhp = character_info.maxhp;
                    //    info.maxmana = character_info.maxmana;
                    //}
                    

                 


                    character_info.SendEveryCharacterInfoToEveryone();


                    character_info.SendEveryCharactersInfoToClient();

                    character_info.SendMobsInfoToClient();

                    //*** WARNING THIS IS IMPORTANT *** LOADING SCREEN GEÇEN KOMUT
                    {
                        byte[] msga = Functions.FromHex("AA 55 09 00 24 13 5B 61 42 4D FB 5D 43 55 AA");
                        stream.Write(msga, 0, msga.Length);  // OYUN AÇILIYOR FAKAT NPCLER YOK KISACASI LOADING EKRANINI GEÇEN KOMUT
                    }
                   

                    byte[] msg = ackMessage.getValue(); // SIngameCharacterInfoAckMessage
                    stream.Write(msg, 0, msg.Length);


                    //msg = info.getValue(); // SCharacterActivateInfoAckMessage
                    //stream.Write(msg, 0, msg.Length);
                    character_info.SendCharacterActivateMessageToClient();


                    PacketTypes.SHealthInfoAckMessage sHealthInfoAck = new PacketTypes.SHealthInfoAckMessage();
                    {
                        sHealthInfoAck.health = character_info.curhp;
                        sHealthInfoAck.mana = character_info.curmana;
                        sHealthInfoAck.uid = Convert.ToUInt16(member.user_id);
                    }

                    msg = sHealthInfoAck.getValue();
                    stream.Write(msg, 0, msg.Length);

                    character_info.SendInventoryInfoToClient();
                }
            }
            catch(Exception e) {
                Console.WriteLine("HandleLoadingScreen: " + e.Message);
            }
        }
    }
}
