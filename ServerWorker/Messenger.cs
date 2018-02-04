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
        public static List<Messenger> test = new List<Messenger>();

        NetworkStream stream;
        StringBuilder builder;
        byte[] data;

        public void Process(TcpClient client)
        {
            test.Add(this);
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

        public void Update()
        {
            string message = "DownloadUpdater";
            data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }


        public static void UpdateAll()
        {
            List<Messenger> remover = new List<Messenger>();

            foreach (Messenger meseng in test)
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
                test.Remove(meseng);
            }

        }
    }
}
