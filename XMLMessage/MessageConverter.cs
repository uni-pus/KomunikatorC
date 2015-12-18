using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using XMLMessage.Models;

namespace XMLMessage
{

    //The commands for interaction between the server and the client
    public enum Command
    {
        Login,      //Log into the server
        Logout,     //Logout of the server
        Message,    //Send a text message to all the chat clients
        Null        //No command
    }


    //The basic idea is here conver from binary->xml->model
    // or opposite model->xml->binary 
    // so as long as the client send data in xml format and with proper nodes
    // the client can  be written java ,c++ or another languages 
   
    public class MessageConverter
    {

        //Converts the bytes into an object of type Data
        public MessageModel toMessage(byte[] data)
        {
            string xml = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                xml = Encoding.UTF8.GetString(data);
                xml = xml.Trim().Substring(0, xml.IndexOf(@"</message>")+10);
                //xml = Regex.Replace(xml, @"\0", "");
                doc.LoadXml(xml);


                MessageModel obj = new MessageModel()
                {
                    Id = doc.DocumentElement.SelectSingleNode("/message/id").InnerText,
                    Time = DateTime.Parse(doc.DocumentElement.SelectSingleNode("/message/time").InnerText),
                    SenderName = doc.DocumentElement.SelectSingleNode("/message/from").InnerText,
                    SenderMessage = doc.DocumentElement.SelectSingleNode("/message/body").InnerText,
                    Reciever = doc.DocumentElement.SelectSingleNode("/message/to").InnerText,
                    SenderCommand = (Command)Enum.Parse(typeof(Command), doc.DocumentElement.SelectSingleNode("/message/command").InnerText)
                };
                return obj;

            }
            catch (XmlException ex) { 
                
                Console.Write(ex.Message);
                
                
                throw ex; }

        }

        //Converts the Data structure into an array of bytes
        public byte[] ToByte(MessageModel model)
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

            userNode = xmlDoc.CreateElement("from");
            userNode.InnerText = model.SenderName;
            rootNode.AppendChild(userNode);


            userNode = xmlDoc.CreateElement("to");
            userNode.InnerText = model.Reciever;
            rootNode.AppendChild(userNode);


            userNode = xmlDoc.CreateElement("body");
            userNode.InnerText = model.SenderMessage;
            rootNode.AppendChild(userNode);

            userNode = xmlDoc.CreateElement("command");
            userNode.InnerText = model.SenderCommand.ToString();
            rootNode.AppendChild(userNode);

            byte[] bytes = Encoding.Default.GetBytes(xmlDoc.OuterXml);
            return bytes;
        }

    }




}
