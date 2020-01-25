using Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Log_Watcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int MaxLogs = 1000;
        private List<LogWindow> LogWindows = new List<LogWindow>();
        MainWindowViewModel vm = new MainWindowViewModel();
        WindowsFormsSynchronizationContext UIThread = new WindowsFormsSynchronizationContext();
        private string SavedLogsPath = "";
        private const string SavedLogsFileName = "SavedLogs.logs";
        public MainWindow()
        {
            SavedLogsPath = Path.Combine(Directory.GetCurrentDirectory(), SavedLogsFileName);
            ReadLogs();
            InitializeComponent();
            Background.MouseLeftButtonDown += Background_Click;
            DataContext = vm;
        }
        public void OnLoaded(object sender, EventArgs e)
        {
            dockManager.ParentWindow = this;
        }
        private void OnClosing(object sender, EventArgs e)
        {
            Properties.Settings.Default.DockingLayoutState = dockManager.GetLayoutAsXml();
            Properties.Settings.Default.Save();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            vm.SaveLog = false;
            vm.Visible = true;
            vm.WindowContextMenu = MainWindowContent.Open;
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            vm.Visible = true;
            vm.WindowContextMenu = MainWindowContent.Load;
        }
        private void Background_Click(object sender, MouseButtonEventArgs e)
        {
            vm.Visible = false;
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
                LogWindows.Add(lw);
                lw.CloseWindow += WindowOnClose;
                lw.Show();
                vm.Path = path;
                if (vm.SaveLog)
                {
                    lock (vm.SavedLogs)
                    {
                        if (!vm.SavedLogs.Any(x => x.Path == path))
                            vm.WindowContextMenu = MainWindowContent.Save;
                        else
                            vm.Visible = false;
                    }
                }
                else
                {
                    vm.Visible = false;
                }
            }
            else
            {
                vm.Visible = false;
            }
        }
        private void SaveLog_Click(object sender, RoutedEventArgs e)
        {
            SavedLog sl = new SavedLog(vm.Path, vm.Alias);
            vm.SavedLogs.Add(sl);
            SaveLog(sl);
            vm.Visible = false;
        }
        private void LoadLog_Click(object sender, RoutedEventArgs e)
        {
            if (vm.SelectedLog == null)
            {
                Log.Write(LogType.Error, "There was not log selected.");
                return;
            }
            LogWindow lw = new LogWindow(System.IO.Path.GetDirectoryName(vm.SelectedLog.Path), System.IO.Path.GetFileName(vm.SelectedLog.Path), this, vm.Alias);
            LogWindows.Add(lw);
            lw.CloseWindow += WindowOnClose;
            lw.Show();
            vm.Visible = false;
        }

        private void SaveLog(SavedLog sl)
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
                LogWindows.Remove(lw);
                lw.Close();
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

            lock (vm.SavedLogs)
            {
                vm.SavedLogs.Add(sl);
            }
        }
        private void CheckBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
