using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RS
{
    class ClientModel
    {
        public Socket socket;   //Socket of the client
        public string strName;  //Name by which the server logged into the RS
        //public string strPass;
    }
}
