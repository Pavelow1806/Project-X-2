using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Login_Server
{
    class Base
    {
        public Base() 
        {
            Status status = new Status();
        }
        ~Base()
        {
            Log.Write(LogType.Information, "Server shutting down.");
        }
    }
}
