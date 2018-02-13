using System;
using System.Diagnostics;
using System.IO;

namespace ClientWorker
{
	public static class Resetter
	{
		public static void Start()
		{
			foreach (Process process in Process.GetProcessesByName("Updater"))
			{
				process.Kill();
			}
			CheckFile(null, null);
		}

		private static bool CheckProcess()
		{
			return Process.GetProcessesByName("Updater").Length != 0;
		}

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
					process.Exited += CheckFile;
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
				CheckFile(Sender, e);
			}
		}
	}
}
