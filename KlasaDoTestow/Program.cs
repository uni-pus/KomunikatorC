using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlasaDoTestow;
using Messages;

namespace KlasaDoTestow
{
    class Program
    {

        static void Main(string[] args)
        {
            /// <summary>
            /// Sprawdzenie poprawnego tworzenia się Messages
            /// </summary>
            /// 

            Messages.Messages nowaWiadomosc = new Messages.Messages("Ja", "Ty", "Czesc", Komendy.TextMessage);
            Messages.MessageGenerator generator = new Messages.MessageGenerator();
            byte[] testByte = generator.koduj(nowaWiadomosc);
            Messages.Messages staraWiadomosc = generator.dekoduj(testByte);



        }
    }
}
