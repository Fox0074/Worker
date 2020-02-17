using System;
namespace ServerWorker.ConsoleView
{
    public class ConsoleView
    {
        public ConsoleView()
        {
        }

        public void CommandLineThread()
        {
            string command;
            do
            {
                Console.Write("Command >> ");
                command = Console.ReadLine();
            } while (command.ToLower() != "exit");
        }
    }
}
