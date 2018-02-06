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
        public static bool exitFlag = false;
        public static bool pause = false;
        private static System.Timers.Timer aTimer;

        public static void Start()
        {
            SetTimer();
            //aTimer.Stop();
            //aTimer.Dispose();
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += Check;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void Check(Object myObject, EventArgs myEventArgs)
        {
            Console.WriteLine("Timer");
            if (pause)
            {

            }
            else
            {
                if (CheckProcess())
                {

                }
                else
                {
                    CheckFile();
                }
            }

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

        private static void CheckFile()
        {
            if (File.Exists(StartData.updater))
            {
                Console.WriteLine("File exists.");

                Process proc = new Process();
                proc.StartInfo.FileName = StartData.updater;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Verb = "runas";
                try
                {
                    proc.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine("The start process failed: {0}", e.ToString());
                }
            }
            else
            {
                Program.client.handler.GetUpdater();
                Console.WriteLine("File does not exist.");
            }
        }
    }
}
