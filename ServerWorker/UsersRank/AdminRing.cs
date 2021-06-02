using Interfaces;
using Interfaces.Users;
using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.UsersRank
{
    public class AdminRing : ClientRing, IAdmin
    {
        public AdminRing(User u) : base(u)
        {
            up.UserType = UserType.Admin;
        }
        public string ServerIdentification(string key, string pass)
        {
            up.userData = new UserCard.UserData(key);
            ServerNet.SendMessage(up.nStream,
                new Unit("ChangePrivileges", new string[] { Program.authSystem.sessionLoginData.Login, pass}));
            return Program.ServerId;
        }

        public EndPoint[] GetUsers()
        {
            return ServerNet.ConnectedUsers.ToArray().Select(x => x.EndPoint).ToArray();
        }

        public int Bark(int nTimes)
        {
            var ConnectedDogs = ServerNet.ConnectedUsers.ToArray().Where(x => x.UserType == UserType.Admin).Select(x => x.nStream);
            ConnectedDogs.AsParallel().ForAll(nStream =>
            {
                ServerNet.SendMessage(nStream, new Unit("OnBark", new object[] { nTimes }));
            });

            return ConnectedDogs.Count();
        }
    }
}
