using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    class Messages
    {
        private string id;
        public String Id { get; }
        public Messages()
        {
            id = Guid.NewGuid().ToString();
        }

    }
}
