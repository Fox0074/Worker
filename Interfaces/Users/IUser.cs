using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces.Users
{
    public interface IUser
    {
        string[] GetAvailableUsers();
        void ChangePrivileges(string Login, string password);
        string TestFunc(string s);
    }
}
