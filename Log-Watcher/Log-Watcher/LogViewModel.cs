using Log_Watcher.Properties;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        private ImageSource serverLogPPImage { get; set; }
        public ImageSource ServerLogPPImage 
        { 
            get { return serverLogPPImage; }
            set
            {
                if (serverLogPPImage != value)
                {
                    serverLogPPImage = value;
                    OnPropertyChanged("ServerLogPPImage");
                }
            }
        }

        public ImageSource allPPImage { get; set; }
        public ImageSource AllPPImage
        {
            get { return allPPImage; }
            set
            {
                if (allPPImage != value)
                {
                    allPPImage = value;
                    OnPropertyChanged("AllPPImage");
                }
            }
        }
        public ImageSource consoleLogPPImage { get; set; }
        public ImageSource ConsoleLogPPImage
        {
            get { return consoleLogPPImage; }
            set
            {
                if (consoleLogPPImage != value)
                {
                    consoleLogPPImage = value;
                    OnPropertyChanged("ConsoleLogPPImage");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(value));
        }

        public LogViewModel()
        {
            ServerLogPPImage = Resources.PauseButtonIcon.ImageSource();
            AllPPImage = Resources.PauseButtonIcon.ImageSource();
            ConsoleLogPPImage = Resources.PauseButtonIcon.ImageSource();
        }
    }
}
