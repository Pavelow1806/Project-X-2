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
        private readonly ServerModel _serverViewModel;
        public MainWindow()
        {
            InitializeComponent();
            _serverViewModel = new ServerModel();
            // The DataContext serves as the starting point of Binding Paths
            DataContext = _serverViewModel;
            _serverViewModel.servers[0].State = "Black";
        }
        public void Something(object sender, EventArgs e)
        {
            Log.Write(LogType.Information, "Helloworld!");
        }
    }
}
