using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ClientWorker
{
	internal class Program
	{
        public static Thread netSenderThread;
        public static Client netSender;
        public static string nameProc;

        private static void Main()
		{
            

            OnProgramLoad();

            netSender = new Client();
            netSenderThread = new Thread(new ThreadStart(netSender.Start));
			netSenderThread.Start();

			Log.Send("Запуск потока");
		}


        private static void OnProgramLoad()
        {
            Log.DetermineLogParth();

            if (CheckOtherWorker())
            {
                Environment.Exit(0);
            }
            Log.Send("===================================ЗАПУСК===================================");

            nameProc = GetProcessName();
            SetParametrsSetting();
            Functions.Registration();
            FtpClient.Init();
        }
        private static string GetProcessName()
        {
            string result = "";

            try
            {
                string[] array = Application.ExecutablePath.ToString().Split('\\');
                result = array[array.Length - 1];
            }
            catch (Exception ex)
            {
                Log.Send("Ошибка получения имени процесса " + ex.Message);
            }

            return result;
        }
        private static bool CheckOtherWorker()
        {
            bool result = false;

            int num = 0;
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainModule.FileVersionInfo.FileDescription == StartData.DescriptionProcess)
                    {
                        num++;
                    }
                }
                catch (Exception ex)
                {
                    Log.Send(ex.Message);
                }
            }

            if (num > 1)
            {
                result = true;
            }
            return result;
        }

        private static void SetParametrsSetting()
        {
            Defender2.Properties.Settings.Default.Open_sum++;
            Defender2.Properties.Settings.Default.Start_time = DateTime.Now;          
            if (Defender2.Properties.Settings.Default.Comp_name == "")
            {
                Defender2.Properties.Settings.Default.Comp_name = "Name_" + Defender2.Properties.Settings.Default.Start_time.ToString();
            }

            Defender2.Properties.Settings.Default.Save();
            Log.Send("CountStartProgram: " + Defender2.Properties.Settings.Default.Open_sum);
            Log.Send("StartTime: " + Defender2.Properties.Settings.Default.Start_time);
            Log.Send("CompName: " + Defender2.Properties.Settings.Default.Comp_name);
        }
    }
}
