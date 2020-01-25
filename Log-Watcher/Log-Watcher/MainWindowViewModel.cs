using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log_Watcher
{
    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool visible = false;
        public bool Visible 
        { 
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    if (!value)
                        WindowContextMenu = MainWindowContent.Open;
                    OnPropertyChanged("Visible");
                }
            }
        }

        private MainWindowContent windowContextMenu = MainWindowContent.Open;
        public MainWindowContent WindowContextMenu 
        { 
            get { return windowContextMenu; } 
            set
            {
                if (windowContextMenu != value)
                {
                    windowContextMenu = value;
                    OnPropertyChanged("WindowContextMenu");
                }
            }
        }

        private ObservableCollection<SavedLog> savedLogs = new ObservableCollection<SavedLog>();
        public ObservableCollection<SavedLog> SavedLogs
        {
            get { return savedLogs; }
            set
            {
                if (savedLogs != value)
                {
                    savedLogs = value;
                    OnPropertyChanged("SavedLogs");
                }
            }
        }
        public SavedLog SelectedLog { get; set; }
        public bool SaveLog { get; set; } = false;

        private string alias = "";
        public string Alias
        {
            get { return alias; }
            set
            {
                if (alias != value)
                {
                    alias = value;
                    OnPropertyChanged("Alias");
                }
            }
        }

        private string path = "";
        public string Path 
        { 
            get { return path; } 
            set
            {
                if (path != value)
                {
                    path = value;
                    OnPropertyChanged("Path");
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
