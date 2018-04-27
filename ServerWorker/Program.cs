using ServerWorker.Crypting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerWorker
{
    static class Program
    {
        static public AuthInProgram authSystem = new AuthInProgram();
        static public ServerNet server;
        static public Thread serverThread;
        static public Form1 form1;
        static public List<string> listDdns = new List<string> { "fokes1.asuscomm.com" };
        static public AviableNetServers aviableServer;



        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AsymmetricalEncrypt encrypt = new AsymmetricalEncrypt();
            AsymmetricalDecrypt decrypt = new AsymmetricalDecrypt(encrypt.RsaEncryptParametrs);

            LockForm lockForm = new LockForm();
            Application.Run(lockForm);

            server = new ServerNet(7777);
            aviableServer = new AviableNetServers();
            server.Start();

            form1 = new Form1();
            //StartServer();

            Application.Run(form1);
        }

        static void StartServer()
        {
            IPAddress[] ipAddress = Dns.GetHostAddresses(listDdns[0]);

            string externalip = new WebClient().DownloadString("http://icanhazip.com");
            IPAddress myIp = IPAddress.Parse(externalip.Replace("\n",""));
            foreach (IPAddress ip in ipAddress)
            {
                if (ip.ToString() == myIp.ToString())
                {
                    //TODO: раскоментить
                    //serverThread = new Thread(new ThreadStart(server.StartServer));
                    //serverThread.Start();
                    return;
                }
            }

            serverThread = new Thread(new ThreadStart(aviableServer.Start));
            serverThread.Start();
        }
    }
}
