using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerWorker
{
    static class Program
    {
        static public ServerNet server;
        static public Thread serverThread;
        static public Form1 form1;
        static public List<string> listDdns = new List<string> { "fokes1.asuscomm.com" };
        static public AviableNetServers aviableServer;

        [STAThread]
        static void Main()
        {
            server = new ServerNet();
            aviableServer = new AviableNetServers();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form1 = new Form1();
            StartServer();

            Application.Run(form1);
        }

        static void StartServer()
        {
            string myHostName = Dns.GetHostName();
            foreach (string serverDns in listDdns)
            {
                if (serverDns == myHostName)
                {
                    serverThread = new Thread(new ThreadStart(server.StartServer));
                    serverThread.Start();
                    break;
                }
            }

            serverThread = new Thread(new ThreadStart(aviableServer.Start));
            serverThread.Start();
        }
    }
}
