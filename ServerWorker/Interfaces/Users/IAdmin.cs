using ServerWorker;
using ServerWorker.Server;
using System.Net;

namespace Interfaces.Users
{
    public interface IAdmin
    {
        UserInfo[] GetUsers();
        string ServerIdentification(string key);
        UserType GetUsertype();
    }
}
