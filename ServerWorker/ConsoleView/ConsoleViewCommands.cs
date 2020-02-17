using System;
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
    }
}
