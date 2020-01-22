using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LogReader ServerLog = new LogReader(@"C:\Users\james\Documents\GitHub\Project-X-2\Console-Application\Console-Application\bin\Debug\Logs", "Server.log");
        private LogReader ConsoleLog = new LogReader(@"C:\Users\james\Documents\GitHub\Project-X-2\Login-Server\Login-Server\bin\Debug\Logs", "Server.log");
        private LogViewModel lvm = new LogViewModel();
        private WindowsFormsSynchronizationContext context = new WindowsFormsSynchronizationContext();
        private const int MAX_LOGS = 1000;
        public MainWindow()
        {
            InitializeComponent();
            ServerLog.Changed += OnServerChanged;
            ConsoleLog.Changed += OnConsoleChanged;
            DataContext = lvm;
            ServerLog.Start();
            ConsoleLog.Start();
        }
        private void OnServerChanged(object source, FileSystemEventArgs e)
        {
            context.Post(new System.Threading.SendOrPostCallback(NewLogEntries), new LogEntryEventArgs(ProcessChange(e.FullPath, ServerLog), ServerLogListBox, lvm.ServerLog));
        }
        private void OnConsoleChanged(object source, FileSystemEventArgs e)
        {
            context.Post(new System.Threading.SendOrPostCallback(NewLogEntries), new LogEntryEventArgs(ProcessChange(e.FullPath, ConsoleLog), ConsoleLogListBox, lvm.ConsoleLog));
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
                e.LogView.Add(new LogItem(log));

            }
            while (e.LogView.Count > MAX_LOGS)
            {
                e.LogView.RemoveAt(0);
            }
        }
    }
}
