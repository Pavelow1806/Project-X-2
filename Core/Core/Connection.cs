using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    class Connection
    {
        public ConnectionType Type;

        private Thread ConnectionThread;

        #region Locking
        private static readonly object lockObj = new object();
        #endregion

        #region Connection
        public int Index = -1;
        public string IP = "";
        public string Username = "";
        public string SessionID = "";
        public bool Connected = false;
        public DateTime ConnectedTime = default;
        #endregion

        #region Network
        private byte[] ReadBuff;
        public TcpClient Socket;
        public NetworkStream Stream;
        #endregion

        public Connection(ConnectionType type, int id)
        {
            Type = type;
            Index = id;
        }

        public virtual void Start()
        {
            ConnectedTime = DateTime.Now;
            ConnectionThread = new Thread(new ThreadStart(BeginThread));
            ConnectionThread.Start();
        }

        public virtual void Close()
        {
            lock (lockObj)
            {
                if (Connected)
                {
                    // Connection
                    IP = "";
                    Username = "";
                    SessionID = "";
                    ConnectedTime = default;

                    // Network
                    ReadBuff = null;
                    if (Stream != null)
                    {
                        Stream.Close();
                        Stream = null;
                    }
                    if (Socket != null)
                    {
                        Socket.Close();
                        Socket = null;
                    }

                    // Rejoin main thread
                    ConnectionThread.Join();
                }
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
                    if (Connected)
                    {
                        Stream.BeginRead(ReadBuff, 0, Socket.ReceiveBufferSize, HandleAsyncConnection, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write($"An error occurred when beginning {nameof(Stream).ToString()}'s BeginRead function", ex);
            }
        }
        private void HandleAsyncConnection(IAsyncResult result)
        {
            StartAccept();
            OnReceiveData(result);
        }

        public void OnReceiveData(IAsyncResult result)
        {
            lock (lockObj)
            {
                try
                {
                    int ReadBytes = Stream.EndRead(result);
                    if (Socket == null)
                    {
                        return;
                    }
                    byte[] Bytes = null;
                    Array.Resize(ref Bytes, ReadBytes);
                    Buffer.BlockCopy(ReadBuff, 0, Bytes, 0, ReadBytes);
                    if (ReadBytes <= 0)
                    {
                        Close();
                        return;
                    }

                    // Process the packet
                    ProcessData.processData(Index, Bytes);

                    Stream.BeginRead(ReadBuff, 0, Socket.ReceiveBufferSize, OnReceiveData, null);
                }
                catch (Exception ex)
                {
                    // Output error message
                    Log.Write("An error occurred when receiving data", ex);
                }
                finally
                {
                    Log.Write(LogType.Information, $"Connection closed with IP {IP}, Index {Index.ToString()}, Username {Username}, Connected for {(DateTime.Now - ConnectedTime).ToString()}\nSession ID: {SessionID}");
                    // Close the connection
                    Close();
                }
            }
        }
    }
}
