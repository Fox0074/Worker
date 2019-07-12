using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces
{
    [Serializable]
    public class LoginData
    {
        public string WebSite { get; }
        public string Login { get; }
        public string Pass { get; }

        public LoginData(string webSite, string login, string pass)
        {
            WebSite = webSite;
            Login = login;
            Pass = pass;
        }
    }
}
