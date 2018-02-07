using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;

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
                DebugCommand();
            }

        }

        static void DebugCommand()
        {
            while (true)
            {
                if ((Console.ReadLine()) == "Worker.Close")
                {
                    foreach (var process in Process.GetProcessesByName("Updater"))
                    {
                        process.Kill();
                    }
                    Environment.Exit(0);
                }
            }
        }

        

    }
}
