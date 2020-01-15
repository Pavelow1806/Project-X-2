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
        private System.Timers.Timer AuthenticateTimer = new System.Timers.Timer();
        public event EventHandler<ServerConnectionEventArgs> OnAuthenticate;
        public event EventHandler<ServerConnectionEventArgs> OnClose;
        private bool AuthenticationSuccessful = false;

        public bool Authenticated
        {
            get
            {
                if (Network.instance.Servers.ContainsKey(Type)) return true;
                return false;
            }
        }

        public Server(ConnectionType type, int id) :
            base(type, id)
        {
            Network.instance.RegisterServerEvents(this);
        }

        public override void Start()
        {
            base.Start();

            Log.Write(LogType.Information, "Waiting for server to authenticate..");

            AuthenticateTimer.Interval = Constants.SecondsToAuthenticateBeforeDisconnect;
            AuthenticateTimer.Elapsed += OnAuthenticationExpiry;
            AuthenticateTimer.AutoReset = false;
            AuthenticateTimer.Enabled = true;
        }

        public override void Close()
        {
            base.Close();
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
            // TODO: send packet to authenticate this server
            AuthenticationSuccessful = true;
            OnAuthenticate(this, new ServerConnectionEventArgs(this, type));
        }
    }
}
