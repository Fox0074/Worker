using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker
{
    public class Messenger
    {
        public static List<Messenger> messangers = new List<Messenger>();

        NetworkStream stream;
        StringBuilder builder;
        public TcpClient client;
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
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();

                    Log.Send("Получено " + client.Client.RemoteEndPoint + " : "+ message);

                    message = "Сообщение " + message + " доставлено";
                    data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    Log.Send("Отправлено " + client.Client.RemoteEndPoint + " : " + message);
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

        public void Update()
        {
            string message = "DownloadUpdater";
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
