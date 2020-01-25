using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Log_Watcher
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : DockingLibrary.DockableContent
    {
        LogViewModel LogViewModel = new LogViewModel();
        LogReader LogReader;
        WindowsFormsSynchronizationContext UIThread = new WindowsFormsSynchronizationContext();
        public LogWindow(string LogFolder, string LogFileName, MainWindow Parent)
        {
            InitializeComponent();
            DockManager = Parent.dockManager;
            DataContext = LogViewModel;
            LogReader = new LogReader(LogFolder, LogFileName);
            LogReader.Start();
            LogReader.Changed += OnChange;
        }

        private void OnChange(object source, FileSystemEventArgs e)
        {
            UIThread.Post(new System.Threading.SendOrPostCallback(NewLogEntries), new LogEntryEventArgs(ProcessChange(e.FullPath, LogReader)));
        }
        private List<string> ProcessChange(string FullPath, LogReader reader)
        {
            List<string> result = new List<string>();
            var fs = new FileStream(FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                if (reader.LastPosition != -1)
                    fs.Seek(reader.LastPosition, SeekOrigin.Begin);

                result = sr.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                reader.LastPosition = fs.Length;
            }
            return result;
        }
        private void NewLogEntries(object state)
        {
            LogEntryEventArgs e = (LogEntryEventArgs)state;
            if (e == null) return;

            foreach (var log in e.Logs)
            {
                lock (LogViewModel.Log)
                {
                    LogViewModel.Log.Add(new LogItem(log));
                }
            }
            while (LogViewModel.Log.Count > MainWindow.MaxLogs)
            {
                lock (LogViewModel.Log)
                {
                    LogViewModel.Log.RemoveAt(0);
                }
            }
        }
        private void PPImage_OnClick(object sender, MouseButtonEventArgs e)
        {
            if (LogReader.Pause)
            {
                LogReader.Pause = false;
                LogViewModel.PPImage = Properties.Resources.PauseButtonIcon.ImageSource();
                OnChange(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, LogReader.LogFolder, LogReader.LogFileName));
            }
            else
            {
                LogReader.Pause = true;
                LogViewModel.PPImage = Properties.Resources.PlayButtonIcon.ImageSource();
            }
        }
    }
}
