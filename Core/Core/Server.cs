using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Core
{
    public class Server : Connection
    {
        private readonly System.Timers.Timer AuthenticateTimer = new System.Timers.Timer();
        public event EventHandler<ServerConnectionEventArgs> OnAuthenticate;
        public event EventHandler<ServerConnectionEventArgs> OnClose;
        private bool AuthenticationSuccessful = false;

        public bool Authenticated
        {
            get
            {
                lock (Network.Instance.Servers)
                {
                    if (Network.Instance.Servers.ContainsKey(Type)) return true;
                    return false;
                }
            }
        }

        public Server(ConnectionType type, int id) :
            base(type, id)
        {
            Network.Instance.RegisterServerEvents(this);
        }

        public override void Start()
        {
            base.Start();

            Log.Write(LogType.Information, "Waiting for server to authenticate..");

            AuthenticateTimer.Interval = Constants.MillisecondsToAuthenticateBeforeDisconnect;
            AuthenticateTimer.Elapsed += OnAuthenticationExpiry;
            AuthenticateTimer.AutoReset = false;
            AuthenticateTimer.Enabled = true;
        }

        /// <summary>
        /// Start connecting to this server.
        /// </summary>
        /// <param name="MaxConnectionAttempts">How many times the connection to the server should retry, leave blank to keep trying.</param>
        public void StartConnecting(int MaxConnectionAttempts = -1)
        {
            if (Type == ConnectionType.NONE || Type == ConnectionType.CLIENT)
            {
                Log.Write(LogType.Error, $"The type of the desintation server object is {Type.ToString()}, which is invalid");
                return;
            }
            if (Network.Instance.MyAssetType == AssetType.NONE)
            {
                Log.Write(LogType.Error, $"The type of this server object is {Network.Instance.MyAssetType.ToString()}, which is invalid");
                return;
            }

            OutgoingConnectionThread = new Thread(new ParameterizedThreadStart(Connect));
            OutgoingConnectionThread.Start(MaxConnectionAttempts);
        }
        private void Connect(object maxConnectionAttempts)
        {
            if (maxConnectionAttempts.GetType() != typeof(int)) return;
            int MaxConnectionAttempts = (int)maxConnectionAttempts;
            int ConnectionAttemptCount = 0;
            while (!Connected && (MaxConnectionAttempts == -1 || ConnectionAttemptCount < MaxConnectionAttempts))
            {
                ++ConnectionAttemptCount;

                Connect();

                Thread.Sleep(Constants.MillisecondsBetweenAttemptingConnect);
            }
        }
        private void Connect()
        {
            // If the socket is already setup, reset it
            if (Socket != null)
            {
                if (Socket.Connected || Connected)
                {
                    Log.Write(LogType.Connection, $"{Type} is already connected.");
                    Connected = true;
                    return;
                }
                Socket.Close();
                Socket = null;
            }
            Socket = new System.Net.Sockets.TcpClient();
            Socket.ReceiveBufferSize = Constants.BufferSize;
            Socket.SendBufferSize = Constants.BufferSize;
            Socket.NoDelay = false;
            Socket.Client.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, 1);
            Array.Resize(ref asyncBuff, Constants.BufferSize * 2);
            Socket.BeginConnect(IP, Port, new AsyncCallback(ConnectCallBack), Socket);
        }
        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                if (Socket != null)
                {
                    Socket.EndConnect(result);
                    if (!Socket.Connected)
                    {
                        Connected = false;
                        Close();
                        return;
                    }
                    else
                    {
                        Socket.NoDelay = true;
                        Stream = Socket.GetStream();
                        Stream.BeginRead(asyncBuff, 0, Network.BufferSize * 2, OnReceive, null);
                        ConnectedTime = DateTime.Now;
                        Log.log("Connection to " + Type.ToString() + ", Successful.", Log.LogType.CONNECTION);
                        Connected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public override void Close()
        {
            base.Close();
            AuthenticationSuccessful = false;
            OnClose(this, new ServerConnectionEventArgs(this, Type));
        }

        private void OnAuthenticationExpiry(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (AuthenticationSuccessful) return;
            AuthenticateTimer.Enabled = false;
            Log.Write(LogType.Error, $"Authentication of server on IP {IP} failed, connection closed");
            Close();
            OnClose(this, new ServerConnectionEventArgs(this, Type));
        }
        public void Authenticate(ConnectionType type)
        {
            AuthenticationSuccessful = true;
            AuthenticateTimer.Enabled = false;
            OnAuthenticate(this, new ServerConnectionEventArgs(this, type));
        }
    }
}
