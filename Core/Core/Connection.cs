using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public class Connection
    {
        public ConnectionType Type;

        private Thread DataThread;
        protected Thread OutgoingConnectionThread;

        #region Locking
        private static readonly object lockObj = new object();
        #endregion

        #region Connection
        public int Index = -1;
        public int Port = 0;
        public string IP = "";
        public string Username = "";
        public string SessionID = "";
        public bool Connected = false;
        public DateTime ConnectedTime = default;
        #endregion

        #region Network
        protected byte[] ReadBuff;
        public TcpClient Socket;
        public NetworkStream Stream;
        public StreamReader Reader = null;
        public StreamWriter Writer = null;
        #endregion

        #region Events
        public EventHandler<PacketEventArgs> OnPacketReceived;
        public EventHandler<PacketEventArgs> OnPacketSent;
        #endregion

        public Connection(ConnectionType type, int id)
        {
            Type = type;
            Index = id;
        }
        public Connection(ConnectionType type, int id, int port, string ip)
        {
            Port = port;
            IP = ip;
            Type = type;
            Index = id;
        }

        public virtual void Start()
        {
            ConnectedTime = DateTime.Now;
            DataThread = new Thread(new ThreadStart(BeginThread));
            DataThread.Start();
        }
        private void CloseConnection()
        {
            if (Connected)
            {
                if (this is Server)
                {
                    Server s = (Server)this;
                    s.Close();
                }
                else
                {
                    Close();
                }
            }
        }
        public virtual void Close()
        {
            Connected = false;
            lock (lockObj)
            {
                // Network
                ReadBuff = null;
                ReadBuff = null;
                if (Socket != null)
                {
                    Socket.Close();
                    Socket = null;
                }
                if (Stream != null)
                {
                    Stream.Close();
                    Stream = null;
                }
                if (Reader != null)
                {
                    Reader.Close();
                    Reader = null;
                }
                if (Writer != null)
                {
                    Writer.Close();
                    Writer = null;
                }

                Connected = false;

                // Rejoin main thread
                DataThread.Join();
            }
        }

        public void BeginThread()
        {
            Socket.SendBufferSize = Constants.BufferSize;
            Socket.ReceiveBufferSize = Constants.BufferSize;
            Stream = Socket.GetStream();
            Array.Resize(ref ReadBuff, Socket.ReceiveBufferSize);
            StartAccept();
        }

        private void StartAccept()
        {
            try
            {
                if (Stream != null)
                {
                    if (Socket != null && Connected)
                    {
                        Stream.BeginRead(ReadBuff, 0, Socket.ReceiveBufferSize, HandleAsyncConnection, null);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        private void HandleAsyncConnection(IAsyncResult result)
        {
            StartAccept();
            OnReceiveData(result);
        }
        protected void OnReceiveData(IAsyncResult result)
        {
            lock (lockObj)
            {
                try
                {
                    if (Socket == null || !Connected)
                        return;
                    int ReadBytes = Stream.EndRead(result);

                    byte[] Bytes = null;
                    Array.Resize(ref Bytes, ReadBytes);
                    Buffer.BlockCopy(ReadBuff, 0, Bytes, 0, ReadBytes);
                    if (ReadBytes <= 0)
                    {
                        CloseConnection();
                        return;
                    }

                    // Process the packet
                    Packet packet = ProcessData.Process(this, Index, Bytes);

                    if (this is Server)
                    {
                        Server s = (Server)this;
                        if (!s.Authenticated)
                        {
                            ByteBuffer.ByteBuffer buffer = new ByteBuffer.ByteBuffer(new List<object>());
                            buffer.WriteBytes(packet.Data);
                            ProcessData.ReadHeader(ref buffer);
                            if (buffer.ReadString() == Network.Instance.AuthenticationCode)
                            {
                                if (!s.Authenticated)
                                {
                                    // Add to list of authenticated servers
                                    s.Authenticate(packet.Source);
                                    // Confirm authentication with reply
                                    SendData.Authenticate(Network.Instance.MyConnectionType, s, Network.Instance.AuthenticationCode);
                                }
                            }
                        }
                    }
                    else
                    {
                        OnPacketReceived(this, new PacketEventArgs(packet, this));
                    }

                    if (Socket == null || !Connected)
                        return;
                    Stream.BeginRead(ReadBuff, 0, Socket.ReceiveBufferSize, OnReceiveData, null);
                }
                catch (ArgumentException aex)
                {
                    Log.Write($"An error occurred when receiving data (ArgumentException) [{aex.Message}]", aex);
                }
                catch (IOException ioex)
                {
                    Log.Write($"An error occurred when receiving data (IOException, closing connection) [{ioex.Message}]", ioex);
                    CloseConnection();
                }
                catch (ObjectDisposedException odex)
                {
                    Log.Write($"An error occurred when receiving data (ArgumentException, closing connection) [{odex.Message}]", odex);
                    CloseConnection();
                }
                catch (Exception ex)
                {
                    Log.Write($"An error occurred when receiving data (Exception, closing connection) [{ex.Message}]", ex);
                    CloseConnection();
                }
            }
        }
    }
}
