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
using System.Windows.Shapes;
using MessagesSpace;

namespace Klient
{
    /// <summary>
    /// Interaction logic for OknoKomunikatora.xaml
    /// </summary>
    public partial class OknoKomunikatora : Window
    {
        Config cfg = Config.Instance;

        public OknoKomunikatora()
        {
            InitializeComponent();
        }

        private void butSend_Click(object sender, RoutedEventArgs e)
        {
            Messages msbox = new Messages(cfg.nickName, txtBoxUserName.Text, txtBoxOknoNowejWiadomosci.Text, Komendy.TextMessage);
            AsynClient.Send(cfg.client, msbox);
            AsynClient.sendDone.WaitOne();
            AsynClient.Receive(cfg.client);
            AsynClient.receiveDone.WaitOne();
        }
    }
}
