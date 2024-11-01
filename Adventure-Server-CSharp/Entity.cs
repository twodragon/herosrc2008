using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Adventure_Server_CSharp
{

    public class Entity
    {

        /// <summary>
        public static List<Entity> m_EntityList = new List<Entity>();
        /// </summary>
        public Entity()
        {
           
        }

        ~Entity()  // finalizer
        {
          
        }

      
    }


    public class Player : Entity
    {
        /// <summary>
        public static List<Player> m_PlayerList = new List<Player>();
        /// </summary>
        public Player()
        {
           
        }

        public Player(IPEndPoint ip)
        {
           
        }

        public Player(IPEndPoint ip, string name, Vector2 position, Vector2 direction, char[,] app)
        {
           
        }

        public Player(IPEndPoint ip, string name, Vector2 position, Vector2 direction, char app)
        {
        }


        ~Player()  // finalizer
        {
           
        }


    }


    
}