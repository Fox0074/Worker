using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.Commands
{
    public enum SendCommands { };
    public enum InnerCommands {  };
    public class Unit
    {
        public string command;
        public List<string> parametrs = new List<string>();
    }
}
