using System;
using System.Collections.Generic;
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

        [STAThread]
        static void Main()
        {
            server = new ServerNet();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);           
            serverThread = new System.Threading.Thread(new ThreadStart(server.StartServer));
            serverThread.Start();

            Application.Run(new Form1());
        }
    }
}
