using System;
using System.Collections.Generic;
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
using XMLMessage.Models;
using XMLMessage;

namespace RS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //The collection of all clients logged into the room (an array of type ClientInfo)
        List<ClientModel> clientList;
        ServerDataBase dbserver;
        //The main socket on which the server listens to the clients
        Socket serverSocket;
        MessageConverterRS msgConverterObj = new MessageConverterRS();
        byte[] byteData = new byte[1024];

        public MainWindow()
        {
            dbserver = new ServerDataBase();
            clientList = new List<ClientModel>();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //We are using TCP sockets
                /* 
                Pakiet TCP wchodzi do baru: 
                - Poproszę piwo. 
                - Piwo? 
                - Tak, piwo.
                */
                serverSocket = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream,
                                          ProtocolType.Tcp);

                //Assign the any IP of the machine and listen on port number 1000
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 8889);

                //Bind and listen on the given address
                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(4);

                //Accept the incoming clients
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                messageTextBox.Text += "Listening\n";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSserverTCP");
            }

        }


        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = serverSocket.EndAccept(ar);
                //Start listening for more clients, for multiple clients
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
                //Once the client connects then start receiving the commands from her
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), clientSocket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSserverTCP");
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;
                clientSocket.EndReceive(ar);

                //Transform the array of bytes received from the user into an
                //intelligent form of object Data

                MessageModelRS msgReceived = msgConverterObj.toMessage(byteData);

                byte[] message;

                switch (msgReceived.SenderCommand)
                {
                    case Command.Login:

                        MessageModelRS msg = new MessageModelRS()
                        {
                            SenderCommand = Command.Login,
                            //SenderMessage = "OK"
                        };
                       // msg.timeStamp();
                        message = msgConverterObj.ToByte(msg);
                        //When a user logs in to the server then we add her to our
                        //list of clients
                        if (true) // dopisać funkcję sprawdzającą haslo 
                        {
                            // to chyba niepotrzebne ;)
//                            clientList.Add(new ClientModel()
 //                           {
  //                              socket = clientSocket,
   //                             strName = clientSocket.GetHashCode().ToString()// msgReceived.SenderName
    //                        });

                            clientSocket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                 new AsyncCallback(OnSend), clientSocket);
                        }
                        break;
                    case Command.LoginServer:
                        string port = dbserver.WolnyPort();
                        MessageModelRS msgRS = new MessageModelRS()
                        {
                            SenderCommand = Command.LoginServer,
                            OtherData = port
                        };
                        byteData = msgConverterObj.ToByte(msgRS);

                        clientSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnSend), clientSocket);
                        break;
                    case Command.Logout:

                        //When a user wants to log out of the server then we search for her 
                        //in the list of clients and close the corresponding connection
                        /*
                        int nIndex = 0;
                        foreach (ClientModel client in clientList)
                        {
                            if (client.socket == clientSocket)
                            {
                                clientList.RemoveAt(nIndex);
                                break;
                            }
                            ++nIndex;
                        }

                        clientSocket.Close();
                        */
                        break;

                    case Command.Message:
                        /*
                        var userExist = clientList.FirstOrDefault(p => p.strName == msgReceived.Reciever);

                        //We will send this object in response the users request
                        MessageModelRS msgToSend = new MessageModelRS()
                        {
                            SenderCommand = msgReceived.SenderCommand,
                            //SenderName = msgReceived.SenderName,
                            //Reciever = msgReceived.Reciever,
                            //SenderMessage = msgReceived.SenderMessage

                        };

                        //Set the text of the message that we will broadcast to all users
                        if (userExist == null)
                        {
                            msgToSend.SenderMessage = "user " + msgToSend.Reciever + " could not be found!";
                        }

                        message = msgConverterObj.ToByte(msgToSend);

                        foreach (ClientModel clientInfo in clientList)
                        {
                            if (clientInfo.strName == msgToSend.Reciever || clientInfo.strName == msgToSend.SenderName)
                            {
                                //Send the message to all users
                                clientInfo.socket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                    new AsyncCallback(OnSend), clientInfo.socket);
                            }
                        }
                        */
                        break;

                }

                // whatever the message display in the server
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (msgReceived.SenderCommand == Command.Login)
                    {

                        messageTextBox.Text += msgReceived.ClientName + " is try login ";// + msgReceived.Reciever + " :" + msgReceived.SenderMessage + "\n";

                    }
                    else
                    {

                        messageTextBox.Text += "Blablablabla";// msgReceived.SenderName + " is " + msgReceived.SenderCommand.ToString() + "\n";

                    }

                }));

                //If the user is logging out then we need not listen from her
                if (msgReceived.SenderCommand != Command.Logout)
                {
                    //Start listening to the message send by the user
                    clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSserverTCP");
            }
        }

        public void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSserverTCP");
            }
        }
    }
}
