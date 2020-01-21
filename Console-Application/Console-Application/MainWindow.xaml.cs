using Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Console_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ServerModel serverViewModel;
        private readonly Monitor monitor;
        public MainWindow()
        {
            InitializeComponent();
            serverViewModel = new ServerModel();
            // The DataContext serves as the starting point of Binding Paths
            DataContext = serverViewModel;
            monitor = new Monitor(serverViewModel);
        }
    }
}
