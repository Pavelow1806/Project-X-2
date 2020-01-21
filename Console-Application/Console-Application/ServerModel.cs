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
    public sealed class ServerModel
    {
        public ObservableCollection<Server> Servers { get; set; } = new ObservableCollection<Server>();
        public ObservableCollection<LogEntry> SelectedServer { get; set; } = new ObservableCollection<LogEntry>();

        public ServerModel()
        {

        }
        private void server_CollectionChanged(object sender, EventArgs e)
        {

        }
    }
}
