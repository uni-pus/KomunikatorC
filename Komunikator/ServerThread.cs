using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using MessagesSpace;

namespace Server
{
    class ServerThread : BaseThread
    {
        private TcpClient client;
        public TcpClient Client() { return client; }
        private string nick;

        public ServerThread(TcpClient client)
        {
            this.client = client;
        }

        public override void RunThread()
        {
            throw new NotImplementedException();
        }

        public void start()
        {
            //byte[] bytes = new byte[256];
            //string data = null;
            //http://csharp.net-informations.com/communications/csharp-multi-threaded-server-socket.htm
            //https://msdn.microsoft.com/en-us/library/system.net.sockets.tcplistener%28v=vs.110%29.aspx
            log("Podlaczono nowego klienta");
            NetworkStream ntStream = client.GetStream();
            int i;
            //StringBuilder sb = new StringBuilder();
            Messages data;
            byte[] bytesFrom = new byte[10025];
 //           string dataFromClient = null;
            try {
                while(true)
                {
                    i = ntStream.Read(bytesFrom, 0, bytesFrom.Length);
                    try
                    {
                        data = MessageGenerator.dekoduj(bytesFrom);

                        log(" >> Data from client - ");// + dataFromClient);
                        string[] str = new string[] {data.from, data.komenda.ToString(), data.body };
                        log("{0}, komenda: {1}, body: \"{2}\" ",str);
                        //StreamWriter out_ = new StreamWriter(client.GetStream());
                        //BufferedStream in_ = new BufferedStream(); 
                    }
                    catch (Exception ex)
                    {
                        log(ex.ToString());
                        //throw;
                    }
                }

            } catch (Exception ex)
            {
                log(ex.ToString());
                //throw;
            }

            log("klient rozlaczony");
            //Program.watki.Remove(this);

        }

        public void log(String _string)
        {
            Console.WriteLine(_string);
        }
        public void log(String _string, string[] var)
        {
            Console.WriteLine(_string, var);
        }
    }
}
