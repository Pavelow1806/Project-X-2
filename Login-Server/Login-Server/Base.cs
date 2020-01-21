using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Login_Server
{
    class Base
    {
        private ProcessPacket ProcessPacket = new ProcessPacket();
        private Network Network = new Network(ConnectionType.LOGINSERVER);
        public Base() 
        {
            Network.OnServerAuthenticated += OnServerAuthenticated;
            Network.LaunchServer();
        }
        ~Base()
        {
            Log.Write(LogType.Information, "Server shutting down.");
        }
        public void OnServerAuthenticated(object sender, ServerConnectionEventArgs e)
        {
            // Once the server has authenticated, register the process event to begin processing the packets
            e.Server.OnPacketReceived += ProcessPacket.Process;
        }
    }
}
