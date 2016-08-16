using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silkroad_Fusion
{
    class Data
    {
        public static List<Server> ServerList = new List<Server>();
        public struct Server
        {
            public ushort ID;
            public String name;
            public uint curPlayers;
            public uint maxPlayers;

            public Server(ushort pID, String pName, uint pCurPlayer, uint pMaxPlayer)
            {
                this.ID = pID;
                this.name = pName;
                this.curPlayers = pCurPlayer;
                this.maxPlayers = pMaxPlayer;
            }
        }
    }
}
