using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ClientWorker
{
	// Token: 0x02000006 RID: 6
	internal class Program
	{
		// Token: 0x06000019 RID: 25 RVA: 0x00002B88 File Offset: 0x00000D88
		private static void Main()
		{
			try
			{
				string[] array = Application.ExecutablePath.ToString().Split(new char[]
				{
					'\\'
				});
				Program.nameProcess = array[array.Length - 1];
			}
			catch (Exception ex)
			{
				Log.Send("Ошибка получения имени процесса " + ex.Message);
			}
			Log.Send("===================================ЗАПУСК===================================");
			int num = 0;
			foreach (Process process in Process.GetProcesses())
			{
				try
				{
					bool flag = process.MainModule.FileVersionInfo.FileDescription == "Service";
					if (flag)
					{
						num++;
					}
				}
				catch
				{
				}
			}
			bool flag2 = num > 1;
			if (flag2)
			{
				Log.Send("Найден другой процесс Service");
			}
			else
			{
				Program.client = new Client();
				Functions.Registration();
				Program.clientThread = new Thread(new ThreadStart(Program.client.Start));
				Program.clientThread.Start();
				Log.Send("Запуск потока");
			}
		}

		// Token: 0x0400000D RID: 13
		public static Thread clientThread;

		// Token: 0x0400000E RID: 14
		public static Client client;

		// Token: 0x0400000F RID: 15
		public static string nameProcess;
	}
}
