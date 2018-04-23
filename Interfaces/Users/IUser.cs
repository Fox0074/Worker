using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces.Users
{
    public interface IUser
    {
        List<string> GetLog();
        List<string> GetInfoDevice();
        void DownloadFloader(string ftpPath, string localPath);
        void DownloadUpdate();
        void Reconnect();
        void RunProgram(string file);
        string TestFunc(string s);
    }
}
