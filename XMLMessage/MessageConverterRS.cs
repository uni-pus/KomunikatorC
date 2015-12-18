using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XMLMessage.Models;

namespace XMLMessage
{
    public class MessageConverterRS
    {

        //Converts the bytes into an object of type Data
        public MessageModelRS toMessage(byte[] data)
        {
            string xml = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                xml = Encoding.UTF8.GetString(data);
                xml = xml.Trim().Substring(0, xml.IndexOf(@"</message>") + 10);
                doc.LoadXml(xml);


                MessageModelRS obj = new MessageModelRS()
                {
                    Id = doc.DocumentElement.SelectSingleNode("/message/id").InnerText,
                    Time = DateTime.Parse(doc.DocumentElement.SelectSingleNode("/message/time").InnerText),
                    ClientName = doc.DocumentElement.SelectSingleNode("/message/nickname").InnerText,
                    ClientPass = doc.DocumentElement.SelectSingleNode("/message/pass").InnerText,
                    OtherData = doc.DocumentElement.SelectSingleNode("/message/other").InnerText,
                    SenderCommand = (Command)Enum.Parse(typeof(Command), doc.DocumentElement.SelectSingleNode("/message/command").InnerText)
                };
                return obj;

            }
            catch (XmlException ex)
            {

                Console.Write(ex.Message);


                throw ex;
            }

        }

        //Converts the Data structure into an array of bytes
        public byte[] ToByte(MessageModelRS model)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration dec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDoc.AppendChild(dec);// Create the root element

            XmlNode rootNode = xmlDoc.CreateElement("message");
            xmlDoc.AppendChild(rootNode);

            XmlNode userNode = xmlDoc.CreateElement("id");
            userNode.InnerText = model.Id;
            rootNode.AppendChild(userNode);

            userNode = xmlDoc.CreateElement("time");
            userNode.InnerText = model.Time.ToString();
            rootNode.AppendChild(userNode);

            userNode = xmlDoc.CreateElement("nickname");
            userNode.InnerText = model.ClientName;
            rootNode.AppendChild(userNode);


            userNode = xmlDoc.CreateElement("pass");
            userNode.InnerText = model.ClientPass;
            rootNode.AppendChild(userNode);


            userNode = xmlDoc.CreateElement("other");
            userNode.InnerText = model.OtherData;
            rootNode.AppendChild(userNode);

            userNode = xmlDoc.CreateElement("command");
            userNode.InnerText = model.SenderCommand.ToString();
            rootNode.AppendChild(userNode);

            byte[] bytes = Encoding.Default.GetBytes(xmlDoc.OuterXml);
            return bytes;
        }

    }
}
