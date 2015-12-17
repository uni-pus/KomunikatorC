using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Klient
{
    public sealed class Config //scaled - uniemozliwia dziedziczenie klasy
    {
        public Socket client;
        public IPAddress ip = IPAddress.Parse("127.0.0.1");
        public int port = 8888;
        public string nickName = "";

        private static Config m_oInstance = null;
        private static readonly object m_oPadLock = new object();
        
        public void GetServerAdress()
        {
            XmlDocument xm = new XmlDocument();
            xm.Load(@"http://dlugiego.net/pus/servers.php");
            //xm = xm.DocumentElement.InnerXml;
            XmlNodeList adress = xm.GetElementsByTagName("server");
            ip = IPAddress.Parse(adress[0].ChildNodes[0].InnerText);
            port = Convert.ToInt32(adress[0].ChildNodes[1].InnerText);
        }

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
