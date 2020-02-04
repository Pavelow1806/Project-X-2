//using Server;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Network : NetworkBase
    {
        public static Network Instance { get; private set; }
        private static void AssignInstance(Network network) { Instance = network; }

        public static bool Running = false;

        #region Connections
        public Dictionary<ConnectionType, Connection> Servers = new Dictionary<ConnectionType, Connection>();
        #endregion

        private int? ServerPort
        {
            get
            {
                return (int)Ports.ClientServerPort;
            }
        }

        #region Events
        public EventHandler<PacketEventArgs> OnPacketReceived;
        #endregion

        public Network(AssetType type) :
            base (type)
        {
            AssignInstance(this);
        }

        /// <summary>
        /// Begin connecting to the login server.
        /// </summary>
        public override List<ConnectionType> Connect()
        {
            List<ConnectionType> result = new List<ConnectionType>();
            ConnectToServer(ConnectionType.LOGINSERVER);
            result.Add(ConnectionType.LOGINSERVER);
            return result;
        }

        protected override void ConnectToServer(ConnectionType Destination, int MaxConnectionAttempts = -1)
        {
            //int Port;
            //if (!Servers.Any(x => x.Value.Type == Destination))
            //{
            //    int? ServerPort = GetDestinationPort(Destination);
            //    if (ServerPort != null)
            //    {
            //        Port = (int)ServerPort;
            //        Connection Server = new Connection(Destination, Port);
            //        lock (ServerQueue)
            //        {
            //            if (ServerQueue.Any(x => x.IP == Server.IP && x.Port == Server.Port))
            //            {
            //                Log.Write(LogType.Error, $"Stopped attempt to create duplicate connection on IP {Server.IP} and port {Server.Port}");
            //                return;
            //            }
            //        }

            //        RegisterServerEvents(Server);
            //        ServerQueue.Add(Server);
            //        Server.StartConnecting(MaxConnectionAttempts);
            //    }
            //    else
            //    {
            //        Log.Write(LogType.Error, Destination, "The port returned for the server was null.");
            //    }
            //}
            //else
            //{
            //    Log.Write(LogType.Connection, Destination, "The connection to the server has already been established");
            //}
        }

        /// <summary>
        /// Given the destination, provides the port you need to connect to.
        /// </summary>
        /// <param name="Destination">The destination connection type.</param>
        /// <returns>The Port you need to use to connect to the destination listener (Can be null).</returns>
        protected override int? GetDestinationPort(ConnectionType Destination)
        {
            switch (Destination)
            {
                case ConnectionType.UNKNOWN:
                    return null;
                case ConnectionType.GAMESERVER:
                    return (int)Ports.GameClientPort;
                case ConnectionType.CLIENT:
                    return null;
                case ConnectionType.LOGINSERVER:
                    return (int)Ports.LoginClientPort;
                case ConnectionType.SYNCSERVER:
                    return null;
                case ConnectionType.TOOL:
                    return null;
                default:
                    return null;
            }
        }

        #region Events
        public void RegisterServerEvents(Connection server)
        {
            if (server == null) return;
            server.OnClose += Server_OnClose;
            server.OnDataReceived += DataReceived;
        }
        private void Server_OnClose(object sender, ConnectionEventArgs e)
        {
            try
            {
                lock (Servers)
                {
                    if (Servers.ContainsKey(e.Type))
                    {
                        Servers[e.Type] = null;
                        Servers.Remove(e.Type);
                        Log.Write(LogType.Connection, $"{e.Type.ToString()}{(e.Type == ConnectionType.UNKNOWN ? $" {e.IP}" : "")} was closed, server removed from servers list");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write("An error was thrown during Server_OnClose", ex);
            }
        }
        private void DataReceived(object sender, NewDataEventArgs e)
        {
            try
            {
                OnPacketReceived(sender, new PacketEventArgs(ProcessData.Process((Connection)sender, e.Data), (Connection)sender));
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
        #endregion
    }
}


