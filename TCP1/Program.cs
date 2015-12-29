using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;
using System.Collections.Generic;
using XMLmsg;

namespace TCPServer
{
    class Program
    {
        //generowanie unikalnego ID serwera
        public static string serverID = Guid.NewGuid().ToString();
        // lista klientów chatu
        public static List<User> clientsList = new List<User>();
        // zmienna do wyłączenia serwera przez komendę od admina (do zrobienia później)
        public static bool wylaczServer = false;
        // obiekt z metodami konwersji danych objekt(wiadomość) -> XML -> byte[] -> XML -> obiekt(wiadomość)
        private static MsgConverter mConv = new MsgConverter();
        /// <summary>
        /// Main ;)
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            IPAddress ip;
            // adres na którym serwer będzie nasłuchiwał (zawsze 127.0.0.1), tzn będzie zbierał cały ruch 
            ip = IPAddress.Parse("127.0.0.1");
            // uruchomienie serwera nasłuchu TCP
            // Pakiet TCP wchodzi do baru:
            // -Poproszę piwo.
            // -Piwo ?
            // -Tak, piwo.
            TcpListener serverSocket = new TcpListener(ip, 8888);
            TcpClient clientSocket = default(TcpClient);
            User nowyUser;
            int counter = 0;
            // start serwera
            serverSocket.Start();
            Console.WriteLine("Chat Server Started ....");
            counter = 0;
            // pętla przyjmująca nowych klientów
            while ((true))
            {
                counter += 1;
                // do TcpListener'a zgłasza się nowy TcpClient
                clientSocket = serverSocket.AcceptTcpClient();
                // tablica bajtów z wiadomoscią
                byte[] bytesFrom = new byte[10025];
                // stworzenie streamu od klienta
                NetworkStream networkStream = clientSocket.GetStream();
                // odczytanie wiadomości (czytanie do bytesFrom, streamu o długości (int)clientSocket.ReceiveBufferSize )
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                // konwersja bajtów do obiektu z wiadomością
                Msg msg = mConv.doMsg(bytesFrom);
                // stworzenie obiektu Usera trzymającego nickName oraz Socketa
                nowyUser = new User()
                {
                    nickName = msg.Nadawca,
                    clientSocket = clientSocket
                };
                // przypisanie GUID nowegoUsera do wiadomosci
                msg.GuidNadawca = nowyUser.GUID;
                // dodanie Usera do Listy userów aktywnych na tym serwerze
                clientsList.Add(nowyUser);
                Console.WriteLine(msg.Nadawca + " Try to login to server ");
                processingNewMessage(msg);
                // utworzenie obiektu trzymającego komunikację Usera z serwerem
                handleClinet client = new handleClinet();
                // uruchomienie clienta (wysłanie Socketa, danych, (adresu) Listy pozostałych klientów
                client.startClient(clientSocket, msg.Nadawca, msg.GuidNadawca, clientsList);
                if (wylaczServer == true)
                    break;
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        } // end Main metod

        public static void processingNewMessage(Msg msg)
        {
            if (msg.Komenda.ToString()==Command.Login.ToString())
            {
                // wysłać msg do RSa w celu weryfikacji
                bool czyLogowaniePoprawne = true;
                if(czyLogowaniePoprawne) //jeżeli dane logowania poprawne
                {
                    // znalezienie Usera w celu zmiany wartości "Zalogowany" 
                    User tempUser = clientsList.Find(r => r.GUID == msg.GuidNadawca);
                    tempUser.zalogowany = true;
                    // wysłanie wiadomości do klienta o zalogowaniu
                    msg.Wiadomosc = "LoginOK";
                    broadcast(msg);//, msg.Nadawca);
                    Console.WriteLine(msg.Nadawca + " is login to server");
                } 
                else
                {
                    msg.Wiadomosc = "Bledny LOGIN/HASLO";
                    broadcast(msg);//, msg.Nadawca);
                    Console.WriteLine(msg.Nadawca + " is not login to server");
                    // obsługa błędnego logowania, 
                    // wysyłka do klienta info o błędnym logowaniu
                    // odłączenie klienta od serwera
                }
            }
            else if(msg.Komenda.ToString()==Command.Message.ToString())
            {
                broadcast(msg);
            }
        }

        public static void broadcast(Msg msg)//, string uName)//, bool flag)
        {
            bool WyslaneDoOdbiorcy = false;
            bool WyslaneDoNadawcy = false;
            foreach (User Item in clientsList)
            {
                if (Item.nickName == msg.Odbiorca || Item.GUID == msg.GuidNadawca)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.clientSocket;//.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    Byte[] broadcastBytes = null;

                    if (Item.zalogowany == true)
                    {
                        if (Item.nickName == msg.Odbiorca)
                            WyslaneDoOdbiorcy = true;
                        if (Item.GUID == msg.GuidNadawca)
                            WyslaneDoNadawcy = true;
                        broadcastBytes = mConv.doByte(msg);
                        broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                        broadcastStream.Flush();
                    }
                } //end if (Item.nickName == msg.Odbiorca || Item.nickName == msg.Nadawca)
            } //end foreach

            if(!WyslaneDoNadawcy)
            {
                //czynności związane z niewysłaniem wiadomości do nadawcy (czy jakiekolwiek?)
            }
            if(WyslaneDoOdbiorcy)
            {
                // wysłać wiadomość do nadawcy, że wiadomość została dostarczona do odbiorcy!
            } else
            {
                // wysłać wiadomość do pozostałych serwerów, 
                // jeżeli na żadnym nie została doręczona wiadomość wysłać do serwera BackUpu
            }
        }  //end broadcast function
    }//end Main class


    public class handleClinet
    {
        TcpClient clientSocket;
        string clNo;
        List<User> clientsList;
        MsgConverter mConv = new MsgConverter();
        string clGuid;

        public void startClient(TcpClient inClientSocket, string clineNo, string clineGuid, List<User> cList)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            this.clGuid = clineGuid;
            this.clientsList = cList;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }

        private void doChat()
        {
            int requestCount = 0;
            byte[] bytesFrom = new byte[10025];
            string rCount = null;
            requestCount = 0;
            Msg msg;
            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    // wyzerowanie tablicy byte[]
                    bytesFrom = new byte[10025];
                    NetworkStream networkStream;
                    try
                    {
                        networkStream = clientSocket.GetStream();
                        networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                        
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    
                    msg = mConv.doMsg(bytesFrom);
                    // przypisanie do wiadomości ID serwera który ją odebrał od clienta
                    msg.IdSerwera = Program.serverID;
                    // przypisanie do wiadomości czasu z serwera
                    msg.Czas = DateTime.Now.ToString();
                    msg.GuidNadawca = clGuid;
                    Console.WriteLine("From client - " + clNo + " to client - " + msg.Odbiorca + " msg: " + msg.Wiadomosc);
                    rCount = Convert.ToString(requestCount);

                    Program.processingNewMessage(msg);
                    //Program.broadcast(msg);//, clNo);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.ToString());
                    break;
                }
            }//end while
            clientSocket.Close();
            clientsList.Remove(clientsList.Find(r => r.nickName == clNo));
            Console.WriteLine("Rozłączono klienta");
        }//end doChat
    } //end class handleClinet
}//end namespace
