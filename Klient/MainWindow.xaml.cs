using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
using MessagesSpace;

namespace Klient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// https://msdn.microsoft.com/pl-pl/library/system.net.sockets.tcpclient%28v=vs.110%29.aspx
    /// </summary>
    public partial class MainWindow : Window
    {
        public string nick = "";
        public Socket client;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void zalogujbutton_Click(object sender, RoutedEventArgs e)
        {
            nick = loginTextBox.Text;
            NetworkConfig nc = new NetworkConfig();
            nc.client = new TcpClient(nc.Ip, Convert.ToInt32(nc.Port));
            Messages wiadomos = new Messages(nick, "haslo", Komendy.Login);
            byte[] wiadomoscBajty = MessageGenerator.koduj(wiadomos);
            NetworkStream ns = nc.client.GetStream();
            ns.Write(wiadomoscBajty, 0, wiadomoscBajty.Length);
        }
    }
}
