using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLMessage;
using XMLMessage.Models;

namespace TCPServer
{
    public sealed class Cfg
    {
        public Queue<MessageModelRS> listaZadan;// = new Queue<ConnectToRS>(); 
        private string port;
        private static Cfg m_oInstance = null;
        private static readonly object m_oPadLock = new object();
        
        public static Cfg Instance
        {
            get
            {
                lock (m_oPadLock)
                {
                    if (m_oInstance == null)
                    {
                        m_oInstance = new Cfg();
                    }
                    return m_oInstance;
                }
            }
        }
       // public 
        public string Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        private Cfg()
        {
            listaZadan = new Queue<MessageModelRS>();
            port = "";
        }
    }
}
