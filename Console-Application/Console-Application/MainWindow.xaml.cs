using Core;
using System;
using System.Collections.Generic;
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
        public string Contents
        {
            set { Console.Text = value; }
        }
        public MainWindow()
        {
            InitializeComponent();
            Contents = "Hi";
            SocketManagement LoginServer = new SocketManagement("Login Server", 1991, true, ConnectionType.Outbound);
            LoginServer.Connect("127.0.0.1", "Hello World!");
        }
    }
}
