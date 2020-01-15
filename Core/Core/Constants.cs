﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    class Constants
    {
        #region Log
        public const string DefaultLogFolderName = "Logs";
        public const string DefaultLogFileName = "Server.log";
        #endregion

        #region Network
        public const int MaxConnections = 100;
        public const int MaxServers = 10;
        public const int ServerPort = 5610;
        public const int ClientPort = 5600;
        public const int BufferSize = 4096;
        // Milliseconds
        public const double SecondsToAuthenticateBeforeDisconnect = 5000.0;
        #endregion
    }
}