using Adventure_Server_CSharp.PacketTypes;
using Adventure_Server_CSharp.Tables;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public static class Randomf
    {
        public static readonly Random random = new Random();
    }


    public class MobSpawner
    {
        public static List<MobSpawner> MobSpawnerList = new List<MobSpawner>();

        public Vector3 position = new Vector3();
        public ushort skinId;

        public int timeToSpawn;
        public int maxSpawn;

        public List<Mob> mobList = new List<Mob>();

        public int mapId;

        public MobSpawner()
        {
            if (!MobSpawnerList.Contains(this))
                MobSpawnerList.Add(this);
        }

        public void Start()
        {

        }

        private int lastSpawn = 0;

        public void Update()
        {
            if (Time.time - lastSpawn > 0 && mobList.Count < maxSpawn)
            {

                Random rnd = Randomf.random;
                Mob mob = new Mob((ushort)rnd.Next(10000, 32766), this);
                mob.skinId = skinId;

                var mobData = Mob.GetMobDataFromId(mob.skinId);

                mob.maxHp = mobData.maxHp;
                mob.curHp = mobData.curHp;

                mob.level = mobData.level;

                if (mob.curHp <= 0 || mob.maxHp <= 0 || mob.level <= 0)
                {
                    return;
                }

                mob.position = position + Vector3.RandomUnitSphere * 20.0f;

                // Console.WriteLine(mob.position.ToStringVector());

                mob.position.z = 0;
                mob.scale = 100;

                mob.m_Range = 10;

                mob.currentMap = (uint)mapId;

                mobList.Add(mob);

                foreach (var user in Server.Singleton.m_InGameUsers)
                {
                    var member = Server.Singleton.GetMemberByConnection(user.m_Connection);

                    if (member == null) continue;

                    var character = member.m_Characters[0];

                    if (character.currentMap != mob.currentMap) continue;

                    mob.Start(member);
                }

                lastSpawn = Time.time + timeToSpawn;
            }
        }
    }

    public class Mob
    {
        public ushort session_id;
        public byte level;
        public Vector3 position = new Vector3();
        public Vector3 spawnPosition = new Vector3();
        public byte scale;
        public uint curHp = 1, maxHp = 1;

        public ushort skinId;

        public float m_Range = 10.0f;

        public Character m_Target;

        public MobSpawner whichSpawnerSpawnedMe = null;

        public float m_Speed = 1f;

        public uint currentMap;

        public Mob()
        {
           // Console.WriteLine("Mob() called.");
            if (!mobList.Contains(this))
                mobList.Add(this);

            spawnPosition = position;
        }

        public Mob(ushort sess_id, MobSpawner mobSpawner)
        {
           // Console.WriteLine("Mob(sess_id) called.");
            if (!mobList.Contains(this))
                mobList.Add(this);

            whichSpawnerSpawnedMe = mobSpawner;

            session_id = sess_id;
        }


        public virtual void Start(Member member)
        {
            PacketTypes.SMobInfo mobInfo = new PacketTypes.SMobInfo();

            mobInfo.skin = skinId;
            mobInfo.session_id = session_id;
            mobInfo.maxHp = maxHp;
            mobInfo.curHp = curHp;
            mobInfo.level = level;
            mobInfo.scale = scale;
            mobInfo.x1 = position.x;
            mobInfo.y1 = position.y;
            mobInfo.z1 = position.z;
            mobInfo.x2 = position.x;
            mobInfo.y2 = position.y;
            mobInfo.z2 = position.z;

            if (member != null)
                Server.Singleton.SendToPlayer(member, mobInfo.getValue());
        }

        public virtual void StartAsPet(Member member)
        {
            PacketTypes.SPetInfo petInfo = new PacketTypes.SPetInfo();

            petInfo.skin = skinId;
            petInfo.session_id = session_id;
            petInfo.maxHp = maxHp;
            petInfo.curHp = curHp;
            petInfo.level = level;
            petInfo.scale = scale;
            petInfo.x1 = position.x;
            petInfo.y1 = position.y;
            petInfo.z1 = position.z;
            petInfo.x2 = position.x;
            petInfo.y2 = position.y;
            petInfo.z2 = position.z;

            if (member != null)
                Server.Singleton.SendToPlayer(member, petInfo.getValue());
        }

        public static PacketTypes.SMobInfo GetMobDataFromId(uint skinId)
        {
            PacketTypes.SMobInfo mobData = new PacketTypes.SMobInfo();

            string filePath = "..\\..\\Tables\\mob_data.csv";

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    if (int.Parse(values[0]) != skinId) continue;

                    if (values.Length > 2)
                    {
                        mobData.skin = ushort.Parse(values[0]);
                        mobData.level = byte.Parse(values[1]);
                        mobData.maxHp = uint.Parse(values[2]);
                        mobData.curHp = uint.Parse(values[2]);

                       // Debug.Log("SKIN: " + mobData.skin + ", LEVEL: " + mobData.level + ", HP: " + mobData.curHp + "/" + mobData.maxHp, ConsoleColor.Cyan);
                    }
                }
            }

            return mobData;
        }

        public Character GetTarget()
        {
            float m_distance = 200f;
            Character closestTarget = null;

            for (int i = 0; i < Server.Singleton.m_InGameUsers.Count; i++)
            {
                float dist = (Server.Singleton.m_InGameUsers[i].m_Characters[0].position - position).sqrMagnitude;

                if (dist <= m_distance)
                {
                    closestTarget = Server.Singleton.m_InGameUsers[i].m_Characters[0];
                    m_distance = dist;
                }
                else
                {
                    closestTarget = null;
                }
            }

            /*
            if (m_distance > m_Range)
                closestTarget = null;
            */

            return closestTarget;
        }


        void DestroyCorpse(int time)
        {
            System.Threading.Thread.Sleep(time);

            Server.Singleton.SendAll(Functions.FromHex("AA 55 10 00 31 02 " + session_id.ToString("X4") + " 04 00 07 00 CA 2A 04 43 07 5E 78 43 55 AA"));
        }

        public virtual void OnDead(Character character = null)
        {
            Console.WriteLine("OnDead() called.");

            if(character != null)
            {
                ulong toGold = (ulong)Randomf.random.Next(30, 50);
                character.m_Gold += toGold;

                PacketTypes.SDropGold sDropGold = new PacketTypes.SDropGold();
                sDropGold.gold = character.m_Gold;

                byte[] x = sDropGold.getValue();
                character.m_Connection.GetStream().Write(x, 0, x.Length);

                DropMobItem(character, this.skinId);

                if (Math.Abs(character.level - this.level) > 9) return;

                var tempXP = character.curexp;

                Debug.Log("ADDIN EXP: " + (float)(10.0f / (float)character.level) * (1 + Math.Abs((this.level - character.level) / 2)), ConsoleColor.Green);
                character.curexp += (float)(10.0f / (float)character.level) * (1 + Math.Abs((this.level - character.level) / 2));

                Random rnd = new Random();

                int calculateXP = (int)character.curexp - (int)tempXP + 1 * 100 + rnd.Next(150,250) * (this.level / 2);

                byte[] getXPData = Functions.FromHex("AA 55 05 00 13 " + BitConverter.ToString(BitConverter.GetBytes(calculateXP)) + " 55 AA");

                Debug.Log("diff: " + calculateXP + ", xp data: " + BitConverter.ToString(getXPData));

                character.m_Connection.GetStream().Write(getXPData, 0, getXPData.Length);

                Debug.Log("EXP: " + character.curexp + "/" + character.maxexp);

                character.CheckPlayerLevelUp(character);

            }

            this.curHp = 0;
            this.m_Range = 0;
            this.m_Speed = 0;

            Server.Singleton.SendAll(Functions.FromHex("AA 55 18 00 16 " + session_id.ToString("X4") + " 5D 02 00 00 00 00 00 00 00 00 FF FF FF FF 00 00 00 00 00 00 00 55 AA"));
            new System.Threading.Thread(() => { DestroyCorpse(5000); }).Start();


            mobList.Remove(this);

            if (whichSpawnerSpawnedMe != null)
                whichSpawnerSpawnedMe.mobList.Remove(this);
        }

        

        private void DropMobItem(Character character, int mobId)
        {

            int droppedItem = CDropItem.GetDroppedItem(mobId);

            if (droppedItem <= 0) return;

            SMobItemDrop sMobItemDrop = new SMobItemDrop();
            sMobItemDrop.sessionId = (ushort)(3000 + ServerSettings.createDropItemIndex);
            sMobItemDrop.posX = this.position.x;
            sMobItemDrop.posY = this.position.y;
            sMobItemDrop.itemId = (uint)droppedItem; // item Id cekme yapilacak
            sMobItemDrop.claimerId = (ushort)character.session_id;

            byte[] x2 = sMobItemDrop.getValue();

            ++ServerSettings.createDropItemIndex;

            character.m_Connection.GetStream().Write(x2, 0, x2.Length);

            CDropItem.DroppedItemsList.Add(sMobItemDrop.sessionId, (int)sMobItemDrop.itemId);

            Debug.Log("Item dropped! itemId: " + sMobItemDrop.itemId + " , session id: " + sMobItemDrop.sessionId);
        }

        Vector3 lastGoingPos = Vector3.zero;


        int cooldownTime = 0, lastTimeWalked2 = 0;

        public virtual void Update()
        {
            try
            {
                if (curHp <= 0)
                {
                    Debug.Log("Dead mob id: " + skinId + ", hp: " + curHp + "/" + maxHp);

                    OnDead();
                    return;
                }

                //Console.WriteLine("Current HP: " + curHp);
                    

                Character character = GetTarget();

                if (character == null || (character != null) && character.currentMap != this.currentMap) return;

                // Debug.Log("character is not null!");

                var mobAutoAttackDistance = 6f;

                if (character != null && currentMap == character.currentMap && character.curhp > 0 && (position - character.position).sqrMagnitude <= mobAutoAttackDistance)
                {
                    if (Time.time - cooldownTime > 0)
                    {
                        if ((position - character.position).sqrMagnitude <= 3f)
                        {
                            
                            Debug.Log("Dist: " + (position - character.position).sqrMagnitude, ConsoleColor.Yellow);
                            PacketTypes.SMobAttackAnimation sMobAttackAnimation = new PacketTypes.SMobAttackAnimation();
                            sMobAttackAnimation.mob_sessionid = session_id;
                            sMobAttackAnimation.user_sessionid = (ushort)character.session_id;
                            byte[] x = sMobAttackAnimation.getValue();
                            Server.Singleton.SendAll(x);

                            int mobDmgCalculation = 5 * (int)this.level - (int)(character.defense * 1.5f); // need calculate mob damage here

                            if (mobDmgCalculation <= 0) mobDmgCalculation = 2;

                            if (AdminCommandModes.GOD_MODE)
                                mobDmgCalculation = 0;

                            character.curhp -= mobDmgCalculation;
                            PacketTypes.SHealthInfoAckMessage sHealthInfoAckMessage = new PacketTypes.SHealthInfoAckMessage();
                            sHealthInfoAckMessage.uid = (ushort)character.session_id;

                            sHealthInfoAckMessage.health = character.curhp; // DAMAGE
                            sHealthInfoAckMessage.mana = character.curmana;

                            sHealthInfoAckMessage.debuffId = 0; // 1 cause felch

                            x = sHealthInfoAckMessage.getValue(isMob: true);
                            Server.Singleton.SendAll(x);
                            cooldownTime = Time.time + 1000;
                            return;
                        }
                        else // come closer to target for attack
                        {

                            Vector3 new_position = position + (character.position - position).normalize * 4;

                            PacketTypes.SMoveAckMessage moveAckMessage = new PacketTypes.SMoveAckMessage();
                            moveAckMessage.uid = session_id;
                            moveAckMessage.position1 = position;
                            moveAckMessage.position2 = new_position;
                            moveAckMessage.runSpeed = 8f;

                            Server.Singleton.SendAll(moveAckMessage.getValue());

                            position = new_position;

                            cooldownTime = Time.time + 1000;
                        }

                    }



                }
                else // walk around, no target
                {

                    if (Time.time - cooldownTime > 0)
                    {
                        Vector3 new_position;

                        if (whichSpawnerSpawnedMe != null)
                        new_position = whichSpawnerSpawnedMe.position + Vector3.RandomUnitSphere * 5;
                        else new_position = spawnPosition + Vector3.RandomUnitSphere * 5;

                        new_position.z = 0;

                        PacketTypes.SMoveAckMessage moveAckMessage = new PacketTypes.SMoveAckMessage();
                        moveAckMessage.uid = session_id;
                        moveAckMessage.position1 = position;
                        moveAckMessage.position2 = new_position;
                        moveAckMessage.runSpeed = 2f;

                        Server.Singleton.SendAll(moveAckMessage.getValue());

                        position = new_position;

                        // Debug.Log("Mob is moving to: " + new_position.x + ", " + new_position.y);
                        // Set a random time for the next walk
                        Random rand = Randomf.random;
                        cooldownTime = Time.time + rand.Next(5000, 6000); // Convert milliseconds to seconds
                    }

                }
            }
            catch(Exception ex)
            {
               // Console.WriteLine("Mob UPDATE: " + ex);
            }
         
        }




        public static List<Mob> mobList = new List<Mob>();
    }

}
