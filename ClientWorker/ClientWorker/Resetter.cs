using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Threading;
using System.Timers;

namespace ClientWorker
{
    public static class Resetter
    {

        public static void Start()
        {
            CheckFile(null, null); 
        }


        private static bool CheckProcess()
        {
            bool isProcess;
            if (Process.GetProcessesByName("Updater").Length > 0)
            {
                //Console.WriteLine(StartData.updater);
                isProcess = true;
            }
            else
            {
                //Console.WriteLine("No " + StartData.updater);
                isProcess = false;
            }
            return isProcess;
        }

        private static void CheckFile(object Sender, EventArgs e)
        {
            if (File.Exists(StartData.updater))
            {
                Console.WriteLine("Файл найден");

                Process proc = new Process();
                proc.StartInfo.FileName = StartData.updater;
                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(CheckFile);
                proc.StartInfo.Verb = "runas";
                try
                {
                    proc.Start();
                    //Process.Start(proc.StartInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при запуске процесса: {0}", ex.ToString());
                }
            }
            else
            {
                Program.client.handler.GetUpdater();
                Console.WriteLine("Файл не найден");
            }
        }
    }
}
