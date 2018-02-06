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
        public static Action AddClient = delegate { };

        //Сервер
        const int port = 7777;
        static TcpListener listener;
        static IPAddress localIp = IPAddress.Parse("192.168.1.10");
        public List<EndPoint> listIp = new List<EndPoint>();      

        //Клиент
        public TcpClient client;
        Thread clientThread;

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
        //Запуск сервера
        public void StartServer()
        {
            try
            {
                localIp = GetLocalIp();

                listener = new TcpListener(localIp, port);
                listener.Start();

                Console.WriteLine(localIp);
                Console.WriteLine("Ожидание подключений...");
                Log.Send("Твой LocalIp " + localIp);
                Log.Send("Ожидание подключений...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Messenger clientObject = new Messenger();
                    // создаем новый поток для обслуживания нового клиента
                    clientThread = new Thread(new ThreadStart(() => clientObject.Process(client)));
                    clientThread.Start();

                    AddClient.Invoke();
                    Console.WriteLine("Подключился клиент : " + client.Client.RemoteEndPoint);
                    Log.Send("Подключился клиент : " + client.Client.RemoteEndPoint);
                    listIp.Add(client.Client.RemoteEndPoint);                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ServerNet.StartServer " + ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();               
            }
        }

        //Остановка сервера
        public void StopServer()
        {
            try
            {
                foreach (Messenger sender in Messenger.messangers)
                {
                    sender.StopClientStream();
                }
                Messenger.messangers.Clear();
                listener.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ServerNet.StopServer " + ex.Message);
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
