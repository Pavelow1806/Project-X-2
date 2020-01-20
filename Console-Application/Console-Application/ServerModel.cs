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
{
    public sealed class ServerModel : INotifyPropertyChanged
    {
        public ServerList servers = new ServerList();
        public LogList SelectedServer { get; set; } = new LogList();

        public ServerModel()
        {
            servers.CollectionChanged += server_CollectionChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void server_CollectionChanged(object sender, EventArgs e)
        {

        }
    }
}
