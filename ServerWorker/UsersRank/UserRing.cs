using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.IO;
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

        public void Identification(string key, string version, bool stateM)
        {
            up.userData = new UserCard.UserData(key);
            up.userData.setting.Version = version;
            up.userData.IsWorkinMiner = stateM;
        }

        public void ChangeStateMiner(bool state)
        {
            up.userData.IsWorkinMiner = state;
        }
    }
}
