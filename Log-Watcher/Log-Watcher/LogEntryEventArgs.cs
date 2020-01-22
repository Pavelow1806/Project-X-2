using Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Log_Watcher
{
    public class LogEntryEventArgs
    {
        public List<string> Logs;
        public System.Windows.Controls.ListBox ListBox;
        public ObservableCollection<LogItem> LogView;
        public LogEntryEventArgs(List<string> logs, System.Windows.Controls.ListBox listBox, ObservableCollection<LogItem> logView)
        {
            Logs = logs;
            ListBox = listBox;
            LogView = logView;
        }
    }
}
