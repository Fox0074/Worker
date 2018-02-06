using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace ClientWorker
{
    class Program
    {
        public static Thread clientThread;
        public static Client client;

        static void Main()
        {

            if (System.Diagnostics.Process.GetProcessesByName(Application.ProductName).Length > 1)
            {
                return;
            }
            else
            {
                client = new Client();

                clientThread = new Thread(new ThreadStart(client.Start));
                clientThread.Start();

                Resetter.Start();
            }

        }

        

    }
}
