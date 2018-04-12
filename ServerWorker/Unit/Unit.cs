using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerWorker.Unit;

namespace ServerWorker.Unit
{
    public class Unit
    {
        public string Identificator { get; }
        public DateTime StartdateTime { get; }
        public IPEndPoint EndPoint { get; }

        protected ICommand commandBehaviour;

        public Unit(string Command, object[] Parameters)
        {
            this.Command = Command;
            if (Parameters != null) this.prms = Parameters;
        }

        public bool IsSync;
        public bool IsEmpty = true;
        public readonly string Command;
        public object ReturnValue;
        public object[] prms;
        public Exception Exception;

    }
}
