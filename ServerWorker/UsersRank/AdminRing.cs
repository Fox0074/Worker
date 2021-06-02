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
        public string ServerIdentification(string key)
        {
            up.userData = new UserCard.UserData(key);
            return Program.ServerId;
        }

        public UserType GetUsertype()
        {
            return Program.authSystem.SessionLoginData.userType;
        }

        public UserInfo[] GetUsers()
        {
            List<UserInfo> list = new List<UserInfo>();
            var z = ServerNet.ConnectedUsers.ToArray();
            z.ToList().ForEach(x => list.Add(new UserInfo() { UserType = x.UserType, EndPoint = x.EndPoint, userData = x.userData }));
            return list.Where(x => x.EndPoint.ToString() != up.EndPoint.ToString() && x.UserType <= up.UserType).ToArray();
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
