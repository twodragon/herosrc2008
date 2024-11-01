using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace Adventure_Server_CSharp
{
    public class Member
    {
        public uint user_id;
        public string username;
        public string password; // sha256

        public Character[] m_Characters;
        public TcpClient m_Connection;

        public int getKickTime;
    }

    public class NPC
    {
        public string nickname;
        public uint type;
        public float posX, posY;
        public uint mapId;
    }



    public class Item
    {
        public Item()
        {

        }
        public Item(Item item)
        {
            databaseindex = item.databaseindex;
            itemSlot = item.itemSlot;
            itemId = item.itemId;
            count = item.count;
        }

        public uint databaseindex;
        public ushort itemSlot;
        public uint itemId;
        public ushort itemType;
        public ushort count;
        public ushort upgrade_level;

        public ushort healthPotMiktar;
        public ushort manaPotMiktar;
        public byte neededLevel;
        public uint itemPrice;
        public uint itemReturnPrice;
        public ushort minDamage;
        public ushort maxDamage;
        public byte attackRate;
        public ushort strength;
        public ushort dexterity;
        public ushort intelligence;
        public ushort statMinDamage;
        public ushort statMaxDamage;
        public ushort minArtsDamage;
        public ushort maxArtsDamage;
        public byte artsAttackRate;
        public ushort artsDefense;
        public byte artsDefenseRate;
        public uint defense;
        public ushort statDefense;
        public ushort maxChi;
        public ushort chiRecovery;
        public ushort maxHp;
        public ushort hpRecovery;
        public float runSpeedStat;
        public ushort accuracy;
        public ushort dodgerate;
        public ushort confusionDefense;
        public ushort paralysisDefense;
        public ushort poisonDefense;
        public byte poisonDamage;
        public uint poisonDuration;
        public byte paralysisDamage;
        public uint paralysisDuration;
        public byte rateDefense;
        public string name;
    }

    public class Inventory
    {
        public List<Item> m_Items = new List<Item>();
        public ulong gold;
    }

    public class Database
    {
        private MySqlConnection mySqlConnectionAuth;

        public Database(string username, string password, string db)
        {
            mySqlConnectionAuth = new MySqlConnection("Server=localhost;Database=" + db + ";Uid=" + username + ";Pwd='" + password + "';");
        }


        public void Connect()
        {
            try
            {
                mySqlConnectionAuth.Open();


                if (mySqlConnectionAuth.State != System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Couldn't connect to database.");
                }

            }
            catch (Exception err)
            {
                Console.WriteLine("Error: " + err.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                mySqlConnectionAuth.Close();


                if (mySqlConnectionAuth.State != System.Data.ConnectionState.Closed)
                {
                    Console.WriteLine("Couldn't close connect to database.");
                }

            }
            catch (Exception err)
            {
                Console.WriteLine("Error: " + err.Message);
            }
        }

        public Item GetItemInfo(uint itemid)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';");
                conn.Open();
                Item item = new Item();

                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM item_list WHERE itemid=@itemid";
                cmd.Parameters.AddWithValue("@itemid", itemid);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    item.neededLevel = reader.GetByte("level");

                    item.itemType = reader.GetUInt16("itemType");

                    item.itemPrice = reader.GetUInt32("price");
                    item.itemReturnPrice = reader.GetUInt32("returnprice");

                    item.minDamage = reader.GetUInt16("minDamage");
                    item.maxDamage = reader.GetUInt16("maxDamage");
                    item.attackRate = reader.GetByte("attackRate");

                    item.strength = reader.GetUInt16("strength");
                    item.dexterity = reader.GetUInt16("dexterity");
                    item.intelligence = reader.GetUInt16("intelligence");

                    item.statMinDamage = reader.GetUInt16("statMinDamage");
                    item.statMaxDamage = reader.GetUInt16("statMaxDamage");

                    item.healthPotMiktar = reader.GetUInt16("healthPotMiktar");
                    item.manaPotMiktar = reader.GetUInt16("manaPotMiktar");

                    item.minArtsDamage = reader.GetUInt16("minArtsDamage");
                    item.maxArtsDamage = reader.GetUInt16("maxArtsDamage");

                    item.artsAttackRate = reader.GetByte("artsAttackRate");

                    item.artsDefense = reader.GetUInt16("artsDefense");
                    item.artsDefenseRate = reader.GetByte("artsDefenseRate");
                    item.defense = reader.GetUInt16("defense");
                    item.statDefense = reader.GetUInt16("statDefense");
                    item.maxChi = reader.GetUInt16("maxChi");
                    item.chiRecovery = reader.GetUInt16("chiRecovery");
                    item.maxHp = reader.GetUInt16("maxHP");
                    item.hpRecovery = reader.GetUInt16("hpRecovery");
                    item.accuracy = reader.GetUInt16("accuracy");
                    item.dodgerate = reader.GetUInt16("dodgerate");
                    item.confusionDefense = reader.GetUInt16("confusionDefense");
                    item.poisonDefense = reader.GetUInt16("poisonDefense");
                    item.poisonDamage = reader.GetByte("poisonDamage");
                    item.poisonDuration = reader.GetUInt32("poisonDuration");

                    item.paralysisDefense = reader.GetUInt16("paralysisDefense");
                    item.paralysisDamage = reader.GetByte("paralysisDamage");
                    item.paralysisDuration = reader.GetUInt32("paralysisDuration");

                    item.runSpeedStat = reader.GetFloat("runSpeedStat");

                    item.rateDefense = reader.GetByte("rateDefense");
                    item.itemId = reader.GetUInt32("itemid");

                    item.name = reader.GetString("nameOfItem");
                }


                reader.Close();

                conn.Close();

                return item;
            }
            catch (Exception e)
            {
                Debug.Log("GetItemInfo ERROR: " + e.Message);
                return null;
            }
        }

        public uint GetCharacterCurrentMap(string characterName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';"))
                {
                    conn.Open();

                    MySqlCommand cmd = conn.CreateCommand();

                    cmd.CommandText = "SELECT currentmap FROM characters WHERE nickname = @nickname";
                    cmd.Parameters.AddWithValue("@nickname", characterName);

                    object result = cmd.ExecuteScalar();

                    conn.Close();

                    if (result != null)
                    {
                        return Convert.ToUInt32(result);
                    }
                    else
                    {
                        Debug.Log("Character with nickname " + characterName + " not found.");
                        return 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetCharacterCurrentMap: " + e);
                Debug.Log("Failed to get current map for " + characterName + ": " + e.Message);
                return 0;
            }
        }

        public uint GetCharacterCurrentSkillPts(string characterName)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';"))
                {
                    conn.Open();

                    MySqlCommand cmd = conn.CreateCommand();

                    cmd.CommandText = "SELECT skillPts FROM characters WHERE nickname = @nickname";
                    cmd.Parameters.AddWithValue("@nickname", characterName);

                    object result = cmd.ExecuteScalar();

                    conn.Close();

                    if (result != null)
                    {
                        return Convert.ToUInt32(result);
                    }
                    else
                    {
                        Debug.Log("Character with nickname " + characterName + " not found.");
                        return 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("GetCharacterCurrentMap: " + e);
                Debug.Log("Failed to get current map for " + characterName + ": " + e.Message);
                return 0;
            }
        }

        public uint GetItemUpgradeLevel(int slotId, int accountId)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';"))
                {
                    conn.Open();

                    MySqlCommand cmd = conn.CreateCommand();

                    cmd.CommandText = "SELECT upgrade_level FROM inventories WHERE (slot = @slot AND account_id = @account_id)";
                    cmd.Parameters.AddWithValue("@slot", slotId);
                    cmd.Parameters.AddWithValue("@account_id", accountId);

                    object result = cmd.ExecuteScalar();

                    conn.Close();

                    if (result != null)
                    {
                        return Convert.ToUInt32(result);
                    }
                    else
                    {
                        Debug.Log("DB: ITEM UPGRADE LVL NOT FOUND");
                        return 0;
                    }
                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public Character GetCharacterInfo(string characterName)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';");
                conn.Open();
                Character character = new Character();

                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM characters WHERE nickname = @nickname";
                cmd.Parameters.AddWithValue("@nickname", characterName);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    character.session_id = BitConverter.ToUInt16(BitConverter.GetBytes(reader.GetUInt32("account_id")), 0);

                    character.nickname = reader.GetString("nickname");
                    character.level = reader.GetByte("level");
                    character.currentMap = reader.GetUInt32("currentmap");
                    character.evolution = reader.GetByte("evolution");
                    character.posX = reader.GetFloat("posX");
                    character.posY = reader.GetFloat("posY");
                    character.position = new Vector3(character.posX, character.posY, 0);

                    character.statpoints = reader.GetUInt16("statpoint");
                    character.naturepoints = reader.GetUInt16("naturepoint");
                    character.str = reader.GetUInt16("str");
                    character.extraStr = reader.GetUInt16("extraStr");
                    character.dex = reader.GetUInt16("dex");
                    character.extraDex = reader.GetUInt16("extraDex");
                    character.intelligence = reader.GetUInt16("intel");
                    character.extraIntelligence = reader.GetUInt16("extraIntelligence");
                    character.wind = reader.GetUInt16("wind");
                    character.extraWind = reader.GetUInt16("extraWind");
                    character.water = reader.GetUInt16("water");
                    character.extraWater = reader.GetUInt16("extraWater");
                    character.fire = reader.GetUInt16("fire");
                    character.extraFire = reader.GetUInt16("extraFire");
                    character.attackpoint = reader.GetUInt16("attackpoint");
                    character.extraAttackPoint = reader.GetUInt16("extraAttackPoint");
                    character.artsatk = reader.GetUInt64("artsattack");
                    character.extraArtsAtk = reader.GetUInt64("extraArtsAtk");
                    character.defense = reader.GetUInt32("defence");
                    character.extraDefense = reader.GetUInt32("extraDefense");
                    character.artsdef = reader.GetUInt32("artsdefence");
                    character.extraArtsDef = reader.GetUInt32("extraArtsDef");
                    character.accuracy = reader.GetUInt16("accuracy");
                    character.extraAccuracy = reader.GetUInt16("extraAccuracy");
                    character.dodge = reader.GetUInt16("dodge");
                    character.extraDodge = reader.GetUInt16("extraDodge");
                    character.critical = reader.GetUInt16("critic");
                    character.extraCritical = reader.GetUInt16("extraCritical");
                    character.atkspeed = reader.GetUInt16("atkspeed");
                    character.extraAtkspeed = reader.GetUInt16("extraAtkspeed");

                    character.curhp = reader.GetInt32("curHP");
                    character.maxhp = reader.GetInt32("maxHP");

                    character.curmana = reader.GetInt32("curMANA");
                    character.maxmana = reader.GetInt32("maxMANA");
                    character.extraMaxHp = reader.GetInt32("extraMaxHp");
                    character.extraMaxMana = reader.GetInt32("extraMaxMana");

                    character.curexp = reader.GetFloat("curEXP");
                    character.maxexp = reader.GetFloat("maxEXP");

                    character.m_Gold = reader.GetUInt64("gold");

                    character.m_Race = reader.GetByte("race");
                    character.SkillPoints = reader.GetUInt16("skillPts");

                }


                reader.Close();


                cmd.CommandText = "SELECT * FROM inventories WHERE account_id = @account_id";
                cmd.Parameters.AddWithValue("@account_id", character.session_id);

                reader = cmd.ExecuteReader();

                Inventory inventory = new Inventory();
                Inventory oldinventory = new Inventory();
                inventory.gold = character.m_Gold;
                oldinventory.gold = character.m_Gold;

                while (reader.Read())
                {
                    Item item = new Item();
                    item.databaseindex = reader.GetUInt32("databaseindex");
                    item.itemId = reader.GetUInt32("itemid");
                    item.itemSlot = reader.GetUInt16("slot");
                    item.count = reader.GetUInt16("count");
                    item.upgrade_level = reader.GetUInt16("upgrade_level");
                    inventory.m_Items.Add(item);
                    oldinventory.m_Items.Add(item);
                }


                reader.Close();


                character.m_Inventory = inventory;
                character.m_OldInventory = oldinventory;

                Member member = GetMemberFromID(character.session_id);

                character.member = member;

                conn.Close();

                return character;
            }
            catch(Exception e)
            {
                Console.WriteLine("GetCharacterInfo: " + e);
                return null;
            }
        }


        public void GetCharacterSkillInfo(int _accountId, NetworkStream stream)
        {
            try
            {
                MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';");
                conn.Open();
                Character character = new Character();

                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM skills WHERE account_id = @accountId";
                cmd.Parameters.AddWithValue("@accountId", _accountId);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows) 
                {
                    Console.WriteLine("No skills found for account: " + _accountId);
                    return; 
                }

                bool isRunSpeedPassiveBookActive = false;
                int runSpeedBook = 0;
                int runSpeedBookLevel = 0;


                bool isLv10BookActive = false;
                bool isLv25BookActive = false;
                bool isLv40BookActive = false;
                bool isLv60BookActive = false;
                bool isLv100BookActive = false;

                ushort slot = 0;
                int book0 = 0;
                int book1 = 0;
                int book2 = 0;
                int book3 = 0;
                int book4 = 0;

                int book0UpgradeLevel = 0;
                int book1UpgradeLevel = 0;
                int book2UpgradeLevel = 0;
                int book3UpgradeLevel = 0;
                int book4UpgradeLevel = 0;

                while (reader.Read())
                {
                    slot = BitConverter.ToUInt16(BitConverter.GetBytes(reader.GetUInt16("slot")), 0);

                    if (slot == 0)
                    {
                            isLv10BookActive = true;
                            book0 = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("item_id")), 0);
                            book0UpgradeLevel = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("book_level")), 0);
                    }

                    if (slot == 1)
                    {
                        isLv25BookActive = true;
                        book1 = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("item_id")), 0);
                        book1UpgradeLevel = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("book_level")), 0);
                    }

                    if (slot == 2)
                    {
                        isLv40BookActive = true;
                        book2 = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("item_id")), 0);
                        book2UpgradeLevel = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("book_level")), 0);
                    }

                    if (slot == 3)
                    {
                        isLv60BookActive = true;
                        book3 = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("item_id")), 0);
                        book3UpgradeLevel = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("book_level")), 0);
                    }

                    if (slot == 4)
                    {
                        isLv100BookActive = true;
                        book4 = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("item_id")), 0);
                        book4UpgradeLevel = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("book_level")), 0);
                    }

                    if (slot == 8)
                    {
                        isRunSpeedPassiveBookActive = true;
                        runSpeedBook = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("item_id")), 0);
                        runSpeedBookLevel = BitConverter.ToInt32(BitConverter.GetBytes(reader.GetUInt32("book_level")), 0);
                    }

                }

                reader.Close();

                Member member = GetMemberFromID(character.session_id);

                character.member = member;

                conn.Close();

                character.InitPlayerSkills(stream, (uint)book0, (uint)book1, (uint)book2, (uint)book3, (uint)book4, isLv10BookActive, isLv25BookActive, isLv40BookActive, isLv60BookActive, isLv100BookActive, (byte)book0UpgradeLevel, (byte)book1UpgradeLevel, (byte)book2UpgradeLevel, (byte)book3UpgradeLevel, (byte)book4UpgradeLevel, (uint)runSpeedBook, isRunSpeedPassiveBookActive, (byte)runSpeedBookLevel);
            }
            catch (Exception e)
            {
                Console.WriteLine("SetCharacterInfo: " + e);
            }
        }

        public void SetCharacterCurrentMap(string characterName, uint newMapId)
        {

            Debug.Log("New map set for " + characterName + " to " + newMapId, ConsoleColor.Yellow);
            try
            {
                using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';"))
                {
                    conn.Open();

                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "UPDATE characters SET currentmap = @currentMap WHERE nickname = @nickname";
                    cmd.Parameters.AddWithValue("@currentMap", newMapId);
                    cmd.Parameters.AddWithValue("@nickname", characterName);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    conn.Close();

                    Debug.Log("Set current map success for " + characterName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SetCharacterCurrentMap: " + e);
                Debug.Log("Set current map failed for " + characterName);

            }
        }

        public void SaveCharacterPos(string characterName, float posX, float posY)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';"))
                {
                    conn.Open();

                    MySqlCommand cmd = conn.CreateCommand();

                    // Update query to set posX and posY for the given character nickname
                    cmd.CommandText = "UPDATE characters SET posX = @posX, posY = @posY WHERE nickname = @nickname";
                    cmd.Parameters.AddWithValue("@posX", posX);
                    cmd.Parameters.AddWithValue("@posY", posY);
                    cmd.Parameters.AddWithValue("@nickname", characterName);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    conn.Close();

                    if (rowsAffected > 0)
                    {
                        Debug.Log("Position updated successfully for " + characterName);
                    }
                    else
                    {
                        Debug.Log("No character found with nickname: " + characterName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("SaveCharacterPos: " + e);
                Debug.Log("Failed to update position for " + characterName + ": " + e.Message);
            }
        }

        public bool SetGold(ulong gold, Character character, Member member)
        {
            try
            {
                if (Database.database.GetCharactersOfMember(member).Where(x => x.nickname == character.nickname).Count() <= 0)
                    return false;


                MySqlCommand cmd = mySqlConnectionAuth.CreateCommand();

                cmd.CommandText = "UPDATE hero_auth.characters SET gold=@gold WHERE account_id=@account_id LIMIT 1";

                cmd.Parameters.AddWithValue("@account_id", character.session_id);
                cmd.Parameters.AddWithValue("@gold", gold);

                int len = cmd.ExecuteNonQuery();

                return len > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("SetGold: " + e);
                return false;
            }
        }

        

        public bool AddItem(Item item, Character character, Member member)
        {
            try
            {
                if (Database.database.GetCharactersOfMember(member).Where(x => x.nickname == character.nickname).Count() <= 0)
                    return false;

                if (item.count <= 0) return false;

                

                MySqlCommand cmd1 = mySqlConnectionAuth.CreateCommand();

                string command1 = "SELECT * FROM item_list WHERE itemid = @itemId ";

                cmd1.CommandText = command1;

                cmd1.Parameters.AddWithValue("@itemId", item.itemId);

                using (var reader = cmd1.ExecuteReader())
                {
                    if (reader.HasRows) { while (reader.Read()) { } }
                    else
                    {
                        Debug.Log("ITEM ID " + item.itemId + " NOT FOUND!", ConsoleColor.Red);
                        return false;
                    }
                }

                MySqlCommand cmd2 = mySqlConnectionAuth.CreateCommand();

                string command2 = "INSERT INTO inventories (account_id, character_id, itemid, slot, count, upgrade_level) SELECT @account_id, 0, @itemid, @slot, @count, @upgradeLv FROM DUAL WHERE NOT EXISTS (SELECT * FROM inventories WHERE (slot = @slot AND account_id = @account_id)) ";

                cmd2.CommandText = command2;//"INSERT INTO hero_auth.inventories (account_id, character_id, itemid, slot, count) VALUES (@account_id, 0, @itemid, @slot, @count)";

                cmd2.Parameters.AddWithValue("@account_id", character.session_id);
                cmd2.Parameters.AddWithValue("@itemid", item.itemId);
                cmd2.Parameters.AddWithValue("@slot", item.itemSlot);
                cmd2.Parameters.AddWithValue("@count", item.count);
                cmd2.Parameters.AddWithValue("@upgradeLv", item.upgrade_level);

                int len2 = cmd2.ExecuteNonQuery();

                if (len2 > 0)
                {
                    Debug.Log("ITEM ID " + item.itemId + " CREATED!", ConsoleColor.Green);
                    return true;
                }
                else
                    return false;

            }
            catch(Exception e)
            {
                Console.WriteLine("AddItem: " + e);
                return false;
            }
        }

        public Item UpgradeItem(Item item, Character character, Member member)
        {
            try
            {
                if (Database.database.GetCharactersOfMember(member).Where(x => x.nickname == character.nickname).Count() <= 0)
                    return null;

                if (item.count <= 0) return null;

                MySqlCommand cmd1 = mySqlConnectionAuth.CreateCommand();

                string command1 = "SELECT * FROM item_list WHERE itemid = @itemId ";

                cmd1.CommandText = command1;

                cmd1.Parameters.AddWithValue("@itemId", item.itemId);

                using (var reader = cmd1.ExecuteReader())
                {
                    if (reader.HasRows) { while (reader.Read()) { } }
                    else
                    {
                        Debug.Log("ITEM ID " + item.itemId + " NOT FOUND!", ConsoleColor.Red);
                        return null;
                    }
                }

                MySqlCommand cmd2 = mySqlConnectionAuth.CreateCommand();

                string command2 = "UPDATE inventories SET upgrade_level = @newUpgradeLevel WHERE (slot = @slot AND account_id = @account_id)";

                cmd2.CommandText = command2;//"INSERT INTO hero_auth.inventories (account_id, character_id, itemid, slot, count) VALUES (@account_id, 0, @itemid, @slot, @count)";

                cmd2.Parameters.AddWithValue("@account_id", character.session_id);
                //cmd2.Parameters.AddWithValue("@itemid", item.itemId);
                cmd2.Parameters.AddWithValue("@slot", item.itemSlot);
                //cmd2.Parameters.AddWithValue("@count", item.count);

                var newUpgradeLevel = item.upgrade_level;
                cmd2.Parameters.AddWithValue("@newUpgradeLevel", newUpgradeLevel);

                int len2 = cmd2.ExecuteNonQuery();

                if (len2 > 0)
                {
                    Debug.Log("ITEM ID " + item.itemId + " UPGRADED!", ConsoleColor.Green);
                    return item;
                }
                else
                {
                    Debug.Log("NO ITEM FOUND FOR " + character.nickname + " item: " + item.itemId + " slot: " + item.itemSlot, ConsoleColor.Red);
                    Debug.Log("NO ITEM FOUND FOR " + character.nickname + " item: " + item.itemId + " slot: " + item.itemSlot, ConsoleColor.Red);
                    Debug.Log("NO ITEM FOUND FOR " + character.nickname + " item: " + item.itemId + " slot: " + item.itemSlot, ConsoleColor.Red);
                    Debug.Log("NO ITEM FOUND FOR " + character.nickname + " item: " + item.itemId + " slot: " + item.itemSlot, ConsoleColor.Red);
                    return null;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("AddItem: " + e);
                return null;
            }
        }

        public int GetSkillBookLevel(uint _accountId, int skillBookSlot, int isPassive = 0)
        {
            MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';");
            conn.Open();
            Character character = new Character();

            MySqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT * FROM skills WHERE (account_id = @accontId AND slot = @slot AND isPassive = @isPassive)";
            cmd.Parameters.AddWithValue("@accontId", _accountId);
            cmd.Parameters.AddWithValue("@slot", skillBookSlot);
            cmd.Parameters.AddWithValue("@isPassive", isPassive);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                Console.WriteLine("No skills found for account: " + _accountId);
                return 0;
            }

            int bookLevel = -1;

            if (reader.Read())
            {
                bookLevel = reader.GetInt32("book_level");
            }

            reader.Close();

            conn.Close();

            return bookLevel;
        }

        public bool AddSkill(Item item, Character character, Member member, int isPassive = 0)
        {
            try
            {
                if (Database.database.GetCharactersOfMember(member).Where(x => x.nickname == character.nickname).Count() <= 0)
                {
                    Debug.Log("DB Error 101", ConsoleColor.Red);
                    return false;
                }

                MySqlCommand cmd2 = mySqlConnectionAuth.CreateCommand();

                string command2 = "INSERT INTO skills (account_id, character_id, slot, item_id, book_level, isPassive) SELECT @account_id, 0, @slot, @item_id, 0, @isPassive FROM DUAL WHERE NOT EXISTS (SELECT * FROM skills WHERE (slot = @slot AND account_id = @account_id AND isPassive = @isPassive)) ";

                cmd2.CommandText = command2;//"INSERT INTO hero_auth.inventories (account_id, character_id, itemid, slot, count) VALUES (@account_id, 0, @itemid, @slot, @count)";

                cmd2.Parameters.AddWithValue("@account_id", character.session_id);
                cmd2.Parameters.AddWithValue("@slot", SkillBook.GetSkillSlotId(item.neededLevel, isPassive));
                cmd2.Parameters.AddWithValue("@item_id", item.itemId);
                cmd2.Parameters.AddWithValue("@isPassive", isPassive);

                int len2 = 0;

                try
                {
                    len2 = cmd2.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);  // Output the error message to help with debugging
                }


                if (len2 > 0)
                {
                    Debug.Log("SKILL BOOK ID " + item.itemId + " CREATED IN SKILL BOOK!", ConsoleColor.Green);
                    return true;
                }
                else
                {
                    Debug.Log("DB Error: Skill book create failed", ConsoleColor.Red);
                    return false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("AddSkillBook: " + e);
                return false;
            }
        }

        public bool UpgradeSkillBook(Character character, Member member, int slot, int bookLevel, int isPassive = 0)
        {
            try
            {
                if (Database.database.GetCharactersOfMember(member).Where(x => x.nickname == character.nickname).Count() <= 0)
                {
                    Debug.Log("DB Error 101", ConsoleColor.Red);
                    return false;
                }

                MySqlCommand cmd2 = mySqlConnectionAuth.CreateCommand();

                string command2 = "UPDATE skills SET book_level = @bookLevel WHERE (slot = @slot AND account_id = @account_id AND isPassive = @isPassive)";

                cmd2.CommandText = command2;//"INSERT INTO hero_auth.inventories (account_id, character_id, itemid, slot, count) VALUES (@account_id, 0, @itemid, @slot, @count)";

                cmd2.Parameters.AddWithValue("@account_id", character.session_id);
                cmd2.Parameters.AddWithValue("@slot", slot);
                cmd2.Parameters.AddWithValue("@bookLevel", bookLevel);
                cmd2.Parameters.AddWithValue("@isPassive", isPassive);

                int len2 = cmd2.ExecuteNonQuery();

                if (len2 > 0)
                {
                    Debug.Log("SKILL BOOK LEVEL UPDATED TO + bookLevel ", ConsoleColor.Green);
                    return true;
                }
                else
                {
                    Debug.Log("DB Error: Skill book update failed", ConsoleColor.Red);
                    return false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("AddSkillBook: " + e);
                return false;
            }
        }

        public bool DragItem(Item item, ushort newSlot, Character character, Member member)
        {
            try
            {
                if (Database.database.GetCharactersOfMember(member).Where(x => x.nickname == character.nickname).Count() <= 0)
                    return false;

                MySqlCommand cmd = mySqlConnectionAuth.CreateCommand();
                
                cmd.CommandText = "UPDATE hero_auth.inventories SET slot=@slot WHERE (account_id=@account_id AND slot=@oldSlot AND itemid=@itemid) LIMIT 1";

                cmd.Parameters.AddWithValue("@account_id", character.session_id);
                cmd.Parameters.AddWithValue("@slot", newSlot);
                cmd.Parameters.AddWithValue("@oldSlot", item.itemSlot);
                cmd.Parameters.AddWithValue("@itemid", item.itemId);

                int len = cmd.ExecuteNonQuery();

                return len > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("DragItem: " + e);
                return false;
            }
        }

        public bool RemoveItem(Item item, Character character, Member member)
        {
            try
            {


                if (Database.database.GetCharactersOfMember(member).Where(x => x.nickname == character.nickname).Count() <= 0)
                    return false;

                MySqlCommand cmd = mySqlConnectionAuth.CreateCommand();

                cmd.CommandText = "DELETE FROM hero_auth.inventories WHERE (account_id=@account_id AND slot=@slot AND itemid=@itemid) LIMIT 1";

                cmd.Parameters.AddWithValue("@account_id", character.session_id);
                cmd.Parameters.AddWithValue("@slot", item.itemSlot);
                cmd.Parameters.AddWithValue("@itemid", item.itemId);

                int len = cmd.ExecuteNonQuery();

                return len > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("RemoveItem: " + e);
                return false;
            }
        }

        public bool RemoveItem(int itemSlot, int itemId, Character character, Member member)
        {
            try
            {


                if (Database.database.GetCharactersOfMember(member).Where(x => x.nickname == character.nickname).Count() <= 0)
                    return false;

                MySqlCommand cmd = mySqlConnectionAuth.CreateCommand();

                cmd.CommandText = "DELETE FROM hero_auth.inventories WHERE (account_id=@account_id AND slot=@slot AND itemid=@itemid) LIMIT 1";

                cmd.Parameters.AddWithValue("@account_id", character.session_id);
                cmd.Parameters.AddWithValue("@slot", itemSlot);
                cmd.Parameters.AddWithValue("@itemid", itemId);

                int len = cmd.ExecuteNonQuery();

                return len > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("RemoveItem: " + e);
                return false;
            }
        }

        public bool SaveCharacterInfo(Character msg, Member member)
        {
            try
            {
              //  Console.WriteLine("Saving character infos.");

                if (Database.database.GetCharactersOfMember(member).Where(x => x.nickname == msg.nickname).Count() <= 0)
                    return false;

                

                MySqlCommand cmd = mySqlConnectionAuth.CreateCommand();

                cmd.CommandText = "UPDATE hero_auth.characters SET level=@level,posX=@posX,posY=@posY,evolution=@evolution,statpoint=@statpoint,naturepoint=@naturepoint,str=@str,dex=@dex,intel=@intel,wind=@wind,water=@water,fire=@fire,attackpoint=@attackpoint,artsattack=@artsattack,defence=@defence,artsdefence=@artsdefence,accuracy=@accuracy,dodge=@dodge,maxHP=@maxHP,maxMANA=@maxMANA,curHP=@curHP,curMANA=@curMANA,critic=@critic,curEXP=@curEXP,maxEXP=@maxEXP,atkspeed=@atkspeed,race=@race, gold=@gold, extraStr=@extraStr,extraDex=@extraDex,extraIntelligence	=@extraIntelligence	,extraWind=@extraWind,extraWater=@extraWater,extraFire=@extraFire,extraAccuracy=@extraAccuracy,extraDodge=@extraDodge,extraCritical=@extraCritical,extraAtkspeed=@extraAtkspeed,extraArtsAtk=@extraArtsAtk,extraAttackPoint=@extraAttackPoint,extraDefense=@extraDefense,extraArtsDef=@extraArtsDef,extraMaxHp=@extraMaxHp,extraMaxMana=@extraMaxMana,skillPts=@SkillPts WHERE (nickname=@nickname) LIMIT 1";

                cmd.Parameters.AddWithValue("@nickname", msg.nickname);

                cmd.Parameters.AddWithValue("@level", msg.level);
               // cmd.Parameters.AddWithValue("@currentmap", msg.currentMap);
                cmd.Parameters.AddWithValue("@posX", (int)msg.posX);
                cmd.Parameters.AddWithValue("@posY", (int)msg.posY);
                cmd.Parameters.AddWithValue("@evolution", msg.evolution);
                cmd.Parameters.AddWithValue("@statpoint", msg.statpoints);
                cmd.Parameters.AddWithValue("@naturepoint", msg.naturepoints);
                cmd.Parameters.AddWithValue("@str", msg.str);
                cmd.Parameters.AddWithValue("@extraStr", msg.extraStr);
                cmd.Parameters.AddWithValue("@dex", msg.dex);
                cmd.Parameters.AddWithValue("@extraDex", msg.extraDex);
                cmd.Parameters.AddWithValue("@intel", msg.intelligence);
                cmd.Parameters.AddWithValue("@extraIntelligence", msg.extraIntelligence);
                cmd.Parameters.AddWithValue("@wind", msg.wind);
                cmd.Parameters.AddWithValue("@extraWind", msg.extraWind);
                cmd.Parameters.AddWithValue("@water", msg.water);
                cmd.Parameters.AddWithValue("@extraWater", msg.extraWater);
                cmd.Parameters.AddWithValue("@fire", msg.fire);
                cmd.Parameters.AddWithValue("@extraFire", msg.extraFire);
                cmd.Parameters.AddWithValue("@attackpoint", msg.attackpoint);
                cmd.Parameters.AddWithValue("@extraAttackPoint", msg.extraAttackPoint);
                cmd.Parameters.AddWithValue("@artsattack", msg.artsatk);
                cmd.Parameters.AddWithValue("@extraArtsAtk", msg.extraArtsAtk);
                cmd.Parameters.AddWithValue("@defence", msg.defense);
                cmd.Parameters.AddWithValue("@extraDefense", msg.extraDefense);
                cmd.Parameters.AddWithValue("@artsdefence", msg.artsdef);
                cmd.Parameters.AddWithValue("@extraArtsDef", msg.extraArtsDef);
                cmd.Parameters.AddWithValue("@accuracy", msg.accuracy);
                cmd.Parameters.AddWithValue("@extraAccuracy", msg.extraAccuracy);
                cmd.Parameters.AddWithValue("@dodge", msg.dodge);
                cmd.Parameters.AddWithValue("@extraDodge", msg.extraDodge);
                cmd.Parameters.AddWithValue("@critic", msg.critical);
                cmd.Parameters.AddWithValue("@extraCritical", msg.extraCritical);
                cmd.Parameters.AddWithValue("@curEXP", msg.curexp);
                cmd.Parameters.AddWithValue("@maxEXP", msg.maxexp);
                cmd.Parameters.AddWithValue("@atkspeed", msg.atkspeed);
                cmd.Parameters.AddWithValue("@extraAtkspeed", msg.extraAtkspeed);
                cmd.Parameters.AddWithValue("@race", msg.m_Race);
                cmd.Parameters.AddWithValue("@curHP", Convert.ToInt32(msg.curhp) > 0 ? msg.curhp : 0);
                cmd.Parameters.AddWithValue("@maxHP", msg.maxhp);
                cmd.Parameters.AddWithValue("@extraMaxHp", msg.extraMaxHp);
                cmd.Parameters.AddWithValue("@extraMaxMana", msg.extraMaxMana);
                cmd.Parameters.AddWithValue("@curMANA", msg.curmana);
                cmd.Parameters.AddWithValue("@maxMANA", msg.maxmana);
                cmd.Parameters.AddWithValue("@SkillPts", msg.SkillPoints);
                cmd.Parameters.AddWithValue("@gold", msg.m_Gold);

                cmd.ExecuteNonQuery();

                //cmd = mySqlConnectionAuth.CreateCommand();

                //for (int i = 0; i < msg.m_ToRemoveFromDatabase.Count; i++)
                //{
                //    cmd.CommandText = "DELETE FROM hero_auth.inventories WHERE (account_id=@account_id AND databaseindex=@databaseindex) LIMIT 1";

                //    cmd.Parameters.AddWithValue("@account_id", msg.session_id);
                //    cmd.Parameters.AddWithValue("@databaseindex", msg.m_ToRemoveFromDatabase[i].databaseindex);

                //    Console.WriteLine("DATABASE INDEX: " + msg.m_ToRemoveFromDatabase[i].databaseindex);

                //    cmd.ExecuteNonQuery();

                //    msg.m_ToRemoveFromDatabase.RemoveAt(i);
                //}

                //var oldInventory = msg.m_OldInventory;
                //var currentInventory = msg.m_Inventory;


                //List<Item> itemsToUpdate = new List<Item>(), itemsToCreate = new List<Item>();

            


                //cmd = mySqlConnectionAuth.CreateCommand();

                //for (int i = 0;i < itemsToUpdate.Count;i++)
                //{
                //    cmd.CommandText = "UPDATE hero_auth.inventories SET itemid=@itemid, slot=@slot, count=@count WHERE (account_id=@account_id AND databaseindex=@databaseindex)";

                //    cmd.Parameters.AddWithValue("@account_id", msg.session_id);
                //    cmd.Parameters.AddWithValue("@databaseindex", itemsToUpdate[i].databaseindex);

                //    cmd.Parameters.AddWithValue("@itemid", itemsToUpdate[i].itemId);
                //    cmd.Parameters.AddWithValue("@slot", itemsToUpdate[i].itemSlot);
                //    cmd.Parameters.AddWithValue("@count", itemsToUpdate[i].count);

                //    cmd.ExecuteNonQuery();
                //}

                //cmd = mySqlConnectionAuth.CreateCommand();

                //for (int i = 0; i < itemsToCreate.Count; i++)
                //{
                //    cmd.CommandText = "INSERT INTO hero_auth.inventories (account_id, character_id, itemid, slot, count) VALUES (@account_id, 0, @itemid, @slot, @count)";

                //    cmd.Parameters.AddWithValue("@account_id", msg.session_id);

                //    cmd.Parameters.AddWithValue("@itemid", itemsToCreate[i].itemId);
                //    cmd.Parameters.AddWithValue("@slot", itemsToCreate[i].itemSlot);
                //    cmd.Parameters.AddWithValue("@count", itemsToCreate[i].count);

                //    cmd.ExecuteNonQuery();
                //}


                //msg.m_OldInventory = currentInventory;

                //Console.WriteLine("Character infos saved.");

            }
            catch (Exception e)
            {
                Console.WriteLine("SaveCharacterInfos: " + e.Message);
                return false;
            }


            return true;
        }


        public bool DeleteCharacter(PacketTypes.CDeleteCharacterReqMessage info, Member member)
        {
            try
            {
                Character character = Database.database.GetCharacterInfo(info.nickname);

                if (character.nickname == null || character.nickname.Trim() == "") return false;

                if (Database.database.GetMemberFromNickname(character.nickname).user_id != member.user_id) return false;

                MySqlCommand cmd = mySqlConnectionAuth.CreateCommand();

                cmd.CommandText = "DELETE FROM hero_auth.characters WHERE nickname=@nickname LIMIT 1";
                cmd.Parameters.AddWithValue("@nickname", info.nickname);

                cmd.ExecuteNonQuery();

                //

                MySqlCommand cmd2 = mySqlConnectionAuth.CreateCommand();

                cmd2.CommandText = "DELETE FROM hero_auth.skills WHERE account_id=@accId";
                cmd2.Parameters.AddWithValue("@accId", character.session_id);

                cmd2.ExecuteNonQuery();

                //

                MySqlCommand cmd3 = mySqlConnectionAuth.CreateCommand();

                cmd3.CommandText = "DELETE FROM hero_auth.inventories WHERE account_id=@accId";
                cmd3.Parameters.AddWithValue("@accId", character.session_id);

                cmd3.ExecuteNonQuery();

                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("DeleteCharacter: " + e);
                return false;
            }
        }


        public bool CreateCharacter(PacketTypes.CRegisterReqMessage info, Member member)
        {
            try
            {
                if (info.nickname.Trim().Length < 4 || member.password == "nul") return false;

                if (Database.database.GetCharactersOfMember(member).Count > 1) return false;

                Character character = Database.database.GetCharacterInfo(info.nickname.Trim());

                if (character.nickname != null)
                    if (character.nickname.Trim() != "")
                        return false;


                if (info.evolution > 0x3B) return false;

                MySqlCommand cmd = mySqlConnectionAuth.CreateCommand();

                cmd.CommandText = "INSERT INTO hero_auth.characters (account_id, nickname, evolution, race) VALUES (@account_id, @nickname, @evolution, @race)";
                cmd.Parameters.AddWithValue("@nickname", info.nickname.Trim());
                cmd.Parameters.AddWithValue("@evolution", info.evolution);
                cmd.Parameters.AddWithValue("@race", info.race);
                cmd.Parameters.AddWithValue("@account_id", member.user_id);


                cmd.ExecuteNonQuery();

                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("Database.CreateCharacter: " + e.Message);
                return false;
            }
        }


        public List<Character> GetCharactersOfMember(Member member)
        {
            try
            {
                if (member == null || member.user_id <= 0) return null;

                MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';");
                conn.Open();

                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM characters WHERE account_id = @account_id LIMIT 1";
                cmd.Parameters.AddWithValue("@account_id", member.user_id);

                MySqlDataReader reader = cmd.ExecuteReader();


                string characters = "";

                while (reader.Read())
                {
                    characters += $"{reader.GetString("nickname")}" + ",";
                }

                // Debug.Log(characters + " character data acquired...");

                reader.Close();

                List<Character> characterList = new List<Character>();

                string[] listnames = characters.Split(',');

                for (int i = 0; i < listnames.Length; i++)
                {
                    if (listnames[i].Trim() != "")
                    {
                        characterList.Add(GetCharacterInfo(listnames[i]));

                    }
                }

                conn.Close();

                return characterList;
            }
             catch (Exception e)
            {
                Debug.Log("GetCharactersOfMember: " + e.Message + "\n" + e.StackTrace);

                return null;
            }
        }


        public Member GetMemberFromNickname(string nickname)
        {
            try
            {
                Member member = new Member();
                member.m_Connection = null;
                member.password = "nul";
                member.username = "nul";
                member.user_id = 0;

                MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';");
                conn.Open();

                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT account_id FROM characters WHERE nickname = @nickname LIMIT 1";
                cmd.Parameters.AddWithValue("@nickname", nickname);

                MySqlDataReader reader = cmd.ExecuteReader();


                int acc_id = 0;

                if (reader.Read())
                {
                    acc_id = reader.GetInt32("account_id");
                }

                reader.Close();

                if (acc_id > 0)
                {
                    cmd.CommandText = "SELECT * FROM users WHERE user_id = @account_id";
                    cmd.Parameters.AddWithValue("@account_id", acc_id);

                    reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        member.user_id = reader.GetUInt32("user_id");
                        member.username = reader.GetString("username");
                        member.password = reader.GetString("password");
                    }

                    reader.Close();
                }

                conn.Close();
                //member.m_Characters = GetCharactersOfMember(member).ToArray();

                return member;
            }
           catch(Exception e)
            {
                Debug.Log("GetMemberFromNickname: " + e);
                return null;
            }
        }


        public Member GetMemberFromUsername(string username)
        {
            try
            {
                Member member = new Member();
                MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';");
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM users WHERE username = @username LIMIT 1";
                cmd.Parameters.AddWithValue("@username", username.Trim());

                MySqlDataReader reader = cmd.ExecuteReader();

                Debug.Log("DATABASE: USERNAME -> " + username.Trim());

                if (reader.Read())
                {
                    member.user_id = reader.GetUInt32("user_id");
                    member.username = $"{reader.GetString("username")}";
                    member.password = $"{reader.GetString("password")}";
                }
                else
                {
                    member.user_id = 0;
                    member.username = "nul";
                    member.password = "nul";
                }

                reader.Close();

                conn.Close();

                //member.m_Characters = GetCharactersOfMember(member).ToArray();

                return member;
            }
           catch(Exception e)
            {
                Debug.Log("GetMemberFromUsername: " + e);
                return null;
            }
        }


        public Member GetMemberFromID(uint id)
        {
            try
            {
                Member member = new Member();
                MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';");
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();

                cmd.CommandText = "SELECT * FROM users WHERE user_id = @user_id LIMIT 1";
                cmd.Parameters.AddWithValue("@user_id", id);

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    member.user_id = reader.GetUInt32("user_id");
                    member.username = $"{reader.GetString("username")}";
                    member.password = $"{reader.GetString("password")}";
                }
                else
                {
                    member.user_id = 0;
                    member.username = "nul";
                    member.password = "nul";
                }

                reader.Close();

                conn.Close();
                //member.m_Characters = GetCharactersOfMember(member).ToArray();

                return member;
            }
            catch (Exception e)
            {
                Debug.Log("GetMemberFromID: " + e);
                return null;
            }
        }


        public bool Register(string username, string password)
        {
            try
            {
                Member member = GetMemberFromUsername(username);

                if (member.password != null || member.password != "nul")
                {
                    // SEND USERNAME ALREADY USED
                    return false;
                }

                MySqlCommand cmd = mySqlConnectionAuth.CreateCommand();

                cmd.CommandText = "INSERT INTO hero_auth.users ( username, password ) VALUES ( @username, @password )";
                cmd.Parameters.AddWithValue("@username", username.Trim());
                cmd.Parameters.AddWithValue("@password", password.Trim());

                cmd.ExecuteNonQuery();

                return true;
            }
            catch(Exception e)
            {
                Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + ": " + e);
                return false;
            }
        }


        public bool CheckLogin(string username, string password)
        {
            try
            {
                Member member = GetMemberFromUsername(username);

                return member.password == password;
            }

            catch (Exception e)
            {
                Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ": " + e);
                return false;
            }
        }


        public List<NPC> GetNPCs()
        {
            try
            {
                string filePath = "..\\..\\Tables\\npcs.csv";

                List<NPC> listOfNPCs = new List<NPC>();

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        NPC npc = new NPC();

                        string[] values = line.Split(',');

                        if (values.Length > 2)
                        {
                            npc.type = ushort.Parse(values[0]);
                            npc.posX = float.Parse(values[1]);
                            npc.posY = float.Parse(values[2]);
                            npc.nickname = values[3];
                            npc.mapId = uint.Parse(values[4]);
                        }
                        else
                            continue;

                        listOfNPCs.Add(npc);
                    }
                }

                return listOfNPCs;
            }
            catch (Exception e)
            {
                Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().Name + ": " + e);
                return null;
            }
        }

        public Vector2 GetCharacterPosition(string characterName)
        {
            try
            {
                Vector2 PlayerPosition = new Vector2();

                using (MySqlConnection conn = new MySqlConnection("Server=localhost;Database=" + Program.DATABASE_AUTHTABLE + ";Uid=" + Program.DATABASE_USERNAME + ";Pwd='" + Program.DATABASE_PASS + "';"))
                {
                    conn.Open();

                    MySqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT posX, posY FROM characters WHERE nickname = @nickname";
                    cmd.Parameters.AddWithValue("@nickname", characterName);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            PlayerPosition.x = reader.GetInt32(0); // posX
                            PlayerPosition.y = reader.GetInt32(1); // posY
                        }
                        else
                        {
                            Debug.Log("Character with nickname " + characterName + " not found.");
                            return Vector2.zero;
                        }
                    }
                }

                return PlayerPosition;
            }
            catch (Exception e)
            {
                Debug.Log("GetCharacterPosition: " + e);
                Debug.Log("Failed to get position for " + characterName + ": " + e.Message);
                return Vector2.zero;
            }
        }

        public static Database database = new Database(Program.DATABASE_USERNAME, Program.DATABASE_PASS, Program.DATABASE_AUTHTABLE);
    }
}
