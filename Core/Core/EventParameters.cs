using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    class ServerConnectionEventArgs : EventArgs
    {
        public Server Server;
        public ConnectionType Type;
        public ServerConnectionEventArgs(Server server, ConnectionType type)
        {
            Server = server;
            Type = type;
        }
    }
}
