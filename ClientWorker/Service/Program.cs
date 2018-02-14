using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ClientWorker
{
	internal class Program
	{
        public static Thread clientThread;
        public static Client client;
        public static string nameProcess;

        private static void Main()
		{
            Log.Send("===================================ЗАПУСК===================================");
            OnProgramLoad();           

            client = new Client();
            clientThread = new Thread(new ThreadStart(client.Start));
			clientThread.Start();

			Log.Send("Запуск потока");
		}


        private static void OnProgramLoad()
        {         
            if (CheckOtherWorker())
            {
                Environment.Exit(0);
            }

            Log.DetermineLogParth();
            nameProcess = GetProcessName();         
            Functions.Registration();

        }
        private static string GetProcessName()
        {
            string procName = "";

            try
            {
                string[] array = Application.ExecutablePath.ToString().Split('\\');
                procName = array[array.Length - 1];
            }
            catch (Exception ex)
            {
                Log.Send("Ошибка получения имени процесса " + ex.Message);
            }

            return procName;
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
    }
}
