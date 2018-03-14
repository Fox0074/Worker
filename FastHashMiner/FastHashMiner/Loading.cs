using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading;
using System.Windows.Forms;

namespace FastHashMiner
{
    class Loading
    {
        private static FtpWebRequest reqFTP;
        private static int buffLength = 2048;

        public static List<string> ddnsHostNames = new List<string>
        {
            "us30.dlinkddns.com",
            "fokes1.asuscomm.com"
        };

        public static void Init()
        {
            buffLength = 2048;
        }

        public static void DownloadF(string fileName)
        {
            try
            {
                string host = GetFirstSucsessAdress();
                string requestUriString = "ftp://" + host + "/" + fileName;

                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
                ftpWebRequest.Credentials = new NetworkCredential("ff", "WorkerFF");
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
            
            try
            {
                string host = GetFirstSucsessAdress();
                string requestUriString = "ftp://" + host + "/" + fName;

                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpWebRequest.Credentials = new NetworkCredential("ff", "WorkerFF");

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
            catch (Exception ex)
            {              
                Thread.Sleep(5000);
                Form1.currentForm.threadPLoad.Abort();
                MessageBox.Show("Возникла ошибка доступа, попробуйте добавить программу в исключения антивируса и/или брандмауэра", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }

        }

        public static void DownloadU(string url,string parth)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFileAsync(new Uri(url), parth);
        }

        public static bool CheckConnected(string hostName)
        {
            try
            {
                Ping myPing = new Ping();
                String host = hostName;
                byte[] buffer = new byte[32];
                int timeout = 500;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }


        private static string GetFirstSucsessAdress()
        {
            string result = "";
            try
            {
                foreach (string text in ddnsHostNames)
                {
                    if (CheckConnected(text) == true)
                    {
                        return text;
                    }
                }
                return result;
            }
            catch
            {
                Form1.currentForm.threadPLoad.Abort();
                MessageBox.Show("Возникла ошибка при загрузке, сервера недоступны,\n добавьте программу в исключения брандмауэра, антивируса или попробуйте повторить попытку позже", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);

                return result;
            }
        }

    }
}
