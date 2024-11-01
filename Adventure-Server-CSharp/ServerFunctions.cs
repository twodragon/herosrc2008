using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Drawing;
using System.Drawing.Text;
using System.Collections;
using System.Reflection;
using static Google.Protobuf.WellKnownTypes.Field.Types;
using MySql.Data.Types;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Adventure_Server_CSharp.PacketTypes;
using System.Runtime.InteropServices.ComTypes;
using Adventure_Server_CSharp.Tables;
using System.Xml.Schema;
using Adventure_Server_CSharp.PaketTurleri;

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

                if (member != null)
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

        public static void CreateDropItem(Character character, int itemId = 10210002)
        {

            int droppedItem = itemId;

            if (droppedItem <= 0) return;

            SMobItemDrop sMobItemDrop = new SMobItemDrop();
            sMobItemDrop.sessionId = (ushort)(3000 + ServerSettings.createDropItemIndex);
            sMobItemDrop.posX = character.posX;
            sMobItemDrop.posY = character.posY;
            sMobItemDrop.itemId = (uint)droppedItem;
            sMobItemDrop.claimerId = (ushort)character.session_id;

            byte[] x2 = sMobItemDrop.getValue();

            ++ServerSettings.createDropItemIndex;

            character.m_Connection.GetStream().Write(x2, 0, x2.Length);

            Debug.Log("Item dropped! itemId: " + sMobItemDrop.itemId);
        }

        public static void UpgradeItem(TcpClient client, byte[] array)
        {
            NetworkStream stream = client.GetStream();

            Member member = Server.Singleton.GetMemberByConnection(client);

            PacketTypes.CGetUpgradeInfo cGetUpgradeInfo = new PacketTypes.CGetUpgradeInfo(array);

            Debug.Log("cGetUpgradeInfo.itemSlot:  " + cGetUpgradeInfo.itemSlot, ConsoleColor.Cyan);

            SItemUpgrade sItemUpgrade = new SItemUpgrade();
            sItemUpgrade.itemSlot = cGetUpgradeInfo.itemSlot;
            sItemUpgrade.itemId = (uint)Utils.GetItemIdByItemSlot(member.m_Characters[0], sItemUpgrade.itemSlot);

            Random rnd = new Random();

            int roll = rnd.Next(0, 101);
            if (roll <= 70)
            {
                Debug.Log("ROLL SUCCESS: " + roll, ConsoleColor.Green);
                sItemUpgrade.isSucceed = 1;
            }
            else
            {
                Debug.Log("ROLL SUCCESS: " + roll, ConsoleColor.Red);
                sItemUpgrade.isSucceed = 0;


                // 
                PacketTypes.SItemDrop sItemDrop = new PacketTypes.SItemDrop();

                sItemDrop.itemId = sItemUpgrade.itemId;
                sItemDrop.itemSlot = sItemUpgrade.itemSlot;

                byte[] msg_deleteitem = sItemDrop.getValue();
                stream.Write(msg_deleteitem, 0, msg_deleteitem.Length);

                Database.database.RemoveItem(sItemDrop.itemSlot, (int)sItemDrop.itemId, member.m_Characters[0], member);

                byte[] msgxx = sItemUpgrade.getValue();
                stream.Write(msgxx, 0, msgxx.Length);

                //
                // DELETE UGPRADE MATERIAL
                PacketTypes.SItemDrop sItemDrop3 = new PacketTypes.SItemDrop();

                sItemDrop3.itemSlot = cGetUpgradeInfo.upgradeMaterialSlot;
                sItemDrop3.itemId = (uint)Utils.GetItemIdByItemSlot(member.m_Characters[0], cGetUpgradeInfo.upgradeMaterialSlot);

                byte[] msg_deleteitem3 = sItemDrop3.getValue();
                stream.Write(msg_deleteitem3, 0, msg_deleteitem3.Length);

                Database.database.RemoveItem(sItemDrop3.itemSlot, (int)sItemDrop3.itemId, member.m_Characters[0], member);

                //
                return;
            }

            //
            // DELETE UGPRADE MATERIAL
            PacketTypes.SItemDrop sItemDrop2 = new PacketTypes.SItemDrop();

            sItemDrop2.itemSlot = cGetUpgradeInfo.upgradeMaterialSlot;
            sItemDrop2.itemId = (uint)Utils.GetItemIdByItemSlot(member.m_Characters[0], cGetUpgradeInfo.upgradeMaterialSlot);

            byte[] msg_deleteitem2 = sItemDrop2.getValue();
            stream.Write(msg_deleteitem2, 0, msg_deleteitem2.Length);

            Database.database.RemoveItem(sItemDrop2.itemSlot, (int)sItemDrop2.itemId, member.m_Characters[0], member);

            //


            byte[] msg = sItemUpgrade.getValue();
            stream.Write(msg, 0, msg.Length);

            Item upgradedItem = new Item();

            upgradedItem.itemId = sItemUpgrade.itemId;
            upgradedItem.itemSlot = sItemUpgrade.itemSlot;
            upgradedItem.count = 1;

            SCreateItemWithUpgradeLevel sCreateItemWithUpgradeLevel = new SCreateItemWithUpgradeLevel();
            var _char = member.m_Characters[0];
            sCreateItemWithUpgradeLevel.itemId = sItemUpgrade.itemId;
            sCreateItemWithUpgradeLevel.itemSlot = sItemUpgrade.itemSlot;
            sCreateItemWithUpgradeLevel.upgradeLevel = (ushort)Database.database.GetItemUpgradeLevel(sCreateItemWithUpgradeLevel.itemSlot, (int)member.user_id) + 1;

            Debug.Log("upgrading from " + (ushort)Database.database.GetItemUpgradeLevel(sCreateItemWithUpgradeLevel.itemSlot, (int)member.user_id) + " to ... " + sCreateItemWithUpgradeLevel.upgradeLevel);

            byte[] msgx = sCreateItemWithUpgradeLevel.getValue();
            stream.Write(msgx, 0, msgx.Length);

            upgradedItem.upgrade_level = (ushort)sCreateItemWithUpgradeLevel.upgradeLevel;

            Database.database.UpgradeItem(upgradedItem, member.m_Characters[0], member);
        }

        public static void LootDroppedItem(TcpClient client, byte[] array)
        {
            NetworkStream stream = client.GetStream();

            Member member = Server.Singleton.GetMemberByConnection(client);

            PacketTypes.CGetLootItemInfo cGetLootItemInfo = new PacketTypes.CGetLootItemInfo(array);

            PacketTypes.SRemoveDroppedLootItem sRemoveDroppedItem = new PacketTypes.SRemoveDroppedLootItem();

            sRemoveDroppedItem.itemPseudoId = cGetLootItemInfo.itemPseudo;

            int itemId = CDropItem.GetItemIdByPseudoId((int)sRemoveDroppedItem.itemPseudoId);

            CreateItemInPlayerInventory(client, (uint)itemId);

            byte[] msg = sRemoveDroppedItem.getValue();
            stream.Write(msg, 0, msg.Length);

            byte[] msg2 = Functions.FromHex("AA 55 07 00 57 28 01 " + BitConverter.ToString(BitConverter.GetBytes(itemId)) + " 55 AA"); // item collect text
            stream.Write(msg2, 0, msg2.Length);

            CDropItem.DroppedItemsList.Remove((int)sRemoveDroppedItem.itemPseudoId);
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

                    if (sItemDrag.itemToSlot < 0xB && (sItemDrag.itemSlot >= 0xB || sItemDrag.itemSlot == 0x4))
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

                        Debug.Log("Item Equipped!", ConsoleColor.Yellow);


                        character.SendOutfitInfoToEveryone();
                        character.SendCharacterActivateMessageToClient();
                    }

                    if ((sItemDrag.itemSlot < 0xB && (sItemDrag.itemToSlot >= 0xB || sItemDrag.itemToSlot == 0x4)))
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

                        Debug.Log("Item Unequipped!", ConsoleColor.Yellow);

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

                Debug.Log("HandleItemDrag: " + e.Message);
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
                Debug.Log("HandleItemDestroy: " + e.Message);
            }

        }

        public static void NPCWindowRequest(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                int requestNPCId = 0;

                int ShopID = 0;
                // 01 weapon_store 02 armorshop 03 banker 05 bookerseo 08 neckshop 0A stableman 06 tavern

                int NPCWindowType = 0;

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

                Debug.Log("NPC ID: " + requestNPCId, ConsoleColor.Green);

                NPC npc = new NPC();

                bool isNPCAvailable = false;

                switch (requestNPCId)
                {
                    case (int)NPCIds.PhysicianTrainer:

                        Debug.Log("PhysicianTrainer opened!");
                        ShopID = 00;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.PhysicianTrainer);
                        isNPCAvailable = true;
                        break;

                    case (int)NPCIds.WeaponStoreHyun:

                        Debug.Log("WeaponStoreHyun opened!");
                        ShopID = 01;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.WeaponStoreHyun);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.WeaponStoreJin:

                        Debug.Log("WeaponStoreJin opened!");
                        ShopID = 01;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.WeaponStoreJin);
                        isNPCAvailable = true;

                        break;


                    case (int)NPCIds.OrnamentStoreWang:

                        Debug.Log("OrnamentStoreWang opened!");
                        ShopID = 08;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.OrnamentStoreWang);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.BlacksmithJang:

                        Debug.Log("BlacksmithJang opened!");
                        ShopID = 01;
                        NPCWindowType = 08;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.BlacksmithJang);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.Blacksmith:

                        Debug.Log("Blacksmith opened!");
                        ShopID = 01;
                        NPCWindowType = 08;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.Blacksmith);
                        isNPCAvailable = true;

                        break;


                    case (int)NPCIds.BlacksmithJinmu:

                        Debug.Log("BlacksmithJinmu opened!");
                        ShopID = 16;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.BlacksmithJinmu);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.BlacksmithKang:

                        Debug.Log("BlacksmithKang opened!");
                        ShopID = 01;
                        NPCWindowType = 08;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.BlacksmithKang);
                        isNPCAvailable = true;

                        break;


                    case (int)NPCIds.BlacksmithMyung:

                        Debug.Log("BlacksmithMyung opened!");
                        ShopID = 01;
                        NPCWindowType = 08;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.BlacksmithMyung);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.BlacksmithUnnamed:

                        Debug.Log("Blacksmith Unnamed opened!");
                        ShopID = 01;
                        NPCWindowType = 08;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.BlacksmithUnnamed);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.LinenShopHwang:

                        Debug.Log("LinenShopHwang opened!");
                        ShopID = 02;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.LinenShopHwang);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.LinenShopBaek:

                        Debug.Log("LinenShopBaek opened!");
                        ShopID = 17;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.LinenShopBaek);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.LinenShopNam:

                        Debug.Log("LinenShopNam opened!");
                        ShopID = 02;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.LinenShopNam);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.LinenShopYeon:

                        Debug.Log("LinenShopYeon opened!");
                        ShopID = 02;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.LinenShopYeon);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.BankerSunny:

                        Debug.Log("BankerSunny opened!");
                        ShopID = 01;
                        NPCWindowType = 05;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.BankerSunny);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.BookManagerSeo:

                        Debug.Log("BookManagerSeo opened!");
                        ShopID = 07;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.BookManagerSeo);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.BookstoreClerkWoon:

                        Debug.Log("BookstoreClerkWoon opened!");
                        ShopID = 18;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.BookstoreClerkWoon);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.TavernClerkSohae:

                        Debug.Log("TavernClerkSohae opened!");
                        ShopID = 06;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkSohae);
                        isNPCAvailable = true;

                        break;


                    case (int)NPCIds.TavernClerkLiu:

                        Debug.Log("TavernClerkLiu opened!");
                        ShopID = 06;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkLiu);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.TavernClerkChung:

                        Debug.Log("TavernClerkChung opened!");
                        ShopID = 06;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkChung);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.TavernClerkHae:

                        Debug.Log("TavernClerkHae opened!");
                        ShopID = 06;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkHae);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.TavernClerkJo:

                        Debug.Log("TavernClerkJo opened!");
                        ShopID = 06;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkJo);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.TavernClerkJun:

                        Debug.Log("TavernClerkJun opened!");
                        ShopID = 06;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkJun);
                        isNPCAvailable = true;

                        break;
                        
                    case (int)NPCIds.TavernClerkLynn:

                        Debug.Log("TavernClerkLynn opened!");
                        ShopID = 06;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkLynn);
                        isNPCAvailable = true;

                        break;


                    case (int)NPCIds.TavernClerkMagi:

                        Debug.Log("TavernClerkMagi opened!");
                        ShopID = 06;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkMagi);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.TavernClerkMing:

                        Debug.Log("TavernClerkMing opened!");
                        ShopID = 06;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkMing);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.TavernClerkSoso:

                        Debug.Log("TavernClerkSoso opened!");
                        ShopID = 06;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkSoso);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.TavernClerkSulHwa:

                        Debug.Log("TavernClerkSulHwa opened!");
                        ShopID = 06;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.TavernClerkSulHwa);
                        isNPCAvailable = true;

                        break;


                    case (int)NPCIds.AssasinTrainer:

                        Debug.Log("AssasinTrainer opened!");
                        ShopID = 00;
                        NPCWindowType = 02;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.AssasinTrainer);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.WarriorTrainer:

                        Debug.Log("WarriorTrainer opened!");
                        ShopID = 00;
                        NPCWindowType = 02;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.WarriorTrainer);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.LordSagun:

                        Debug.Log("LordSagun opened!");
                        ShopID = 00;
                        NPCWindowType = 02;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.LordSagun);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.HunterTrainer:

                        Debug.Log("HunterTrainer opened!");
                        ShopID = 00;
                        NPCWindowType = 02;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.HunterTrainer);
                        isNPCAvailable = true;

                        break;

                    case (int)NPCIds.StableMA:

                        Debug.Log("StableMA opened!");
                        ShopID = 10;
                        NPCWindowType = 03;
                        npc = Server.Singleton.m_GameNPCs.Find(x => x.type == (uint)NPCIds.StableMA);
                        isNPCAvailable = true;

                        break;

                    default:
                        break;
                }


                if (!isNPCAvailable) return;

                /*
                Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                
                float diffX = character.posX - npc.posX;
                float diffY = character.posY - npc.posY;

                double diff = Math.Sqrt(diffX * diffX + diffY * diffY);

                Debug.Log("diff: " + diff);
                if (diff < 15)
                */
                {

                    //shopid 01 window, shopid 03 shop, shopid
                    byte[] msg;
                    if (ShopID < 10)
                        msg = Functions.FromHex("AA 55 07 00 57 " + NPCWindowType.ToString("00") + " 01 " + ShopID.ToString("00") + " 00 00 00 55 AA");
                    else
                        msg = Functions.FromHex("AA 55 07 00 57 " + NPCWindowType.ToString("00") + " 01 " + ShopID.ToString("X2") + " 00 00 00 55 AA");

                    stream.Write(msg, 0, msg.Length); //npc click window
                    Debug.Log("Window Type: " + NPCWindowType.ToString("X2") + ", Shop ID:" + ShopID.ToString());
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

                //arr[5] == 02 NPC_TALK
                //arr[5] == 05 bank
                //arr[5] == 07 idk
                //arr[5] == 08 blacksmith
                //arr[5] == 0C SFX_1
                //arr[5] == 0D CREATE GUILD
                //arr[5] == 0F COMBINE
                //arr[5] == 12 NOTICE_START
                //arr[5] == 16 DISMANTLE
                //arr[5] == 17 EXTRACT
                //arr[5] == 18 WANTED
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

        public static void HandleUnknownPacket(TcpClient client, byte[] array)
        {

            try
            {
                
                NetworkStream stream = client.GetStream();

                // PacketTypes.CLoginReqMessage reqMessage = new PacketTypes.CLoginReqMessage(array);

                if (true)
                {

                    byte[] msg = Functions.FromHex("AA 55 09 00 24 13 5B 61 42 4D FB 5D 43 55 AA");
                    stream.Write(msg, 0, msg.Length); // loading screen remover

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

                if (member.password.ToUpper() == reqMessage.password.ToUpper().Trim() && member.user_id > 0 || true)
                {
                    PacketTypes.SLoginAckMessage ackMessage = new PacketTypes.SLoginAckMessage();

                    ackMessage.nickname = member.username;
                    ackMessage.password = member.password;

                    byte[] msg = ackMessage.getValue();
                    stream.Write(msg, 0, msg.Length); // login accept

                    Debug.Log("username: " + reqMessage.username);
                    Debug.Log("password: " + reqMessage.password);
                    Debug.Log("pass: " + member.password.ToUpper() + "reqpas: " + reqMessage.password.ToUpper().Trim());

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

        public static Member HandleRedirectPlayer(TcpClient client, byte[] array)
        {
            Member member = null;

            try
            {
                PacketTypes.CRedirectFromServer redirectFromServer = new PacketTypes.CRedirectFromServer(array);
                string username = redirectFromServer.nickname;

                Console.WriteLine(username + " connecting..");

                member = Database.database.GetMemberFromUsername(username);
                List<Character> characters = Database.database.GetCharactersOfMember(member);

                if (characters != null)
                {
                    member.m_Characters = characters.ToArray();
                    member.m_Connection = client;

                    for (int i = 0; i < member.m_Characters.Length; i++)
                    {
                        if (member.m_Characters[i] != null)
                        { 
                            member.m_Characters[i].m_Connection = client;
                            Debug.Log("Character " + member.m_Characters[i].nickname + " initialized.");
                        }
                    }

                    member.getKickTime = DateTime.Now.Second;

                    Server.Singleton.m_InGameUsers.Add(member);

                    
                }

                return member;

            }
            catch (Exception e)
            {
                Debug.Log("HandleRedirectPlayer: " + e.Message);
                return member;
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

                Console.WriteLine("IP adres: " + address.ToString() + " connected!");

                PacketTypes.SGameServerAddressAckMessage ackMessage = new PacketTypes.SGameServerAddressAckMessage();

                ackMessage.ip_address = Program.SERVER_IP;
                ackMessage.port = Program.SERVER_PORT;

                if (Functions.IsPrivateIPAddress(ip_addr))
                {
                    ackMessage.ip_address = ServerSettings.LOCAL_IPV4;
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
                Debug.Log("HandleItemCombine: " + e.Message);
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
                Debug.Log("HandleItemSeperate: " + e.Message);
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


                            if (ackMessage.characterSlot < 0xB && ackMessage.inventorySlot >= 0xB && ackMessage.characterSlot != 0x4)
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

                            if (ackMessage.characterSlot >= 0xB && ackMessage.inventorySlot < 0xB && ackMessage.inventorySlot != 0x4)
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

        public static void HandleRequestCharacterInfo(TcpClient client, byte[] array, Member memberReference = null)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                // AA 55 39 02 01 02 0A 00 01 02 "+how many character+" " + character-slot + " AC 99 01 00 " + len(nickname) + nickname + " 35 02 "+ level +" 00 00 00 "+ map +" 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA

                Member member = Server.Singleton.GetMemberByConnection(client);

                if(memberReference != null)
                {
                    member = memberReference;
                    Debug.Log(memberReference.username + " connected with second method!");
                }

                if(member == null)
                {
                    Debug.Log("ERROR, MEMBER IS NULL!", ConsoleColor.Red);

                    return;
                }

                member.m_Characters = Database.database.GetCharactersOfMember(member).ToArray();

                if (member.m_Characters != null && member.m_Characters.Length > 0)
                {
                    // Console.WriteLine("JOINING CHARACTER !!!!!!!!!\nJOINING CHARACTER !!!!!!!!!\nJOINING CHARACTER !!!!!!!!!\n");

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
            catch (Exception e)
            {
                Debug.Log("HandleRequestCharacterInfo: " + e.Message);

                NetworkStream stream = client.GetStream();

                PacketTypes.SCharacterInfoAckMessage ackMessage = new PacketTypes.SCharacterInfoAckMessage();
                ackMessage.charactercount = 0;
                byte[] msg = ackMessage.getValue();//Functions.FromHex("AA 55 21 01 01 02 0A 00 01 02 01 00 0F 47 01 00 08 44 72 61 63 48 79 70 65 35 02 0A 00 00 00 02 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 05 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 AA");
                stream.Write(msg, 0, msg.Length);
            }

        }

        public static void HandleMoveRequest(TcpClient client, byte[] array)
        {
            var arr = BitConverter.ToString(array);
            arr = arr.Substring(0, (arr.LastIndexOf("55-AA") + "AA-55".Length));

           // Debug.Log("move pkt: " + arr);

            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CMoveReqMessage reqMessage = new PacketTypes.CMoveReqMessage(array);

                PacketTypes.SMoveAckMessage ackMessage = new PacketTypes.SMoveAckMessage(); // if other players move problem check run type and run speed

                ackMessage.uid = Convert.ToUInt16(Server.Singleton.GetMemberByConnection(client).user_id);
                ackMessage.x1 = reqMessage.oldX;
                ackMessage.x2 = reqMessage.newX;
                ackMessage.y1 = reqMessage.oldY;
                ackMessage.y2 = reqMessage.newY;
                ackMessage.z1 = reqMessage.oldZ;
                ackMessage.z2 = reqMessage.newZ;
                ackMessage.runMode = reqMessage.runMode;

                //Debug.Log(ackMessage.runMode.ToString());

                if(ackMessage.runMode == 290)
                {
                    ackMessage.runSpeed = reqMessage.playerSpeed;
                    ackMessage.runType = 01;
                }
                else
                {
                    ackMessage.runSpeed = reqMessage.playerSpeed * 2f;
                    ackMessage.runType = 02;
                }

                

                // Console.WriteLine($"Moving FROM: {ackMessage.x1},{ackMessage.y1},{ackMessage.z1} TO: {ackMessage.x2},{ackMessage.y2},{ackMessage.z2}");

                Member member = Server.Singleton.GetMemberByConnection(client);

                if (member.m_Characters.Length > 0)
                {
                    // Console.WriteLine("USER " + member.user_id + " is moving...");

                    // if (Database.database.SaveCharacterPosition(ackMessage.x1, ackMessage.y1, member, ))
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


                if (Database.database.RemoveItem(toRemoveItem, character, member))
                {
                    character.m_Inventory.m_Items.Remove(toRemoveItem);



                    if (Database.database.SetGold(character.m_Gold + item.itemReturnPrice * itemCount, character, member))
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
            catch (Exception e)
            {
                Debug.Log("HandleItemSell: " + e.Message);
            }

        }


        public static void HandleBuyItem(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CBuyRequestMessage cBuyRequestMessage = new PacketTypes.CBuyRequestMessage(client, array);

                Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                ulong itemPrice = Database.database.GetItemInfo(cBuyRequestMessage.itemId).itemPrice; // TO DO

                cBuyRequestMessage.itemCount = Math.Min(cBuyRequestMessage.itemCount, Convert.ToUInt16(9999));
                cBuyRequestMessage.itemCount = Math.Max(cBuyRequestMessage.itemCount, Convert.ToUInt16(1));

                if (character.m_Gold >= itemPrice * cBuyRequestMessage.itemCount)
                {
                    Debug.Log("FREE INV PAGE: " + cBuyRequestMessage.inventorySlot);

                    if(cBuyRequestMessage.inventorySlot == 999) // 999 null
                    {
                        return;
                    }

                    Item item = new Item();

                    item.count = cBuyRequestMessage.itemCount;
                    item.itemId = cBuyRequestMessage.itemId;
                    item.itemSlot = cBuyRequestMessage.inventorySlot;

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

                            byte[] msg = sInventoryInfo.getValue(client);
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

        public static void ChangeMap(TcpClient client, byte[] array, string adminCommandMapId = "-1")
        {
            NetworkStream stream = client.GetStream();

            PacketTypes.CChangeMap cChangeMap = new PacketTypes.CChangeMap(client, array);

            Debug.Log("PORTAL ID: " + cChangeMap.PortalId, ConsoleColor.Cyan);

            PacketTypes.SChangeMap sChangeMap = new PacketTypes.SChangeMap();

            Member member = Server.Singleton.GetMemberByConnection(client);

            Character character = member.m_Characters[0];

            int _portalId = cChangeMap.PortalId;
            int _mapId = 0;
            Vector2 toTeleportPosition = new Vector2();

            string filePath = "..\\..\\Tables\\portals.csv";

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                if (adminCommandMapId != "-1")
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] values = line.Split(',');

                        if (values.Length > 2)
                        {
                            _mapId = byte.Parse(values[1]);


                            if (int.Parse(adminCommandMapId) == ushort.Parse(values[1]))
                            {
                                _portalId = ushort.Parse(values[0]);

                                break;
                            }

                        }
                    }
                }

                string line2;
                while ((line2 = reader.ReadLine()) != null)
                {
                    string[] values = line2.Split(',');

                    if (values.Length > 2)
                    {
                        if (_portalId == ushort.Parse(values[0]))
                        {
                            _mapId = ushort.Parse(values[1]);
                            toTeleportPosition.x = int.Parse(values[2]);
                            toTeleportPosition.y = int.Parse(values[3]);

                            break;
                        }
                    }
                }
            }

            Debug.Log("PORTAL ID: " + _portalId + ", MAP ID: " + _mapId, ConsoleColor.Cyan);

            sChangeMap.mapID = _mapId;
            sChangeMap.posX = ServerFunctions.GetSafeMapPosition(_mapId).x;
            sChangeMap.posY = ServerFunctions.GetSafeMapPosition(_mapId).y;

            byte[] msg = sChangeMap.getValue();
            stream.Write(msg, 0, msg.Length);

            Database.database.SetCharacterCurrentMap(character.nickname, (uint)sChangeMap.mapID);

            if (adminCommandMapId == "-1")
                ServerFunctions.HandleLoadingScreenInfos(client, array, toTeleportPosition, true); // normal teleport
            else
                ServerFunctions.HandleLoadingScreenInfos(client, array, toTeleportPosition, false); // gm teleport


            
        }

        public static int CalculateSkillDamage (int skillId)
        {
            int minDmg = 1;
            int maxDmg = 2;

            string filePath = "..\\..\\Tables\\skill_table.csv";

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    if (int.Parse(values[0]) != skillId) continue;

                    if (values.Length > 2)
                    {
                        minDmg = int.Parse(values[1]);
                        maxDmg = int.Parse(values[2]);

                        break;
                    }
                }
            }

            Random rnd = new Random();

            return rnd.Next(minDmg, maxDmg);
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

                    switch (itemInfo.itemType)
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
                                    sHealthInfo.debuffId = 1;

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
                                    Debug.Log("Item used so removed from inventory!", ConsoleColor.Cyan);

                                    if (item.count > 1)
                                    {
                                        Database.database.AddItem(newItemToAdd, character, member);
                                        character.m_Inventory.m_Items.Add(newItemToAdd);
                                    }

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

                    var skillBooks = SkillBook.GetCombatSkillBooksInfo();

                    foreach (var book in skillBooks)
                    {
                        if (book == itemInfo.itemId)
                        {
                            PacketTypes.SReadCombatArtBook sCombatArkBook = new PacketTypes.SReadCombatArtBook();
                            sCombatArkBook.itemId = itemInfo.itemId;
                            sCombatArkBook.skillSlot = SkillBook.GetSkillSlotId(itemInfo.neededLevel);

                            Debug.Log("Skill book is adding to slot " + sCombatArkBook.skillSlot);

                            byte[] msg = sCombatArkBook.getValue();
                            stream.Write(msg, 0, msg.Length);

                            Database.database.AddSkill(itemInfo, character, member, isPassive: 0);

                            break;
                        }
                    }

                    var passiveSkillBooks = SkillBook.GetPassiveSkillBooks();

                    foreach (var book in skillBooks)
                    {
                        if (book == itemInfo.itemId)
                        {
                            PacketTypes.SReadPassiveArtBook sPassiveArtBook = new PacketTypes.SReadPassiveArtBook();
                            sPassiveArtBook.itemId = itemInfo.itemId;
                            sPassiveArtBook.skillSlot = SkillBook.GetSkillSlotId(itemInfo.neededLevel, isPassive: 1);

                            Debug.Log("Skill book is adding to slot " + sPassiveArtBook.skillSlot);

                            byte[] msg = sPassiveArtBook.getValue();
                            stream.Write(msg, 0, msg.Length);

                            Database.database.AddSkill(itemInfo, character, member, isPassive: 1);

                            break;
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Debug.Log("HandleItemUsage: " + e);
            }
        }


        public static bool isFastModeEnabled = false;

        public static void HandleChatSlashMessage(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                PacketTypes.CSlashChatMessage chatMessage = new PacketTypes.CSlashChatMessage(array);

                var mapId = Database.database.GetCharacterCurrentMap(Server.Singleton.GetMemberByConnection(client).m_Characters[0].nickname);

                if (chatMessage.message == null) return;

                if (chatMessage.message.Contains("/home") || chatMessage.message.Contains("/ home"))
                {
                    
                    PacketTypes.STeleportMessage sTeleportMessage = new PacketTypes.STeleportMessage();

                    sTeleportMessage.posX = GetSafeMapPosition((int)mapId).x;
                    sTeleportMessage.posY = GetSafeMapPosition((int)mapId).y;

                    byte[] msg = sTeleportMessage.getValue();
                    stream.Write(msg, 0, msg.Length);


                    //Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    //character.posX = GetSafeMapPosition((int)mapId).x;
                    //character.posY = GetSafeMapPosition((int)mapId).y;

                    //character.SendEveryCharacterInfoToEveryone();

                    Debug.Log("[CSlashChatMessage] FIX CHARACTER POSITION!", ConsoleColor.Red);
                    
                }

                if (chatMessage.message.Contains("/createmob") || chatMessage.message.Contains("/ createmob"))
                {
                    string msg = chatMessage.message.Replace(" ", "");

                    if(msg.Length <= 12)
                    {
                        Debug.Log("ERROR, MOB ID NOT FOUND!");
                        return;
                    }

                    msg = msg.Substring(10, msg.Length - 10);

                    Mob mob = new Mob();
                    mob.session_id = (ushort)(500 + ServerSettings.createPseudoIndex);
                    mob.skinId = Convert.ToUInt16(msg);

                    var mobData = Mob.GetMobDataFromId(mob.skinId);
                    var clientData = Server.Singleton.GetMemberByConnection(client);

                    mob.level = mobData.level;
                    mob.maxHp = mobData.maxHp;
                    mob.curHp = mobData.curHp;
                    mob.scale = 100;
                    mob.position.x = clientData.m_Characters[0].posX;
                    mob.position.y = clientData.m_Characters[0].posY;
                    mob.position.z = 0;
                    mob.currentMap = clientData.m_Characters[0].currentMap;

                    Mob.mobList.Add(mob);

                    mob.Start(clientData);

                    Debug.Log("Mob " + mob.skinId + " spawned! PSEUDO: " + mob.session_id);

                    ServerSettings.createPseudoIndex++;
                }

                if (chatMessage.message.Contains("/createpet") || chatMessage.message.Contains("/ createpet"))
                {
                    string msg = chatMessage.message.Replace(" ", "");

                    if (msg.Length <= 12)
                    {
                        Debug.Log("ERROR, PET ID NOT FOUND!");
                        return;
                    }

                    msg = msg.Substring(10, msg.Length - 10);

                    Mob mob = new Mob();
                    mob.session_id = (ushort)(500 + ServerSettings.createPseudoIndex);
                    mob.skinId = Convert.ToUInt16(msg);

                    var mobData = Mob.GetMobDataFromId(mob.skinId);
                    var clientData = Server.Singleton.GetMemberByConnection(client);

                    mob.level = mobData.level;
                    mob.maxHp = mobData.maxHp;
                    mob.curHp = mobData.curHp;
                    mob.scale = 100;
                    mob.position.x = clientData.m_Characters[0].posX;
                    mob.position.y = clientData.m_Characters[0].posY;
                    mob.position.z = 0;
                    mob.currentMap = clientData.m_Characters[0].currentMap;

                    Mob.mobList.Add(mob);

                    mob.StartAsPet(clientData);

                    Debug.Log("Pet " + mob.skinId + " spawned! PSEUDO: " + mob.session_id);

                    ServerSettings.createPseudoIndex++;
                }

                if (chatMessage.message.Contains("/testmonsy") || chatMessage.message.Contains("/ testmonsy"))
                {
                    string msg = chatMessage.message.Replace(" ", "");

                    if (msg.Length <= 12)
                    {
                        Debug.Log("ERROR, MOB ID NOT FOUND!");
                        return;
                    }

                    msg = msg.Substring(10, msg.Length - 10);

                    Mob mob = new Mob();
                    mob.session_id = (ushort)(500 + ServerSettings.createPseudoIndex);
                    mob.skinId = Convert.ToUInt16(msg);

                    var mobData = Mob.GetMobDataFromId(mob.skinId);
                    var clientData = Server.Singleton.GetMemberByConnection(client);

                    mob.level = mobData.level;
                    mob.maxHp = 5000;
                    mob.curHp = 5000;
                    mob.scale = 100;
                    mob.position.x = clientData.m_Characters[0].posX;
                    mob.position.y = clientData.m_Characters[0].posY;
                    mob.position.z = 0;
                    mob.currentMap = clientData.m_Characters[0].currentMap;

                    Mob.mobList.Add(mob);


                    mob.Start(clientData);

                    Debug.Log("Mob " + mob.skinId + " spawned! PSEUDO: " + mob.session_id, ConsoleColor.Cyan);

                    ServerSettings.createPseudoIndex++;
                }

                if (chatMessage.message.Contains("/map") || chatMessage.message.Contains("/ map"))
                {
                    
                    string msg = chatMessage.message.Replace(" ", "");

                    if (msg.Length <= 3)
                    {
                        Debug.Log("ERROR, MOB ID NOT FOUND!");
                        return;
                    }

                    msg = msg.Substring(4, msg.Length - 4);
                    

                    ServerFunctions.ChangeMap(client, array, msg);

                    // ServerFunctions.HandleRequestCharacterInfo(client, array, Convert.ToUInt16(msg));
                }

                if (chatMessage.message.Contains("/tp") || chatMessage.message.Contains("/ tp"))
                {

                    ServerFunctions.TeleportCustomPosition(client, array, 0, 100, 100);

                    Server.Singleton.GetMemberByConnection(client).m_Characters[0].SendEveryCharacterInfoToEveryone();

                    // ServerFunctions.HandleRequestCharacterInfo(client, array, Convert.ToUInt16(msg));
                }

                if (chatMessage.message.Contains("/drop") || chatMessage.message.Contains("/ drop"))
                {
                    Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    string msg = chatMessage.message.Replace(" ", "");

                    if (msg.Length <= 3)
                    {
                        Debug.Log("ERROR, MOB ID NOT FOUND!");
                        return;
                    }

                    msg = msg.Substring(4, msg.Length - 4);

                    ServerFunctions.CreateDropItem(character);
                }


                if (chatMessage.message.Contains("/god") || chatMessage.message.Contains("/ god"))
                {
                    AdminCommandModes.GOD_MODE = !AdminCommandModes.GOD_MODE;
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
                        int oldLevel = character.level;
                        character.level = newLevel;
                        if (Database.database.SaveCharacterInfo(character, character.member))
                        {
                            character.SkillPoints += (ushort)(Math.Abs(newLevel - oldLevel));
                            character.SendCharacterActivateMessageToClient();
                        }

                    }

                }

                if (chatMessage.message.Contains("/givestr") || chatMessage.message.Contains("/ givestr"))
                {
                    Member member = Server.Singleton.GetMemberByConnection(client);

                    HandleStatPointAdd(member.m_Connection, new byte[512], 2, true);
                }

                if (chatMessage.message.Contains("/givedex") || chatMessage.message.Contains("/ givedex"))
                {
                    Member member = Server.Singleton.GetMemberByConnection(client);

                    HandleStatPointAdd(member.m_Connection, new byte[512], 1, true);
                }

                if (chatMessage.message.Contains("/giveint") || chatMessage.message.Contains("/ giveint"))
                {
                    Member member = Server.Singleton.GetMemberByConnection(client);

                    HandleStatPointAdd(member.m_Connection, new byte[512], 0, true);
                }

                
                if (chatMessage.message.Contains("/speed") || chatMessage.message.Contains("/ speed"))
                {
                    isFastModeEnabled = !isFastModeEnabled;

                    Member member = Server.Singleton.GetMemberByConnection(client);

                    Character character_info = member.m_Characters[0];

                    if (isFastModeEnabled)
                        character_info.SendCharacterActivateMessageToClient(isAdminCommand: true, ADMIN_MODE.SpeedMode);
                    else
                        character_info.SendCharacterActivateMessageToClient(isAdminCommand: true, ADMIN_MODE.Normal);
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

        public static void CreateItemInPlayerInventory(TcpClient client, uint itemId)
        {
            NetworkStream stream = client.GetStream();
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



            Item item = new Item();
            item.itemId = itemId;
            item.count = 1;
            item.itemSlot = sInventoryInfo.itemSlot;

            if (Database.database.AddItem(item, character, character.member))
            {
                character.m_Inventory.m_Items.Add(item);

                sInventoryInfo.itemId = itemId;

                byte[] x = sInventoryInfo.getValue();
                stream.Write(x, 0, x.Length);
            }

        }
        

        public static void TeleportMapSpawnPoint(TcpClient client, byte[] array, int mapId)
        {
            NetworkStream stream = client.GetStream();

            PacketTypes.STeleportMessage sTeleportMessage = new PacketTypes.STeleportMessage();

            sTeleportMessage.posX = 100;
            sTeleportMessage.posY = 100;

            switch (mapId)
            {
                case 1: // dragon castle
                    sTeleportMessage.posX = 337;
                    sTeleportMessage.posY = 201;
                    break;

                case 2: // highlands
                    sTeleportMessage.posX = 399;
                    sTeleportMessage.posY = 423;
                    break;

                case 3: // zehirli bataklik
                    sTeleportMessage.posX = 456;
                    sTeleportMessage.posY = 446;
                    break;

                case 4:
                    sTeleportMessage.posX = 16;
                    sTeleportMessage.posY = 43;
                    break;

                case 5:
                    sTeleportMessage.posX = 81;
                    sTeleportMessage.posY = 443;
                    break;

                case 6:
                    sTeleportMessage.posX = 113;
                    sTeleportMessage.posY = 135;
                    break;

                case 7:
                    sTeleportMessage.posX = 470;
                    sTeleportMessage.posY = 55;
                    break;

                case 8:
                    sTeleportMessage.posX = 449;
                    sTeleportMessage.posY = 67;
                    break;

                case 9:
                    sTeleportMessage.posX = 453;
                    sTeleportMessage.posY = 65;
                    break;

                case 10:
                    sTeleportMessage.posX = 289;
                    sTeleportMessage.posY = 281;
                    break;

                case 11:
                    sTeleportMessage.posX = 389;
                    sTeleportMessage.posY = 203;
                    break;

                case 12:
                    sTeleportMessage.posX = 249;
                    sTeleportMessage.posY = 281;
                    break;

                case 13:
                    sTeleportMessage.posX = 457;
                    sTeleportMessage.posY = 423;
                    break;

                case 14:
                    sTeleportMessage.posX = 261;
                    sTeleportMessage.posY = 151;
                    break;

                case 15:
                    sTeleportMessage.posX = 261;
                    sTeleportMessage.posY = 151;
                    break;

                case 16:
                    sTeleportMessage.posX = 217;
                    sTeleportMessage.posY = 91;
                    break;

                case 17:
                    sTeleportMessage.posX = 435;
                    sTeleportMessage.posY = 183;
                    break;

                case 18:
                    sTeleportMessage.posX = 139;
                    sTeleportMessage.posY = 427;
                    break;

                case 19:
                    sTeleportMessage.posX = 47;
                    sTeleportMessage.posY = 33;
                    break;

                default:
                    break;
            }

            byte[] msg = sTeleportMessage.getValue();
            stream.Write(msg, 0, msg.Length);

            var _char = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

            _char.posX = sTeleportMessage.posX;
            _char.posY = sTeleportMessage.posY;

            Database.database.SaveCharacterPos(_char.nickname, _char.posX, _char.posY);
        }

        public static void TeleportCustomPosition(TcpClient client, byte[] array, int mapId, int _posX, int _posY)
        {
            NetworkStream stream = client.GetStream();

            PacketTypes.STeleportMessage sTeleportMessage = new PacketTypes.STeleportMessage();

            sTeleportMessage.posX = _posX;
            sTeleportMessage.posY = _posY;

            byte[] msg = sTeleportMessage.getValue();
            stream.Write(msg, 0, msg.Length);

            var _char = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

            _char.posX = sTeleportMessage.posX;
            _char.posY = sTeleportMessage.posY;

            Database.database.SaveCharacterPos(_char.nickname, _char.posX, _char.posY);

        }

        public static Vector2 GetSafeMapPosition(int mapId)
        {
            Vector2 sTeleportMessage = new Vector2();

            sTeleportMessage.x = 100;
            sTeleportMessage.y = 100;

            switch (mapId)
            {
                case 1:
                    sTeleportMessage.x = 337;
                    sTeleportMessage.y = 201;
                    break;

                case 2:
                    sTeleportMessage.x = 399;
                    sTeleportMessage.y = 423;
                    break;

                case 3:
                    sTeleportMessage.x = 456;
                    sTeleportMessage.y = 446;
                    break;

                case 4:
                    sTeleportMessage.x = 16;
                    sTeleportMessage.y = 43;
                    break;

                case 5:
                    sTeleportMessage.x = 81;
                    sTeleportMessage.y = 443;
                    break;

                case 6:
                    sTeleportMessage.x = 113;
                    sTeleportMessage.y = 135;
                    break;

                case 7:
                    sTeleportMessage.x = 470;
                    sTeleportMessage.y = 55;
                    break;

                case 8:
                    sTeleportMessage.x = 449;
                    sTeleportMessage.y = 67;
                    break;

                case 9:
                    sTeleportMessage.x = 453;
                    sTeleportMessage.y = 65;
                    break;

                case 10:
                    sTeleportMessage.x = 289;
                    sTeleportMessage.y = 281;
                    break;

                case 11:
                    sTeleportMessage.x = 389;
                    sTeleportMessage.y = 203;
                    break;

                case 12:
                    sTeleportMessage.x = 249;
                    sTeleportMessage.y = 281;
                    break;

                case 13:
                    sTeleportMessage.x = 457;
                    sTeleportMessage.y = 423;
                    break;

                case 14:
                    sTeleportMessage.x = 261;
                    sTeleportMessage.y = 151;
                    break;

                case 15:
                    sTeleportMessage.x = 261;
                    sTeleportMessage.y = 151;
                    break;

                case 16:
                    sTeleportMessage.x = 217;
                    sTeleportMessage.y = 91;
                    break;

                case 17:
                    sTeleportMessage.x = 435;
                    sTeleportMessage.y = 183;
                    break;

                case 18:
                    sTeleportMessage.x = 139;
                    sTeleportMessage.y = 427;
                    break;

                case 19:
                    sTeleportMessage.x = 47;
                    sTeleportMessage.y = 33;
                    break;

                default:
                    break;
            }

            Debug.Log("Teleporting to: " + sTeleportMessage.x + ", " + sTeleportMessage.y);

            return sTeleportMessage;
        }

        public static void HandleStatPointAdd(TcpClient client, byte[] array, int statType = 0, bool isAdminCommand = false) //2 str 1 dex 0 int
        {
            try
            {
                NetworkStream stream = client.GetStream();

                Member member = Server.Singleton.GetMemberByConnection(client);

                //Console.WriteLine("Member id: " + member.user_id);


                if (member.user_id > 0 && member.m_Characters.Length > 0)
                {

                    PacketTypes.CStatPointAddReqMessage cAddStatReq = new PacketTypes.CStatPointAddReqMessage(array, statType);

                    Character character_info = member.m_Characters[0];

                    if (character_info.statpoints >= cAddStatReq.howmanystat || character_info.naturepoints >= cAddStatReq.howmanystat || isAdminCommand)
                    {

                        if (cAddStatReq.whichstat == 0)
                        {
                            if (isAdminCommand) cAddStatReq.howmanystat = 10;

                            member.m_Characters[0].str += cAddStatReq.howmanystat;
                            member.m_Characters[0].maxhp += Convert.ToInt32(10 * cAddStatReq.howmanystat);
                            member.m_Characters[0].attackpoint += Convert.ToUInt16(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].artsatk += Convert.ToUInt64(2 * cAddStatReq.howmanystat);
                            member.m_Characters[0].accuracy += Convert.ToUInt16(1 * cAddStatReq.howmanystat);

                        }

                        else if (cAddStatReq.whichstat == 1)
                        {
                            if (isAdminCommand) cAddStatReq.howmanystat = 10;

                            member.m_Characters[0].dex += cAddStatReq.howmanystat;
                            member.m_Characters[0].defense += Convert.ToUInt32(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].artsdef += Convert.ToUInt32(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].dodge += Convert.ToUInt16(1 * cAddStatReq.howmanystat);
                        }

                        else if (cAddStatReq.whichstat == 2)
                        {
                            if (isAdminCommand) cAddStatReq.howmanystat = 10;

                            member.m_Characters[0].intelligence += cAddStatReq.howmanystat;
                            member.m_Characters[0].maxmana += Convert.ToInt32(3 * cAddStatReq.howmanystat);
                            member.m_Characters[0].artsatk += Convert.ToUInt64(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].artsdef += Convert.ToUInt32(2 * cAddStatReq.howmanystat);
                        }
                        else if (cAddStatReq.whichstat == 3)
                        {
                            member.m_Characters[0].wind += cAddStatReq.howmanystat;
                            member.m_Characters[0].attackpoint += Convert.ToUInt16(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].maxhp += Convert.ToInt32(10 * cAddStatReq.howmanystat);
                        }
                        else if (cAddStatReq.whichstat == 4)
                        {
                            member.m_Characters[0].water += cAddStatReq.howmanystat;
                            member.m_Characters[0].defense += Convert.ToUInt32(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].artsdef += Convert.ToUInt32(1 * cAddStatReq.howmanystat);
                        }
                        else if (cAddStatReq.whichstat == 5)
                        {
                            member.m_Characters[0].fire += cAddStatReq.howmanystat;
                            member.m_Characters[0].artsatk += Convert.ToUInt64(1 * cAddStatReq.howmanystat);
                            member.m_Characters[0].maxmana += Convert.ToInt32(3 * cAddStatReq.howmanystat);
                        }

                        if (cAddStatReq.whichstat >= 0 && cAddStatReq.whichstat <= 2 && !isAdminCommand)
                        {
                            member.m_Characters[0].statpoints -= cAddStatReq.howmanystat;
                        }
                        if (cAddStatReq.whichstat >= 3 && cAddStatReq.whichstat <= 5 && !isAdminCommand)
                        {
                            member.m_Characters[0].naturepoints -= cAddStatReq.howmanystat;
                        }



                    }



                    if (Database.database.SaveCharacterInfo(member.m_Characters[0], Server.Singleton.GetMemberByConnection(client)))
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


                    Debug.Log("[HandleChatMessage] Send Message called!", ConsoleColor.Red);
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

                if (member.password != "nul" && cRegisterReq.race > 0 && cRegisterReq.nickname.Length < 10 && cRegisterReq.nickname.Length > 3)
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
                        /*
                        PacketTypes.SMessageBox sMessageBox = new PacketTypes.SMessageBox();
                        sMessageBox.message = "Lutfen gereksinimleri karsiladiginizdan emin olun. (A-Z, a-z, 4-9, max 1 karakter)";

                        byte[] msg = sMessageBox.getValue();
                        stream.Write(msg, 0, msg.Length);
                        */
                    }
                }
            }
            catch { }
        }




        public static void AddPartyMember(TcpClient client, byte[] array)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                Member member = Server.Singleton.GetMemberByConnection(client);

                PacketTypes.CPartyRequestAnswer cPartyReqAnswer = new PacketTypes.CPartyRequestAnswer(array);

                if (cPartyReqAnswer.isAccepted == 1) // pt accepted
                {
                    PacketTypes.SAddPartyMember sAddPartyMember = new PacketTypes.SAddPartyMember();

                    var _char = member.m_Characters[0];

                    sAddPartyMember.curHP = _char.curhp;
                    sAddPartyMember.maxHP = _char.maxhp;
                    sAddPartyMember.curMP = _char.curmana;
                    sAddPartyMember.maxMP = _char.maxmana;
                    sAddPartyMember.memberUID = _char.session_id;
                    sAddPartyMember.nickname = _char.nickname;
                    sAddPartyMember._class = _char.evolution;
                    sAddPartyMember._level = _char.level;

                    Debug.Log("Party req sending to: " + _char.nickname);
                    byte[] msg = sAddPartyMember.getValue();
                    Server.Singleton.SendToPlayer(CParty.LATEST_PARTY_TARGET_MEMBER, msg);


                    PacketTypes.SAddPartyMember sAddPartyMember2 = new PacketTypes.SAddPartyMember();
                    var _char2 = CParty.LATEST_PARTY_TARGET_MEMBER.m_Characters[0];

                    sAddPartyMember2.curHP = _char2.curhp;
                    sAddPartyMember2.maxHP = _char2.maxhp;
                    sAddPartyMember2.curMP = _char2.curmana;
                    sAddPartyMember2.maxMP = _char2.maxmana;
                    sAddPartyMember2.memberUID = _char2.session_id;
                    sAddPartyMember2.nickname = _char2.nickname;
                    sAddPartyMember2._class = _char2.evolution;
                    sAddPartyMember2._level = _char2.level;

                    Debug.Log("Party req sending to: " + _char2.nickname);
                    byte[] msg2 = sAddPartyMember2.getValue();
                    Server.Singleton.SendToPlayer(member, msg2);
                }
            }
            catch { }
        }

        public static void SendPartyRequest(TcpClient client, byte[] array)
        {
            try
            {// cSendPartyInfo.uid

                NetworkStream stream = client.GetStream();

                Member member = Server.Singleton.GetMemberByConnection(client);

                PacketTypes.CGetTargetPartyMemberInfo cSendPartyInfo = new PacketTypes.CGetTargetPartyMemberInfo(array);

                PacketTypes.SSendPartyInvite sSendPartyInvite = new PacketTypes.SSendPartyInvite();

                var targetPlayer = Server.Singleton.GetMemberFromSessionId(cSendPartyInfo.uid);

                sSendPartyInvite.nickname = member.m_Characters[0].nickname;

                byte[] msg = sSendPartyInvite.getValue();
                targetPlayer.m_Connection.GetStream().Write(msg, 0, msg.Length);

                CParty.LATEST_PARTY_TARGET_MEMBER = member;
            }
            catch { }
        }


        public static void HandleAttackInfo(TcpClient client, byte[] array, bool isSkill = false)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                PacketTypes.CSkillUseInfo cSkillUseInfo = null;
                PacketTypes.CAttackInfo cCharacterAttack = null;

                ushort mobid = 0;

                if (isSkill)
                {
                    cSkillUseInfo = new PacketTypes.CSkillUseInfo(array);
                    mobid = cSkillUseInfo.mobId;
                    Debug.Log("SKILL MOB ID: " + mobid);

                }
                else
                {
                    cCharacterAttack = new PacketTypes.CAttackInfo(array);
                    mobid = cCharacterAttack.mobId;

                    Debug.Log("NORMAL MOB ID: " + mobid);
                }
                
                Mob mob = Mob.mobList.Find(x => x.session_id == mobid);

                if (mob != null)
                    Debug.Log("Attacking mob, pseudo: " + mob.session_id);
                else
                {
                    Debug.Log("Mob is null ??!!", ConsoleColor.Red);
                }

                if(mob != default(Mob))
                {

                    int damage = character.GetDamage();


                    if (isSkill)
                    {
                        Random rnd = new Random();

                        Debug.Log("USED SKILL ID: " + cSkillUseInfo.skillId, ConsoleColor.Cyan);

                        damage += CalculateSkillDamage(cSkillUseInfo.skillId) * rnd.Next(2, 4);
                    }

                    if ( Convert.ToInt64(mob.curHp) - damage <= 0)
                    {
                        Debug.Log("Mob dead dmg: " + damage);
                        mob.OnDead(character);

                        mob.curHp = 0;
                    }
                    else
                    {
                        int hp = (int)mob.curHp - damage;

                        if (hp <= 0) hp = 0;

                        mob.curHp = (uint)hp;
                        Debug.Log("Mob alive Player Damage: " + damage);
                    }

                    PacketTypes.SHealthInfoAckMessage sHealthInfoAckMessage = new PacketTypes.SHealthInfoAckMessage();
                    Debug.Log("sHealthInfoAckMessage MOB CUR HP: " + mob.curHp,ConsoleColor.Yellow);
                    sHealthInfoAckMessage.health = (int)mob.curHp;
                    sHealthInfoAckMessage.uid = mob.session_id;
                    sHealthInfoAckMessage.debuffId = 0;

                    Server.Singleton.SendAll(sHealthInfoAckMessage.getValue());


                    if(!isSkill)
                    {

                    PacketTypes.SCharacterAttack sCharacterAttack = new PacketTypes.SCharacterAttack();
                    sCharacterAttack.CharSession_id = (ushort)Server.Singleton.GetMemberByConnection(client).m_Characters[0].session_id;
                    sCharacterAttack.MobSession_id = mobid;

                    Debug.Log(Server.Singleton.GetMemberByConnection(client).m_Characters[0].nickname + " attacking to " + "mob!");

                    Server.Singleton.SendAll(sCharacterAttack.getValue());
                    }
                    else
                    {
                        PacketTypes.SRemoteSkillAttack sRemoteSkillAttack = new PacketTypes.SRemoteSkillAttack();
                        var _char = Server.Singleton.GetMemberByConnection(client).m_Characters[0];
                        sRemoteSkillAttack.CharSession_id = (ushort)Server.Singleton.GetMemberByConnection(client).m_Characters[0].session_id;
                        sRemoteSkillAttack.MobSession_id = mobid;
                        sRemoteSkillAttack.skillID = cSkillUseInfo.skillId;
                        sRemoteSkillAttack.posX = _char.posX;
                        sRemoteSkillAttack.posY = _char.posY;
                        sRemoteSkillAttack.posZ = 0;

                        Debug.Log(Server.Singleton.GetMemberByConnection(client).m_Characters[0].nickname + " usingn skill to " + "mob!");

                        Server.Singleton.SendAll(sRemoteSkillAttack.getValue());
                    }

                }


                //Console.WriteLine("MOB sesid:" + sCharacterAttack.MobSession_id + "char ses id:" + sCharacterAttack.CharSession_id);
            }
            catch (Exception ex) { Console.WriteLine("ERROR 1002" + ex); }

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

        public static void RespawnPlayer(TcpClient client, byte[] array)
        {
            NetworkStream stream = client.GetStream();

            Member member = Server.Singleton.GetMemberByConnection(client);

            var player = member.m_Characters[0];

            player.SendCharacterActivateMessageToClient();

            TeleportMapSpawnPoint(client, array, (int)player.currentMap);

            PacketTypes.SHealthInfoAckMessage sHealthInfoAck = new PacketTypes.SHealthInfoAckMessage();
            {
                Debug.Log("player max hp: " + player.maxhp);
                sHealthInfoAck.health = player.maxhp;
                sHealthInfoAck.mana = player.maxmana;
                sHealthInfoAck.uid = Convert.ToUInt16(member.user_id);
            }

            var msg = sHealthInfoAck.getValue();
            stream.Write(msg, 0, msg.Length);

            player.curhp = player.maxhp;

            player.SendInventoryInfoToClient();
        }

        public static void RespawnPlayerCurrentPos(TcpClient client, byte[] array)
        {
            NetworkStream stream = client.GetStream();

            Member member = Server.Singleton.GetMemberByConnection(client);

            var player = member.m_Characters[0];

            player.SendCharacterActivateMessageToClient();

            TeleportCustomPosition(client, array, (int)player.currentMap, (int)player.posX, (int)player.posY);

            PacketTypes.SHealthInfoAckMessage sHealthInfoAck = new PacketTypes.SHealthInfoAckMessage();
            {
                Debug.Log("player max hp: " + player.maxhp);
                sHealthInfoAck.health = player.maxhp;
                sHealthInfoAck.mana = player.maxmana;
                sHealthInfoAck.uid = Convert.ToUInt16(member.user_id);
            }

            var msg = sHealthInfoAck.getValue();
            stream.Write(msg, 0, msg.Length);

            player.curhp = player.maxhp;

            player.SendInventoryInfoToClient();

            player.SendEveryCharacterInfoToEveryone();
        }

        public static void UpgradeSkillBook(TcpClient client, byte[] array)
        {
            NetworkStream stream = client.GetStream();

            Member member = Server.Singleton.GetMemberByConnection(client);

            var player = member.m_Characters[0];

            var skillPts = Database.database.GetCharacterCurrentSkillPts(player.nickname);

            if (skillPts <= 0) return;

            skillPts -= 1;

            PacketTypes.CUpgradedSkillBookSlot cSkillBookSlot = new PacketTypes.CUpgradedSkillBookSlot(array);

            int _skillBookLevel = 0;

            PacketTypes.SUpgradeSkillBook sUpgradeSkillBook = new PacketTypes.SUpgradeSkillBook();
            {
                sUpgradeSkillBook.skillBookSlot = cSkillBookSlot.upgradedSkillBookSlot;

                _skillBookLevel = Database.database.GetSkillBookLevel(player.session_id, cSkillBookSlot.upgradedSkillBookSlot);

                Debug.Log("CURRENT SKILL BOOK LEVEL :" + _skillBookLevel);

                _skillBookLevel += 1;

                Debug.Log("NEXT SKILL BOOK LEVEL :" + _skillBookLevel);

                sUpgradeSkillBook.skillBookLevel = (byte)_skillBookLevel;
            }

            var msg = sUpgradeSkillBook.getValue();
            stream.Write(msg, 0, msg.Length);

            player.SkillPoints = (ushort)skillPts;

            Database.database.UpgradeSkillBook(player, member, sUpgradeSkillBook.skillBookSlot, _skillBookLevel);

            player.SendCharacterActivateMessageToClient();
        }

        public static void UpgradePassiveSkillBook(TcpClient client, byte[] array)
        {
            NetworkStream stream = client.GetStream();

            Member member = Server.Singleton.GetMemberByConnection(client);

            var player = member.m_Characters[0];

            var skillPts = Database.database.GetCharacterCurrentSkillPts(player.nickname);

            if (skillPts <= 0) return;

            skillPts -= 1;

            PacketTypes.CUpgradedSkillBookSlot cSkillBookSlot = new PacketTypes.CUpgradedSkillBookSlot(array);

            int _skillBookLevel = 0;

            PacketTypes.SUpgradePassiveSkillBook sUpgradeSkillBook = new PacketTypes.SUpgradePassiveSkillBook();
            {
                sUpgradeSkillBook.skillBookSlot = cSkillBookSlot.upgradedSkillBookSlot;

                _skillBookLevel = Database.database.GetSkillBookLevel(player.session_id, cSkillBookSlot.upgradedSkillBookSlot, isPassive: 1);

                Debug.Log("CURRENT SKILL BOOK LEVEL :" + _skillBookLevel);

                _skillBookLevel += 1;

                Debug.Log("NEXT SKILL BOOK LEVEL :" + _skillBookLevel);

                sUpgradeSkillBook.skillBookLevel = (byte)_skillBookLevel;
            }

            var msg = sUpgradeSkillBook.getValue();
            stream.Write(msg, 0, msg.Length);

            player.SkillPoints = (ushort)skillPts;

            Database.database.UpgradeSkillBook(player, member, sUpgradeSkillBook.skillBookSlot, _skillBookLevel, isPassive: 1);

            player.SendCharacterActivateMessageToClient();
        }

        public static void HandleLoadingScreenInfos(TcpClient client, byte[] array, Vector2 toTeleportPosition = null, bool isCustomTeleport = false)
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

                    var currentMap = Database.database.GetCharacterCurrentMap(character_info.nickname);
                    /// SIngameCharacterInfoAckMessage
                    /// 

                    PacketTypes.SIngameCharacterInfoAckMessage ackMessage = new PacketTypes.SIngameCharacterInfoAckMessage();
                    {

                        ackMessage.nickname = character_info.nickname;
                        ackMessage.evolution = character_info.evolution;
                        ackMessage.currentMap = Database.database.GetCharacterCurrentMap(character_info.nickname);
                        ackMessage.posX = 100;
                        ackMessage.posY = 100;
                        ackMessage.uid = Convert.ToUInt16(member.user_id);
                        ackMessage.race = character_info.m_Race;

                        ackMessage.intel = Convert.ToUInt16(character_info.intelligence + character_info.extraIntelligence);
                        ackMessage.str = Convert.ToUInt16(character_info.str + character_info.extraStr);
                        ackMessage.dex = Convert.ToUInt16(character_info.dex + character_info.extraDex);
                        ackMessage.water = Convert.ToUInt16(character_info.water + character_info.extraWater);
                        ackMessage.wind = Convert.ToUInt16(character_info.wind + character_info.extraWind);
                        ackMessage.fire = Convert.ToUInt16(character_info.fire + character_info.extraFire);
                    }


                    //if (ClientSettings.VERSION != 222)
                    {
                        character_info.SendEveryCharacterInfoToEveryone(); // has problem in v222 need to fix

                        character_info.SendEveryCharactersInfoToClient();
                    }

                    if (ServerSettings.ENABLE_MOBS == true)
                        character_info.SendMobsInfoToClient();

                    //*** WARNING THIS IS IMPORTANT *** LOADING SCREEN GEÇEN KOMUT
                    {
                        //                   
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

                    
                    //var currentMap = Database.database.GetCharacterCurrentMap(character_info.nickname);
                    Debug.Log(character_info.nickname + ": " +"Current map: " + (int)currentMap);



                    if (!isCustomTeleport)
                    { 
                        TeleportMapSpawnPoint(client, array, (int)currentMap);
                        Debug.Log("TELEPORTED TO SAFE-ZONE!", ConsoleColor.Green);
                    }
                    else
                    {
                        TeleportCustomPosition(client, array, (int)currentMap, toTeleportPosition.x, toTeleportPosition.y);
                        Debug.Log("TELEPORTED TO GATE ENTERANCE!", ConsoleColor.Green);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("HandleLoadingScreen: " + e.Message);
            }
        }
    }
}
