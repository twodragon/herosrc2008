using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public static class Utils
    {
        public static async Task WaitForSeconds(float seconds)
        {
            await Task.Delay((int)seconds * 1000); 
        }

        public static int GetItemIdByItemSlot(Character _char, int itemSlotId)
        {
            return (int)_char.m_Inventory.m_Items.Find(x => x.itemSlot == itemSlotId).itemId;
        }

        public static int FindEmptyInventorySlot(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            Member member = Server.Singleton.GetMemberByConnection(client);

            Character character = member.m_Characters[0];

            int[] inventorySlots = new int[56];

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i] = 0;
            }

            foreach (var item in character.m_Inventory.m_Items)
            {
                int index = item.itemSlot - 11;
                if (index >= 0 && index < inventorySlots.Length)
                {
                    //Debug.Log("item slot: " + index + " , itemType: " + item.itemType + ", itemID: " + item.itemId);
                    inventorySlots[index] = 1;
                }
            }

            int emptySlotId = 999 - 11;// 999 is null

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i] == 0)
                {
                    emptySlotId = i;
                  //  Debug.Log("Slot " + (11 + emptySlotId) + " is empty! Item placed there.", ConsoleColor.Green);
                    break;
                }
            }

            return 11 + emptySlotId;
        }
    }
}
