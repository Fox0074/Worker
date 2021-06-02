using System.Collections.Generic;

namespace Interfaces.Users
{
    public interface ISystem : IAdmin
    {
        void ConnectAllUsers(string host, int port);
        void ConnectExcludeUsers(List<string> usersId, string host, int port);
    }
}
