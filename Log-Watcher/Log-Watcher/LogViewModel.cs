using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Log_Watcher
{
    public sealed class LogViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<LogItem> serverLog = new ObservableCollection<LogItem>();
        public ObservableCollection<LogItem> ServerLog
        {
            get { return serverLog; }
            set
            {
                if (serverLog != value)
                {
                    serverLog = value;
                    OnPropertyChanged("ServerLog");
                }
            }
        }
        private ObservableCollection<LogItem> consoleLog = new ObservableCollection<LogItem>();
        public ObservableCollection<LogItem> ConsoleLog
        {
            get { return consoleLog; }
            set
            {
                if (consoleLog != value)
                {
                    consoleLog = value;
                    OnPropertyChanged("ConsoleLog");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }
    }
}
