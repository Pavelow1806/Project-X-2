﻿using Log_Watcher.Properties;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        private ImageSource saveImage;
        public ImageSource SaveImage
        {
            get { return saveImage; }
            set
            {
                if (saveImage != value)
                {
                    saveImage = value;
                    OnPropertyChanged("SaveImage");
                }
            }
        }

        private bool logNotSaved { get; set; } = true;
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
        private void OnLogsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Log");
        }

        public LogViewModel()
        {
            PPImage = Resources.PauseButtonIcon.ImageSource();
            Log.CollectionChanged += OnLogsChanged;
        }
    }
}
