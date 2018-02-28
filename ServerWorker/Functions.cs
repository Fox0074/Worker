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
        public static Action onGettingLog = delegate { };
        public static Action OnGettingInfoDevice = delegate { };

        public static void AnalysisAnswer(string answer, Messenger client)
        {
            string head;
            List<string> parametrs = new List<string>();
            Console.WriteLine(answer);
            try
            {
                head = answer.Split('_')[0];
                parametrs.AddRange(answer.Split('_'));
                parametrs.Remove(head);
                parametrs.RemoveAt(parametrs.Count - 1);
            }
            catch
            {
                head = answer;
                Log.Send("Старая команда" );
            }

            switch (head)
            {
                case "StartInfoDevice":
                    client.setting.infoDevice = parametrs;
                    OnGettingInfoDevice.Invoke();
                    break;

                case "StartSetting":
                    try
                    {
                        client.setting.name = parametrs[0];
                        //client.setting.name = parametrs[1];
                        client.setting.openCount = int.Parse(parametrs[2]);
                        client.setting.startTime = parametrs[3];
                        client.setting.Version = parametrs[4];
                    }
                    catch (Exception ex)
                    {
                        Log.Send("Ошибка получения настроек " + ex.Message);
                    }
                    break;
                case "StartLog":
                    client.clientLog.messages = parametrs;
                    onGettingLog.Invoke();
                    break;
                case "GetListUsers":
                    SendListUsers(client.stream);
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
