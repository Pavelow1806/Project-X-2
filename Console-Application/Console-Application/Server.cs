using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Console_Application
{
    sealed class Server
    {
        public string Name { get; set; }
        public string ReStart { get; set; }
        public Color State { get; set; }
        public Server(string name, string reStart, Color state)
        {
            Name = name;
            ReStart = reStart;
            State = state;
        }
    }
}
