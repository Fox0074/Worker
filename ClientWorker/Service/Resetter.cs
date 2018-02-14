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

		private static void CheckFile(object Sender, EventArgs e)
		{
			try
			{
				if (File.Exists(StartData.updater))
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
