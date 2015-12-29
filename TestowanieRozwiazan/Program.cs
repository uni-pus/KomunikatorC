using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLmsg;

namespace TestowanieRozwiazan
{
    class Program
    {
        /// <summary>
        /// Klasa testująca poprawność biblioteki klas XMLmsg
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // stworzenie list pomocnicznych trzymających wartości własności obiektów klasy Msg w postaci stringa
            List<string> nwLista = new List<string>();
            List<string> nnwLista = new List<string>();
            // stworzenie przykładowego obiektu Msg
            Msg nw = new Msg()
            {
                Komenda = Command.Login.ToString(),
                Nadawca = "J",
                Odbiorca = "T",
                Wiadomosc = "PIWO"
            };
            // wypisanie pierwszego obiektu
            Console.WriteLine("Jaka komenda: {0}, kto wysyła: {1}, kto odbiera: {2}, jaka tresc: {3}", nw.Komenda,nw.Nadawca,nw.Odbiorca,nw.Wiadomosc);
            MsgConverter mConv = new MsgConverter();
            // konwersja pierwszego obiektu do bytów
            byte[] bajty = mConv.doByte(nw);
            // odkodowanie pierwszego obiektu i stworzenie identycznego drugiego
            Msg nnw = mConv.doMsg(bajty);
            //wypisanie drugiego obiektu
            Console.WriteLine("Jaka komenda: {0}, kto wysyła: {1}, kto odbiera: {2}, jaka tresc: {3}", nnw.Komenda, nnw.Nadawca, nnw.Odbiorca, nnw.Wiadomosc);

            var s = nw;
            // w foreachu wpisanie jako stringi wartości wszystkich własności obiektu pierwszego
            string temp;
            foreach (var p in s.GetType().GetProperties().Where(p => p.GetGetMethod().GetParameters().Count() == 0))
            {
                temp = p.GetValue(s, null) != null ? p.GetValue(s, null).ToString() : "null";
                nwLista.Add(temp);
                //Console.WriteLine(p.GetValue(s, null));
            }
            // w foreachu wpisanie jako stringi wartości wszystkich własności obiektu drugiego
            s = nnw;
            foreach (var p in s.GetType().GetProperties().Where(p => p.GetGetMethod().GetParameters().Count() == 0))
            {
                temp = p.GetValue(s, null) != null ? p.GetValue(s, null).ToString() : "null";
                nnwLista.Add(temp);
            }


            // porownanie obydwu list, jeżeli wszystkie wartości są identyczne oba obiekty są identyczne
            string str = czyIdentyczne(nwLista,nnwLista) ? "TEST POPRAWNY - OBIE KLASY CHYBA SĄ IDENTYCZNE!" : "TEST BŁĘDNY!";
            Console.WriteLine("{0}", str);
            Console.ReadKey(true);
            
        }

        public static bool czyIdentyczne(List<string> l1, List<string> l2)
        {
            if (l1.Count != l2.Count)
                return false;
            for (int i = 0; i < l2.Count; i++)
                if (l1[i] != l2[i])
                    return false;
            return true;
        }
    }
}
