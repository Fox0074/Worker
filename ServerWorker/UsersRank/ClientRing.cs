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

        public void ChangePrivileges(string login, string password)
        {
            var loginData = SessionLoginData.GetSessionData(login,password);
            
            if (loginData == null)
                throw new Exception("Ошибка авторизации");

            switch (loginData.userType)
            {
                case UserType.User:
                    up.ClassInstance = new UserRing(up);
                    break;
                case UserType.Admin:
                    up.ClassInstance = new AdminRing(up);
                    break;
                case UserType.System:
                    up.ClassInstance = new SystemRing(up);
                    break;
                default:
                    throw new Exception("Новые права не могут быть назначены");
            }
            Program.server.Events.OnAuthorized.Invoke();
        }
    }
}
