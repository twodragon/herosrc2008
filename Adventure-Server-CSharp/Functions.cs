using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public class Functions
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);

        public static byte[] GetMidBigEndianFloat(byte[] arr)
        {

            byte[] newArr = { arr[2], arr[3], arr[0], arr[1] };
            return newArr;
        }

        public static byte[] GetFloatLittleEndian(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return bytes;
        }

        public static byte[] GetBigEndianFloat(byte[] arr)
        {
            if (arr.Length != 4)
            {
                throw new ArgumentException("Input byte array must have exactly 4 bytes.");
            }

            // Reverse the byte order for big-endian to little-endian conversion
            byte[] bigEndianBytes = { arr[3], arr[2], arr[1], arr[0] };  // Switch byte order

            // Convert the byte array back to a float
            return bigEndianBytes;
        }

        public static bool IsPrivateIPAddress(string ipAddress)
        {
            int[] ipParts = ipAddress.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => int.Parse(s)).ToArray();
            // in private ip range
            if (ipParts[0] == 10 ||
                (ipParts[0] == 192 && ipParts[1] == 168) ||
                (ipParts[0] == 172 && (ipParts[1] >= 16 && ipParts[1] <= 31)))
            {
                return true;
            }

            if (ipParts[0] == 127 && ipParts[1] == 0 || ipParts[2] == 0 || ipParts[3] == 1)
                return true;

            // IP Address is probably public.
            // This doesn't catch some VPN ranges like OpenVPN and Hamachi.
            return false;
        }

        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        static T FromByteArray<T>(byte[] stream)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            if (stream == null)
                return default(T);

            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                T result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }

        public static void PrintByteArray(byte[] bytes)
        {
            int curIn = 0;
            var sb = new StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.Append(b + "(index:" + curIn + "), ");

                curIn++;
            }
            sb.Append("}\n");

            Console.WriteLine(sb.ToString());
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static UInt16 ReverseBytes16(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }


        public static UInt32 ReverseBytes32(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            hex = hex.Replace(" ", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return raw;
        }
    }
}
