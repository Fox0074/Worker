using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using ServerWorker.Server;
using Interfaces;

namespace ServerWorker.ConsoleView
{
    public class ConsoleViewChatCommands : IConsoleCommands
    {
        public Action<IConsoleCommands> PushCommander {get; set;} = delegate { };
        public Action PopCommander {get; set;} = delegate { };
        private User _user;

        private List<String> _excludedMethods = new List<string>()
        {
            "Equals",
            "GetHashCode",
            "GetType",
            "ToString"
        };

        public ConsoleViewChatCommands(User user)
        {
            _user = user;
            _user.UsersCom.StartChat();
            _user.OnSendChatMessage += ReadMessage;
        }

        public void Send(string message)
        {
            _user.UsersCom.ReadMessage(DateTime.Now.ToString("HH:mm") + " God: " + message);
        }

        private void ReadMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("User: " + message);
            Console.ForegroundColor = ConsoleColor.Green;
        }

        public void Back()
        {
            _user.UsersCom.StopChat();
            _user.OnSendChatMessage -= ReadMessage;
            PopCommander();
        }

        public void Help()
        {
            var methods = GetType().GetMethods().ToList();
            methods.Where(x => !_excludedMethods.Contains(x.Name)).ToList().ForEach(x => 
            {
                var parametrs = x.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name).ToList();

                Console.WriteLine("{0}({1})",x.Name, string.Join(",", parametrs));
            });
        }
    }
}