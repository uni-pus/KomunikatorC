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
using System.Net.Sockets;

namespace Klient
{
    /// <summary>
    /// Interaction logic for OknoKomunikatora.xaml
    /// </summary>
    public partial class OknoKomunikatora : Window
    {
        //Config cfg = Config.Instance;
        public Socket clientSocket;
        public string strName;
        private byte[] byteData = new byte[1024];

        //convert from byte to MessageModel or otherwise
        //MessageConverter msgConverterObj = new MessageConverter();

        public OknoKomunikatora(Socket cSocket, string sName)
        {
            clientSocket = cSocket;
            strName = sName;

            InitializeComponent();
            //Window_Loaded();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = strName;

            //The user has logged into the system so we now request the server to send
            //the names of all users who are in the chat room

            byteData = new byte[1024];
            //Start listening to the data asynchronously
            clientSocket.BeginReceive(byteData,
                                       0,
                                       byteData.Length,
                                       SocketFlags.None,
                                       new AsyncCallback(OnReceive),
                                       null);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to leave the chat room?", "SGSclient: " + strName,
                MessageBoxButton.YesNo) == MessageBoxResult.No)
            {

                e.Cancel = true;
                return;
            }

            try
            {
                //Send a message to logout of the server

                byte[] b = MessageGenerator.koduj(new Messages(strName, Komendy.Logout));

                clientSocket.Send(b, 0, b.Length, SocketFlags.None);
                clientSocket.Close();
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclientTCP: " + strName);
            }
        }

        private void butSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Fill the info for the message to be send

                byte[] byteData = MessageGenerator.koduj(new Messages(strName, txtBoxUserName.Text, txtBoxOknoNowejWiadomosci.Text, Komendy.TextMessage));


                //Send it to the server
                clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

                txtBoxOknoNowejWiadomosci.Text = null;
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to send message to the server.", "SGSclientTCP: " + strName);
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    clientSocket.EndSend(ar);
                }
                catch (ObjectDisposedException)
                { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SGSclientTCP: " + strName);
                }
            }));
        }

        private void OnReceive(IAsyncResult ar)
        {

            this.Dispatcher.Invoke((Action)(() =>
            {
                try
                {
                    clientSocket.EndReceive(ar);

                    Messages msgReceived = MessageGenerator.dekoduj(byteData);


                    if (msgReceived.body != null)

                        txtBoxOknoRozmowy.Text += msgReceived.from + ": " + msgReceived.body + Environment.NewLine;

                    byteData = new byte[1024];

                    clientSocket.BeginReceive(byteData,
                                              0,
                                              byteData.Length,
                                              SocketFlags.None,
                                              new AsyncCallback(OnReceive),
                                              null);

                }
                catch (ObjectDisposedException)
                { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SGSclientTCP: " + strName);
                }
            }));
        }
    }
}
