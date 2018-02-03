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
                    ServerNet clientObject = new ServerNet(client);

                    // создаем новый поток для обслуживания нового клиента
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
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

        public void Process()
        {
            NetworkStream stream = null;
            try
            {
                stream = client.GetStream();
                byte[] data = new byte[64];
                while (true)
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();

                    Console.WriteLine(message);

                    message = "Сообщение " + message + " доставлено";
                    data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
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
