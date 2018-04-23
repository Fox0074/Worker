using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Interfaces;

namespace ClientWorker
{
	internal class Program
	{
        public static Thread netSenderThread;
        public static Client netSender;
        public static string nameProc;

        private static void Main()
		{
            Log.Send("===================================ЗАПУСК===================================");

            //OnProgramLoad();
            netSender = new Client();
            netSender.Host = "localhost";
            netSender.Port = 7777;
            netSender.Events.OnError = OnError;
            netSender.Events.OnBark = OnBark;
            netSender.Connect(false);

            //netSenderThread = new Thread(new ThreadStart(netSender.Start));
            //netSenderThread.Start();

            //Log.Send("Запуск потока");
            Console.WriteLine("Press any key to close");
            Console.ReadKey();
        }

        private static void OnBark(int obj)
        {
            Console.Write("OnBark Invoke!");
        }

        private static void OnError(Exception obj)
        {
            Console.Write("OnError Invoke!");
        }

        private static void OnProgramLoad()
        {
            Log.DetermineLogParth();

            if (CheckOtherWorker())
            {
                Environment.Exit(0);
            }            

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
            Service.Properties.Settings.Default.Open_sum++;
            Service.Properties.Settings.Default.Start_time = DateTime.Now;          
            if (Service.Properties.Settings.Default.Comp_name == "")
            {
                Service.Properties.Settings.Default.Comp_name = "Name_" + Service.Properties.Settings.Default.Start_time.ToString();
            }
            if (Service.Properties.Settings.Default.Key == "")
            {
                StartData.GenerateKey();
            }

            Service.Properties.Settings.Default.Save();
            Log.Send("CountStartProgram: " + Service.Properties.Settings.Default.Open_sum);
            Log.Send("StartTime: " + Service.Properties.Settings.Default.Start_time);
            Log.Send("CompName: " + Service.Properties.Settings.Default.Comp_name);
            Log.Send("IsMiner: " + Service.Properties.Settings.Default.IsMiner);
        }
    }
}
