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
        
        [STAThread]
        static void Main()
        {
            server = new ServerNet();            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form1 = new Form1();

            serverThread = new Thread(new ThreadStart(server.StartServer));
            serverThread.Start();   
            
            Application.Run(form1);
        }
    }
}
