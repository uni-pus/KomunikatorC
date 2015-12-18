using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLMessage.Models
{
    [Serializable]
    public class MessageModelRS
    {
        public string Id { get; set; }// = Guid.NewGuid().ToString(); 
        public string ClientName { get; set; }
        public string ClientPass { get; set; }
        public Command SenderCommand { get; set; }
        public string OtherData { get; set; }
        //public string Reciever { get; set; }
        public DateTime Time { get; set; }
        public void timeStamp()
        {
            Time = DateTime.Now;
        }
        public MessageModelRS()
        {
            Id = Guid.NewGuid().ToString();
            timeStamp();
        }
    }
}
