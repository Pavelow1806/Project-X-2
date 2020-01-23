using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Core
{
    public class LogReader
    {
        private string logFolder = "";
        public string LogFolder { get { return logFolder; } }
        private string logFileName = "";
        public string LogFileName { get { return logFileName; } }
        private FileSystemWatcher fsw = new FileSystemWatcher();
        public event FileSystemEventHandler Changed
        {
            add 
            { 
                fsw.Changed += value;
                SubscriberCount++;
                ChangeState();
            }
            remove 
            { 
                fsw.Changed -= value;
                SubscriberCount--;
                ChangeState();
            }
        }
        private int SubscriberCount = 0;
        private bool pause { get; set; } = false;
        public bool Pause
        {
            get { return pause; }
            set
            {
                if (pause != value)
                {
                    pause = value;
                    ChangeState();
                }
            }
        }
        public long LastPosition = -1;

        public LogReader(string logFolder = "", string logFileName = "")
        {
            this.logFolder = logFolder;
            this.logFileName = logFileName;
        }
        public void OpenFile()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.InitialDirectory = @"C:\";
            fd.Title = "Browse for log";
            fd.CheckFileExists = true;
            fd.CheckPathExists = true;
            fd.ShowReadOnly = true;
            fd.ReadOnlyChecked = true;
            fd.ShowDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                logFolder = Path.GetDirectoryName(fd.FileName);
                logFileName = Path.GetFileName(fd.FileName);
            }
        }
        public void Start()
        {
            if (!Directory.Exists(logFolder))
            {
                Log.Write(LogType.Error, $"The LogReader directory of {logFolder} didn't exist.");
                return;
            }
            string fullPath = Path.Combine(logFolder, logFileName);
            if (!File.Exists(fullPath))
            {
                Log.Write(LogType.Error, $"The LogReader file of {fullPath} didn't exist.");
                return;
            }
            if (SubscriberCount <= 0)
            {
                Log.Write(LogType.Error, "There were no subscribers to the file changed event.");
                return;
            }
            fsw.Path = logFolder;
            fsw.Filter = logFileName;
            fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.LastAccess;
            fsw.EnableRaisingEvents = !Pause;
        }
        private void ChangeState()
        {
            if (fsw != null && SubscriberCount > 0 && fsw.Path != "")
                fsw.EnableRaisingEvents = !Pause;
            else
                fsw.EnableRaisingEvents = false;
        }
    }
}
