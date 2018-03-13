using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace FastHashMiner
{
    class Loading
    {
        private static FtpWebRequest reqFTP;
        private static int buffLength = 2048;


        public static void Init()
        {
            buffLength = 2048;
        }

        public static void DownloadF(string fileName)
        {
            try
            {
                string requestUriString = "ftp://" + "fokes1.asuscomm.com" + "/" + fileName;
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
                ftpWebRequest.Credentials = new NetworkCredential("ff", "FF");
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

                Stream responseStream = ftpWebResponse.GetResponseStream();
                FileStream fileStream = new FileStream(fileName, FileMode.Create);
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
            }
        }

        public static void DownloadF(string fName, string parth)
        {
            string requestUriString = "ftp://" + "fokes1.asuscomm.com" + "/" + fName;
            try
            {
                if (CheckConnected())
                {
                    FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
                    ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    ftpWebRequest.Credentials = new NetworkCredential("ff", "FF");

                    FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                    FileStream fileStream = new FileStream(parth + fName, FileMode.Create);
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
                else
                {
                    Form1.currentForm.threadPLoad.Abort();
                    MessageBox.Show("Возникла ошибка при загрузке, сервера недоступны,\n добавьте программу в исключения брандмауэра, антивируса или попробуйте повторить попытку позже", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Возникла ошибка при загрузке, сервера недоступны,\n добавьте программу в исключения брандмауэра, антивируса или попробуйте повторить попытку позже", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static void DownloadU(string url,string parth)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFileAsync(new Uri(url), parth);
        }

        public static bool CheckConnected()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "fokes1.asuscomm.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
