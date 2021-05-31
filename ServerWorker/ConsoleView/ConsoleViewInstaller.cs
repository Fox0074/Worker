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
            ConsoleKeyInfo inputKey;
            do
            {
                Console.Write("Введите логин: ");
                login = Console.ReadLine();
                Console.Write("Введите пароль: ");
                pass = "";

                do
                {
                    inputKey = Console.ReadKey(true);
                    switch(inputKey.Key)
                    {
                        case ConsoleKey.Enter:
                            continue;
                        case ConsoleKey.Backspace:
                            pass = pass.Length > 0 ? pass.Remove(pass.Length - 1,1) : pass;
                            break;
                        default:
                            pass += inputKey.KeyChar;
                        break;
                    }
                } while(inputKey.Key != ConsoleKey.Enter);
                Console.WriteLine();

                isAutorize = Program.authSystem.Authorization(login, pass);

                if (!isAutorize) Console.Write(">>Неверный логин или пароль\n");

            } while (!isAutorize);

            View.OnProgramClose += OnProgramClose;
        }

        public void StartCommandLineThread()
        {
            CommandThread = new Thread(new ThreadStart(View.CommandLineThread));
            CommandThread.Start();
        }

        public void OnProgramClose()
        {
            Program.server?.Stop();
            Program.serverThread?.Abort();
        }
    }
}
