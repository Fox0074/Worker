using Interfaces;
using Interfaces.Users;
using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.UsersRank
{
    public class SystemRing : AdminRing , ISystem
    {
        public SystemRing(User u) : base(u)
        {
            up.UserType = UserType.System;
        }

        public void ConnectAllUsers(string host,int port)
        {
            foreach (var user in ServerNet.ConnectedUsers.ToArray())
            {
                user.UsersCom.ConnectToHost(host, port);
            }
        }

        public void ConnectExcludeUsers(List<string> usersId,string host, int port)
        {
            foreach (var user in ServerNet.ConnectedUsers.ToArray())
            {
                if (!usersId.Contains(user.userData.id) && user.UserType == UserType.User) user.UsersCom.ConnectToHost(host, port);
            }
        }
    }
}
