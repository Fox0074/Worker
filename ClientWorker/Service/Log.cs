using System;
using System.Collections.Generic;
using System.IO;

namespace ClientWorker
{
	// Token: 0x02000005 RID: 5
	public static class Log
	{
		// Token: 0x06000017 RID: 23 RVA: 0x00002B00 File Offset: 0x00000D00
		public static void Send(string message)
		{
			Log.messages.Add(message);
			try
			{
				using (StreamWriter streamWriter = File.AppendText("log.txt"))
				{
					streamWriter.WriteLineAsync(message);
				}
			}
			catch
			{
				Log.ErrorLog.Add(message);
			}
		}

		// Token: 0x0400000A RID: 10
		public static List<string> messages = new List<string>();

		// Token: 0x0400000B RID: 11
		public static List<string> ErrorLog = new List<string>();

		// Token: 0x0400000C RID: 12
		private static StreamWriter w;
	}
}
