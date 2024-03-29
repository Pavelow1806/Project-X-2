﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Listener : TcpListener
    {
        private static readonly object lockObj = new object();

        readonly AssetType Type = AssetType.CLIENT;

        public Listener(IPEndPoint localEP, AssetType type = AssetType.CLIENT) : base(localEP) { Type = type; }
        public Listener(IPAddress localaddr, int port, AssetType type = AssetType.CLIENT) : base(localaddr, port) { Type = type; }

        public EventHandler<ListenerCallbackEventArgs> OnConnect;

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
            try
            {
                OnConnect(this, new ListenerCallbackEventArgs(EndAcceptTcpClient(result)));
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
        //public void OnConnect(IAsyncResult result)
        //{
        //    lock (lockObj)
        //    {
        //        try
        //        {
        //            TcpClient socket = EndAcceptTcpClient(result);
        //            socket.NoDelay = false;

        //            if (Type == AssetType.CLIENT)
        //            {
        //                try
        //                {
        //                    bool SocketFree = false;
        //                    for (int i = 0; i < Constants.MaxConnections; i++)
        //                    {
        //                        if (NetworkBase.Instance.Clients[i].Available)
        //                        {
        //                            SocketFree = true;
        //                            Client client = NetworkBase.Instance.Clients[i];
        //                            client.Connected = true;
        //                            client.Socket = socket;
        //                            client.IP = socket.Client.RemoteEndPoint.ToString();
        //                            client.Start();
        //                            Log.Write(LogType.Connection, $"Client connected with IP {client.IP} on Index {client.Index}");
        //                            break;
        //                        }
        //                    }
        //                    if (!SocketFree)
        //                    {
        //                        Log.Write(LogType.Warning, $"Connection from IP {socket.Client.RemoteEndPoint.ToString()} failed due to no server sockets being available, consider raising the current maximum of {Constants.MaxConnections.ToString()}");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Log.Write(ex);
        //                }
        //            }
        //            else if (Type == AssetType.SERVER)
        //            {
        //                Server server = new Server(ConnectionType.UNKNOWN, -1);

        //                try
        //                {
        //                    server.IP = socket.Client.RemoteEndPoint.ToString();

        //                    lock (NetworkBase.Instance.ServerQueue)
        //                    {
        //                        if (NetworkBase.Instance.ServerQueue.Any(x => x.IP == server.IP))
        //                        {
        //                            Log.Write(LogType.Error, "A server with an identical IP address and port has connected, disconnecting");
        //                            server.Close();
        //                            return;
        //                        }
        //                    }

        //                    server.Connected = true;
        //                    server.Socket = socket;
        //                    server.Username = "System";
        //                    server.SessionID = "System";

        //                    NetworkBase.Instance.ServerQueue.Add(server);

        //                    Log.Write(LogType.Connection, $"Server connected with IP {server.IP} and was added to the Server Queue, waiting for handshake..");
        //                }
        //                catch (Exception ex)
        //                {
        //                    Log.Write(ex);
        //                }

        //                server.Start();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Write(ex);
        //        }
        //    }
        //}
    }
}
