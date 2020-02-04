using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class NetworkBase
    {

        public static bool Running = false;

        public readonly AssetType MyAssetType = AssetType.NONE;
        public readonly ConnectionType MyConnectionType = ConnectionType.UNKNOWN;
        private int? ClientPort
        {
            get
            {
                switch (MyConnectionType)
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
                    default:
                        return null;
                }
            }
        }
        private int? ServerPort
        {
            get
            {
                switch (MyConnectionType)
                {
                    case ConnectionType.UNKNOWN:
                        return null;
                    case ConnectionType.GAMESERVER:
                        return (int)Ports.GameServerPort;
                    case ConnectionType.CLIENT:
                        return (int)Ports.ClientServerPort;
                    case ConnectionType.LOGINSERVER:
                        return (int)Ports.LoginServerPort;
                    case ConnectionType.SYNCSERVER:
                        return (int)Ports.SyncServerPort;
                    case ConnectionType.TOOL:
                        return (int)Ports.ToolServerPort;
                    default:
                        return null;
                }
            }
        }

        public NetworkBase(AssetType type) 
        {
            MyAssetType = type;
        }

        /// <summary>
        /// Attempt to connect to the other servers in the cluster
        /// </summary>
        public virtual List<ConnectionType> Connect()
        {
            List<ConnectionType> result = new List<ConnectionType>();
            if (MyAssetType == AssetType.NONE)
            {
                Log.Write(LogType.Error, $"The asset type I have is {MyAssetType.ToString()}, which is invalid");
            }
            if (MyAssetType == AssetType.TOOL)
            {
                ConnectToServer(ConnectionType.LOGINSERVER);
                result.Add(ConnectionType.LOGINSERVER);
                ConnectToServer(ConnectionType.GAMESERVER);
                result.Add(ConnectionType.GAMESERVER);
                ConnectToServer(ConnectionType.SYNCSERVER);
                result.Add(ConnectionType.SYNCSERVER);
            }
            else if (MyAssetType == AssetType.CLIENT)
            {
                ConnectToServer(ConnectionType.LOGINSERVER);
                result.Add(ConnectionType.LOGINSERVER);
            }
            else
            {
                // If this is a server, connect to the other servers in the cluster
                switch (MyConnectionType)
                {
                    case ConnectionType.UNKNOWN:
                        Log.Write(LogType.Error, "The connection type of this application is not setup correctly");
                        break;
                    case ConnectionType.CLIENT:
                        Log.Write(LogType.Error, "The connection type of this application is not setup correctly");
                        break;
                    case ConnectionType.GAMESERVER:
                        ConnectToServer(ConnectionType.LOGINSERVER);
                        result.Add(ConnectionType.LOGINSERVER);
                        ConnectToServer(ConnectionType.SYNCSERVER);
                        result.Add(ConnectionType.SYNCSERVER);
                        ConnectToServer(ConnectionType.TOOL, Constants.MaxToolConnectAttempts);
                        result.Add(ConnectionType.TOOL);
                        break;
                    case ConnectionType.LOGINSERVER:
                        ConnectToServer(ConnectionType.GAMESERVER);
                        result.Add(ConnectionType.GAMESERVER);
                        ConnectToServer(ConnectionType.SYNCSERVER);
                        result.Add(ConnectionType.SYNCSERVER);
                        ConnectToServer(ConnectionType.TOOL, Constants.MaxToolConnectAttempts);
                        result.Add(ConnectionType.TOOL);
                        break;
                    case ConnectionType.SYNCSERVER:
                        ConnectToServer(ConnectionType.LOGINSERVER);
                        result.Add(ConnectionType.LOGINSERVER);
                        ConnectToServer(ConnectionType.GAMESERVER);
                        result.Add(ConnectionType.GAMESERVER);
                        ConnectToServer(ConnectionType.TOOL, Constants.MaxToolConnectAttempts);
                        result.Add(ConnectionType.TOOL);
                        break;
                    default:
                        Log.Write(LogType.Error, "The connection type of this application is not setup correctly");
                        break;
                }
            }
            return (result.Count == 0 ? null : result);
        }

        protected virtual void ConnectToServer(ConnectionType Destination, int MaxConnectionAttempts = -1)
        {
            //int Port;
            //if (!ServerAuthenticated(Destination))
            //{
            //    int? ServerPort = GetDestinationPort(Destination);
            //    if (ServerPort != null)
            //    {
            //        Port = (int)ServerPort;
            //        Server Server = new Server(Destination, Port);
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
        protected virtual int? GetDestinationPort(ConnectionType Destination)
        {
            switch (Destination)
            {
                case ConnectionType.UNKNOWN:
                    return null;
                case ConnectionType.GAMESERVER:
                    if (MyAssetType == AssetType.CLIENT)
                        return (int)Ports.GameClientPort;
                    else if (MyAssetType != AssetType.NONE)
                        return (int)Ports.GameServerPort;
                    else
                        return null;
                case ConnectionType.CLIENT:
                    if (MyAssetType == AssetType.CLIENT)
                        return null;
                    else if (MyAssetType != AssetType.NONE)
                        return (int)Ports.ClientServerPort;
                    else
                        return null;
                case ConnectionType.LOGINSERVER:
                    if (MyAssetType == AssetType.CLIENT)
                        return (int)Ports.LoginClientPort;
                    else if (MyAssetType != AssetType.NONE)
                        return (int)Ports.LoginServerPort;
                    else
                        return null;
                case ConnectionType.SYNCSERVER:
                    if (MyAssetType == AssetType.CLIENT)
                        return null;
                    else if (MyAssetType != AssetType.NONE)
                        return (int)Ports.SyncServerPort;
                    else
                        return null;
                case ConnectionType.TOOL:
                    if (MyAssetType == AssetType.CLIENT)
                        return null;
                    else if (MyAssetType != AssetType.NONE)
                        return (int)Ports.ToolServerPort;
                    else
                        return null;
                default:
                    return null;
            }
        }
    }
}


