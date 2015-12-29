using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XMLmsg;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // stworzenie Socketu dla klienta
        TcpClient clientSocket = new TcpClient();
        // stworzenie NetworkStream (Strumień danych przechodzących przez TcpClienta
        NetworkStream serverStream = default(NetworkStream);
        // zmienna string, która przetrzymuje stringi wyświetlane na ekranie klienta
        string readData = null;
        // wątek, ktory będzie trzymał wiadomości wysyłane przez serwer do klienta
        Thread ctThread;
        // obiekt z metodami konwersji danych objekt(wiadomość) -> XML -> byte[] -> XML -> obiekt(wiadomość)
        MsgConverter mConv = new MsgConverter();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                clientSocket.Connect(ip, 8888);
                serverStream = clientSocket.GetStream();
                readData = "Łączenie z serwerem ...";
                // message() czyli "wypisanie na ekran" stringa readData - metoda później uruchamiana spełnia to samo założenie
                message();
            }
            catch (Exception)
            {
                readData = "BŁĄD w połączeniu z serwerem ...";
                message();
                return;
            }
            // tworzenie obiektu Msg (wiadomości) do wysłania przez Socketa
            Msg outMsg = new Msg()
            {
                Komenda = Command.Login.ToString(),
                Nadawca = tBoxNickName.Text,
                Odbiorca = tBoxPassword.Text
            };
            // zamiana obiektu do XMLa a później do tablicy byte[]
            byte[] outStream = mConv.doByte(outMsg);
            // wysłanie outStreama w kosmos (powinien złapać dane serwer)
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();

            // wystartowanie metody getMessage (zbiera odpowiedzi od serwera) w nowym wątku,
            ctThread = new Thread(getMessage);
            // wystartkowanie nowego wątku
            ctThread.Start();
        }

       /// <summary>
       /// metoda działająca w nowym wątku - żeby nie blokowała reszty aplikacji
       /// za zadanie ma odbierać wiadomości od serwera (dlatego działa w pętli while(true)
       /// </summary>
        private void getMessage()
        {
            Msg msg;
            while (true)
            {
                serverStream = clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[10025];
                buffSize = clientSocket.ReceiveBufferSize;
                serverStream.Read(inStream, 0, buffSize);
                msg = mConv.doMsg(inStream);
                // przekazanie wiadomości od serwera do nowej metody - żeby nie zaśmiecać kodem metody getMessage 
                processingNewMessage(msg);
            }
        }

        private void processingNewMessage(Msg msg)
        {
            string login = Command.Login.ToString();
            if(Command.Login.ToString()==msg.Komenda)
            {
                if (msg.Wiadomosc == "LoginOK")
                {
                    readData = "poprawne logowanie do serwera";
                }
                else
                {
                    readData = "błędne logowanie do serwera, zły login lub hasło";
                }
            }
            else if(Command.Message.ToString()==msg.Komenda)
            {
                readData = msg.Nadawca + ": " + msg.Wiadomosc;
            }
            else
            {
                readData = "nieobsługiwana komenda z serwera";
            }
            

            message();
        }

        /// <summary>
        /// wypisanie stringa "readData" do okna aplikacji 
        /// </summary>
        private void message()
        {
            // sprawdenie czy nie jest WPF zablokowany i można wykonać operacje
            if (!CheckAccess())
            {
                Dispatcher.Invoke(() => message());
                return;
            }

            tBlockChat.Text = " >> " + readData + Environment.NewLine + tBlockChat.Text  ;
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            Msg msg = new Msg()
            {
                Komenda = Command.Message.ToString(),
                Nadawca = tBoxNickName.Text,
                Wiadomosc = tBoxMessage.Text,
                Odbiorca = tBoxOdbiorca.Text
            };

            byte[] outStream = mConv.doByte(msg);
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }

        /// <summary>
        /// operacye wykonywane podczas zamknięcia okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            if (MessageBox.Show("Czy na pewno chcesz wyjść z aplikacji?", "Zamknij Chat",
                MessageBoxButton.YesNo) == MessageBoxResult.No)
            {

                e.Cancel = true;
                return;
            }
            if(ctThread != null)
                ctThread.Abort();
            if(serverStream !=null)
                serverStream.Close();
            if(clientSocket!=null)
                clientSocket.Close();

        }



    }
}
