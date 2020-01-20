using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Console_Application
{
    public class ServerList : ObservableCollection<Server>
    {
        public ServerList() : base()
        {
            Add(new Server("Login Server", "Start", Colors.DarkRed));
            Add(new Server("Game Server", "Start", Colors.DarkRed));
            Add(new Server("Synchronization Server", "Start", Colors.DarkRed));
        }
    }
    public class LogList : ObservableCollection<LogEntry>
    {
        public LogList() : base()
        {
            Add(new LogEntry(DateTime.Now, LogType.Connection, "Example server connected"));
            Add(new LogEntry(DateTime.Now, LogType.Debug, "Example debug"));
            Add(new LogEntry(DateTime.Now, LogType.Error, "Example error"));
            Add(new LogEntry(DateTime.Now, LogType.Information, "Example information"));
            Add(new LogEntry(DateTime.Now, LogType.TransmissionIn, "Example transmission in"));
            Add(new LogEntry(DateTime.Now, LogType.TransmissionOut, "Example transmission out"));
            Add(new LogEntry(DateTime.Now, LogType.Warning, "Example warning"));
        }
    }
}
