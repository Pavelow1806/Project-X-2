using Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Log_Watcher
{
    public class LogEntryEventArgs
    {
        public List<string> Logs;
        public LogEntryEventArgs(List<string> logs)
        {
            Logs = logs;
        }
    }
}
