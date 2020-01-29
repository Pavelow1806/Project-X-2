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
        MainWindowViewModel vm = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();
            Background.MouseLeftButtonDown += vm.Background_Click;
            DataContext = vm;
        }
        public void OnLoaded(object sender, EventArgs e)
        {

        }
        private void OnClosing(object sender, EventArgs e)
        {

        }
    }
}
