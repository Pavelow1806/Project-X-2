using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Console_Application
{
    public sealed class Server : INotifyPropertyChanged
    {
        private string name { get; set; }
        public string Name 
        {
            get { return name; } 
            set 
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChange("Name");
                }
            } 
        }
        private string reStart { get; set; }
        public string ReStart 
        {
            get { return reStart; }
            set 
            {
                if (reStart != value)
                {
                    reStart = value;
                    OnPropertyChange("ReStart");
                }
            } 
        }
        private string state { get; set; } = Colors.DarkRed.ToString();
        public string State 
        { 
            get { return state; }
            set
            {
                if (value != State)
                {
                    state = value;
                    OnPropertyChange("State");
                }
            }
        }
        public ObservableCollection<LogEntry> Logs { get; set; } = new ObservableCollection<LogEntry>();
        public Server(string name, string reStart, Color state)
        {
            Name = name;
            ReStart = reStart;
            State = state.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
