using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XMLMessage;
using XMLMessage.Models;

namespace TCPServer
{

    class ConnectToRS
    {
        public Socket clientSocket;
        private byte[] byteData = new byte[1024];
        MessageConverterRS msgConverterObj = new MessageConverterRS();
        MessageModelRS msgModel;

        public void ConnecToServer(MessageModelRS mes)
        {
            msgModel = mes;
            try
            {

                string[] lines = { @"127.0.0.1", "8889"};

                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPAddress ipAddress = IPAddress.Parse(lines[0]);
                //Server is listening on port 1000
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, int.Parse(lines[1]));


                //Connect to the server
                clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);

                byteData = new byte[1024];
                //Start listening to the data asynchronously
                clientSocket.BeginReceive(byteData,
                                           0,
                                           byteData.Length,
                                           SocketFlags.None,
                                           new AsyncCallback(OnReceive),
                                           null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient");
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            
           // Dispatcher.Invoke((Action)(() =>
            //{
                try
                {
                    clientSocket.EndConnect(ar);

                //We are connected so we login into the server

                byte[] b = msgConverterObj.ToByte(new MessageModelRS()
                {
                    ClientName = msgModel.ClientName,
                    ClientPass = msgModel.ClientPass,
                    Id = msgModel.Id,
                    OtherData = msgModel.OtherData,
                    Time = msgModel.Time,
                    SenderCommand = msgModel.SenderCommand

                });

                    //Send the message to the server
                    clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(OnSend), null);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SGSclient");
                }
           // }));
        }

        private void OnSend(IAsyncResult ar)
        {
    //        Dispatcher.Invoke((Action)(() =>
     //       {
                try
                {
                    clientSocket.EndSend(ar);

      //              strName = userName.Text;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SGSserver");
                }
    //        }));
        }

        private void OnReceive(IAsyncResult ar)
        {
       //     this.Dispatcher.Invoke((Action)(() =>
       //     {
                try
                {
                    clientSocket.EndReceive(ar);

                    MessageModelRS msgReceived = msgConverterObj.toMessage(byteData);


                    if (msgReceived.OtherData == "OK")
                    {
                        //ClientWindow clientForm = new ClientWindow(clientSocket, strName);
                        //this.Hide();
                        //clientForm.ShowDialog();

                        //Close();
                    }

                    byteData = new byte[1024];

                    clientSocket.BeginReceive(byteData,
                                              0,
                                              byteData.Length,
                                              SocketFlags.None,
                                              new AsyncCallback(OnReceive),
                                              null);

                }
                catch (ObjectDisposedException)
                { }
                catch (Exception ex)
                {
                MessageBox.Show(ex.Message, "SGSclientTCP: ");// + strName);
                }
         //   }));
        }

    }
}
