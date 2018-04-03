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

        //====================================>> command: HEAD_PARAMETR0_PARAMETR1_PARAMETR2_ ...
        public static void AnalysisAnswer(string answer, Messenger client)
        {
            string head;
            List<string> parametrs = new List<string>();
            
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
                        client.setting.countStartProgram = int.Parse(parametrs[2]);
                        client.setting.startTime = parametrs[3];
                        client.setting.version = parametrs[4];
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
                    SendListUsers(client);
                    break;

                case "Key":
                    //client.EndReadClient();
                    //ConnectedServers connectedServers = new ConnectedServers(client.authStream, client.client);
                    break;

                default:
                    Log.Send("UnknownCommand " + answer);
                    break;
            }
        }

        private static void SendListUsers(Messenger client)
        {
            try
            {
                string message = "";
                Log.Send("Отправка списка пользователей клиенту ");
                foreach (Messenger user in Messenger.messangers)
                {
                    message += ("_Name" + "Fox" + "_Ip" + user.client.Client.RemoteEndPoint.ToString() + "|&");
                }

                client.SendMessage(message);
                Log.Send("Отправлено клиентов : " + Messenger.messangers.Count);
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                Log.Send(ex.Message);
            }

        }

    }   
}
