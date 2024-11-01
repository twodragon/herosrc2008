using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.IO;
using System.Configuration;

namespace Adventure_Server_CSharp
{
 

    public class Server
    {

        public Server(int port)
        {
            try
            {
                m_Port = port;

                m_TcpListener = new TcpListener(IPAddress.Any, port);
            }
            catch(Exception e)
            {
                Debug.Log("ERROR: " + e.ToString());
            }
        }

        public void StartListening()
        {
            try
            {
                m_TcpListener.Start();

                WaitForClientConnect();

                new Thread(MainServerThread).Start();

                Debug.Log("Started TCP listening on " + m_Port);
            }
            catch(Exception e)
            {
                Debug.Log("ERROR: " + e.ToString());
            }
        }
    

        public void KickIfNoMessage()
        {
            while(true)
            {
                m_InGameUsers = m_InGameUsers.Distinct().ToList();


                for (int i = 0; i < m_InGameUsers.Count; i++)
                {
                    try
                    {
                        int diff = DateTime.Now.Second - m_InGameUsers[i].getKickTime;

                        if (diff > 30)
                        {
                            Database.database.SaveCharacterInfo(m_InGameUsers[i].m_Characters[0], m_InGameUsers[i]);
                            PacketTypes.SRemoveCharacterQuitMessage quitMessage = new PacketTypes.SRemoveCharacterQuitMessage();
                            quitMessage.uid = Convert.ToUInt16(m_InGameUsers[i].user_id);
                            m_InGameUsers[i].m_Connection.Close();
                            m_InGameUsers.Remove(m_InGameUsers[i]);

                            SendAll(quitMessage.getValue());
                        }
                    }
                    catch
                    {

                    }
                    
                }

                Thread.Sleep(5000);
            }
        }

        public void SaveAllCharacters()
        {
            try
            {
                for (int i = 0; i < m_InGameUsers.Count; i++)
                {
                    Database.database.SaveCharacterInfo(m_InGameUsers[i].m_Characters[0], m_InGameUsers[i]);
                }
            }
            catch (Exception x) { };
        }

        public void Init()
        {
            m_GameNPCs = Database.database.GetNPCs();

            //

            string filePath = "..\\..\\Tables\\mobs.csv";

            int i = 0;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(',');

                    if (values.Length > 4)
                    {
                        int mobId = int.Parse(values[0].Trim('"'));
                        int mapId = int.Parse(values[1]);

                        int posX = int.Parse(values[2]) + 10; // old hero pos tilt
                        int posY = int.Parse(values[3]);

                        int mobCount = int.Parse(values[4]);

                        if (mobCount > 5) mobCount = 5;

                        // Console.WriteLine($"mobId: {mobId}, mapId: {mapId}, posX: {posX}, posY: {posY}, mobCount: {mobCount}");

                        MobSpawnerController.SpawnMob(new Vector3(posX, posY, 0), (ushort)mobId, 5, mobCount, mapId);
                        i++;
                    }
                }
                Debug.Log("------------------ " + i + " MOB SPAWNER CREATED --------------");
            }
        }

        public void MobSpawnersThread()
        {

            for (int i = 0; i < MobSpawner.MobSpawnerList.Count; i++)
            {
                MobSpawner.MobSpawnerList[i].Start();
            }

            while (ServerSettings.ENABLE_MOBS)
            {
                for (int i = 0; i < MobSpawner.MobSpawnerList.Count; i++)
                {
                    MobSpawner.MobSpawnerList[i].Update();
                }

                Thread.Sleep(100);
            }
        }

        public void MobsThread()
        {

            while (ServerSettings.ENABLE_MOBS)
            {

                for(int i = 0;i < Mob.mobList.Count;i++)
                {
                    if (Mob.mobList[i] == null) continue;

                    Mob.mobList[i].Update();
                }

                Thread.Sleep(1);
            }
        }

        public void MainServerThread()
        {
            new Thread(Init).Start();

            new Thread(KickIfNoMessage).Start();

            new Thread(MobSpawnersThread).Start();
            new Thread(MobsThread).Start();

            while (true)
            {

                SaveAllCharacters();

                Thread.Sleep(5000);
            }
        }

        public void HandlePlayerQuitErrors()
        {

            //m_InGameUsers = m_InGameUsers.Distinct().ToList();


            for (int i = 0; i < m_InGameUsers.Count; i++)
            {
                byte[] x = { 0 };
                try
                {
                    m_InGameUsers[i].m_Connection.GetStream().Write(x, 0, x.Length);
                }
                catch(Exception e)
                {
                    try
                    {
                        Database.database.SaveCharacterInfo(m_InGameUsers[i].m_Characters[0], m_InGameUsers[i]);
                        PacketTypes.SRemoveCharacterQuitMessage quitMessage = new PacketTypes.SRemoveCharacterQuitMessage();
                        quitMessage.uid = Convert.ToUInt16(m_InGameUsers[i].user_id);
                        m_InGameUsers[i].m_Connection.Close();
                        m_InGameUsers.RemoveAt(i);

                        SendAll(quitMessage.getValue());
                    }
                    catch
                    {
                        Debug.Log("Connection problem detected!");
                    }

                    
                }
            }


        }


        public TcpListener m_TcpListener = new TcpListener(IPAddress.Any, 7777);
        int m_Port = 7777;

        public List<Member> m_InGameUsers = new List<Member>();
        public List<NPC> m_GameNPCs = new List<NPC>();


        private void WaitForClientConnect()
        {
            try
            {
                object obj = new object();
                m_TcpListener.BeginAcceptTcpClient(new System.AsyncCallback(OnClientConnect), obj);
            }
            catch (Exception e)
            {
                Debug.Log("ERROR: " + e.ToString());
            }

        }


        private void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                TcpClient client = m_TcpListener.EndAcceptTcpClient(asyn);

                lock (Program.clients)
                {
                    Program.clients.Add(client);  // Add the client to the list
                }

                ClientRequest clientReq = new ClientRequest(client);
            }
            catch (Exception se)
            {
                Debug.Log("ERROR: " + se.ToString());
            }

            WaitForClientConnect();  // Continue to listen for other clients
        }



        public void SendAll(byte[] array)
        {
            for (int i = 0; i < m_InGameUsers.Count; i++)
            {
                try
                {
                    if (m_InGameUsers[i] == null) Debug.Log("GetUser Error!", ConsoleColor.Red);
                    if (m_InGameUsers[i].m_Connection.GetStream() == null) Debug.Log("GetStream Error!", ConsoleColor.Red);

                    m_InGameUsers[i].m_Connection.GetStream().Write(array, 0, array.Length);
                   // Debug.Log("Message sended to " + m_InGameUsers[i].m_Characters[0].nickname, ConsoleColor.Green);
                }
                catch (Exception e)
                {
                    Debug.Log("HATA SENDALL: " + e.Message);
                    HandlePlayerQuitErrors();
                    //m_InGameUsers.RemoveAt(i);
                }
            }
        }

        public void SendToPlayer(Member member, byte[] array)
        {

            try
            {

                member.m_Connection.GetStream().Write(array, 0, array.Length);
                // Debug.Log("Message sended to " + m_InGameUsers[i].m_Characters[0].nickname, ConsoleColor.Green);
            }
            catch (Exception e)
            {
                Debug.Log("HATA SEND TO PLAYER: " + e.Message);
                HandlePlayerQuitErrors();
                //m_InGameUsers.RemoveAt(i);
            }
        }

        public void SendAllExceptThis(byte[] array, TcpClient client)
        {

            for (int i = 0; i < m_InGameUsers.Count; i++)
            {
                try
                {
                    string ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    string ip2 = ((IPEndPoint)m_InGameUsers[i].m_Connection.Client.RemoteEndPoint).Address.ToString();

                    int port = ((IPEndPoint)client.Client.RemoteEndPoint).Port;
                    int port2 = ((IPEndPoint)m_InGameUsers[i].m_Connection.Client.RemoteEndPoint).Port;

                    if (ip.Trim() == ip2.Trim() && port == port2)
                        continue;

                    m_InGameUsers[i].m_Connection.GetStream().WriteAsync(array, 0, array.Length);
                }
                catch (Exception e)
                {
                    HandlePlayerQuitErrors();
                    //m_InGameUsers.RemoveAt(i);
                }
            }
        }




        private readonly object _lockObject = new object();

        public Member GetMemberByConnection(TcpClient client)
        {
            try
            {
                if (client == null || client.Client == null)
                {
                    Debug.Log("ERROR 1002: Client or socket is null.");
                    return null;
                }

                if (!client.Client.Connected || client.Client.RemoteEndPoint == null)
                {
                    // Debug.Log("ERROR 1003: Client socket has been closed or disposed.");
                    return null;
                }

                lock (_lockObject)
                {
                    Member member = m_InGameUsers.Find(x =>
                        x != null &&
                        x.m_Connection != null &&
                        x.m_Connection.Client != null &&
                        x.m_Connection.Client.Connected &&
                        x.m_Connection.Client.RemoteEndPoint != null &&
                        ((IPEndPoint)x.m_Connection.Client.RemoteEndPoint).Address.ToString() == ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString() &&
                        ((IPEndPoint)x.m_Connection.Client.RemoteEndPoint).Port == ((IPEndPoint)client.Client.RemoteEndPoint).Port
                    );

                    return member ?? null;
                }
            }
            catch (ObjectDisposedException ode)
            {
                Debug.Log("ERROR 1004: Socket has been disposed. " + ode.Message);
            }
            catch (Exception ex)
            {
                Server.Singleton.HandlePlayerQuitErrors();
                Debug.Log("ERROR 1001: " + ex);
            }

            return null;
        }


        public Member GetMemberFromNickname(string nickname)
        {
            Member member = m_InGameUsers.Find(x => x.m_Characters[0].nickname == nickname); ;

            if (member != default(Member))
                return member;

            return null;
        }


        public Member GetMemberFromUsername(string username)
        {
            Member member = m_InGameUsers.Find(x => x.username == username); ;

            if (member != default(Member))
                return member;

            return null;
        }

        public Member GetMemberFromSessionId(int sessionId)
        {
            Member member = m_InGameUsers.Find(x => x.user_id == sessionId); ;

            if(member != null)
            Debug.Log("member attached: " + member.user_id);

            if (member != default(Member))
                return member;

            return null;
        }

        public static Server Singleton = new Server(7777);
    }
}
