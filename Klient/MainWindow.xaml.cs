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
        public Socket clientSocket;
        public string strName;

        Config cfg = Config.Instance;
        //private static Socket client;
        private const int port = 8888;
        //public string nick = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void testButton_Click(object sender, RoutedEventArgs e)
        {

        }
        
        private void zalogujbutton_Click(object sender, RoutedEventArgs e)
        {
            strName = loginTextBox.Text;
            try
            {

                string[] lines = {"","" };// = System.IO.File.ReadAllLines(@"IpConfiguration.txt");
                lines[0] = "127.0.0.1";
                lines[1] = "8888";

                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPAddress ipAddress = IPAddress.Parse(lines[0]);
                //Server is listening on port 1000
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, int.Parse(lines[1]));


                //Connect to the server
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient");
            }
        }




        private void OnConnect(IAsyncResult ar)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    clientSocket.EndConnect(ar);

                    //We are connected so we login into the server

                    byte[] b = MessageGenerator.koduj(new Messages(strName, "haslo", Komendy.Login));


                        //Send the message to the server
                        clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SGSclient");
                }
            }));
        }

        private void OnSend(IAsyncResult ar)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    clientSocket.EndSend(ar);

                    //strName = userName.Text;

                    OknoKomunikatora clientForm = new OknoKomunikatora(clientSocket, strName);
                    this.Hide();
                    clientForm.ShowDialog();


                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SGSclient");
                }
            }));
        }
    }
}
    

