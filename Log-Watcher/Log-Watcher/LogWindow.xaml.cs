using Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class LogWindow : Window
    {
        LogViewModel LogViewModel = new LogViewModel();
        LogReader LogReader;
        WindowsFormsSynchronizationContext UIThread = new WindowsFormsSynchronizationContext();
        MainWindowViewModel MWVM;
        string FullPath = "";

        public EventHandler CloseWindow;
        public LogWindow(string LogFolder, string LogFileName, MainWindowViewModel Parent, string Alias = "", bool LogNotSaved = true)
        {
            InitializeComponent();
            LogViewModel.LogNotSaved = LogNotSaved;
            MWVM = Parent;
            DataContext = LogViewModel;
            LogViewModel.Title = $"{(!string.IsNullOrEmpty(Alias) ? $"{Alias} [{LogFileName}]" : $"{LogFileName}")}";
            LogPP.MouseLeftButtonUp += PPImage_OnClick;
            LogSave.MouseLeftButtonUp += SaveIcon_OnClick;
            LogReader = new LogReader(LogFolder, LogFileName);
            FullPath = System.IO.Path.Combine(LogFolder, LogFileName);
            LogReader.Changed += OnChange;
            LogReader.Start();
            // Manual refresh
            OnChange(this, new FileSystemEventArgs(WatcherChangeTypes.Changed, LogFolder, LogFileName));
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
                    fs.Seek(reader.LastPosition, SeekOrigin.Current);

                result = sr.ReadToEnd().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Take(MainWindowViewModel.MaxLogs).ToList();
                reader.LastPosition = fs.Length;
            }
            return result;
        }
        private void NewLogEntries(object state)
        {
            LogEntryEventArgs e = (LogEntryEventArgs)state;
            if (e == null) return;
            if (e.Logs == null)
            {
                Log.Write(LogType.Error, "The Logs list sent was null.");
                return;
            }
            if (e.Logs.Count == 0)
            {
                Log.Write(LogType.Information, "The Log doesn't contain anything yet.");
                return;
            }

            Log.Write(LogType.Debug, $"Setting the Logs view model value, which contains {e.Logs.Count} lines...");
            foreach (var item in e.Logs)
            {
                lock (LogViewModel.Log)
                {
                    LogViewModel.Log.Add(new LogItem(item));
                }
            }
            Log.Write(LogType.Debug, "Logs view model value set.");
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

        private void SaveIcon_OnClick(object sender, MouseButtonEventArgs e)
        {
            if (MWVM == null)
                return;

            LogReader.Pause = true;

            MWVM.Path = FullPath;
            MWVM.WindowContextMenu = MainWindowContent.Save;
            MWVM.PropertyChanged += LogSavedReturn;
            MWVM.Visible = true;
        }
        private void LogSavedReturn(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Visible")
                return;

            LogReader.Pause = false;

            lock (MWVM.SavedLogs)
            {
                if (!MWVM.SavedLogs.Any(x => x.Path == System.IO.Path.Combine(LogReader.LogFolder, LogReader.LogFileName)))
                    LogViewModel.LogNotSaved = true;
                else
                    LogViewModel.LogNotSaved = false;
            }
            MWVM.PropertyChanged -= LogSavedReturn;
        }

        private void DockableContent_Closed(object sender, EventArgs e)
        {
            LogReader.Pause = true;
            LogReader = null;

            LogPP.MouseLeftButtonUp -= PPImage_OnClick;
            LogSave.MouseLeftButtonUp -= SaveIcon_OnClick;
            MWVM.PropertyChanged -= LogSavedReturn;

            CloseWindow?.Invoke(this, new EventArgs());
        }
    }
}
