using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerWorker
{
    public class Messenger
    {
        public static List<Messenger> messangers = new List<Messenger>();

        public NetworkStream stream;
        StringBuilder builder;
        public TcpClient client;
        public ClientLog clientLog = new ClientLog();
        public ClientSetting setting = new ClientSetting();
        byte[] data;


        public void Process(TcpClient client)
        {
            messangers.Add(this);
            this.client = client;
            stream = null;
            try
            {
                stream = client.GetStream();
                data = new byte[64];
                while (true)
                {
                        builder = new StringBuilder();
                        int bytes = 0;
                        do
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));

                            if (bytes == 0)
                            {
                                Log.Send(client.Client.RemoteEndPoint + " Пришло 0 байт, клиент отключен");
                                messangers.Remove(this);
                                stream.Close();
                                client.Close();
                                return;
                            }

                        }
                        while (stream.DataAvailable);

                        string message = builder.ToString();

                        Log.Send("Получено " + client.Client.RemoteEndPoint + " : " + message);

                        Functions.AnalysisAnswer(message, this);                  
                }
            }
            catch (Exception ex)
            {
                Log.Send(ex.Message);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

        public void RequestLog()
        {
            Log.Send("Запрос логов у клиента " + client.Client.RemoteEndPoint);
            List<string> log = new List<string>();
            byte[] data = Encoding.Unicode.GetBytes("GetLogList");
            stream.Write(data, 0, data.Length);

        }

        public void Update()
        {
            string message = "DownlUpd";
            data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        //Ну я хз
        public void CheckLostClient()
        {
            try
            {
                string message = "TestConnect";
                data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
            catch
            {
                messangers.Remove(this);
            }

                
        }

        public static void UpdateAll()
        {
            List<Messenger> remover = new List<Messenger>();

            foreach (Messenger meseng in messangers)
            {
                try
                {
                    meseng.Update();
                }
                catch
                {
                    remover.Add(meseng);
                }
            }

            foreach (Messenger meseng in remover)
            {
                messangers.Remove(meseng);
            }

        }

        public void StopClientStream()
        {
            stream.Close();
        }
    }
}
