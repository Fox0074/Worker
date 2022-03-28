using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using ServerWorker.Server;

namespace ServerWorker.ConsoleView
{
    public class ConsoleViewCommands : IConsoleCommands
    {
        public Action<IConsoleCommands> PushCommander {get; set;} = delegate { };
        public Action PopCommander {get; set;} = delegate { };
        private string drawUsersTableFormat = "{0,-7}|{1, -7}|{2, -25}|{3, -20}|{4, -25}|{5, -7}|{6, -2}|{7, -4}";
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

        public void StartServer()
        {
            Program.StartServer(7777);
        }

        public void ShowUsers()
        {
            Console.WriteLine("Список клиентов:");
            var usersList = ServerNet.ConnectedUsers.ToArray().ToList();
            usersList.AddRange(ServerNet.ConnectedServers.ToArray());

            foreach (var user in usersList)
            {
                if (user.userData != null)
                {
                    Console.WriteLine(String.Format(drawUsersTableFormat, 
                    user.UserType.ToString(),
                    user.userData.setting.Version ?? "",
                    user.userData.setting.Comp_name ?? user.userData.id ?? "",
                    user.EndPoint.ToString(),
                    user.userData.infoDevice.GPUVideoProcessor.Count > 0 ? user.userData.infoDevice.GPUVideoProcessor[0] : "",
                    user.userData.IsWorkinMiner.ToString(),
                    user.userData.IsGettingLoginData ? "V" : "",
                    user.Ping.ToString()));
                }
                else
                {
                    Console.WriteLine(String.Format(drawUsersTableFormat, 
                        user.UserType.ToString(),
                        "",
                        "",
                        user.EndPoint.ToString(),
                        "",
                        "",
                        "",
                        ""));
                }
            }
        }

        public void UpdateAllUsers()
        {
            foreach (var user in ServerNet.ConnectedUsers.ToArray())
            {
                UpdateUser(user.EndPoint.ToString());
            }
        }

        public void UpdateUser(string userEndPoint)
        {
            var user = ServerNet.ConnectedUsers.ToArray().FirstOrDefault(usr => usr.EndPoint.ToString() == userEndPoint);

            if (user == null)
            {
                Console.WriteLine("Пользователь с адресом {0} не найден", userEndPoint);
                return;
            }

            Console.WriteLine(user.EndPoint + " >> DownloadUpdate");
            user.UsersCom.DownloadUpdate();
        }

        public void Select(string userEndPoint)
        {
            var user = ServerNet.ConnectedUsers.ToArray().FirstOrDefault(usr => usr.EndPoint.ToString() == userEndPoint);

            if (user == null)
            {
                Console.WriteLine("Пользователь с адресом {0} не найден", userEndPoint);
                return;
            }

            PushCommander.Invoke(new ConsoleViewUserCommands(user));
        }

        public void Exit() 
        {
            Program.StopServer();
        }

        public void ConnectToServer(string host)
        {
            string pass;
            do
            {
                Console.Write("Enter password: ");
                pass = Console.ReadLine();
            } while (SessionLoginData.CreateMD5(pass) != Program.authSystem.SessionLoginData.Md5Pass);

            new SubServer(host, pass);
            Program.SubServer.ServerUser.AdminCom.GetUsers().ToList().ForEach(x =>
            {
                if (x.UserType == UserType.User)
                    ServerNet.ConnectedUsers.Add(new User(x));
                if (x.UserType == UserType.Admin || x.UserType == UserType.System)
                    ServerNet.ConnectedServers.Add(new User(x));
            });
        }
    }
}
