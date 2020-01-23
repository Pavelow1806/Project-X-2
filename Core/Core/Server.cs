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
        private System.Timers.Timer AuthenticateTimer;
        public event EventHandler<ServerConnectionEventArgs> OnAuthenticate;
        public event EventHandler<ServerConnectionEventArgs> OnClose;
        private bool AuthenticationSuccessful = false;
        private bool RepeatConnections = false;
        private int MaxConnectionAttempts = -1;

        public bool Authenticated { get { return AuthenticationSuccessful; } }

        /// <summary>
        /// Constructor for inbound server connections.
        /// </summary>
        /// <param name="type">The server that has connected.</param>
        public Server(ConnectionType type) :
            base(type, -1)
        {
            Network.Instance.RegisterServerEvents(this);
        }

        /// <summary>
        /// Constructor for outbound server connections.
        /// </summary>
        /// <param name="type">The server you want to connect to.</param>
        /// <param name="port">The port of the server.</param>
        /// <param name="IP">The IP address of the server. (Default 127.0.0.1)</param>
        public Server(ConnectionType type, int port, string IP = "127.0.0.1") :
            base(type, -1, port, IP)
        {
            Network.Instance.RegisterServerEvents(this);
        }

        /// <summary>
        /// Called after the server has been setup via an async callback from the server listener.
        /// </summary>
        public override void Start()
        {
            base.Start();

            Log.Write(LogType.Information, "Waiting for server to authenticate..");

            AuthenticateTimer = new System.Timers.Timer(Constants.MillisecondsToAuthenticateBeforeDisconnect);
            AuthenticateTimer.Elapsed += OnAuthenticationExpiry;
            AuthenticateTimer.AutoReset = false;
            AuthenticateTimer.Enabled = true;
        }

        /// <summary>
        /// Start the connection loop to this server.
        /// </summary>
        /// <param name="MaxConnectionAttempts">How many times the connection to the server should retry, leave blank to keep trying.</param>
        public void StartConnecting(int MaxConnectionAttempts = -1)
        {
            if (Type == ConnectionType.UNKNOWN || Type == ConnectionType.CLIENT)
            {
                Log.Write(LogType.Error, $"The type of the desintation server object is {Type.ToString()}, which is invalid");
                return;
            }
            if (Network.Instance.MyAssetType == AssetType.NONE)
            {
                Log.Write(LogType.Error, $"The type of this server object is {Network.Instance.MyAssetType.ToString()}, which is invalid");
                return;
            }
            if (Port == 0)
            {
                Log.Write(LogType.Error, "The Port of this server object was not set on initialisation, unable to connect to remote server");
                return;
            }
            if (string.IsNullOrEmpty(IP))
            {
                Log.Write(LogType.Error, "The IP of this server object was not set on initialisation, unable to connect to remote server");
                return;
            }
            if (Connected && Authenticated)
            {
                Log.Write(LogType.Error, "The server is already connected and authenticated");
            }

            if (MaxConnectionAttempts == -1) RepeatConnections = true;
            OutgoingConnectionThread = new Thread(new ParameterizedThreadStart(Connect));
            OutgoingConnectionThread.Name = $"OutgoingConnectionThread - {Type.ToString()}";
            OutgoingConnectionThread.Start(MaxConnectionAttempts);
        }
        private void Connect(object maxConnectionAttempts)
        {
            if (maxConnectionAttempts.GetType() != typeof(int)) return;
            MaxConnectionAttempts = (int)maxConnectionAttempts;
            int ConnectionAttemptCount = 0;
            while ((!Connected && (MaxConnectionAttempts == -1 || ConnectionAttemptCount < MaxConnectionAttempts)) || !Authenticated)
            {
                ++ConnectionAttemptCount;

                Log.Write(LogType.Connection, Type, "Attempting to connect");

                Connect();

                if (Connected)
                {
                    break;
                }
                if (!Connected)
                {

                    if (MaxConnectionAttempts == -1)
                        Log.Write(LogType.Connection, Type, $"Unable to connect, trying again in {(Constants.MillisecondsBetweenAttemptingConnect / 1000).ToString()} seconds..");
                    else
                        Log.Write(LogType.Connection, Type, $"Connection attempt {ConnectionAttemptCount.ToString()} of {MaxConnectionAttempts.ToString()} failed, trying again in {(Constants.MillisecondsBetweenAttemptingConnect / 1000).ToString()} seconds..");

                    Thread.Sleep(Constants.MillisecondsBetweenAttemptingConnect);
                }
            }
            OutgoingConnectionThread.Join();
        }
        private void Connect()
        {
            try
            {
                // If the socket is already setup, reset it
                if (Socket != null)
                {
                    if (Socket.Connected || Connected)
                    {
                        Log.Write(LogType.Connection, Type, "The server is already connected.");
                        Connected = true;
                        return;
                    }
                    Socket.Close();
                    Socket = null;
                }
                Socket = new System.Net.Sockets.TcpClient
                {
                    ReceiveBufferSize = Constants.BufferSize,
                    SendBufferSize = Constants.BufferSize,
                    NoDelay = false
                };
                Socket.Client.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, 1);
                Array.Resize(ref ReadBuff, Constants.BufferSize * 2);
                Socket.BeginConnect(IP, Port, new AsyncCallback(ConnectCallback), Socket);
            }
            catch (Exception ex)
            {
                Log.Write("The server failed to connect", Type, ex);
            }
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
                        Stream.BeginRead(ReadBuff, 0, Constants.BufferSize * 2, base.OnReceiveData, null);
                        ConnectedTime = DateTime.Now;
                        Log.Write(LogType.Connection, Type, "Connection to server successful! Handshaking..");
                        Connected = true;
                        SendData.Authenticate(Network.Instance.MyConnectionType, Type, Network.Instance.AuthenticationCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write("The server failed to connect", Type, ex);
            }
        }

        public override void Close()
        {
            if (RepeatConnections) 
            {
                Log.Write(LogType.Connection, $"The Connection for type {Type.ToString()} was closed, attempting to reconnect..");
                Socket = null;
                Connected = false;
                Connect(MaxConnectionAttempts);
            }
            else
            {
                Log.Write(LogType.Connection, $"The Connection for type {Type.ToString()} was closed");
                OnClose(this, new ServerConnectionEventArgs(this, IP, Type));
                base.Close();
                AuthenticationSuccessful = false;
                if (OutgoingConnectionThread != null) OutgoingConnectionThread.Join();
            }
        }

        private void OnAuthenticationExpiry(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (AuthenticationSuccessful) return;
            AuthenticateTimer.Enabled = false;
            Log.Write(LogType.Error, $"Authentication of server on IP {IP} failed, connection closed");
            Close();
            OnClose(this, new ServerConnectionEventArgs(this, IP, Type));
        }
        public void Authenticate(ConnectionType type)
        {
            AuthenticationSuccessful = true;
            if (AuthenticateTimer != null) AuthenticateTimer.Enabled = false;
            OnAuthenticate(this, new ServerConnectionEventArgs(this, IP, type));
        }
    }
}
