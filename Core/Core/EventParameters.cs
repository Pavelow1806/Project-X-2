//using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ListenerCallbackEventArgs : EventArgs
    {
        public TcpClient NewConnection;
        public ListenerCallbackEventArgs(TcpClient newConnection)
        {
            NewConnection = newConnection;
        }
    }

    public class NewConnectionEventArgs : EventArgs
    {
        public Connection Connection;
        public string IP;
        public ConnectionType Type;
        public NewConnectionEventArgs(Connection connection, string ip, ConnectionType type)
        {
            Connection = connection;
            IP = ip;
            Type = type;
        }
    }

    public class NewDataEventArgs : EventArgs
    {
        public byte[] Data;
        public NewDataEventArgs(byte[] data)
        {
            Data = data;
        }
    }

    public class PacketEventArgs : EventArgs
    {
        public Packet Packet;
        public Connection Source;
        public PacketEventArgs(Packet packet, Connection source)
        {
            Packet = packet;
            Source = source;
        }
    }

    public class ConnectionEventArgs : EventArgs
    {
        public Connection Connection;
        public string IP;
        public ConnectionType Type;
        public ConnectionEventArgs(Connection connection, string ip, ConnectionType type)
        {
            Connection = connection;
            IP = ip;
            Type = type;
        }
    }
}
