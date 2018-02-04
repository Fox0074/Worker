using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ClientWorker
{
    public class Client
    {
        int port = 7778;
        string address = "fokes1.asuscomm.com";
        TcpClient client;
        NetworkStream stream;
        StringBuilder builder;
        public Functions handler;

        public void Clear()
        {
            client.Close();
            stream.Close();
            client = client = new TcpClient(address, port);
            stream = client.GetStream();
        }


        public void Start()
        {
            handler = new Functions();
            handler.Start();
            handler.Registration();

            client = null;
            try
            {
                address = GetFirstSucsessAdress();
                StartData.currentUser = address;

                client = new TcpClient(address, port);
                stream = client.GetStream();

                string message = "FirstConnect";
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                while (true)
                {
                    data = new byte[64];
                    builder = new StringBuilder();
                    int bytes = 0;

                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    message = builder.ToString();
                    Console.WriteLine("Сервер: {0}", message);
                    handler.AnalysisAnswer(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                handler.Reconnect();
                return;
            }

            finally
            {
                try
                {
                    //client.Close();
                    Console.WriteLine("Tcp connected close");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Tcp connected cant close" +  ex.Message);
                }
            }
        }


        private IPAddress[] GetIpDns(string ddns)
        {
            IPAddress[] HostIp = Dns.GetHostAddresses(ddns);

            return HostIp;
        }
        private IPStatus PingIp(string hostName)
        {
            Ping pinger = new Ping();
            PingReply reply = pinger.Send(hostName);

            return reply.Status;
        }
        private string GetFirstSucsessAdress()
        {
            string sucsess = "";

            foreach (string ddns in StartData.ddnsHostName)
            {
                if (PingIp(ddns) == IPStatus.Success)
                {
                    return ddns;
                }
            }

            return sucsess;
           
        }

    }
}
