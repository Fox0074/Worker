using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ServerWorker
{
    public class ServerNet
    {

        //Сервер
        const int port = 7778;
        static TcpListener listener;
        static IPAddress localIp = IPAddress.Parse("192.168.1.10");

        //Клиент
        public TcpClient client;
        #region Конструкторы
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="tcpClient"></param>
        public ServerNet(TcpClient tcpClient)
        {
            client = tcpClient;
        }
        public ServerNet()
        {
        }
        #endregion

        public void StartServer()
        {
            try
            {
                localIp = GetLocalIp();

                listener = new TcpListener(localIp, port);
                listener.Start();

                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Messenger clientObject = new Messenger();
                    // создаем новый поток для обслуживания нового клиента
                    Thread clientThread = new Thread(new ThreadStart(() => clientObject.Process(client)));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }

        private IPAddress GetLocalIp()
        {
            IPHostEntry host;
            string hostName = Dns.GetHostName();

            host = Dns.GetHostEntry(hostName);
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            return IPAddress.Parse("192.168.1.10");
        }
    }
}
