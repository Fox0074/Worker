using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerWorker
{

    interface IBase
    {
        void Start();
    }
    public class ServerBehaviour : IBase
    {
        //protected virtual void Start() { }
        public ServerBehaviour()
        {
            Start();
        }

        public virtual void Start()
        {

        }

    }
}
