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
        void DownloadAndRun(string file);
        void DeleteFile(string file);
        void GetSetting();
        string TestFunc(string s);
    }
}
