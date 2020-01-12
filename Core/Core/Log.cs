using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public enum LogType
    {
        Information,
        Error,
        Debug,
        Warning,
        Connection
    }
    public class Log
    {
        private const string DefaultLogFolderName = "Logs";
        private const string DefaultLogFileName = "Server.log";
        private static string LogFileName = string.Empty;
        private static string LogLocation = string.Empty;

        private static string fullPath = string.Empty;
        public static string FullPath { get { return fullPath; } }

        private static bool ready = false;
        public static bool Ready { get { return ready; } }

        public static void Init()
        {
            // Load log location
            LogLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), DefaultLogFolderName);
            if (!Directory.Exists(LogLocation)) Directory.CreateDirectory(LogLocation);
            // Load log file name
            LogFileName = ConfigurationManager.AppSettings["LogFileName"] ?? DefaultLogFileName;
            // Build full path
            fullPath = Path.Combine(LogLocation, LogFileName);

            ready = true;
        }

        public static void Write(LogType Type, string Message)
        {
            if (!ready) Init();
            using (StreamWriter writer = new StreamWriter(fullPath, true))
            {
                writer.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss.fff")} {Type.ToFriendlyString()} [{Thread.GetDomainID().ToString("00")}][{Process.GetCurrentProcess().Id.ToString()}] - {Message}");
            }
        }
        public static void Write(LogType Type, string Source, string Message)
        {
            if (!ready) Init();
            using (StreamWriter writer = new StreamWriter(fullPath, true))
            {
                writer.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss.fff")} {Type.ToFriendlyString()} [{Thread.GetDomainID().ToString("00")}][{Process.GetCurrentProcess().Id.ToString()}] - ({Source}) {Message}");
            }
        }
        public static void Write(Exception exception)
        {
            if (!ready) Init();
            using (StreamWriter writer = new StreamWriter(fullPath, true))
            {
                writer.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss.fff")} {LogType.Error.ToFriendlyString()} [{Thread.GetDomainID().ToString("00")}][{Process.GetCurrentProcess().Id.ToString()}] - {exception.StackTrace}");
            }
        }
        public static void Write(string Message, Exception exception)
        {
            if (!ready) Init();
            using (StreamWriter writer = new StreamWriter(fullPath, true))
            {
                writer.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss.fff")} {LogType.Error.ToFriendlyString()} [{Thread.GetDomainID().ToString("00")}][{Process.GetCurrentProcess().Id.ToString()}] - {Message}\n{exception.StackTrace}");
            }
        }
    }
    public static class LogTypeExtensions
    {
        public static string ToFriendlyString(this LogType me)
        {
            switch (me)
            {
                case LogType.Information:
                    return "INFO ";
                case LogType.Error:
                    return "ERROR";
                case LogType.Debug:
                    return "DEBUG";
                case LogType.Warning:
                    return "WARN ";
                case LogType.Connection:
                    return "CONN";
                default:
                    return "NONE";
            }
        }
    }
}

