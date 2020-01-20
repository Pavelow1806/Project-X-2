using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Console_Application
{
    public sealed class Server
    {
        public string Name { get; set; }
        public string ReStart { get; set; }
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
        public LogList Logs { get; set; } = new LogList();
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
