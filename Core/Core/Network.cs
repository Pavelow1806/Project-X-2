using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Network
    {
        private static Network _instance = null;
        public static Network Instance
        {
            get
            {
                lock (_instance)
                {
                    if (_instance == null)
                    {
                        _instance = new Network();
                    }
                    return _instance;
                }
            }
        }

        public static bool Running = false;

        #region Connections
        public string AuthenticationCode = "";
        public Client[] Clients = new Client[Constants.MaxConnections];

        public List<Server> ServerQueue = new List<Server>();
        public Dictionary<ConnectionType, Server> Servers = new Dictionary<ConnectionType, Server>();
        #endregion

        public readonly AssetType MyAssetType = AssetType.NONE;
        private ConnectionType MyConnectionType = ConnectionType.NONE;
        private int? clientPort
        {
            get
            {
                switch (MyConnectionType)
                {
                    case ConnectionType.NONE:
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
        private int? serverPort
        {
            get
            {
                switch (MyConnectionType)
                {
                    case ConnectionType.NONE:
                        return null;
                    case ConnectionType.GAMESERVER:
                        return (int)Ports.GameServerPort;
                    case ConnectionType.CLIENT:
                        return (int)Ports.ClientServerPort;
                    case ConnectionType.LOGINSERVER:
                        return (int)Ports.LoginServerPort;
                    case ConnectionType.SYNCSERVER:
                        return (int)Ports.SyncServerPort;
                    default:
                        return null;
                }
            }
        }

        #region TCP
        private readonly Listener ServerListener = null;
        private readonly Listener ClientListener = null;
        #endregion

        #region Events
        public EventHandler<ServerConnectionEventArgs> OnServerAuthenticated;
        #endregion

        public Network(ConnectionType type) 
        { 
            MyConnectionType = type;
            if (type != ConnectionType.CLIENT)
                MyAssetType = AssetType.SERVER;
            else if (type != ConnectionType.NONE)
                MyAssetType = AssetType.SERVER;
            else
                MyAssetType = AssetType.NONE;

            int Port;

            int? ServerPort = serverPort;
            if (serverPort != null)
            {
                Port = (int)ServerPort;
                ServerListener = new Listener(IPAddress.Parse("127.0.0.1"), Port, AssetType.SERVER);
            }
            else
            {
                Log.Write(LogType.Error, "The port returned for the server listener was null.");
            }

            int? ClientPort = clientPort;
            if (clientPort != null)
            {
                Port = (int)ServerPort;
                ClientListener = new Listener(IPAddress.Any, Port);
            }
            else
            {
                Log.Write(LogType.Error, "The port returned for the client listener was null.");
            }
        }

        public bool LaunchServer()
        {
            try
            {
                AuthenticationCode = "123";
                //AuthenticationCode = Database.instance.RequestAuthenticationCode();
                if (string.IsNullOrEmpty(AuthenticationCode))
                {
                    Log.Write(LogType.Error, "Couldn't load authentication code");
                    return false;
                }
                if (ServerListener != null)
                {
                    ServerListener.StartAccept();
                    Log.Write(LogType.Connection, "Server Listener started");
                }

                for (int i = 0; i < Constants.MaxConnections; i++)
                {
                    Clients[i] = new Client(ConnectionType.CLIENT, i);
                }
                Log.Write(LogType.Information, $"Created {Constants.MaxConnections} client sockets");

                if (ClientListener != null)
                {
                    ClientListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    ClientListener.StartAccept();
                    Log.Write(LogType.Connection, "Client Listener started");
                }
            }
            catch (Exception ex)
            {
                Log.Write("An error occurred when starting the server", ex);
                return false;
            }
            Log.Write(LogType.Information, "Server started");
            return Running = true;
        }

        public void Connect()
        {
            if (MyAssetType == AssetType.NONE)
            {
                Log.Write(LogType.Error, $"The asset type I have is {MyAssetType.ToString()}, which is invalid");
                return;
            }
            if (MyAssetType == AssetType.CLIENT)
            {
                // If this is a client, start connecting to the login server
                Server LoginServer = new Server(ConnectionType.LOGINSERVER, -1);
                Servers.Add(ConnectionType.LOGINSERVER, LoginServer);
                LoginServer.StartConnecting();
            }
            else
            {
                // If this is a server, connect to the other servers in the cluster

            }
        }

        /// <summary>
        /// Given the destination, provides the port you need to connect to.
        /// </summary>
        /// <param name="Destination">The destination connection type.</param>
        /// <returns>The Port you need to use to connect to the destination listener (Can be null).</returns>
        private int? GetDestinationPort(ConnectionType Destination)
        {
            switch (Destination)
            {
                case ConnectionType.NONE:
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
                default:
                    return null;
            }
        }

        #region Events
        public void RegisterServerEvents(Server server)
        {
            if (server == null) return;
            server.OnAuthenticate += Authentication_AuthenticatedEvent;
            server.OnClose += Server_OnClose;
        }
        private void Authentication_AuthenticatedEvent(object sender, ServerConnectionEventArgs e)
        {
            if (Servers.ContainsKey(e.Type))
            {
                Log.Write(LogType.Error, $"{e.Type.ToString()} authentication failed, a server of this type is already connected");
                return;
            }
            Servers.Add(e.Type, e.Server);
            ServerQueue.Remove(e.Server);
            Log.Write(LogType.Information, $"{e.Type.ToString()} successfully authenticated");
            OnServerAuthenticated(this, e);
        }
        private void Server_OnClose(object sender, ServerConnectionEventArgs e)
        {
            if (Servers.ContainsKey(e.Type))
            {
                Servers[e.Type] = null;
                Servers.Remove(e.Type);
                return;
            }
            if (ServerQueue.Contains(e.Server))
            {
                e.Server = null;
                ServerQueue.Remove(e.Server);
            }
        }
        #endregion
    }
}


