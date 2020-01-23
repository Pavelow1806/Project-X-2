﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    class Listener : TcpListener
    {
        private static readonly object lockObj = new object();

        readonly AssetType Type = AssetType.CLIENT;

        public Listener(IPEndPoint localEP, AssetType type = AssetType.CLIENT) : base(localEP) { Type = type; }
        public Listener(IPAddress localaddr, int port, AssetType type = AssetType.CLIENT) : base(localaddr, port) { Type = type; }

        public void StartAccept()
        {
            try
            {
                Start();
                BeginAcceptTcpClient(HandleAsyncConnection, this);
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
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
                    TcpClient socket = EndAcceptTcpClient(result);
                    socket.NoDelay = false;

                    if (Type == AssetType.CLIENT)
                    {
                        try
                        {
                            bool SocketFree = false;
                            for (int i = 0; i < Constants.MaxConnections; i++)
                            {
                                if (Network.Instance.Clients[i].Available)
                                {
                                    SocketFree = true;
                                    Client client = Network.Instance.Clients[i];
                                    client.Connected = true;
                                    client.Socket = socket;
                                    client.IP = socket.Client.RemoteEndPoint.ToString();
                                    client.Start();
                                    Log.Write(LogType.Connection, $"Client connected with IP {client.IP} on Index {client.Index}");
                                    break;
                                }
                            }
                            if (!SocketFree)
                            {
                                Log.Write(LogType.Warning, $"Connection from IP {socket.Client.RemoteEndPoint.ToString()} failed due to no server sockets being available, consider raising the current maximum of {Constants.MaxConnections.ToString()}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Write(ex);
                        }
                    }
                    else if (Type == AssetType.SERVER)
                    {
                        Server server = new Server(ConnectionType.UNKNOWN, -1);

                        try
                        {
                            server.Connected = true;
                            server.Socket = socket;
                            server.IP = socket.Client.RemoteEndPoint.ToString();
                            server.Username = "System";
                            server.SessionID = "System";

                            Network.Instance.ServerQueue.Add(server);

                            Log.Write(LogType.Connection, $"Server connected with IP {server.IP} and was added to the Server Queue, waiting for handshake..");
                        }
                        catch (Exception ex)
                        {
                            Log.Write(ex);
                        }

                        server.Start();
                    }
                }
                catch (Exception ex)
                {
                    Log.Write(ex);
                }
            }
        }
    }
}
