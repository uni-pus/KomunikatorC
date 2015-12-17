using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessagesSpace;


namespace Server
{
    class Program
    {

        static public List<SocketWithNick> handlerList;// = new List<SocketWithNick>();
        static public Socket sSocket;
        static byte[] byteData = new byte[1024];
        // State object for reading client data asynchronously
        public class StateObject
        {
            // Client  socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 1024;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        public class AsynchronousSocketListener
        {
            public static int Main(String[] args)
            {
                handlerList = new List<SocketWithNick>();
                StartListening();
                return 0;
            }
            // Thread signal.
            public static ManualResetEvent allDone = new ManualResetEvent(false);

            public AsynchronousSocketListener()
            {
            }

            public static void StartListening()
            {
                // Data buffer for incoming data.
                byte[] bytes = new Byte[1024];

                // Establish the local endpoint for the socket.
                // The DNS name of the computer
                // running the listener is "host.contoso.com".
                //IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8888);

                // Create a TCP/IP socket.
                /*
                Wchodzi pakiet TCP do baru i mówi:
                - Poproszę piwo.
                - Piwo ?
                - Tak, piwo.
               */
                sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.
                try
                {
                    sSocket.Bind(localEndPoint);
                    sSocket.Listen(100);
                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    sSocket.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        null
                        );
                    // Wait until a connection is made before continuing.
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                //Console.WriteLine("\nPress ENTER to continue...");
                //Console.Read();

            }

            public static void AcceptCallback(IAsyncResult ar)
            {
                // Signal the main thread to continue.
                //allDone.Set();

                // Get the socket that handles the client request.
                //Socket listener = (Socket)ar.AsyncState;
                Socket handler = sSocket.EndAccept(ar);

                // Create the state object.
                StateObject state = new StateObject();
                //state.workSocket = handler;
                handler.BeginReceive(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(ReadCallback), handler);
            }

            public static void ReadCallback(IAsyncResult ar)
            {
                String content = String.Empty;

                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                Socket handler = (Socket)ar.AsyncState;
                handler.EndReceive(ar);
                //SocketWithNick handler = new SocketWithNick();
                //handler.socket = state.workSocket;
                //handlerList.Add(handler);
                Messages msg = MessageGenerator.dekoduj(byteData);
                // Read data from the client socket. 
                try {
                    //int bytesRead = handler.socket.EndReceive(ar);
                    //Messages data = MessageGenerator.dekoduj(state.buffer);
                    msg.stempelCzasu();
                    switch (msg.komenda)
                    {
                        case Komendy.Login:
                            handlerList.Add(new SocketWithNick() { socket = handler, nickname = msg.from });
                            // dopisac metode sprawdzajaca haslo
                            Console.WriteLine("user: {0} zalogowany", msg.from);
                            msg.body = "OK";
                            Send(handler, msg);
                            break;
                        case Komendy.Help:
                            break;
                        case Komendy.Logout:
                            break;
                        case Komendy.TextMessage:
                            bool istnieje = false;
                            Console.WriteLine("user: {0} wysłał wiadomość do usera {1}", msg.from, msg.to);
                            foreach(SocketWithNick odbiorca in handlerList)
                            {
                                if(odbiorca.nickname == msg.to)
                                {
                                    istnieje = true;
                                    Send(odbiorca.socket, msg);
                                    //wyslac potwierdzenie do nadawcy
                                }
                            }
                            //if(!istnieje)
                              //wyslac informacje do nadawcy o braku mozliwosci wyslania wiadomosci  
                            break;
                        default:
                            break;
                    }

                } catch (Exception ex)
                {
                    Console.WriteLine("Rozłączono klienta");
                   // handlerList.Remove(Socket);
                }

                if(msg.komenda != Komendy.Logout)
                {
                    handler.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(ReadCallback), handler);
                }
            }

            private static void Send(Socket handler, Messages data)
            {
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = MessageGenerator.koduj(data); //Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.
                handler.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), handler);
            }

            public static void SendCallback(IAsyncResult ar)
            {
                try
                {
                    Socket client = (Socket)ar.AsyncState;

                    client.EndSend(ar);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message, "");
                }
            }
            /*
            private static void SendCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.
                    Socket handler = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.
                    int bytesSent = handler.EndSend(ar);
                    Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                    handler.Shutdown(SocketShutdown.Both);
                  //  handler.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }*/
        }
    }
    
}






        /*
        public static int serverPort = 8888;
        private static IPAddress localAddr = IPAddress.Parse("127.0.0.1");
        public static HashSet<ServerThread> watki = new HashSet<ServerThread>();
        private static TcpListener ssock = null;

        static void Main(string[] args)
        {
            ssock = new TcpListener(localAddr, serverPort);
            ssock.Start();
            log("Server uruchomiony");
            for (;;)
            {
                
                TcpClient client = ssock.AcceptTcpClient();
                //log("Polaczono klienta");
                ServerThread thread = new ServerThread(client);
                watki.Add(thread);
                thread.start();
            }
        }

        public static void log(String string_)
        {
            Console.WriteLine(string_);
        }
    }
}
*/
  