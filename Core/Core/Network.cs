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
        #region Locking
        private static readonly object lockObj = new object();
        #endregion

        private static Network _instance = null;
        public static Network instance
        {
            get
            {
                lock (lockObj)
                {
                    if (_instance == null)
                    {
                        _instance = new Network();
                    }
                    return _instance;
                }
            }
        }

        private SendData SD = new SendData();
        private ProcessData PD = new ProcessData();

        public static bool Running = false;

        #region Connections
        public string AuthenticationCode = "";
        public Client[] Clients = new Client[Constants.MaxConnections];

        public List<Server> ServerQueue = new List<Server>();
        public Dictionary<ConnectionType, Server> Servers = new Dictionary<ConnectionType, Server>();
        #endregion

        #region TCP
        private Listener ServerListener = new Listener(IPAddress.Parse("127.0.0.1"), Constants.ServerPort, AssetType.SERVER);
        private Listener ClientListener = new Listener(IPAddress.Any, Constants.ClientPort);
        #endregion

        #region Events
        public EventHandler<ServerConnectionEventArgs> OnServerAuthenticated;
        #endregion

        public Network() {}

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
                ServerListener.StartAccept();

                for (int i = 0; i < Constants.MaxConnections; i++)
                {
                    Clients[i] = new Client(ConnectionType.CLIENT, i);
                }
                Log.Write(LogType.Information, $"Created {Constants.MaxConnections} client sockets");
                ClientListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                ClientListener.StartAccept();
            }
            catch (Exception ex)
            {
                Log.Write("An error occurred when starting the server", ex);
                return false;
            }
            Running = true;
            Log.Write(LogType.Information, "Server started");
            return true;
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


