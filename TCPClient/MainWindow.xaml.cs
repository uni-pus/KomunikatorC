using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using XMLMessage;
using XMLMessage.Models;


namespace TCPClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Socket clientSocket;
        public string strName;
        private byte[] byteData = new byte[1024];

        //convert from byte to MessageModel or otherwise
        MessageConverter msgConverterObj = new MessageConverter();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
            
                string[] lines = System.IO.File.ReadAllLines(@"IpConfiguration.txt");
                lines[0] = @"127.0.0.1";
                lines[1] = "8888";

                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPAddress ipAddress = IPAddress.Parse(lines[0]);
                //Server is listening on port 1000
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress,int.Parse(lines[1]));


                //Connect to the server
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);

                byteData = new byte[1024];
                //Start listening to the data asynchronously
                clientSocket.BeginReceive(byteData,
                                           0,
                                           byteData.Length,
                                           SocketFlags.None,
                                           new AsyncCallback(OnReceive),
                                           null);

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

                      byte[] b = msgConverterObj.ToByte(new MessageModel()
                      {

                          SenderCommand = Command.Login,
                          SenderName = userName.Text,
                          SenderMessage = userPass.Text,
                          Reciever = null

                      });

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

                    strName = userName.Text;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SGSclient");
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

                    MessageModel msgReceived = msgConverterObj.toMessage(byteData);


                    if (msgReceived.SenderMessage == "OK")
                    {
                        ClientWindow clientForm = new ClientWindow(clientSocket, strName);
                        this.Hide();
                        clientForm.ShowDialog();

                        Close();
                    }

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
