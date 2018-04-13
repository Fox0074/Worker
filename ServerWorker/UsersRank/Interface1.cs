using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWorker.UsersRank
{
    public interface ICommon
    {
        string[] GetAvailableUsers();
        void ChangePrivileges(string Login, string password);
    }

    public interface IWorker
    {
        bool TryFindObject(out object obj);
        int Bark(int nTimes);
    }

    public interface IServer
    {
        void CutTheText(ref string Text);
    }

    public interface IProxy
    {
        void CutTheText(ref string Text);
    }

    public interface IAndroid
    {
        void CutTheText(ref string Text);
    }
}
