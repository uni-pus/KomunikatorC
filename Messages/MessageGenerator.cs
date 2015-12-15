using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MessagesSpace
{
    /// <summary>
    /// klasa odpowiedzialna za generowanie wiadomosci i ich dekodowanie
    /// bez konstruktora, zbior metod statycznych
    /// </summary>
    public class MessageGenerator
    {
        static public Messages dekoduj(byte[] data)
        {
            string xmlmessage;

            XmlDocument doc = new XmlDocument();
            xmlmessage = Encoding.UTF8.GetString(data);
            xmlmessage = xmlmessage.Trim();
            doc.LoadXml(xmlmessage);
            string id = doc.DocumentElement.SelectSingleNode("/message/id").InnerText;
            Messages mes = new Messages(id);
            mes.body = doc.DocumentElement.SelectSingleNode("/message/body").InnerText;
            mes.to = doc.DocumentElement.SelectSingleNode("/message/to").InnerText;
            mes.from = doc.DocumentElement.SelectSingleNode("/message/from").InnerText;
            mes.body = doc.DocumentElement.SelectSingleNode("/message/body").InnerText;
            mes.stringToKomenda(doc.DocumentElement.SelectSingleNode("/message/komenda").InnerText);
            mes.Czas = doc.DocumentElement.SelectSingleNode("/message/czas").InnerText;
            return mes;
        }

        static public byte[] koduj(Messages mes)
        {
            //http://www.devx.com/tips/Tip/21168
            XmlDocument XmlDoc = new XmlDocument();

            XmlNode rootNode = XmlDoc.CreateElement("message");
            XmlDoc.AppendChild(rootNode);

            XmlNode parendNode = XmlDoc.CreateElement("from");
            parendNode.InnerText = mes.from;
            rootNode.AppendChild(parendNode);

            parendNode = XmlDoc.CreateElement("to");
            parendNode.InnerText = mes.to;
            rootNode.AppendChild(parendNode);

            parendNode = XmlDoc.CreateElement("id");
            parendNode.InnerText = mes.Id;
            rootNode.AppendChild(parendNode);

            parendNode = XmlDoc.CreateElement("body");
            parendNode.InnerText = mes.body;
            rootNode.AppendChild(parendNode);

            parendNode = XmlDoc.CreateElement("komenda");
            parendNode.InnerText = mes.komenda.ToString();
            rootNode.AppendChild(parendNode);

            parendNode = XmlDoc.CreateElement("czas");
            parendNode.InnerText = mes.Czas;
            rootNode.AppendChild(parendNode);

            byte[] bajty = Encoding.Default.GetBytes(XmlDoc.OuterXml);
            return bajty;
        }
    }
}
