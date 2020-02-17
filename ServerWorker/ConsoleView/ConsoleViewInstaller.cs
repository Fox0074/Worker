using System;
using System.Threading;

namespace ServerWorker.ConsoleView
{
    public class ConsoleViewInstaller
    {
        public Thread CommandThread;
        public ConsoleView View;

        public ConsoleViewInstaller()
        {
            View = new ConsoleView();
        }

        public void Autorize()
        {
            string login;
            string pass;
            bool isAutorize;
            do
            {
                Console.Write("Введите логин: ");
                login = Console.ReadLine();
                Console.Write("Введите пароль: ");
                pass = Console.ReadLine();

                isAutorize = Program.authSystem.Authorization(new SessionLoginData(login, pass));

                if (!isAutorize) Console.Write(">>Неверный логин или пароль\n");

            } while (!isAutorize);
        }

        public void StartCommandLineThread()
        {
            CommandThread = new Thread(new ThreadStart(View.CommandLineThread));
            CommandThread.Start();
        }
    }
}
