using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    public static class FTP
    {
        private static FtpWebRequest reqFTP;
        private static int buffLength = 2048;
       
        public static void DownloadF(string fName, string parth, string newFName)
        {
            Log.Send("DownloadF(string, string");
            string requestUriString = "ftp://" + "fokes1.asuscomm.com/" + fName;
            try
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpWebRequest.Credentials = new NetworkCredential("ff", "WorkerFF");

                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                FileStream fileStream = new FileStream(parth + newFName, FileMode.Create);
                Stream responseStream = ftpWebResponse.GetResponseStream();
                byte[] array = new byte[buffLength];
                int count;
                while ((count = responseStream.Read(array, 0, array.Length)) > 0)
                {
                    fileStream.Write(array, 0, count);
                }
                fileStream.Close();
                ftpWebResponse.Close();
            }
            catch (Exception ex)
            {
                Log.Send(ex.Message + "Ошибка скачивания файла");
            }
        }

    }
}
