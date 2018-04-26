using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.UsersRank
{
    public class UserRing : AbstractRing
    {
        public UserRing(User u) : base(u)
        {
            up.UserType = UserType.User;
        }

        public void SetKey(string key)
        {
        }
    }
}
