using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ServerConnectionEventArgs : EventArgs
    {
        public Server Server;
        public string IP;
        public ConnectionType Type;
        public ServerConnectionEventArgs(Server server, string ip, ConnectionType type)
        {
            Server = server;
            IP = ip;
            Type = type;
        }
    }

    public class PacketEventArgs : EventArgs
    {
        public Packet Packet;
        public PacketEventArgs(Packet packet)
        {
            Packet = packet;
        }
    }
}
