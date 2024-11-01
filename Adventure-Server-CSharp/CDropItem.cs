using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Protobuf.WellKnownTypes.Field.Types;

namespace Adventure_Server_CSharp.Tables
{
    public static class CDropItem
    {
        public static Dictionary<int, int> DroppedItemsList = new Dictionary<int, int>();

        public static int GetItemIdByPseudoId(int pseudoId)
        {
            // Check if the pseudoId exists in the dictionary
            if (DroppedItemsList.TryGetValue(pseudoId, out int itemId))
            {
                Debug.Log("TRYING TO COLLECT ITEM: " + pseudoId, ConsoleColor.Green);

                return itemId; // Return the itemId if the pseudoId is found
            }
            else
            {
                Debug.Log("TRYING TO COLLECT ITEM: " + pseudoId + " FAILED!", ConsoleColor.Green);

                return 0;
            }
        }

        public static int GetDroppedItem(int mobId)
        {
            string filePath = "..\\..\\Tables\\item_drops.csv";
            string[] lines = File.ReadAllLines(filePath);

            int _itemId = -1;

            int[] itemId = null;
            float[] rate = null;

            int itemCount = -1;

            foreach (string line in lines)
            {
                string[] values = line.Split(',');

                if (mobId != int.Parse(values[0])) continue;

                itemCount = int.Parse(values[1]);

                itemId = new int[itemCount];
                rate = new float[itemCount];

                for (int i = 0; i < itemCount; i++)
                {
                    itemId[i] = int.Parse(values[2 + i * 2]);
                    rate[i] = float.Parse(values[3 + i * 2], CultureInfo.InvariantCulture);
                }

                Console.WriteLine($"MobID: {values[0]}");
                for (int i = 0; i < itemCount; i++)
                {
                  //  Console.WriteLine($"Item {i + 1}: ID = {itemId[i]}, Rate = {rate[i]}");
                }
            }

            if (itemCount == -1) return -1;

            _itemId = -1;

            Random rnd = new Random();

            for (int i = 0; i < itemCount; i++)
            {
                float randomValue = (float)(rnd.NextDouble() * 100);


                if (randomValue <= (rate[i] * ServerSettings.DROP_RATE))
                {
                   // Debug.Log("Required rate lower than: " + rate[i] + " , you got " + randomValue, ConsoleColor.Green);

                    _itemId = itemId[i];
                    break;
                }
                else
                {
                    _itemId = -1;
                  //  Debug.Log("Required rate lower than: " + rate[i] + " , you got " + randomValue, ConsoleColor.Red);
                }
            }

            return _itemId;
        }
    }
}
