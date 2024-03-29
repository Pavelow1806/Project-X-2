﻿using Core;
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
        public string AuthenticationCode = "123";
        public Client[] Clients = new Client[Constants.MaxConnections];

        public List<Server> ServerQueue = new List<Server>();
        public Dictionary<ConnectionType, Server> Servers = new Dictionary<ConnectionType, Server>();
        #endregion

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
        private readonly string ClusterIP = "";

        private int serverIndex = 0;
        public int ServerIndex
        {
            get
            {
                return serverIndex++;
            }
        }

        #region TCP
        private readonly Listener ServerListener = null;
        private readonly Listener ClientListener = null;
        #endregion

        #region Events
        public EventHandler<ServerConnectionEventArgs> OnServerAuthenticated;
        public EventHandler<PacketEventArgs> OnPacketReceived;
        #endregion

        public Network(AssetType type, string ServerClusterIP = Constants.ClusterLocalIP) :
            base(type)
        {
            AssignInstance(this);
            ClusterIP = ServerClusterIP;

            int Port;

            int? ServerPort = this.ServerPort;
            if (ServerPort != null)
            {
                Port = (int)ServerPort;
                ServerListener = new Listener(IPAddress.Parse(ClusterIP), Port, AssetType.SERVER);
            }
            else
            {
                Log.Write(LogType.Error, "The port returned for the server listener was null.");
            }

            int? ClientPort = this.ClientPort;
            if (ClientPort != null)
            {
                Port = (int)ClientPort;
                ClientListener = new Listener(IPAddress.Any, Port);
            }
            else
            {
                Log.Write(LogType.Error, "The port returned for the client listener was null.");
            }
        }

        /// <summary>
        /// Launch the server by starting the listeners and attempting to connect if required.
        /// </summary>
        /// <returns>Whether startup was successful.</returns>
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
                    ServerListener.OnConnect += Server_OnConnect;
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
                    ClientListener.OnConnect += Client_OnConnect;
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
            return (Running = true);
        }
        private void Server_OnConnect(object sender, ListenerCallbackEventArgs e)
        {
            Server server = new Server(ConnectionType.UNKNOWN, -1);

            try
            {
                server.IP = e.NewConnection.Client.RemoteEndPoint.ToString();

                lock (ServerQueue)
                {
                    if (ServerQueue.Any(x => x.IP == server.IP))
                    {
                        Log.Write(LogType.Error, "A server with an identical IP address and port has connected, disconnecting");
                        server.Close();
                        return;
                    }
                }

                server.Connected = true;
                server.Socket = e.NewConnection;
                server.Username = "System";
                server.SessionID = "System";

                ServerQueue.Add(server);

                Log.Write(LogType.Connection, $"Server connected with IP {server.IP} and was added to the Server Queue, waiting for handshake..");
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }

            server.Start();
        }
        private void Client_OnConnect(object sender, ListenerCallbackEventArgs e)
        {
            try
            {
                bool SocketFree = false;
                for (int i = 0; i < Constants.MaxConnections; i++)
                {
                    if (Clients[i].Available)
                    {
                        SocketFree = true;
                        Connection client = Clients[i];
                        client.Connected = true;
                        client.Socket = e.NewConnection;
                        client.IP = e.NewConnection.Client.RemoteEndPoint.ToString();
                        client.Start();
                        Log.Write(LogType.Connection, $"Client connected with IP {client.IP} on Index {client.Index}");
                        break;
                    }
                }
                if (!SocketFree)
                {
                    Log.Write(LogType.Warning, $"Connection from IP {e.NewConnection.Client.RemoteEndPoint.ToString()} failed due to no server sockets being available, consider raising the current maximum of {Constants.MaxConnections.ToString()}");
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        /// <summary>
        /// Attempt to connect to the other servers in the cluster
        /// </summary>
        public override List<ConnectionType> Connect()
        {
            List<ConnectionType> result = new List<ConnectionType>();
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
            return result;
        }

        protected override void ConnectToServer(ConnectionType Destination, int MaxConnectionAttempts = -1)
        {
            int Port;
            if (!ServerAuthenticated(Destination))
            {
                int? ServerPort = GetDestinationPort(Destination);
                if (ServerPort != null)
                {
                    Port = (int)ServerPort;
                    Server Server = new Server(Destination, Port);
                    lock (ServerQueue)
                    {
                        if (ServerQueue.Any(x => x.IP == Server.IP && x.Port == Server.Port))
                        {
                            Log.Write(LogType.Error, $"Stopped attempt to create duplicate connection on IP {Server.IP} and port {Server.Port}");
                            return;
                        }
                    }

                    RegisterServerEvents(Server);
                    ServerQueue.Add(Server);
                    Server.StartConnecting(MaxConnectionAttempts);
                }
                else
                {
                    Log.Write(LogType.Error, Destination, "The port returned for the server was null.");
                }
            }
            else
            {
                Log.Write(LogType.Connection, Destination, "The connection to the server has already been established");
            }
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
        /// <summary>
        /// Check if the server at the destination type passed in has been authenticated.
        /// </summary>
        /// <param name="Destination">The server you want to check.</param>
        /// <returns>Whether it's authenticated.</returns>
        public bool ServerAuthenticated(ConnectionType Destination)
        {
            lock (Servers)
            {
                if (Servers.ContainsKey(Destination) && Servers[Destination].Authenticated)
                    return true;
                else
                    return false;
            }
        }

        #region Events
        public void RegisterServerEvents(Server server)
        {
            if (server == null) return;
            server.OnAuthenticate += Authentication_AuthenticatedEvent;
            server.OnClose += Connection_OnClose;
            server.OnDataReceived += DataReceived;
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
        private void Connection_OnClose(object sender, ConnectionEventArgs e)
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
                lock (ServerQueue)
                {
                    if (ServerQueue.Contains((server)e.Connection))
                    {
                        e.Server = null;
                        ServerQueue.Remove(e.Server);
                        Log.Write(LogType.Connection, $"{e.Type.ToString()}{(e.Type == ConnectionType.UNKNOWN ? $" {e.IP}" : "")} was closed, server removed from server queue");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write("An error was thrown during Server_OnClose", ex);
            }
        }
        #endregion
    }
}


