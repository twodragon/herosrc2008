using Adventure_Server_CSharp.PacketTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public class Character
    {
        //

        public Member member;

        public uint session_id;

        public string nickname;

        public byte level;

        public uint currentMap;

        public byte evolution;

        public float posX;

        public float posY;

        public Vector3 position
        {
            get
            {
                return new Vector3(posX, posY);
            }
            set
            {
                posX = value.x;
                posY = value.y;
            }
        }

        public ushort statpoints;

        public ushort naturepoints;

        public float curexp, maxexp;

        public ushort str, extraStr, dex, extraDex, intelligence, extraIntelligence, wind, extraWind, water, extraWater, fire, extraFire, atkspeed, extraAtkspeed;

        public int maxhp, maxmana, extraMaxHp, extraMaxMana;

        public int curhp, curmana;

        public ushort attackpoint, extraAttackPoint;

        public uint defense, extraDefense;

        public ulong artsatk, extraArtsAtk;

        public uint artsdef, extraArtsDef;

        public ushort accuracy, extraAccuracy, dodge, extraDodge, critical, extraCritical;

        public byte m_Race;

        public ulong m_Gold;

        public ushort SkillPoints;

        public TcpClient m_Connection;
        public Inventory m_Inventory;
        public Inventory m_OldInventory;


        public List<Item> m_ToRemoveFromDatabase = new List<Item>(), m_ToUpdateInDatabase = new List<Item>(), m_ToCreateInDatabase = new List<Item>();


        public int GetDamage()
        {

            int random = Randomf.random.Next(1 * level, 10 * level);

            // int a = this.attackpoint + extraAttackPoint + random + extraFire + extraWater + extraStr + (int)(critical * 0.888);
            int a = this.attackpoint + random + fire + water + str + intelligence + (int)(critical * 0.888) + level;

            /*
            Debug.Log("----DAMAGE RESULT-------");
            Debug.Log("atkpont: " + this.attackpoint);
            Debug.Log("rrndm: " + random);
            Debug.Log("fire: " + fire);
            Debug.Log("water: " + water);
            Debug.Log("str: " + str);
            Debug.Log("int: " + intelligence);
            Debug.Log("int: " + intelligence);
            Debug.Log("exatkl: " + extraAttackPoint);
            Debug.Log("lv: " + level);
            Debug.Log("-");
            Debug.Log("TOTALDMG: " + a);
            */

            if (a < 0)
                return 0;
            else
                return a;

        }

        public int GetCalculateDamage(Character character)
        {
            int b = (GetDamage() - (int)(character.defense - character.dodge) - (character.dex - character.accuracy));

            if (b < 0)
                return 0;
            else
                return b;
        }

        public void CheckPlayerLevelUp(Character character = null)
        {
            if (character == null)
                character = this;

            if (character.curexp >= character.maxexp)
            {
                float diff = character.curexp - character.maxexp;

                character.curexp = diff;
                character.level += 1;
                character.statpoints += 4;

                if (character.level >= 50)
                    character.statpoints += 4;

                if (character.level >= 100)
                    character.naturepoints += 4;

                character.maxhp += 50;
                character.maxmana += 50;

                character.curhp = character.maxhp;
                character.curmana = character.maxmana;

                character.SkillPoints += 1;
            }

            character.SendCharacterActivateMessageToClient();
        }

        public void InitPlayerSkills(NetworkStream stream, uint book0, uint book1, uint book2, uint book3, uint book4, bool _10, bool _20, bool _45, bool _60, bool _100, byte book0UpgradeLevel, byte book1UpgradeLevel, byte book2UpgradeLevel, byte book3UpgradeLevel, byte book4UpgradeLevel, uint runSpeedBook, bool isRunSpeedActive, byte runSpeedLevel)
        {
            PacketTypes.SReadCombatArtBook sArtBook = new PacketTypes.SReadCombatArtBook();

            PacketTypes.SReadPassiveArtBook sPassiveBook = new PacketTypes.SReadPassiveArtBook();

            PacketTypes.SUpgradeSkillBook sUpgradeSkillBook = new PacketTypes.SUpgradeSkillBook();

            PacketTypes.SUpgradePassiveSkillBook sUpgradePassiveBook = new PacketTypes.SUpgradePassiveSkillBook();

            if (isRunSpeedActive && runSpeedBook != 0)
            {
                sPassiveBook.skillSlot = 8;
                sPassiveBook.itemId = runSpeedBook;

                byte[] msg = sPassiveBook.getValue();
                stream.Write(msg, 0, msg.Length);

                sUpgradePassiveBook.skillBookSlot = sPassiveBook.skillSlot;
                sUpgradePassiveBook.skillBookLevel = runSpeedLevel;

                byte[] msg2 = sUpgradePassiveBook.getValue();
                stream.Write(msg2, 0, msg2.Length);
            }


            if (_10 && book0 != 0)
            {
                sArtBook.skillSlot = 0;
                sArtBook.itemId = book0;

                byte[] msg = sArtBook.getValue();
                stream.Write(msg, 0, msg.Length);

                sUpgradeSkillBook.skillBookSlot = sArtBook.skillSlot;
                sUpgradeSkillBook.skillBookLevel = book0UpgradeLevel;

                byte[] msg2 = sUpgradeSkillBook.getValue();
                stream.Write(msg2, 0, msg2.Length);
            }

            if (_20 && book1 != 0)
            {
                sArtBook.skillSlot = 1;
                sArtBook.itemId = book1;

                byte[] msg = sArtBook.getValue();
                stream.Write(msg, 0, msg.Length);

                sUpgradeSkillBook.skillBookSlot = sArtBook.skillSlot;
                sUpgradeSkillBook.skillBookLevel = book1UpgradeLevel;

                byte[] msg2 = sUpgradeSkillBook.getValue();
                stream.Write(msg2, 0, msg2.Length);
            }

            if (_45 && book2 != 0)
            {
                sArtBook.skillSlot = 2;
                sArtBook.itemId = book2;

                byte[] msg = sArtBook.getValue();
                stream.Write(msg, 0, msg.Length);

                sUpgradeSkillBook.skillBookSlot = sArtBook.skillSlot;
                sUpgradeSkillBook.skillBookLevel = book2UpgradeLevel;

                byte[] msg2 = sUpgradeSkillBook.getValue();
                stream.Write(msg2, 0, msg2.Length);
            }

            if (_60 && book3 != 0)
            {
                sArtBook.skillSlot = 3;
                sArtBook.itemId = book3;

                byte[] msg = sArtBook.getValue();
                stream.Write(msg, 0, msg.Length);

                sUpgradeSkillBook.skillBookSlot = sArtBook.skillSlot;
                sUpgradeSkillBook.skillBookLevel = book3UpgradeLevel;

                byte[] msg2 = sUpgradeSkillBook.getValue();
                stream.Write(msg2, 0, msg2.Length);
            }

            if (_100 && book4 != 0)
            {
                sArtBook.skillSlot = 4;
                sArtBook.itemId = book4;

                byte[] msg = sArtBook.getValue();
                stream.Write(msg, 0, msg.Length);

                sUpgradeSkillBook.skillBookSlot = sArtBook.skillSlot;
                sUpgradeSkillBook.skillBookLevel = book4UpgradeLevel;

                byte[] msg2 = sUpgradeSkillBook.getValue();
                stream.Write(msg2, 0, msg2.Length);
            }
        }

        bool isWeaponEquipped = false;
        public void SendOutfitInfoToEveryone()
        {
            Character testmsg = this;

            PacketTypes.CCharacterOutfitInfoAckMessage ackMessagex = new PacketTypes.CCharacterOutfitInfoAckMessage();

            ackMessagex.uid = Convert.ToUInt16(testmsg.session_id);

            foreach (var item in testmsg.m_Inventory.m_Items)
            {
              //  Debug.Log("ITEM SLOT: " + item.itemSlot + " , ITEM ID:" + item.itemId + " , ITEM NAME:" + item.name + ", ITEM UPGRADE: " + item.upgrade_level, ConsoleColor.Yellow);
            }

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 1) != default(Item))
            {
              //  Debug.Log("ITEM TYPE: Mask" + ConsoleColor.Cyan);
                var mask = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 1);

                ackMessagex.maskId = mask.itemId;
                ackMessagex.maskUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(mask.itemSlot, (int)this.session_id);

            }

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 2) != default(Item))
            {
              //  Debug.Log("ITEM TYPE: Armor" + ConsoleColor.Cyan);
                var armor = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 2);
                ackMessagex.armorId = armor.itemId;
                ackMessagex.armorUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(armor.itemSlot, (int)this.session_id);
               // Debug.Log("armor upgrade lv: " + ackMessagex.armorUpgradeLv);
            }

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 3) != default(Item) && !isWeaponEquipped)
            {
                var wep = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 3);
                ackMessagex.weaponId = wep.itemId;
                ackMessagex.wepUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(wep.itemSlot, (int)this.session_id);

             //   Debug.Log("WEAPON ID: " + ackMessagex.weaponId + " TAKILDI.", ConsoleColor.Green);

                isWeaponEquipped = true;
            }

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 4) != default(Item) && !isWeaponEquipped)
            {
                var wep2 = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 4);
                ackMessagex.weaponId = wep2.itemId;
                ackMessagex.wep2UpgradeLv = (byte)Database.database.GetItemUpgradeLevel(wep2.itemSlot, (int)this.session_id);

              //  Debug.Log("WEAPON ID: " + ackMessagex.weaponId + " TAKILDI.", ConsoleColor.Green);

                isWeaponEquipped = true;
            }

            isWeaponEquipped = false;

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0) != default(Item))
            {
             //   Debug.Log("ITEM TYPE: Helmet", ConsoleColor.Cyan);

                var helm = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0);
                ackMessagex.helmetId = helm.itemId;
                ackMessagex.helmetUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(helm.itemSlot, (int)this.session_id);
            }

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 9) != default(Item))
            {
             //   Debug.Log("ITEM TYPE: Shoes", ConsoleColor.Cyan);
                var shoes = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 9);
                ackMessagex.shoesId = shoes.itemId;
                ackMessagex.shoesUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(shoes.itemSlot, (int)this.session_id);
            }

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0xA) != default(Item))
            {
             //   Debug.Log("ITEM TYPE: Pet", ConsoleColor.Cyan);

                ackMessagex.petId = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0xA).itemId;
            }

            byte[] msg = ackMessagex.getValue();

            Debug.Log("Item wear called!", ConsoleColor.Cyan);
            Server.Singleton.SendAll(msg);
        }

        public bool fastMode = false;
        public void SendCharacterActivateMessageToClient(bool isAdminCommand = false, ADMIN_MODE adminMode = ADMIN_MODE.Normal, int _runSpeed = -1)
        {
            if (isAdminCommand && adminMode == ADMIN_MODE.SpeedMode)
            {
                Debug.Log("Admin - Speedmod activated!", ConsoleColor.Green);
                fastMode = true;
            }

            if (isAdminCommand && adminMode == ADMIN_MODE.Normal)
            {
                Debug.Log("Admin - Speedmod deactivated!", ConsoleColor.Red);
                fastMode = false;
            }

            Debug.Log("----------------------", ConsoleColor.Red);
            try
            {
                if (m_Connection == null || !m_Connection.Connected)
                {
                    Debug.Log("Connection is null or not established.", ConsoleColor.Red);
                    return;
                }

                NetworkStream stream = m_Connection.GetStream();

                PacketTypes.SGetPlayerStats info = new PacketTypes.SGetPlayerStats();
                info.level = this.level;
                info.curexp = (float)this.curexp;
                Debug.Log("CUR EXP: " + (float)info.curexp);
                info.maxexp = (float)this.maxexp;
                Debug.Log("MAX EXP: " + (float)info.maxexp);
                info.naturepoints = this.naturepoints;
                info.statpoints = this.statpoints;
                info.intel = Convert.ToUInt16(this.intelligence + this.extraIntelligence);
                info.str = Convert.ToUInt16(this.str + this.extraStr);
                info.dex = Convert.ToUInt16(this.dex + this.extraDex);
                info.water = Convert.ToUInt16(this.water + this.extraWater);
                info.wind = Convert.ToUInt16(this.wind + this.extraWind);
                info.fire = Convert.ToUInt16(this.fire + this.extraFire);
                info.acc = Convert.ToUInt16(this.accuracy + this.extraAccuracy);
                info.artsatk = Convert.ToUInt32(this.artsatk + this.extraArtsAtk);
                info.artsdef = Convert.ToUInt32(this.artsdef + this.extraArtsDef);
                info.def = Convert.ToUInt32(this.defense + this.extraDefense);
                info.atkspeed = Convert.ToUInt16(this.atkspeed + this.extraAtkspeed);
                info.attackpoint = Convert.ToUInt16(this.attackpoint + this.extraAttackPoint);
                info.critical = Convert.ToUInt16(this.critical + this.extraCritical);
                info.dodge = Convert.ToUInt16(this.dodge + this.extraDodge);
                info.race = this.m_Race;
                info.maxhp = Convert.ToUInt32(this.maxhp + this.extraMaxHp);
                info.maxmana = Convert.ToUInt32(this.maxmana + this.extraMaxMana);
                info.skillPoints = SkillPoints;

                if (fastMode)
                {
                    info.runSpeed = 20.0f;
                }
                else
                {
                    info.runSpeed = 5.5f;
                }

                if(_runSpeed != -1)
                {
                    info.runSpeed = _runSpeed;
                }
                
                byte[] msg = info.getValue();

                Debug.Log("HEX COUNT: " + msg.Length, ConsoleColor.Red);

                Debug.Log("---------DONE-------------", ConsoleColor.Red);

                stream.Write(msg, 0, msg.Length);

                Database.database.SaveCharacterInfo(this, this.member);

                Database.database.GetCharacterSkillInfo((int)session_id, stream);
                Debug.Log("Character skill set success!", ConsoleColor.Cyan);

            }
            catch (Exception e)
            {
                Debug.Log("SendCharacterActivateMessageToClient: " + e.Message);
            }
        }

        public void SendOnlineDataToEveryone()
        {
            Character testmsg = this;

            PacketTypes.SIngameRemoteCharacterInfoAckMessage infox = new PacketTypes.SIngameRemoteCharacterInfoAckMessage();

            infox.uid = Convert.ToUInt16(member.user_id);
            infox.nickname = testmsg.nickname;
            infox.race = testmsg.m_Race;
            infox.evolution = testmsg.evolution;
            infox.posX = (int)testmsg.posX;
            infox.posY = (int)testmsg.posY;

            byte[] msg = infox.getValue();
            Server.Singleton.SendAll(msg);
        }

        public void SendEveryCharacterInfoToEveryone()
        {
            Character testmsg = this;

            var charPos = Database.database.GetCharacterPosition(testmsg.nickname);
            Database.database.SaveCharacterPos(testmsg.nickname, charPos.x, charPos.y);

            PacketTypes.SIngameRemoteCharacterInfoAckMessage infox = new PacketTypes.SIngameRemoteCharacterInfoAckMessage();

            infox.uid = Convert.ToUInt16(member.user_id);
            infox.nickname = testmsg.nickname;
            infox.race = testmsg.m_Race;
            infox.evolution = testmsg.evolution;
            infox.posX = testmsg.posX;
            infox.posY = testmsg.posY;

            byte[] msg = infox.getValue();
            Server.Singleton.SendAll(msg);
            
            PacketTypes.CCharacterOutfitInfoAckMessage ackMessagex = new PacketTypes.CCharacterOutfitInfoAckMessage();

            ackMessagex.uid = Convert.ToUInt16(testmsg.session_id);

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 1) != default(Item))
            {
                Debug.Log("ITEM TYPE: Mask" + ConsoleColor.Cyan);
                var mask = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 1);

                ackMessagex.maskId = mask.itemId;
                ackMessagex.maskUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(mask.itemSlot, (int)this.session_id);

            }

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 2) != default(Item))
            {
                Debug.Log("ITEM TYPE: Armor" + ConsoleColor.Cyan);
                var armor = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 2);
                ackMessagex.armorId = armor.itemId;
                ackMessagex.armorUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(armor.itemSlot, (int)this.session_id);
                Debug.Log("armor upgrade lv: " + ackMessagex.armorUpgradeLv);
            }

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 3) != default(Item) && !isWeaponEquipped)
            {
                var wep = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 3);
                ackMessagex.weaponId = wep.itemId;
                ackMessagex.wepUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(wep.itemSlot, (int)this.session_id);

                Debug.Log("WEAPON ID: " + ackMessagex.weaponId + " TAKILDI.", ConsoleColor.Green);

                isWeaponEquipped = true;
            }

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 4) != default(Item) && !isWeaponEquipped)
            {
                var wep2 = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 4);
                ackMessagex.weaponId = wep2.itemId;
                ackMessagex.wep2UpgradeLv = (byte)Database.database.GetItemUpgradeLevel(wep2.itemSlot, (int)this.session_id);

                Debug.Log("WEAPON ID: " + ackMessagex.weaponId + " TAKILDI.", ConsoleColor.Green);

                isWeaponEquipped = true;
            }

            isWeaponEquipped = false;

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0) != default(Item))
            {
                Debug.Log("ITEM TYPE: Helmet", ConsoleColor.Cyan);

                var helm = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0);
                ackMessagex.helmetId = helm.itemId;
                ackMessagex.helmetUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(helm.itemSlot, (int)this.session_id);
            }

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 9) != default(Item))
            {
                Debug.Log("ITEM TYPE: Shoes", ConsoleColor.Cyan);
                var shoes = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 9);
                ackMessagex.shoesId = shoes.itemId;
                ackMessagex.shoesUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(shoes.itemSlot, (int)this.session_id);
            }

            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0xA) != default(Item))
                ackMessagex.petId = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0xA).itemId;



            msg = ackMessagex.getValue();
            Server.Singleton.SendAll(msg);
        }

        public void SendInventoryInfoToClient()
        {

            if (m_Connection == null) return;

            NetworkStream stream = m_Connection.GetStream();

            var character_info = this;

            PacketTypes.SInventoryInfo sInventoryInfo = new PacketTypes.SInventoryInfo();
            PacketTypes.SCreateItemWithUpgradeLevel sCreateItemWithUpgradeLevel = new PacketTypes.SCreateItemWithUpgradeLevel();

            var invent = character_info.m_Inventory.m_Items;

            sInventoryInfo.gold = character_info.m_Gold;

            foreach (var daitem in invent)
            {
                if (daitem.upgrade_level <= 0)
                {
                    sInventoryInfo.gold = character_info.m_Gold;

                    sInventoryInfo.itemId = daitem.itemId;
                    sInventoryInfo.itemSlot = daitem.itemSlot;
                    sInventoryInfo.itemCount = daitem.count;

                    byte[] msgx = sInventoryInfo.getValue();

                    stream.Write(msgx, 0, msgx.Length);

                }
                else
                {
                    
                    sCreateItemWithUpgradeLevel.itemSlot = (byte)daitem.itemSlot;
                    sCreateItemWithUpgradeLevel.upgradeLevel = daitem.upgrade_level;
                    sCreateItemWithUpgradeLevel.itemId = daitem.itemId;

                    byte[] msgx = sCreateItemWithUpgradeLevel.getValue();

                    stream.Write(msgx, 0, msgx.Length);
                }
            }

            byte[] msg = sInventoryInfo.getValue();

            stream.Write(msg, 0, msg.Length);

        }

        public void SwitchItems(Item itemFrom, Item itemTo)
        {
            try
            {
                NetworkStream stream = m_Connection.GetStream();

                Character character = Server.Singleton.GetMemberByConnection(m_Connection).m_Characters[0];

                Item toItem = itemTo;
                Item fromItem = itemFrom;


                if (toItem != default(Item) && fromItem != default(Item))
                {

                    character.SendOutfitInfoToEveryone();
                    character.SendCharacterActivateMessageToClient();

                    if (Database.database.DragItem(fromItem, itemTo.itemSlot, character, character.member))
                    {
                        ushort oldFrom = fromItem.itemSlot;
                        fromItem.itemSlot = itemTo.itemSlot;

                        Item item_info = Database.database.GetItemInfo(itemFrom.itemId);

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


                        if (Database.database.DragItem(toItem, oldFrom, character, character.member))
                        {
                            toItem.itemSlot = oldFrom;

                            PacketTypes.SItemSwitchAckMessage ackMessage = new PacketTypes.SItemSwitchAckMessage();

                            ackMessage.characterSlot = itemFrom.itemSlot;
                            ackMessage.inventorySlot = itemTo.itemSlot;
                            ackMessage.toItemId = itemTo.itemId;
                            ackMessage.fromItemId = itemFrom.itemId;


                            item_info = Database.database.GetItemInfo(itemTo.itemId);

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


                            byte[] msg = ackMessage.getValue();
                            stream.Write(msg, 0, msg.Length);
                            character.SendCharacterActivateMessageToClient();
                            character.SendOutfitInfoToEveryone();
                        }


                    }

                }


            }
            catch (Exception e)
            {
                Debug.Log("SwitchItems: " + e.Message);
            }
        }

        public void SendEveryCharactersInfoToClient()
        {
            TcpClient client = m_Connection;

            if (client == null) return;

            NetworkStream stream = client.GetStream();
            for (int i = 0; i < Server.Singleton.m_InGameUsers.Count; i++)
            {
                // CHECK IF THIS == THIS
                {
                    string ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    string ip2 = ((IPEndPoint)Server.Singleton.m_InGameUsers[i].m_Connection.Client.RemoteEndPoint).Address.ToString();

                    int port = ((IPEndPoint)client.Client.RemoteEndPoint).Port;
                    int port2 = ((IPEndPoint)Server.Singleton.m_InGameUsers[i].m_Connection.Client.RemoteEndPoint).Port;

                    if (ip == ip2 && port == port2) continue;
                }

                if (Server.Singleton.m_InGameUsers[i].m_Characters.Length > 0)
                {

                    Character testmsg = Server.Singleton.m_InGameUsers[i].m_Characters[0];
                    // SEND INFOS TO CLIENT
                    {
                        // SIngameRemoteCharacterInfoAckMessage
                        {
                            PacketTypes.SIngameRemoteCharacterInfoAckMessage infox = new PacketTypes.SIngameRemoteCharacterInfoAckMessage();

                            infox.uid = Convert.ToUInt16(member.user_id);
                            infox.nickname = testmsg.nickname;
                            infox.race = testmsg.m_Race;
                            infox.evolution = testmsg.evolution;
                            infox.posX = (int)testmsg.posX;
                            infox.posY = (int)testmsg.posY;

                            byte[] msgx = infox.getValue();
                            stream.Write(msgx, 0, msgx.Length);
                        }


                        // CCharacterOutfitInfoAckMessage
                        {
                            PacketTypes.CCharacterOutfitInfoAckMessage ackMessagex = new PacketTypes.CCharacterOutfitInfoAckMessage();

                            ackMessagex.uid = Convert.ToUInt16(testmsg.session_id);

                            foreach (var item in testmsg.m_Inventory.m_Items)
                            {
                               // Debug.Log("ITEM SLOT: " + item.itemSlot + " , ITEM ID:" + item.itemId + " , ITEM NAME:" + item.name + ", ITEM UPGRADE: " + item.upgrade_level, ConsoleColor.Yellow);
                            }

                            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 1) != default(Item))
                            {
                              //  Debug.Log("ITEM TYPE: Mask" + ConsoleColor.Cyan);
                                var mask = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 1);

                                ackMessagex.maskId = mask.itemId;
                                ackMessagex.maskUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(mask.itemSlot, (int)this.session_id);

                            }

                            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 2) != default(Item))
                            {
                               // Debug.Log("ITEM TYPE: Armor" + ConsoleColor.Cyan);
                                var armor = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 2);
                                ackMessagex.armorId = armor.itemId;
                                ackMessagex.armorUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(armor.itemSlot, (int)this.session_id);
                               // Debug.Log("armor upgrade lv: " + ackMessagex.armorUpgradeLv);
                            }

                            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 3) != default(Item) && !isWeaponEquipped)
                            {
                                var wep = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 3);
                                ackMessagex.weaponId = wep.itemId;
                                ackMessagex.wepUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(wep.itemSlot, (int)this.session_id);

                               // Debug.Log("WEAPON ID: " + ackMessagex.weaponId + " TAKILDI.", ConsoleColor.Green);

                                isWeaponEquipped = true;
                            }

                            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 4) != default(Item) && !isWeaponEquipped)
                            {
                                var wep2 = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 4);
                                ackMessagex.weaponId = wep2.itemId;
                                ackMessagex.wep2UpgradeLv = (byte)Database.database.GetItemUpgradeLevel(wep2.itemSlot, (int)this.session_id);

                               // Debug.Log("WEAPON ID: " + ackMessagex.weaponId + " TAKILDI.", ConsoleColor.Green);

                                isWeaponEquipped = true;
                            }

                            isWeaponEquipped = false;

                            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0) != default(Item))
                            {
                              //  Debug.Log("ITEM TYPE: Helmet", ConsoleColor.Cyan);

                                var helm = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0);
                                ackMessagex.helmetId = helm.itemId;
                                ackMessagex.helmetUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(helm.itemSlot, (int)this.session_id);
                            }

                            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 9) != default(Item))
                            {
                              //  Debug.Log("ITEM TYPE: Shoes", ConsoleColor.Cyan);
                                var shoes = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 9);
                                ackMessagex.shoesId = shoes.itemId;
                                ackMessagex.shoesUpgradeLv = (byte)Database.database.GetItemUpgradeLevel(shoes.itemSlot, (int)this.session_id);
                            }

                            if (testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0xA) != default(Item))
                            {
                             //   Debug.Log("ITEM TYPE: Pet", ConsoleColor.Cyan);

                                ackMessagex.petId = testmsg.m_Inventory.m_Items.Find(x => x.itemSlot == 0xA).itemId;
                            }

                            byte[] msgx = ackMessagex.getValue();
                            stream.Write(msgx, 0, msgx.Length);
                        }
                    }

                }
            }
        }


        // Keep track of mobs that have been sent to the client
        private Dictionary<int, bool> sentMobs = new Dictionary<int, bool>();

        public async void SendMobsInfoToClient()
        {
            
            TcpClient client = m_Connection;
            if (client == null) return;

            NetworkStream stream = client.GetStream();
            var _character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

            for (int i = 0; i < Mob.mobList.Count; i++)
            {
                Mob mobmsg = Mob.mobList[i];

                if (mobmsg == null) continue;

                // Check if the mob is in the same map as the player
                if (mobmsg.currentMap != _character.currentMap) continue;

                // Calculate the distance between the mob and the player
                float distanceSquared = (mobmsg.position - _character.position).sqrMagnitude;

                // If the mob is within 64 meters and has already been sent, skip sending it
                if (distanceSquared <= 64)
                {
                    if (!sentMobs.ContainsKey(mobmsg.session_id))
                    {
                        // If the mob has not been sent, send the mob info
                        PacketTypes.SMobInfo infox = new PacketTypes.SMobInfo();

                        infox.session_id = Convert.ToUInt16(mobmsg.session_id);
                        infox.skin = mobmsg.skinId;
                        infox.curHp = mobmsg.curHp;
                        infox.maxHp = mobmsg.maxHp;
                        infox.x1 = (int)mobmsg.position.x;
                        infox.y1 = (int)mobmsg.position.y;
                        infox.z1 = (int)mobmsg.position.z;
                        infox.x2 = (int)mobmsg.position.x;
                        infox.y2 = (int)mobmsg.position.y;
                        infox.z2 = (int)mobmsg.position.z;
                        infox.level = mobmsg.level;
                        infox.scale = mobmsg.scale;

                        byte[] msgx = infox.getValue();
                        try
                        {
                            stream.Write(msgx, 0, msgx.Length);
                        }
                        catch(Exception ex) { }

                        // Mark the mob as sent

                        if (sentMobs.ContainsKey(mobmsg.session_id) == false)
                        {
                            try
                            {
                                sentMobs.Add(mobmsg.session_id, true);
                            }
                            catch (Exception ex) { Debug.Log("SendMobsInfoToClient: " + ex); }
                        }
                    }
                }
                else
                {
                    // If the mob is outside 64 meters, remove it from the sentMobs list
                    if (sentMobs.ContainsKey(mobmsg.session_id))
                    {
                        sentMobs.Remove(mobmsg.session_id);
                    }
                }

                await Task.Delay(50);
            }
        }
    }
}
