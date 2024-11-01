using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    internal class DataLengthCalculator
    {
        public static string CalculateHexDataLength(string hexString)
        {
            string result = hexString;
            result = RemoveSpaces(result);
            result = AddSpacesEveryTwoCharacters(result);
            ushort dataLength = CountSpaces(result);
            dataLength++;
            // Console.WriteLine("\nDATA LENGTH: " + dataLength);

            return Functions.ReverseBytes16(dataLength).ToString("X4");
        }

        public static string CalculateSendDataLength(string hexString)
        {
            string result = hexString;
            result = RemoveSpaces(result);
            result = AddSpacesEveryTwoCharacters(result);
            ushort dataLength = CountSpaces(result);
            dataLength -= 5;
           //  Console.WriteLine("\nDATA LENGTH: " + dataLength);
            return Functions.ReverseBytes16(dataLength).ToString("X4");
        }

        static string RemoveSpaces(string input)
        {
            input = input.Replace("-", "");
            input = input.Replace(" ", "");
            return input;
        }

        static string AddSpacesEveryTwoCharacters(string input)
        {
            var result = new System.Text.StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                if (i > 0 && i % 2 == 0)
                {
                    result.Append(' ');
                }
                result.Append(input[i]);
            }
            return result.ToString();
        }

        static ushort CountSpaces(string input)
        {
            ushort spaceCount = 0;
            foreach (char c in input)
            {
                if (c == ' ')
                {
                    spaceCount++;
                }
            }
            return spaceCount;
        }
    }
}
