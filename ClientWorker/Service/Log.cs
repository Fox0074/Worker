using System;
using System.Collections.Generic;
using System.IO;

namespace ClientWorker
{
	public static class Log
	{
        public static List<string> messages = new List<string>();
        public static List<string> ErrorLog = new List<string>();
        private static StreamWriter w;

        public static void Send(string message)
		{
			messages.Add(message);
			try
			{
				using (StreamWriter streamWriter = File.AppendText("log.txt"))
				{
					streamWriter.WriteLine(message);
				}
			}
			catch
			{
				ErrorLog.Add(message);
			}
		}
	}
}
