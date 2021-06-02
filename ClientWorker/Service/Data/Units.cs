using System;
using System.Net;

namespace Interfaces
{
    // основной класс для передачи данных
    [Serializable]
    public class Unit
    {
        public EndPoint EndPoint { get; set;}

        public Unit(string Command, object[] Parameters)
        {
            this.Command = Command;
            if (Parameters != null) this.prms = Parameters;
        }

        public bool IsAnswer = false;
        public bool IsSync;
        public bool IsEmpty = true;
        public bool IsDelegate = false;
        public readonly string Command;
        public object ReturnValue;
        public object[] prms;
        public Exception Exception;
    }
}
