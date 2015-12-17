using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Klient
{
    public sealed class Config //scaled - uniemozliwia dziedziczenie klasy
    {
        public Socket client;
        public IPAddress ip = IPAddress.Parse("127.0.0.1");
        public int port = 8888;

        private static Config m_oInstance = null;
        private static readonly object m_oPadLock = new object();
        //private int m_nCounter = 0;

        public static Config Instance
        {
            get
            {
                lock (m_oPadLock)
                {
                    if (m_oInstance == null)
                    {
                        m_oInstance = new Config();
                    }
                    return m_oInstance;
                }
            }
        }

        private Config()
        {
        }

    }
}
