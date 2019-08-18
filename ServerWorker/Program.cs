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
        static public string ServerId = "FoxServer";


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
            server.Start();

            form1 = new Form1();
            //StartServer();

            Application.Run(form1);
        }
    }
}
