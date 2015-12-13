using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Server
{
    class Program
    {
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
