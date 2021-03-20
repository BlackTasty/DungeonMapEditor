using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Patcher
{
    static class ServerList
    {
        public static readonly List<Server> DEFAULT_SERVERS = new List<Server>()
        {
#if DEBUG
            new Server("http://localhost/dme/"), 
#endif
            new Server("http://blacktasty.bplaced.net/dme/"),
            new Server("https://vibrance.lima-city.de/dme/")
        };
    }
}
