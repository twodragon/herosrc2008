using Adventure_Server_CSharp.PacketTypes;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{



    public static class FloatExtension
    {
        public static string ReverseFloat(float value, bool reverse = true)
        {
            // Convert the float to a 32-bit unsigned integer
            byte[] bytes = BitConverter.GetBytes(value);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes); // Make sure the bytes are in little-endian format
            }

            uint intRepresentation = BitConverter.ToUInt32(bytes, 0);

            // Perform the same bit-reversal logic as before
            uint x = (intRepresentation & 0x000000FFU) << 24 |
                     (intRepresentation & 0x0000FF00U) << 8 |
                     (intRepresentation & 0x00FF0000U) >> 8 |
                     (intRepresentation & 0xFF000000U) >> 24;

            string data = x.ToString("X8");

            // Reverse the string and swap every two characters
            char[] reversed = data.ToCharArray();
            Array.Reverse(reversed);
            string finalData = new string(reversed);
            finalData = SwapEveryTwoCharacters(finalData);

            return finalData;
        }

        public static byte[] ConvertToBigEndianABCD(float value)
        {
            // Float değerini Little Endian byte dizisine çevir
            byte[] littleEndianBytes = BitConverter.GetBytes(value);

            // Byte dizisini ters çevirerek Big Endian hale getir
            Array.Reverse(littleEndianBytes);

            return littleEndianBytes;
        }

        static string SwapEveryTwoCharacters(string input)
        {
            if (input.Length % 2 != 0)
            {
                input = "0" + input; // Add a zero if the length is odd
            }

            char[] characters = input.ToCharArray();

            for (int i = 0; i < characters.Length; i += 2)
            {
                char temp = characters[i];
                characters[i] = characters[i + 1];
                characters[i + 1] = temp;
            }

            return new string(characters);
        }
    }

    public static class LongExtension
    {
        public static string Reverse(this UInt64 value, bool reverse = true)
        {

            // Reverse only the lower 32 bits (4 bytes)
            uint lower32Bits = (uint)(value & 0xFFFFFFFF);

            uint x = (lower32Bits & 0x000000FFU) << 24 |
                     (lower32Bits & 0x0000FF00U) << 8 |
                     (lower32Bits & 0x00FF0000U) >> 8 |
                     (lower32Bits & 0xFF000000U) >> 24;

            string data = x.ToString("X8");

           // Debug.Log("data: " + data, ConsoleColor.Yellow);

            char[] reversed = data.ToCharArray();

            Array.Reverse(reversed);

            string finalData = new string(reversed);

            finalData = SwapEveryTwoCharacters(finalData);

           //Debug.Log("final data: " + finalData, ConsoleColor.Red);

            return finalData;
        }

        public static string ReverseFloat(this float value, bool reverse = true)
        {

            // Reverse only the lower 32 bits (4 bytes)
            uint lower32Bits = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);

            uint x = (lower32Bits & 0x000000FFU) << 24 |
                     (lower32Bits & 0x0000FF00U) << 8 |
                     (lower32Bits & 0x00FF0000U) >> 8 |
                     (lower32Bits & 0xFF000000U) >> 24;

            string data = x.ToString("X8");

            // Debug.Log("data: " + data, ConsoleColor.Yellow);

            char[] reversed = data.ToCharArray();

            Array.Reverse(reversed);

            string finalData = new string(reversed);

            finalData = SwapEveryTwoCharacters(finalData);

            //Debug.Log("final data: " + finalData, ConsoleColor.Red);

            return finalData;
        }

        static string SwapEveryTwoCharacters(string input)
        {
            if (input.Length % 2 != 0)
            {
               Debug.Log("SwapEveryTwoCharacters Error! The length was singular but added one more zero to fix... data: " + input);
                input = "0" + input;
              //  throw new ArgumentException("String length must be even.");
            }
            
            char[] characters = input.ToCharArray();

            for (int i = 0; i < characters.Length; i += 2)
            {
                // Swap the characters at positions i and i+1
                char temp = characters[i];
                characters[i] = characters[i + 1];
                characters[i + 1] = temp;
            }

            return new string(characters);
        }
    }

    public class ClientRequest
    {


        public ClientRequest(TcpClient client)
        {
            new Thread(() =>
            {
                Handle(client);
            }).Start();

            new Thread(() =>
            {
                ExploreNPCData(client);
            }).Start();

            new Thread(() =>
            {
                ExploreMobData(client);
            }).Start();

            new Thread(() =>
            {
                ExplorePlayerData(client);
            }).Start();
        }

        private async void ExploreMobData(TcpClient client)
        {
            while (true)
            {
                await Task.Delay(1000);

                if (client == null) continue;
                if (Server.Singleton.GetMemberByConnection(client) == null) continue;
                if (Server.Singleton.GetMemberByConnection(client).m_Characters[0] == null) continue;

                try
                {
                    foreach (var user in Server.Singleton.m_InGameUsers)
                    {
                        var member = Server.Singleton.GetMemberByConnection(user.m_Connection);

                        if (member == null) continue;

                        Character character = member.m_Characters[0];

                        character.currentMap = Database.database.GetCharacterCurrentMap(character.nickname);

                        // Debug.Log(character.posX + ", " + character.posY);

                        character.SendMobsInfoToClient();

                        //Debug.Log("User: " + user.username + " interacted with mobs!");
                    }
                }
                catch (Exception ex) { Debug.Log("ExploreMobDataError: " + ex); }
            }

        }

        public async void SendInitialNPCs(TcpClient client)
        {
            var character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];
            var stream = client.GetStream();

            foreach (var npc in Server.Singleton.m_GameNPCs)
            {
                await Task.Delay(50);

                // Aynı haritada olup olmadıklarını kontrol ediyoruz
                if (character.currentMap != npc.mapId) continue;

                // NPC bilgilerini ilk girişte gönderiyoruz
                PacketTypes.SNPCInfo sNPCInfo = new PacketTypes.SNPCInfo
                {
                    npctype = (ushort)npc.type,
                    posX = npc.posX,
                    posY = npc.posY,
                    pseudoNPCId = (byte)npc.type,
                    mapId = npc.mapId
                };

                byte[] msg = sNPCInfo.getValue();

                try
                {
                    await stream.WriteAsync(msg, 0, msg.Length);
                }
                catch (Exception ex) { }
            }

            Debug.Log(character.nickname + " has spawned and NPCs are sent.");
        }

        private async void ExploreNPCData(TcpClient client)
        {


            while (true)
            {

                if (client == null) continue;
                if (Server.Singleton.GetMemberByConnection(client) == null) continue;
                if (Server.Singleton.GetMemberByConnection(client).m_Characters[0] == null) continue;

                try
                {
                    var character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                    foreach (var npc in Server.Singleton.m_GameNPCs)
                    {
                        // Aynı haritada olup olmadıklarını kontrol ediyoruz
                        if (character.currentMap != npc.mapId) continue;

                        // Karakter ve NPC arasındaki mesafeyi hesaplıyoruz
                        float distanceSqr = (npc.posX - character.position.x) * (npc.posX - character.position.x) +
                                            (npc.posY - character.position.y) * (npc.posY - character.position.y);

                        // 50 birimden yakınsa NPC bilgisi gönder
                        if (distanceSqr < 50 * 50)
                        {
                            if (!interactionTracker.HasInteracted(character.nickname, "NPC_" + npc.type.ToString()))
                            {
                                PacketTypes.SNPCInfo sNPCInfo = new PacketTypes.SNPCInfo
                                {
                                    npctype = (ushort)npc.type,
                                    posX = npc.posX,
                                    posY = npc.posY,
                                    pseudoNPCId = (byte)npc.type,
                                    mapId = npc.mapId
                                };

                                byte[] msg = sNPCInfo.getValue();

                                if (character == null || character.m_Connection == null || character.m_Connection.GetStream() == null) continue;

                                await character.m_Connection.GetStream().WriteAsync(msg, 0, msg.Length);

                                Debug.Log(character.nickname + " interacted with NPC_" + npc.type);
                                interactionTracker.RegisterInteraction(character.nickname, "NPC_" + npc.type.ToString());
                            }
                        }
                        else
                        {
                            // 50 birimden uzaksa etkileşim kaydını temizle
                            interactionTracker.RemoveInteraction(character.nickname, "NPC_" + npc.type.ToString());
                        }
                    }

                    // CPU'yu rahatlatmak için kısa bir gecikme
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    Debug.Log("Error 1004: " + ex.Message);
                    await Task.Delay(2000); // Hata durumunda 2 saniye bekle
                }
            }
        }

        InteractionTracker interactionTracker = new InteractionTracker();

        private async void ExplorePlayerData(TcpClient client)
        {
            while (true)
            {
                try
                {
                    // m_InGameUsers.Count ile sınır kontrolü yapılıyor
                    int userCount = Server.Singleton.m_InGameUsers.Count;

                    for (int i = 0; i < userCount; i++)
                    {
                        var user = Server.Singleton.m_InGameUsers[i];
                        if (user == null) continue;

                        for (int j = i + 1; j < userCount; j++) // j, i + 1 olduğu için sayıyı aşmaması önemli
                        {
                            var user2 = Server.Singleton.m_InGameUsers[j];
                            if (user2 == null) continue;

                            float distanceSqr = (user.m_Characters[0].position - user2.m_Characters[0].position).sqrMagnitude;

                            if (user.m_Characters[0].currentMap == user2.m_Characters[0].currentMap)
                            {
                                if (distanceSqr < 50)
                                {
                                    if (!interactionTracker.HasInteracted(user.username, user2.username))
                                    {
                                        user.m_Characters[0].SendEveryCharacterInfoToEveryone();
                                        user2.m_Characters[0].SendEveryCharacterInfoToEveryone();

                                        Debug.Log(user.username + " interacted with " + user2.username);
                                        interactionTracker.RegisterInteraction(user.username, user2.username);
                                    }
                                }
                                else
                                {
                                    interactionTracker.RemoveInteraction(user.username, user2.username);
                                }
                            }

                            await Task.Delay(500);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("Error: " + ex.Message);
                    await Task.Delay(2000);
                }
            }

        }

        private void Handle(TcpClient client)
        {
            Byte[] array = new Byte[512];
            Mutex mutex = new Mutex();


            try
            {
                NetworkStream stream = client.GetStream();


                int i;
                
                while ((i = stream.Read(array, 0, array.Length)) > 0)
                {
                    ClientPackets packet_id = (ClientPackets)BitConverter.ToUInt16(new byte[2] { array[4], array[5] }, 0);
                    int packet_id2 = array[4];

                    string pkt = BitConverter.ToString(array).Replace("-", " ");

                    pkt = pkt.Substring(0, (pkt.LastIndexOf("55 AA") + "55 AA".Length));

                    if (packet_id != ClientPackets.PACKET_DUMMY && packet_id != ClientPackets.PACKET_MOVEREQUEST && packet_id != ClientPackets.PACKET_MOVERUNSWIFTREQUEST)
                    {
                        Debug.Log(packet_id + ":" + pkt, ConsoleColor.White);
                    }


                    if (packet_id == ClientPackets.PACKET_SERVERJOIN)
                    {
                        new Thread(() =>
                        {
                            byte[] msg = Functions.FromHex("AA 55 02 00 2B 00 55 AA ");
                            stream.Write(msg, 0, msg.Length);
                        }).Start();
                    }

                    if(packet_id == ClientPackets.PACKET_UPGRADESKILL)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.UpgradeSkillBook(client, array);
                        }).Start();  
                    }

                    if (packet_id == ClientPackets.PACKET_UPGRADEPASSIVESKILL)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.UpgradePassiveSkillBook(client, array);
                        }).Start();
                    }

                    if (packet_id == ClientPackets.PACKET_UPGRADEITEM)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.UpgradeItem(client, array);
                        }).Start();
                    }

                    if (packet_id == ClientPackets.PACKET_RESPAWNHERE)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.RespawnPlayerCurrentPos(client, array);
                        }).Start();

                    }

                    if (packet_id == ClientPackets.PACKET_RESPAWNSAFEZONE)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.RespawnPlayer(client, array);
                        }).Start();

                    }

                    if (packet_id == ClientPackets.PACKET_PKON)
                    {
                        new Thread(() =>
                        {
                            byte[] msg = Functions.FromHex("AA 55 02 00 2B 01 55 AA");
                            stream.Write(msg, 0, msg.Length);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_PKOFF)
                    {
                        new Thread(() =>
                        {
                            byte[] msg = Functions.FromHex("AA 55 02 00 2B 00 55 AA");
                            stream.Write(msg, 0, msg.Length);
                        }).Start();
                        
                    }
                  
                    if (packet_id == ClientPackets.PACKET_CHANGEMAP1)
                    {
                        /*
                        var member = Server.Singleton.GetMemberByConnection(client);

                        var mapId = Database.database.GetCharacterCurrentMap(member.m_Characters[0].nickname);

                        new Thread(() =>
                        {
                            byte[] msg = Functions.FromHex("AA 55 0E 00 73 01 01 00 00 01 01 00 00 00 01 00 00 01 55 AA");
                            stream.Write(msg, 0, msg.Length);
                        }).Start();*/
                    }

                    if (packet_id == ClientPackets.PACKET_CHANGEMAP2)
                    {
                        /*
                        var member = Server.Singleton.GetMemberByConnection(client);

                        var mapId = Database.database.GetCharacterCurrentMap(member.m_Characters[0].nickname);

                        new Thread(() =>
                        {
                            byte[] msg = Functions.FromHex("AA 55 07 00 01 B9 03 00 00 01 00 55 AA");
                            stream.Write(msg, 0, msg.Length);
                        }).Start();
                        */
                    }

                    if (packet_id == ClientPackets.PACKET_PARTYREQ)
                    {
                        new Thread(() => // AA 55 0D 00 52 01 0A 00 09 05 50 4C 41 59 45 52 11 55 AA
                        {
                            ServerFunctions.SendPartyRequest(client, array);
                        }).Start();
                    }

                    if (packet_id == ClientPackets.PACKET_PARTYREQ_ANSWER)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.AddPartyMember(client, array);
                        }).Start();
                    }

                    if (packet_id == ClientPackets.PACKET_PARTY_DISBAND)
                    {
                        new Thread(() =>
                        {
                            byte[] msg = Functions.FromHex("AA 55 02 00 52 05 55 AA");
                            stream.Write(msg, 0, msg.Length);
                        }).Start();
                    }

                    if (packet_id == ClientPackets.PACKET_LOGIN)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleLoginRequest(client, array);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_UNKNOWN)
                    {
                        new Thread(() =>
                        {

                            if (ClientSettings.VERSION == 222)
                            {
                                //   ServerFunctions.HandleLoadingScreenInfos(client, array); // v222
                            }
                            //ServerFunctions.HandleUnknownPacket(client, array);
                            if (ClientSettings.VERSION != 222)
                            {
                                ServerFunctions.HandleLoadingScreenInfos(client, array);
                                Debug.Log("HANDLE LOADING SCREEN INFOS 2 - CALLED!");

                            }
                            // ServerFunctions.HandleRequestCharacterInfo(client, array);

                        }).Start();
                    }

                    if (packet_id == ClientPackets.PACKET_LOADINGUNKNOWN2)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleLoadingScreenInfos(client, array);
                            Debug.Log("HANDLE LOADING SCREEN INFOS 3 - CALLED!");


                            Character character = Server.Singleton.GetMemberByConnection(client).m_Characters[0];

                            if (character == null) return;

                            character.currentMap = Database.database.GetCharacterCurrentMap(character.nickname);

                            character.posX = Database.database.GetCharacterPosition(character.nickname).x;
                            character.posY = Database.database.GetCharacterPosition(character.nickname).y;

                            Debug.Log("Character pos: " + character.posX + ", " + character.posY);

                            foreach (var user in Server.Singleton.m_InGameUsers)
                            {
                                user.m_Characters[0].SendEveryCharacterInfoToEveryone();
                                user.m_Characters[0].SendEveryCharactersInfoToClient();
                            }

                            SendInitialNPCs(client);
                        }).Start();

                        
                    }
                    
                    /*
                    if (packet_id == ClientPackets.PACKET_UNKQUIT)
                    {
                        //
                    }
                    */

                    if (packet_id == ClientPackets.PACKET_AFTERSPAWNUNK)
                    {
                        /*
                        new Thread(() =>
                        {

                        }).Start();

                        */
                        
                        // Debug.Log("INIT CHARACTER CALLED!", ConsoleColor.Red);
                        //byte[] x = Functions.FromHex("AA-55-CD-05-14-02-00-05-F4-01-05-05-05-05-05-05-05-05-05-05-05-00-05-05-06-06-05-05-05-05-05-05-05-05-05-05-05-05-05-05-64-0C-05-0C-05-06-05-05-05-05-05-05-05-01-55-AA");
                        // stream.Write(x, 0, x.Length);
                    }

                    if(pkt.Contains("AA 55 05 00 28")) // MAP CHANGE
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.ChangeMap(client, array);
                        }).Start();
                    }

                    if(pkt.Contains("AA 55 16 00 42") || pkt.Contains("AA 55 18 00 42")) // SINGLE - MULTI
                    {
                            new Thread(() =>
                            {
                                ServerFunctions.HandleAttackInfo(client, array, isSkill: true);
                            }).Start();
                    }

                    if (packet_id == ClientPackets.PACKET_CREATECHARACTER)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleCreateCharacter(client, array);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_SELECTSERVER)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleSelectServer(client, array);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_DELETECHARACTER || packet_id == ClientPackets.PACKET_DELETECHARACTER2)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleDeleteCharacter(client, array);
                        }).Start();
                        
                    }


                    if (packet_id == ClientPackets.PACKET_LOGINACCEPTANDCHANNELSELECT)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleRequestServerList(client, array);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_JOINCHANNEL)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleRequestJoinChannel(client, array);
                        }).Start();

                        
                    }

                   
                    if (packet_id == ClientPackets.PACKET_REQUESTCHARACTERMENU)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleRequestCharacterInfo(client, array);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_STARTGAMEBUTTON) // start pressed
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleStartGameRequest(client, array);
                        }).Start();
                        
                    }


                    if (pkt.Contains("AA 55 05 00 27")) 
                    {
                        /*
                        new Thread(() =>
                        {
                            ExploreMobData(client);
                        }).Start();*/
                        
                    }

                    if (packet_id == ClientPackets.PACKET_ITEMDROP)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleItemDrop(client, array);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_LOOTITEM)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.LootDroppedItem(client, array);
                        }).Start();

                    }

                    if (packet_id == ClientPackets.PACKET_ITEMDRAG)
                    {

                        new Thread(() =>
                        {
                     
                            ServerFunctions.HandleItemDrag(client, array);
                      
                        }).Start();

                    }

                    if(packet_id == ClientPackets.PACKET_ITEMUSAGE)
                    {
                        new Thread(() =>
                        {
                   
                            ServerFunctions.HandleItemUsage(client, array);
                    
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_ITEMDESTROY)
                    {
                        new Thread(() =>
                        {
                         
                            ServerFunctions.HandleItemDestroy(client, array);
                         
                        }).Start();
                        
                    }

                    if(packet_id == ClientPackets.PACKET_ITEMSELL)
                    {
                        new Thread(() =>
                        {
                 
                            ServerFunctions.HandleItemSell(client, array);
                        
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_JOINCHANNELACCEPT) // darkness connect
                    {
                        var member = ServerFunctions.HandleRedirectPlayer(client, array);

                        ServerFunctions.HandleRequestCharacterInfo(client, array, member);
                    }

                    if (packet_id == ClientPackets.PACKET_PLAYERPOSITIONINFO)
                    {
                        new Thread(() =>
                        {
                            Debug.Log("HANDLE LOADING SCREEN INFOS 5 - CALLED!");


                            ServerFunctions.HandleLoadingScreenInfos(client, array);
                            // ServerFunctions.NPCS(client, array);
                        }).Start();

                    }

                    if (packet_id == ClientPackets.PACKET_MOVEREQUEST || packet_id == ClientPackets.PACKET_MOVERUNSWIFTREQUEST || packet_id == ClientPackets.PACKET_MOVEFLY)
                    {
                        new Thread(() =>
                        {
                            // Debug.Log("move arr: " + BitConverter.ToString(array), ConsoleColor.Cyan);
                            ServerFunctions.HandleMoveRequest(client, array);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_CHATMSG)
                    {
                        new Thread(() =>
                        {
          
                            ServerFunctions.HandleChatMessage(client, array);
                   
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_CHATINFOWITHSLASH)
                    {
                        new Thread(() =>
                        {
                   
                            ServerFunctions.HandleChatSlashMessage(client, array);
                        
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_SELECTCHARACTER)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.SelectCHAR(client, array);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_MEDITATION)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleMeditationInfo(client, array);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_ATTMODE || packet_id == ClientPackets.PACKET_DEFMODE)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleAttackDefendModeInfo(client, array);
                        }).Start();
                        
                    }

                    if (array[2] == 4 && array[3] == 0 && array[4] == 0xC4) // stat point
                    {
                        new Thread(() =>
                        {
                      
                            ServerFunctions.HandleStatPointAdd(client, array);
                       
                        }).Start();
                        
                    }  // stat point

                    if (packet_id == ClientPackets.PACKET_GIVESTR)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleStatPointAdd(client, array, 2);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_GIVEDEX)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleStatPointAdd(client, array, 1);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_GIVEINT)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleStatPointAdd(client, array, 0);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_DUMMY)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.HandleDummyPackets(client, array);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_OPENNPCWINDOW)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.NPCWindowRequest(client, array);
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_OPENNPCSHOP)
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.NPCShopRequest(client, array);
                        }).Start();
                        
                    }

                    if((array[2] == 3 && array[3] == 0 && array[4] == 0xCF) || (array[2] == 8 && array[3] == 0 && array[4] == 0x57))
                    {
                        new Thread(() =>
                        {
                            ServerFunctions.NPCWindowRequest(client, array);
                        }).Start();
                        
                    }

                    if(packet_id == ClientPackets.PACKET_BUYITEMREQUEST)
                    {
                        new Thread(() =>
                        {
  
                            ServerFunctions.HandleBuyItem(client, array);
   
                        }).Start();
                        
                    }

                    if(packet_id == ClientPackets.PACKET_ITEMSWITCHREQUEST)
                    {
                        new Thread(() =>
                        {
                  
                            ServerFunctions.HandleSwitchItemRequest(client, array);
          
                        }).Start();
                        
                    }

                    if (packet_id == ClientPackets.PACKET_SWITCHWEAPON)
                    {
                        new Thread(() =>
                        {

                            ServerFunctions.HandleSwitchWeaponRequest(client, array);
                        }).Start();
                    }

                    if(packet_id == ClientPackets.PACKET_ITEMSEPERATE)
                    {
                        new Thread(() =>
                        {

                        ServerFunctions.HandleItemSeperate(client, array);
                        }).Start();
                    }

                    if(packet_id == ClientPackets.PACKET_ITEMCOMBINE)
                    {
                        new Thread(() =>
                        {
                     
                            ServerFunctions.HandleItemCombine(client, array);
                       
                        }).Start();
                    }

                    // PLAYER ATTACK
                    if(((int)(321 + (float)(packet_id - 321) / 256.0f) == 321 + (float)(packet_id - 321) / 256.0f) || packet_id == ClientPackets.PACKET_FIRSTATTACK || packet_id == ClientPackets.PACKET_ATKDATA || packet_id == ClientPackets.PACKET_BOWATTACK1 || packet_id == ClientPackets.PACKET_BOWATTACK2) // || ((int)(322 + (float)(packet_id - 322) / 256.0f) == 322 + (float)(packet_id - 322) / 256.0f) || ((int)(323 + (float)(packet_id - 323) / 256.0f) == 323 + (float)(packet_id - 323) / 256.0f) || ((int)(324 + (float)(packet_id - 324) / 256.0f) == 324 + (float)(packet_id - 324) / 256.0f) || ((int)(325 + (float)(packet_id - 325) / 256.0f) == 325 + (float)(packet_id - 325) / 256.0f)
                    {
                        new Thread(() =>
                        {

                        ServerFunctions.HandleAttackInfo(client, array);
                        }).Start();
                    }
                    
                }   

            }
            catch (Exception e)
            {
                new Thread(() =>
                {

                    Debug.Log("Handle: " + e.ToString());
                Server.Singleton.HandlePlayerQuitErrors();
                }).Start();
            }

        }
    }
}
