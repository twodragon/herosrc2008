using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure_Server_CSharp
{
    public class InteractionTracker
    {
        private HashSet<(string, string)> interactedUsers = new HashSet<(string, string)>();

        public bool HasInteracted(string user1, string user2)
        {
            return interactedUsers.Contains((user1, user2)) || interactedUsers.Contains((user2, user1));
        }

        public void RegisterInteraction(string user1, string user2)
        {
            interactedUsers.Add((user1, user2));
        }

        public void RemoveInteraction(string user1, string user2)
        {
            interactedUsers.Remove((user1, user2));
            interactedUsers.Remove((user2, user1));
        }
    }
}
