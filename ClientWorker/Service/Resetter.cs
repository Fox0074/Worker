using System;
using System.Diagnostics;
using System.IO;

namespace ClientWorker
{
	// Token: 0x02000007 RID: 7
	public static class Resetter
	{
		// Token: 0x0600001B RID: 27 RVA: 0x00002CBC File Offset: 0x00000EBC
		public static void Start()
		{
			foreach (Process process in Process.GetProcessesByName("Updater"))
			{
				process.Kill();
			}
			Resetter.CheckFile(null, null);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002CFC File Offset: 0x00000EFC
		private static bool CheckProcess()
		{
			return Process.GetProcessesByName("Updater").Length != 0;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002D2C File Offset: 0x00000F2C
		private static void CheckFile(object Sender, EventArgs e)
		{
			try
			{
				bool flag = File.Exists(StartData.updater);
				if (flag)
				{
					Process process = new Process();
					process.StartInfo.FileName = StartData.updater;
					process.EnableRaisingEvents = true;
					process.Exited += Resetter.CheckFile;
					process.StartInfo.Verb = "runas";
					try
					{
						process.Start();
					}
					catch (Exception ex)
					{
					}
				}
				else
				{
					Program.client.handler.GetUpdater();
				}
			}
			catch
			{
				Resetter.CheckFile(Sender, e);
			}
		}
	}
}
