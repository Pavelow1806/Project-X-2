using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Core;

namespace Console_Application
{// INotifyPropertyChanged notifies the View of property changes, so that Bindings are updated.
    sealed class ServerModel : INotifyPropertyChanged
    {
        private ObservableCollection<Server> servers;

        public string Name 
        {
            get
            { return loginServer.Name; }
            set 
            {
                if (loginServer.Name != value)
                {
                    loginServer.Name = value;
                    OnPropertyChange("Name");
                }
            } 
        }
        public string ReStart 
        {
            get
            { return loginServer.ReStart; }
            set
            {
                if (loginServer.ReStart != value)
                {
                    loginServer.ReStart = value;
                    OnPropertyChange("ReStart");
                }
            }
        }
        public Color State 
        { 
            get
            { return loginServer.State; }
            set
            {
                if (loginServer.State != value)
                {
                    loginServer.State = value;
                    OnPropertyChange("State");
                }
            }
        }
        public void SetState(ServerState state)
        {
            switch (state)
            {
                case ServerState.None:
                    State = Colors.Red;
                    break;
                case ServerState.Starting:
                    State = Colors.Orange;
                    break;
                case ServerState.Running:
                    State = Colors.Green;
                    break;
                case ServerState.ShuttingDown:
                    State = Colors.Purple;
                    break;
                default:
                    State = Colors.Red;
                    break;
            }
        }

        public ServerModel()
        {
            Server loginServer = new Server(ConnectionType.LOGINSERVER.GetDescription(), "Start", Colors.DarkRed);
            servers.Add(loginServer);
            SetState(ServerState.None);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void LoginServerStart()
        {
            Log.Write(LogType.Information, $"Starting {loginServer.Name}");
        }
    }
}
