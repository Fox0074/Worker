using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ClientWorker
{
    class Program
    {
        public static Thread clientThread;
        public static Client client;
        public static Resetter resetter;

        static void Main()
        {
            resetter = new Resetter();
            resetter.Start();

            if (System.Diagnostics.Process.GetProcessesByName(Application.ProductName).Length > 1)
            {
                return;
            }
            else
            {
                client = new Client();


                clientThread = new Thread(new ThreadStart(client.Start));
                clientThread.Start();
            }

        }

        

    }
}
