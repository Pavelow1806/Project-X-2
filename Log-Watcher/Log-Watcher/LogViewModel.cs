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
        private ObservableCollection<LogItem> log = new ObservableCollection<LogItem>();
        public ObservableCollection<LogItem> Log
        {
            get { return log; }
            set
            {
                if (log != value)
                {
                    log = value;
                    OnPropertyChanged("Log");
                }
            }
        }

        private ImageSource ppImage;
        public ImageSource PPImage 
        { 
            get { return ppImage; }
            set
            {
                if (ppImage != value)
                {
                    ppImage = value;
                    OnPropertyChanged("PPImage");
                }
            }
        }

        private bool logNotSaved { get; set; }
        public bool LogNotSaved
        {
            get { return logNotSaved; }
            set
            {
                if (logNotSaved != value)
                {
                    logNotSaved = value;
                    OnPropertyChanged("LogNotSaved");
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
            PPImage = Resources.PauseButtonIcon.ImageSource();
        }
    }
}
