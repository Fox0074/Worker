﻿using System;
using System.Collections.Generic;
using System.IO;

namespace ClientWorker
{
	public static class Log
	{
        public static List<string> messages = new List<string>();
        public static List<string> ErrorLog = new List<string>();
        public static string logParth = "log.txt";
        public static string errorLogParth = "Error.txt";

        public static void Send(string message)
		{
            try
            {
                messages.Add(message);			
				using (StreamWriter streamWriter = File.AppendText(logParth))
				{
					streamWriter.WriteLine(message);
				}
			}
			catch
			{
                try
                {
                    ErrorLog.Add(message);
                    using (StreamWriter errorStreamWriter = File.AppendText(errorLogParth))
                    {
                        errorStreamWriter.WriteLine(message);
                    }
                }
                catch { }
            }
		}

        public static void DetermineLogParth()
        {
            if (Directory.GetCurrentDirectory() == Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory))
            {
                logParth = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + StartData.floaderNewCopy + "log.txt";
            }
        }
    }
}
