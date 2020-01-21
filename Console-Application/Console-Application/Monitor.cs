using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Console_Application
{
    class Monitor
    {
        private Network Network = new Network(ConnectionType.TOOL);
        private ServerModel ServerModel = null;
        private List<ConnectionType> Connections = null;
        public Monitor(ServerModel serverModel)
        {
            ServerModel = serverModel;
            Network.OnServerAuthenticated += OnServerAuthenticated;
            Connections = Network.Connect();
            if (Connections == null)
            {
                Log.Write(LogType.Error, "The connections received from the network object was null.");
                return;
            }
            Connections.ForEach(x => ServerModel.Servers.Add(new Server(x.GetDescription(), "Start", Colors.DarkRed)));
        }

        private void OnServerAuthenticated(object sender, ServerConnectionEventArgs e)
        {
            Server server = ServerModel.Servers.SingleOrDefault(x => x.Name == e.Type.GetDescription());
            if (server == null)
            {
                Log.Write(LogType.Error, $"A server was authenticated with the type {e.Type.ToString()}, however it wasn't found in the list");
                return;
            }
            server.State = "Green";
            Send.RequestLogs(e.Type);
        }
    }
}
