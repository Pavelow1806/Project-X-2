using Core;
using Log_Watcher.Properties;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Log_Watcher
{
    public sealed class LogViewModel : PaneViewModel
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

        private void OnLogsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Log");
        }

        #region Title

        private string _title = null;
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        #endregion
        public LogViewModel(string alias, string logFileName)
        {
            IsDirty = true;
            Title = $"{(!string.IsNullOrEmpty(alias) ? $"{alias} [{logFileName}]" : $"{logFileName}")}";
            PPImage = Resources.PauseButtonIcon.ImageSource();
            Log.CollectionChanged += OnLogsChanged;
        }
        #region IsDirty

        private bool _isDirty = false;
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    OnPropertyChanged("IsDirty");
                    OnPropertyChanged("FileName");
                }
            }
        }

        #endregion
    }
}
