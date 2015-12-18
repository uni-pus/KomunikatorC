using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLMessage.Models
{
    [Serializable]
    public class MessageModel
    {
        public string Id { get; set; }// = Guid.NewGuid().ToString(); 
        public string SenderName { get; set; }
        public string SenderMessage { get; set; }
        public Command SenderCommand { get; set; }
        public string Reciever { get; set; }
        public DateTime Time { get; set; }
        public void timeStamp()
        {
            Time = DateTime.Now;
        }
        public MessageModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
