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
        const int port = 7778;
        static TcpListener listener;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

                try
                {
                    listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                    listener.Start();
                    Console.WriteLine("Ожидание подключений...");

                    while (true)
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        ServerNet clientObject = new ServerNet(client);

                        // создаем новый поток для обслуживания нового клиента
                        Thread clientThread = new System.Threading.Thread(new ThreadStart(clientObject.Process));
                        clientThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (listener != null)
                        listener.Stop();
                }

                Application.Run(new Form1());
        }
    }
}
