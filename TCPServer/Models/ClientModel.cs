using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{

    //The ClientInfo structure holds the required information about every
    //client connected to the server
    class ClientModel
    {
        public Socket socket;   //Socket of the client
        public string strName;  //Name by which the user logged into the chat room
    }
}
