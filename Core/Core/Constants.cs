using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Constants
    {
        #region Log
        public const string DefaultLogFolderName = "Logs";
        public const string DefaultLogFileName = "Server.log";
        #endregion

        #region Network
        public const int MaxConnections = 100;
        public const int MaxServers = 10;
        public const int ClientPort = 5600;
        public const int BufferSize = 4096;
        public const int MillisecondsBetweenAttemptingConnect = 5000;
        public const double MillisecondsToAuthenticateBeforeDisconnect = 30000.0;
        public const int MaxToolConnectAttempts = 5;
        public const string ClusterLocalIP = "127.0.0.1";
        #endregion
    }
}
