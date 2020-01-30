using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Log_Watcher
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public static int MaxLogs = 1000;
        WindowsFormsSynchronizationContext UIThread = new WindowsFormsSynchronizationContext();
        private string SavedLogsPath = "";
        private const string SavedLogsFileName = "SavedLogs.logs";

        private bool visible = false;
        public bool Visible 
        { 
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    if (!value)
                        WindowContextMenu = MainWindowContent.Open;
                    OnPropertyChanged("Visible");
                }
            }
        }

        private MainWindowContent windowContextMenu = MainWindowContent.Open;
        public MainWindowContent WindowContextMenu 
        { 
            get { return windowContextMenu; } 
            set
            {
                if (windowContextMenu != value)
                {
                    windowContextMenu = value;
                    OnPropertyChanged("WindowContextMenu");
                }
            }
        }

        private ObservableCollection<SavedLog> savedLogs = new ObservableCollection<SavedLog>();
        public ObservableCollection<SavedLog> SavedLogs
        {
            get { return savedLogs; }
            set
            {
                if (savedLogs != value)
                {
                    savedLogs = value;
                    OnPropertyChanged("SavedLogs");
                }
            }
        }
        public SavedLog SelectedLog { get; set; }
        public bool SaveLog { get; set; } = false;

        private string alias = "";
        public string Alias
        {
            get { return alias; }
            set
            {
                if (alias != value)
                {
                    alias = value;
                    OnPropertyChanged("Alias");
                }
            }
        }

        private string path = "";
        public string Path 
        { 
            get { return path; } 
            set
            {
                if (path != value)
                {
                    path = value;
                    OnPropertyChanged("Path");
                }
            }
        }

        private ObservableCollection<LogWindow> logWindows = new ObservableCollection<LogWindow>();
        private ReadOnlyObservableCollection<LogWindow> readOnlyLogWindows = null;

        public ReadOnlyObservableCollection<LogWindow> LogWindows
        {
            get
            {
                if (readOnlyLogWindows == null)
                    readOnlyLogWindows = new ReadOnlyObservableCollection<LogWindow>(logWindows);

                return readOnlyLogWindows;
            }
        }

        public bool CanExecute
        {
            get { return true; }
        }

        public MainWindowViewModel()
        {
            SavedLogsPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), SavedLogsFileName);
            ReadLogs();
        }

        private void Open()
        {
            SaveLog = false;
            Visible = true;
            WindowContextMenu = MainWindowContent.Open;
        }
        private void Load()
        {
            Visible = true;
            WindowContextMenu = MainWindowContent.Load;
        }
        public void Background_Click(object sender, MouseButtonEventArgs e)
        {
            Visible = false;
        }

        private void OpenLog_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Open a log",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "log",
                Filter = "log files (*.log)|*.log"
            };
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = fd.FileName;
                LogWindow lw = new LogWindow(System.IO.Path.GetDirectoryName(path), System.IO.Path.GetFileName(path), this);
                logWindows.Add(lw);
                lw.CloseWindow += WindowOnClose;
                //lw.Show();
                Path = path;
                if (SaveLog)
                {
                    lock (SavedLogs)
                    {
                        if (!SavedLogs.Any(x => x.Path == path))
                            WindowContextMenu = MainWindowContent.Save;
                        else
                            Visible = false;
                    }
                }
                else
                {
                    Visible = false;
                }
            }
            else
            {
                Visible = false;
            }
        }
        private void saveLog()
        {
            SavedLog sl = new SavedLog(Path, Alias);
            SavedLogs.Add(sl);
            SaveLogToFile(sl);
            Visible = false;
        }
        private void LoadLog()
        {
            if (SelectedLog == null)
            {
                Log.Write(LogType.Error, "There was no log selected.");
                return;
            }
            LogWindow lw = new LogWindow(System.IO.Path.GetDirectoryName(SelectedLog.Path), System.IO.Path.GetFileName(SelectedLog.Path), this, SelectedLog.Alias);
            logWindows.Add(lw);
            lw.CloseWindow += WindowOnClose;
            Visible = false;
        }

        private void SaveLogToFile(SavedLog sl)
        {
            using (StreamWriter sw = new StreamWriter(SavedLogsPath, true))
            {
                sw.WriteLine($"{sl.Path}|{sl.Alias}");
            }
        }
        private void WindowOnClose(object sender, EventArgs e)
        {
            LogWindow lw = (LogWindow)sender;
            lock (LogWindows)
            {
                logWindows.Remove(lw);
                //lw.Close();
                lw = null;
            }
        }
        private void ReadLogs()
        {
            if (!File.Exists(SavedLogsPath))
            {
                Log.Write(LogType.Information, "The saved logs file was not found, or you haven't saved any logs yet.");
                return;
            }
            string output = "";
            using (StreamReader sr = new StreamReader(SavedLogsPath))
            {
                output = sr.ReadToEnd();
            }
            if (string.IsNullOrEmpty(output))
            {
                Log.Write(LogType.Error, "The output from loading the saved log file was empty or null");
            }
            string[] SplitSavedLogs = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var savedLog in SplitSavedLogs)
            {
                string[] savedLogSplit = savedLog.Split('|');
                string path = savedLogSplit[0];
                string alias = savedLogSplit[1];
                SavedLog sl = new SavedLog(path, alias);

                UIThread.Post(new System.Threading.SendOrPostCallback(NewSavedLog), sl);
            }
        }
        private void NewSavedLog(object state)
        {
            SavedLog sl = (SavedLog)state;

            lock (SavedLogs)
            {
                SavedLogs.Add(sl);
            }
        }
        #region Commands
        private ICommand OpenCommand = null;
        public ICommand Open_Click
        {
            get
            {
                return OpenCommand ?? (OpenCommand = new CommandHandler(() => Open(), () => CanExecute));
            }
        }

        private ICommand LoadCommand = null;
        public ICommand Load_Click
        {
            get
            {
                return LoadCommand ?? (LoadCommand = new CommandHandler(() => Load(), () => CanExecute));
            }
        }

        private ICommand LoadLogCommand = null;
        public ICommand LoadLog_Click
        {
            get
            {
                return LoadLogCommand ?? (LoadLogCommand = new CommandHandler(() => LoadLog(), () => CanExecute));
            }
        }

        private ICommand SaveLogCommand = null;
        public ICommand SaveLog_Click
        {
            get
            {
                return SaveLogCommand ?? (SaveLogCommand = new CommandHandler(() => saveLog(), () => CanExecute));
            }
        }
        #endregion
    }
}
