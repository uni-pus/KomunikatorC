using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagesSpace;

/*
Wchodzi pakiet TCP do baru i mówi:
- Poproszę piwo.
- Piwo ?
- Tak, piwo.
*/

namespace Server
{
    class Program
    {
        //The collection of all clients logged into the room (an array of type ClientInfo)
        static List<SocketWithNick> clientList;
        //The main socket on which the server listens to the clients
        static Socket serverSocket;

        //convert from byte to MessageModel or otherwise
        // MessageConverter msgConverterObj = new MessageConverter();

        static byte[] byteData = new byte[1024];

         public static int Main(String[] args)
        {
              clientList   = new List<SocketWithNick>();
            StartListening();
            return 0;
        }

        static private void StartListening()
        {
            try
            {
                //We are using TCP sockets
                serverSocket = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream,
                                          ProtocolType.Tcp);

                //Assign the any IP of the machine and listen on port number 1000
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 8888);

                //Bind and listen on the given address
                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(100);

                //Accept the incoming clients
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                Console.WriteLine("Listening\n");
                for(;;)
                {
                    string k = Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message, "SGSserverTCP");
            }

        }


        static private void OnAccept(IAsyncResult ar)
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
                //MessageBox.Show(ex.Message, "SGSserverTCP");
            }
        }

        static private void OnReceive(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;
                clientSocket.EndReceive(ar);

                //Transform the array of bytes received from the user into an
                //intelligent form of object Data

                Messages msg = MessageGenerator.dekoduj(byteData);
                msg.stempelCzasu();

                byte[] message;

                switch (msg.komenda)
                {
                    case Komendy.Login:

                        //When a user logs in to the server then we add her to our
                        //list of clients

                        clientList.Add(new SocketWithNick()
                        {
                            socket = clientSocket,
                            nickname = msg.from
                        });

                        break;

                    case Komendy.Logout:

                        //When a user wants to log out of the server then we search for her 
                        //in the list of clients and close the corresponding connection

                        int nIndex = 0;
                        foreach (SocketWithNick client in clientList)
                        {
                            if (client.socket == clientSocket)
                            {
                                clientList.RemoveAt(nIndex);
                                break;
                            }
                            ++nIndex;
                        }

                        clientSocket.Close();
                        break;

                    case Komendy.TextMessage:

                        var userExist = clientList.FirstOrDefault(p => p.nickname == msg.to);

                        //We will send this object in response the users request
                        Messages msgToSend = msg;// new Messages(msg.from, msg.to, msg.body, msg.komenda);
                        /*
                        {
                            SenderCommand = msg.SenderCommand,
                            SenderName = msg.SenderName,
                            Reciever = msg.Reciever,
                            SenderMessage = msg.SenderMessage

                        };*/

                        //Set the text of the message that we will broadcast to all users
                        if (userExist == null)
                        {
                            msgToSend.body = "user " + msgToSend.to + " could not be found!";
                        }

                        message = MessageGenerator.koduj(msg);// msgConverterObj.ToByte(msgToSend);

                        foreach (SocketWithNick clientInfo in clientList)
                        {
                            if (clientInfo.nickname == msgToSend.to || clientInfo.nickname == msgToSend.from)
                            {
                                //Send the message to all users
                                clientInfo.socket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                    new AsyncCallback(OnSend), clientInfo.socket);
                            }
                        }

                        break;

                }

                // whatever the message display in the server
                /*
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (msg.SenderCommand == Command.Message)
                    {

                        messageTextBox.Text += msg.SenderName + " sent message to " + msg.Reciever + " :" + msg.SenderMessage + "\n";

                    }
                    else
                    {

                        messageTextBox.Text += msg.SenderName + " is " + msg.SenderCommand.ToString() + "\n";

                    }

                }));
                */
                //If the user is logging out then we need not listen from her
                if (msg.komenda != Komendy.Logout)
                {
                    //Start listening to the message send by the user
                    clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "SGSserverTCP");
            }
        }

        static public void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "SGSserverTCP");
            }
        }
    }



}