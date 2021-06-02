using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerWorker
{
    public class SessionLoginData
    {
        public string Login { get; private set; }
        public string Md5Pass { get; private set; }
        public UserType userType;
        public SessionLoginData(string login, string md5Pass)
        {
            Login = login;
            Md5Pass = md5Pass;
        }

        public static SessionLoginData GetSessionData(string login, string pass)
        {
            var md5Pass = CreateMD5(pass);
            return aviableLoginData.FirstOrDefault(x => x.Login == login && x.Md5Pass == md5Pass);
        }

        private static List<SessionLoginData> aviableLoginData = new List<SessionLoginData>()
        {
            new SessionLoginData("Worker","52E6BE8AEC0A509594F4174EC9B13E94"){ userType = UserType.Admin},
            new SessionLoginData("System","15963CF47607DA0F4144D8AB2D4357F0"){ userType = UserType.System},
            new SessionLoginData("User","7655696647507059ABD4D408A592C061"){ userType = UserType.User}
        };

        public static string CreateMD5(string input)
        { 
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
    
    public class AuthInProgram
    {
        public bool IsAuthorizate { get; set; } = false;
        public SessionLoginData SessionLoginData;
        public bool Authorization (string login, string pass)
        {
            SessionLoginData = SessionLoginData.GetSessionData(login,pass);
            IsAuthorizate = SessionLoginData != null;
            return IsAuthorizate;
        }
    }
}
