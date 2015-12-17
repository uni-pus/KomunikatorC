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
using System.Net;
using System.Threading;

namespace Klient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// https://msdn.microsoft.com/pl-pl/library/system.net.sockets.tcpclient%28v=vs.110%29.aspx
    /// </summary>
    public partial class MainWindow : Window
    {
        Config cfg = Config.Instance;
        //private static Socket client;
        private const int port = 8888;
        //public string nick = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void zalogujbutton_Click(object sender, RoutedEventArgs e)
        {
            cfg.nickName = loginTextBox.Text;
            AsynClient.StartClient();

            Messages msbox = new Messages(cfg.nickName, "haslo", Komendy.Login);

            AsynClient.Send(cfg.client, msbox);
            AsynClient.sendDone.WaitOne();
            AsynClient.Receive(cfg.client);
            AsynClient.receiveDone.WaitOne();
            if(AsynClient.response2.komenda==Komendy.Login && AsynClient.response2.body=="OK")
            {
                this.Visibility = Visibility.Hidden;
                OknoKomunikatora noweOkno = new OknoKomunikatora();
                noweOkno.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Klasa do wyrzucenia
        /// </summary>
        /// <param name="sender"></param>
        private void testButton_Click(object sender, RoutedEventArgs e)
        {
            Messages wiadomos = new Messages(cfg.nickName, "tresc", Komendy.TextMessage);
            byte[] wiadomoscBajty = MessageGenerator.koduj(wiadomos);
        }



    }
}
