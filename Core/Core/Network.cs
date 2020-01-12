﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    class Network
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

        #region TCP
        private const int MaxConnections = 100;

        private const int ServerPort = 5610;
        private TcpListener ServerListener = new TcpListener(IPAddress.Parse("127.0.0.1"), ServerPort);
        private const int ClientPort = 5600;
        private TcpListener ClientListener = new TcpListener(IPAddress.Any, ClientPort);

        public const int BufferSize = 4096;
        #endregion

        #region Connections
        public Dictionary<ConnectionType, Server> Servers = new Dictionary<ConnectionType, Server>();

        public const double SecondsToAuthenticateBeforeDisconnect = 5.0;
        public string AuthenticationCode = "";
        public bool GameServerAuthenticated = false;
        public bool SyncServerAuthenticated = false;
        private int ServerNumber = 10;
        public Client[] Clients = new Client[MaxConnections];
        #endregion

        public Network()
        {
            _instance = this;
        }

        public bool LaunchServer()
        {
            AuthenticationCode = Database.instance.RequestAuthenticationCode();
            if (AuthenticationCode == "")
            {
                Log.log("Critical Error! Authentication code could not be loaded.", Log.LogType.ERROR);
            }
            else
            {
                Log.log("Authentication code loaded.", Log.LogType.SUCCESS);
            }
            try
            {
                for (int i = 0; i < MaxConnections; i++)
                {
                    Clients[i] = new Client(ConnectionType.CLIENT, i);
                }
                Listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                Listener.Start();
                StartAccept();
            }
            catch (Exception e)
            {
                Log.log("An error occurred when attempting to start the server. > " + e.Message, Log.LogType.ERROR);
                return false;
            }
            Running = true;
            return true;
        }

        public void StartAccept()
        {
            Listener.BeginAcceptTcpClient(HandleAsyncConnection, Listener);
        }
        public void HandleAsyncConnection(IAsyncResult result)
        {
            StartAccept();
            OnConnect(result);
        }
        public void OnConnect(IAsyncResult result)
        {
            lock (lockObj)
            {
                try
                {
                    TcpClient socket = Listener.EndAcceptTcpClient(result);
                    socket.NoDelay = false;
                    if (!GameServerAuthenticated || !SyncServerAuthenticated)
                    {
                        Servers.Add((ConnectionType)ServerNumber, new Server(ConnectionType.SYNCSERVER, ServerNumber));
                        Servers[(ConnectionType)ServerNumber].Connected = true;
                        Servers[(ConnectionType)ServerNumber].Socket = socket;
                        Servers[(ConnectionType)ServerNumber].IP = socket.Client.RemoteEndPoint.ToString();
                        Servers[(ConnectionType)ServerNumber].Username = "System";
                        Servers[(ConnectionType)ServerNumber].SessionID = "System";
                        Servers[(ConnectionType)ServerNumber].Start();
                        Log.Write(LogType.Connection, socket.Client.RemoteEndPoint.ToString(), $"Incoming server connection, awaiting authentication packet.");
                        ++ServerNumber;
                    }
                    else
                    {
                        for (int i = 0; i < MaxConnections; i++)
                        {
                            if (Clients[i].Socket == null)
                            {
                                Clients[i].Connected = true;
                                Clients[i].Socket = socket;
                                Clients[i].IP = socket.Client.RemoteEndPoint.ToString();
                                Clients[i].Start();
                                Log.log("A client has connected to the server:", Log.LogType.CONNECTION);
                                Log.log("IP: " + Clients[i].IP, Log.LogType.CONNECTION);
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }
        }
        public void CheckAuthentication()
        {
            foreach (KeyValuePair<ConnectionType, Server> server in Servers)
            {
                if (!(server.Key == ConnectionType.GAMESERVER || server.Key == ConnectionType.SYNCSERVER) && !server.Value.Authenticated)
                {
                    Servers.Remove(server.Key);
                }
            }
        }
        public int GetClient(string ip)
        {
            for (int i = 0; i < Clients.Length; i++)
            {
                if (Clients[i].Connected && Clients[i].IP.Length > 1 && Clients[i].IP.Substring(0, Clients[i].IP.IndexOf(':')) == ip)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
