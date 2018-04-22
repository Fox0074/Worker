using Interfaces;
using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.UsersRank
{
    public class Dog_Ring0 : Dog_Ring1, IDog
    {
        public Dog_Ring0(User u) : base(u)
        {
            up.UserType = UserType.Admin;
        }

        public int Bark(int nTimes)
        {
            var ConnectedDogs = ServerNet.ConnectedUsers.ToArray().Where(x => x.UserType == UserType.Admin).Select(x => x.nStream);
            ConnectedDogs.AsParallel().ForAll(nStream =>
            {
                // инициировать событие у клиента
                ServerNet.SendMessage(nStream, new Unit("OnBark", new object[] { nTimes }));
            });

            return ConnectedDogs.Count();
        }
    }
}
