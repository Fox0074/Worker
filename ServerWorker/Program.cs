﻿using ServerWorker.ConsoleView;
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
        static public ConsoleViewInstaller Console;
        static public Form1 form1;
        static public string ServerId = "FoxServer";


        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "-GUI")
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    LockForm lockForm = new LockForm();
                    Application.Run(lockForm);

                    StartServer(7777);

                    form1 = new Form1();
                    Application.Run(form1);
                }
            }
            else
            {
                Console = new ConsoleViewInstaller();
                Console.Autorize();
                Console.StartCommandLineThread();
            }
        }

        public static void StartServer(int port)
        {
            server = new ServerNet(port);
            serverThread = new Thread(new ThreadStart(server.Start));
        }
    }
}
