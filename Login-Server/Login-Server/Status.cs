using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Login_Server
{
    class Status
    {
        Thread ConsoleThread;
        public Status()
        {
            Log.Write(LogType.Information, "Starting Console TcpListener..");
            ConsoleThread = new Thread(new ThreadStart(Console.StartListening));
        }
        ~Status()
        {
            ConsoleThread.Join();
        }
    }
}
