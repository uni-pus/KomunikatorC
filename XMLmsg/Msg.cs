using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace XMLmsg
{
    //lista komend
    public enum Command
    {
        Login,          // logowanie Usera do serwerów
        LoginServer,    // Zalogowanie Serwera FLS do RS
        Logout,         // Wylogowanie Usera z serwerów
        Message,        // Wysłanie wiadomości przez Usera do innego Usera
        MessageTransfer,// przesłanie Msg między serwerami
        Null
    }

    /// <summary>
    /// Klasa do konwersji wiadomosci do postaci BYTE[]
    /// </summary>
    public class MsgConverter
    {
        // metoda pomocnicza przycinająca tablice byte[] do rozmiarów "realnej" wiadomości
        private byte[] SubArray(byte[] data, int index, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        /// <summary>
        /// metoda konwersji z danych typu byte[] do obiektu Msg (Wiadomosci XML)
        /// </summary>
        /// <param name="dane"></param>
        /// <returns></returns>
        public Msg doMsg(byte[] dane)
        {
            // Jeżeli tablica byte[] jest pusta rzuć wyjątkiem
            if (dane == null || dane.Length == 0)
            {
                throw new InvalidOperationException();
            }
            // pomocniczny pusty byte (potrzebny do znalezienia końca faktycznej wiadomosci)
            byte[] bytePusty = new byte[1];
            // dlugość faktycznej wiadomości
            int koniecDanych = Array.IndexOf(dane, bytePusty[0]);
            if (koniecDanych == 0)
                throw new InvalidOperationException();

            // skopiowanie faktycznej wiadomości do pomocniczej zmiennej
            byte[] daneTemp;
            if (koniecDanych > 0)
                daneTemp = SubArray(dane, 0, koniecDanych);
            else
                daneTemp = dane;

            // serializacja (Deserializacja), tablicy bytów do Obiektu
            XmlSerializer serializer = new XmlSerializer(typeof(Msg));

            using (MemoryStream memoryStream = new MemoryStream(daneTemp))
            {
                using (XmlReader xmlReader = XmlReader.Create(memoryStream))
                {
                    return (Msg)serializer.Deserialize(xmlReader);
                }
            }
        }
        /// <summary>
        /// metoda konwersji z danych typu Obiekt Msg do XMLa (tablicy byte[])
        /// </summary>
        /// <param name="dane"></param>
        /// <returns></returns>
        public byte[] doByte(Msg dane)
        {
            if (dane == null)
            {
                throw new ArgumentNullException();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Msg));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream))
                {
                    serializer.Serialize(xmlWriter, dane);

                    return memoryStream.ToArray();
                }
            }
        }
    }

    /// <summary>
    /// Klasa Msg w postaci umożliwiającej serializację do XMLa
    /// </summary>
    [XmlRoot("Msg")]
    public class Msg
    {
        string id = Guid.NewGuid().ToString();
        string idSerwera;
        string nadawca;
        string wiadomosc;
        Command komenda;
        string odbiorca;
        bool delivered = false;
        DateTime czas;
        string guidNadawca;
        int transferredCount = 0; // przy przepychaniu wiadomości do innych FLSów, wartość będzie zwiększana 


        //XMLElement - nazwy Nodów skrócone do 3 liter - to wszystko przechodzi przez neta - szkoda tracić przepustowość łącza
        [XmlElement("Nad")]
        public string Nadawca
        {
            get { return nadawca; }
            set { nadawca = value; }
        }

        [XmlElement("Wia")]
        public string Wiadomosc
        {
            get { return wiadomosc; }
            set { wiadomosc = value; }
        }

        [XmlElement("Odb")]
        public string Odbiorca
        {
            get { return odbiorca; }
            set { odbiorca = value; }
        }

        [XmlElement("Kom")]
        public string Komenda
        {
            get { return komenda.ToString(); }
            set { komenda = (Command)Enum.Parse(typeof(Command), value); }
        }

        [XmlElement("Del")]
        public string Delivered
        {
            get { return delivered.ToString(); }
            set { delivered = bool.Parse(value); }
        }

        [XmlElement("Id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [XmlElement("Cza")]
        public string Czas
        {
            get { return czas.ToString(); }
            set { czas = DateTime.Parse(value); }
        }

        [XmlElement("IdS")]
        public string IdSerwera
        {
            get { return idSerwera; }
            set { idSerwera = value; }
        }

        [XmlElement("GuN")]
        public string GuidNadawca
        {
            get { return guidNadawca; }
            set { guidNadawca = value; }
        }

        [XmlElement("TrC")]
        public int TransferredCount
        {
            get { return transferredCount; }
            set { transferredCount = value; }
        }
    }
}
