using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class User
    {
        public string nickName;
        public TcpClient clientSocket;
        public string grupaName = String.Empty;
        public string GUID = Guid.NewGuid().ToString();
        public bool zalogowany = false;
    }
}
