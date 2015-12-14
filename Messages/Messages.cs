using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class Messages
    {
        private string id;
        public string Id { get { return id; } }
        public string to;
        public string from;
        public string body;
        public Komendy komenda;
        public Messages(string from, Komendy komenda)
        {
            id = Guid.NewGuid().ToString();
            this.komenda = komenda;
        }
        public Messages(string from, string body, Komendy komenda)
        {
            id = Guid.NewGuid().ToString();
            this.from = from;
            this.body = body;
            this.komenda = komenda;
        }
        public Messages (string from, string to, string body, Komendy komenda)
        {
            id = Guid.NewGuid().ToString();
            this.komenda = komenda;
            this.from = from;
            this.to = to;
            this.body = body;
        }
        public Messages(string id)
        {
            this.id = id;
        }
        public void stringToKomenda(string komendaString)
        {
            switch(komendaString.ToUpper())
            {
                case "LOGIN":
                    komenda = Komendy.Login;
                    break;
                case "LOGOUT":
                    komenda = Komendy.Logout;
                    break;
                case "TEXTMESSAGE":
                    komenda = Komendy.TextMessage;
                    break;
                default:
                    komenda = Komendy.Help;
                    break;
            }
        }
    }
}
