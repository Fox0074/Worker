using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.UsersRank
{
    public abstract class Ring
    {
        public readonly User up;

        public Ring(User up)
        {
            this.up = up;
        }
    }
}
