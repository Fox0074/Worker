using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker
{
    public static class AvailableLocalIp
    {
        private static List<IPAddress> listIp = new List<IPAddress>();
        private static List<IPAddress> listAviableIp = new List<IPAddress>();

        public static List<IPAddress> ListIp
        {
            get { return listIp; }
            set
            {
                listIp.Clear();
                listIp.AddRange(value);
            }
        }
        public static List<IPAddress> ListAviableIp
        {
            get { return listAviableIp; }
            set
            {
                listAviableIp.Clear();
                listAviableIp.AddRange(value);
            }
        }

        public static void CheckAviableNetworkConnections()
        {
            IPHostEntry host;
            listIp.Clear();
            listAviableIp.Clear();

            try
            {
                string hostName = Dns.GetHostName();
                host = Dns.GetHostEntry(hostName);
               
                foreach (IPAddress ip in host.AddressList)
                {
                    listIp.Add(ip);
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        listAviableIp.Add(ip);
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Send("Ошибка при получении ip адресов :" + ex.Message);
            }
        }
        public static IPAddress GetFirstAviableIp()
        {
            if (ListAviableIp[0] != null)
            {
                return ListAviableIp[0];
            }
            return IPAddress.Parse("127.0.0.1");
        }

    }
}
