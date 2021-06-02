using System.Net;

namespace Interfaces.Users
{
    public interface IAdmin
    {
        EndPoint[] GetUsers();
        string ServerIdentification(string key, string pass);
    }
}
