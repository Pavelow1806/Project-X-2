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
    public abstract class Connection
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
        public EventHandler<NewDataEventArgs> OnDataReceived;
        public EventHandler<NewDataEventArgs> OnDataSent;
        public event EventHandler<ConnectionEventArgs> OnClose;
        #endregion

        public bool Available
        {
            get
            {
                if (Socket == null)
                    return true;
                return false;
            }
        }

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

        #region Inbound Connections
        public virtual void Start()
        {
            ConnectedTime = DateTime.Now;
            DataThread = new Thread(new ThreadStart(BeginThread));
            DataThread.Start();
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
            catch (Exception) { }
        }
        private void HandleAsyncConnection(IAsyncResult result)
        {
            StartAccept();
            OnReceiveData(result);
        }
        #endregion

        #region On Data Receive
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
                        Close();
                        return;
                    }

                    // Process the packet
                    OnDataReceived(this, new NewDataEventArgs(Bytes));

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
                    Close();
                }
                catch (ObjectDisposedException odex)
                {
                    Log.Write($"An error occurred when receiving data (ArgumentException, closing connection) [{odex.Message}]", odex);
                    Close();
                }
                catch (Exception ex)
                {
                    Log.Write($"An error occurred when receiving data (Exception, closing connection) [{ex.Message}]", ex);
                    Close();
                }
            }
        }
        #endregion
    }
}
