using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KlasaDoTestow;
using MessagesSpace;
using System.Timers;
using System.Threading;

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

            MessagesSpace.Messages nowaWiadomosc = new MessagesSpace.Messages("Ja", "Ty", "Czesc", Komendy.TextMessage);
            nowaWiadomosc.stempelCzasu();
            //Messages.MessageGenerator generator = new Messages.MessageGenerator();
            byte[] testByte = MessageGenerator.koduj(nowaWiadomosc);
            MessagesSpace.Messages staraWiadomosc = MessageGenerator.dekoduj(testByte);

            if (porownanieWiadomosci(nowaWiadomosc,staraWiadomosc))
            {
                Console.WriteLine("OK");
            }
            else
                Console.WriteLine("BLAD");
            Thread.Sleep(2);
            
            

        }

        static private bool porownanieWiadomosci(MessagesSpace.Messages nowa, MessagesSpace.Messages stara)
        {
            if (nowa.body != stara.body)
                return false;
            if (nowa.Czas != stara.Czas)
                return false;
            if (nowa.from != stara.from)
                return false;
            if (nowa.Id != stara.Id)
                return false;
            if (nowa.komenda != stara.komenda)
                return false;
            if (nowa.to != stara.to)
                return false;
            

            return true;
        }
    }
}
