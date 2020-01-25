using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

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
        public MainWindow()
        {
            InitializeComponent();
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
            vm.Visible = true;
            vm.WindowContextMenu = MainWindowContent.Open;
        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            vm.Visible = true;
            vm.WindowContextMenu = MainWindowContent.Load;
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
                lw.Show();
            }
            vm.Visible = false;
        }
        private void SaveLog_Click(object sender, RoutedEventArgs e)
        {
            vm.Visible = false;
        }
        private void LoadLog_Click(object sender, RoutedEventArgs e)
        {
            vm.Visible = false;
        }

        private void SaveLog()
        { }
    }
}
