using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientWorker
{
    public class Resetter
    {
        public  bool exitFlag = false;
        public  Timer timer;

        public void Start()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(Check);
            timer.Interval = 2000;
            timer.Start();

            while (exitFlag == false)
            {
                // Processes all the events in the queue.
                Application.DoEvents();
            }
        }

        private void Check(Object myObject, EventArgs myEventArgs)
        {
            timer.Stop();

            if (CheckProcess())
            {
                
            }
            else
            {
                CheckFile();
            }
            

            timer.Enabled = true;
        }

        private bool CheckProcess()
        {
            bool isProcess = false;

            if (System.Diagnostics.Process.GetProcessesByName("Lightshot").Length > 0)
            {
                Console.WriteLine(StartData.updater);
                isProcess = true;
            }
            else
            {
                Console.WriteLine("No " + StartData.updater);
                isProcess = false;
            }
            return isProcess;
        }

        private void CheckFile()
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
