using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerWorker
{
    public class SessionLoginData
    {
        public string Login { get; private set; }
        public string Pass { get; private set; }
        public UserType userType;
        public SessionLoginData(string login, string pass)
        {
            Login = login;
            Pass = pass;
        }
    }
    public class AuthInProgram
    {
        public bool IsAuthorizate { get; set; } = false;
        public SessionLoginData sessionLoginData;
        public bool Authorization (SessionLoginData loginData)
        {
            IsAuthorizate = false;
            foreach (var data in aviableLoginData)
            {
                if (data.Login == loginData.Login && data.Pass == loginData.Pass)
                {
                    sessionLoginData = loginData;
                    sessionLoginData.userType = data.userType;
                    IsAuthorizate = true;
                }
            }

            return IsAuthorizate;
        }

        private List<SessionLoginData> aviableLoginData = new List<SessionLoginData>()
        {
            new SessionLoginData("Worker","hex34"){ userType = UserType.Admin},
            new SessionLoginData("System","92934q9f"){ userType = UserType.System}
        };
    }
}
