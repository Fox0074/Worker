using System;

namespace ServerWorker.ConsoleView
{
    public interface IConsoleCommands
    {
        Action<IConsoleCommands> SwithCommander {get; set;}
        void Help();
    }
}