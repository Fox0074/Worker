using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Reflection;

namespace ServerWorker
{
    public class ServerNet : ServerBehaviour
    {
        public static Action AddClient = delegate { };

        //Сервер
        const int port = 7777;
        public static TcpListener listener;
        //public static IPAddress localIp = IPAddress.Parse("127.0.0.1");
        public static IPAddress localIp = null;
        public List<EndPoint> listIp = new List<EndPoint>();      

        //Клиент
        public TcpClient client;
        Thread clientThread;

        #region Конструкторы

        public ServerNet(TcpClient tcpClient)
        {
            client = tcpClient;
        }
        public ServerNet() { }
        #endregion

        public override void Start()
        {
            AvailableLocalIp.CheckAviableNetworkConnections();
        }

        #region Server
        //Запуск сервера
        public void StartServer()
        {
            try
            {
                if (localIp == null)
                {
                    localIp = AvailableLocalIp.GetFirstAviableIp();
                }

                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

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
                    Log.Send("Подключился клиент : " + client.Client.RemoteEndPoint);
                    listIp.Add(client.Client.RemoteEndPoint);                    
                }
            }
            catch (Exception ex)
            {
                Log.Send("ServerNet.StartServer " + ex.Message);
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
                clientThread.Abort();
            }
            catch (Exception ex)
            {
                Log.Send("Ошибка при остановке сервера " + ex.Message);
            }
        }
        #endregion

        #region AddingFunctions
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
#endregion
    }
}
