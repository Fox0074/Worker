using System;

namespace ServerWorker.ConsoleView
{
    public interface IConsoleCommands
    {
        Action<IConsoleCommands> PushCommander {get; set;}
        Action PopCommander {get; set;}
        void Help();
    }
}