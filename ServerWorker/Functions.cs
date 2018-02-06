using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker
{
    public static class Functions
    {
        public static void AnalysisAnswer(string answer, NetworkStream stream)
        {
            switch (answer)
            {
                case "GetListUsers":
                    SendListUsers(stream);
                    break;

                default:
                    Log.Send("UnknownCommand " + answer);
                    break;
            }
        }

        private static void SendListUsers(NetworkStream stream)
        {
            try
            {
                stream.ReadTimeout = 1000;
                byte[] data = new byte[64];
                Log.Send("Отправка списка пользователей клиенту ");
                foreach (Messenger client in Messenger.messangers)
                {
                    data = Encoding.Unicode.GetBytes("_Name" + "Fox" + "_Ip" + client.client.Client.RemoteEndPoint.ToString() + "|&");
                    stream.Write(data, 0, data.Length);
                }

                data = Encoding.Unicode.GetBytes("\n");
                stream.Write(data, 0, data.Length);
                Log.Send("Отправлено клиентов : " + Messenger.messangers.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Log.Send(ex.Message);
            }

        }

    }   
}
