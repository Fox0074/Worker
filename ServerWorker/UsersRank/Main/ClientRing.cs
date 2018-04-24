using ServerWorker.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.UsersRank
{
    public class ClientRing : AbstractRing
    {
        public ClientRing(User u) : base(u)
        {

        }

        public string[] GetAvailableUsers()
        {
            return new string[] { "User", "Admin", "System" };
        }

        public void ChangePrivileges(string Animal, string password)
        {
            switch (Animal)
            {
                case "User":
                    if (password != "IUser") throw new Exception("Не верный пароль");
                    up.ClassInstance = new UserRing(up);
                    break;
                case "Admin":
                    if (password != "ImIsAdmin") throw new Exception("Не верный пароль");
                    up.ClassInstance = new AdminRing(up);
                    break;
                case "System":
                    if (password != "hex34") throw new Exception("Не верный пароль");
                    up.ClassInstance = new SystemRing(up);
                    break;
                default:
                    throw new Exception("Такого пользователя не существует");
            }
            Program.server.Events.OnAuthorized.Invoke();
        }
    }
}
