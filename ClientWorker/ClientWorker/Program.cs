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
        static void Main()
        {

            if (System.Diagnostics.Process.GetProcessesByName(Application.ProductName).Length > 1)
            {
                return;
            }
            else
            {
                Client client = new Client();


                Thread clientThread = new Thread(new ThreadStart(client.Start));
                clientThread.Start();
            }

        }
    }
}
