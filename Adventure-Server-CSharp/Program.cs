using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;


namespace Adventure_Server_CSharp
{
    class Program
    {
        public static List<TcpClient> clients = new List<TcpClient>();
        public static readonly string SERVER_IP = "85.97.76.108";
        public static readonly ushort SERVER_PORT = 7777;
        public static readonly string DATABASE_USERNAME = "root";
        public static readonly string DATABASE_PASS = "";

        public static readonly string DATABASE_AUTHTABLE = "hero_auth";


        static void Initialize()
        {
            Database.database.Connect();

            Server.Singleton = new Server(7777);

            Server.Singleton.StartListening();

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(ProcessExit);

        }

        static void ProcessExit(object sender, EventArgs e)
        {
            Server.Singleton.SaveAllCharacters();
        }


        static void Main(string[] args)
        {
            Initialize();

            while (true)
            {
                byte[] inputBuffer = new byte[2048];
                Stream inputStream = Console.OpenStandardInput(inputBuffer.Length);
                Console.SetIn(new StreamReader(inputStream, Console.InputEncoding, false, inputBuffer.Length));

                string inputMessage = Console.ReadLine();

                if (inputMessage.Length < 6) continue;

                char[] inputCharArray = inputMessage.ToCharArray();

                if (inputCharArray[0] != '!')
                {
                    var inputLength = DataLengthCalculator.CalculateSendDataLength(inputMessage);

                    inputCharArray[6] = inputLength[0];
                    inputCharArray[7] = inputLength[1];
                    inputCharArray[9] = inputLength[2];
                    inputCharArray[10] = inputLength[3];
                }
                else
                    inputCharArray[0] = ' ';

                inputMessage = new string(inputCharArray);
                inputMessage = inputMessage.Replace("-", " ");

                // Check if there are any connected clients
                lock (Program.clients) // Lock to prevent concurrent access issues
                {
                    if (Program.clients.Count > 0)
                    {
                        byte[] x = Functions.FromHex(inputMessage);

                        // Iterate through all connected clients and send data
                        foreach (TcpClient client in Program.clients)
                        {
                            if (client != null && client.Connected)
                            {
                                try
                                {
                                    NetworkStream stream = client.GetStream();
                                    stream.Write(x, 0, x.Length); // Send the message to the client
                                    Debug.Log("DATA SEND SUCCESS!", ConsoleColor.Green);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Exception while sending data: " + e.Message);
                                }
                            }
                        }
                        Debug.Log(inputMessage, ConsoleColor.Green);
                    }
                    else
                    {
                        Console.WriteLine("No connected clients.");
                    }
                }
            }
        }
    }
}
