﻿using System;
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

        NetworkStream stream;
        StringBuilder builder;
        public TcpClient client;
        byte[] data;

        bool gettingLog = false;

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


                    if (gettingLog)
                    {
                        Log.Send( message );
                        if (message == "EndLog")
                        {
                            gettingLog = false;
                        }
                    }
                    else
                    {
                        Log.Send("Получено " + client.Client.RemoteEndPoint + " : " + message);

                        Functions.AnalysisAnswer(message, stream);
                    }                      
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
            string message = "";

            byte[] data = Encoding.Unicode.GetBytes("GetLogList");
            stream.Write(data, 0, data.Length);

            gettingLog = true;
            //while (message != "EndLog")
            //{
            //    builder = new StringBuilder();
            //    int bytes = 0;
            //    do
            //    {
            //        bytes = stream.Read(data, 0, data.Length);
            //        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));

            //        if (bytes == 0)
            //        {
            //            Log.Send(client.Client.RemoteEndPoint + " Пришло 0 байт, клиент отключен");
            //            messangers.Remove(this);
            //            stream.Close();
            //            client.Close();
            //            Log.Send("Ошибка, клиент был отключен");
            //            return log;
            //        }
            //    } while (stream.DataAvailable);

            //    message = builder.ToString();
            //    log.Add(message);
            //}
            //gettingLog = false;
            Log.Send("Логи получены");          
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
