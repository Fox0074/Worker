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

    }
}
