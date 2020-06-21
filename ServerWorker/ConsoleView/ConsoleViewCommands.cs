using System;
using System.Linq;
using System.Net;

namespace ServerWorker.ConsoleView
{
    public class ConsoleViewCommands
    {
        public ConsoleViewCommands()
        {
        }

        public void Help()
        {
            var methods = GetType().GetMethods();

            foreach (var method in methods)
            {
                Console.WriteLine(method.Name);
            }
        }

        public void StartServer()
        {
            Program.StartServer(7777);
        }

        public void ShowUsers()
        {
            Console.WriteLine("Список клиентов:");

            foreach (var user in ServerNet.ConnectedUsers.ToArray())
            {
                Console.WriteLine(user.EndPoint);
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
    }
}
