using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.UsersRank
{
    public class Dog_Ring1 : Ring2
    {
        public Dog_Ring1(User u) : base(u)
        {
            up.UserType = UserType.Admin;
        }

        public bool TryFindObject(out object obj)
        {
            obj = "TheBall";
            return true;
        }
    }
}
