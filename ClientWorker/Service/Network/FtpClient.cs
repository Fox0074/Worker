using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace ClientWorker
{
    public static class FtpClient
	{
        private static FtpWebRequest reqFTP;
        private static int buffLength = 2048;

        public static void Init()
        {
            buffLength = 2048;
        }

        public static void CreateDirectory(string path)
        {
            string uriString = "ftp://" + StartData.currentServer + "/" + path;
            reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(uriString));
            reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
            reqFTP.Credentials = new NetworkCredential("user", "pass");
            var resp = (FtpWebResponse)reqFTP.GetResponse();
        }

        public static void UploadF(string uploadPath, string fileName)
        {
            Log.Send("UploadFile");
            FileInfo fileInfo = new FileInfo(fileName);
            string uriString = "ftp://" + StartData.currentServer + "/" + uploadPath;
            reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(uriString));
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            reqFTP.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);

            reqFTP.ContentLength = fileInfo.Length;

            byte[] buffer = new byte[buffLength];
            FileStream fileStream = fileInfo.OpenRead();
            try
            {
                Stream requestStream = reqFTP.GetRequestStream();
                for (int i = fileStream.Read(buffer, 0, buffLength); i != 0; i = fileStream.Read(buffer, 0, buffLength))
                {
                    requestStream.Write(buffer, 0, i);
                }
                requestStream.Close();
                fileStream.Close();
            }
            catch (Exception ex)
            {
                Log.Send(ex.Message + "Ошибка загрузки файла");
            }
        }

        public static void DownloadF(string fileName)
        {
            Log.Send("DownloadFile)");
            try
            {
                string requestUriString = "ftp://" + StartData.currentServer + "/" + fileName;
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
                ftpWebRequest.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);
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
                Log.Send(ex.Message + "Ошибка скачивания файла");
            }
        }

        public static void DownloadF(string FileName, string localPath)
        {
            Log.Send("DownloadF(string, string");
            string requestUriString = "ftp://" + StartData.currentServer + "/" + FileName;
            try
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpWebRequest.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);

                List<string> splitPath = new List<string>(FileName.Split('\\'));
                var directory = localPath + FileName.Replace(splitPath[splitPath.Count - 1], "");
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                FileStream fileStream = new FileStream(localPath + FileName, FileMode.Create);
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

        public static void DownloadFtpDirectory(string url, string localPath)
        {
            FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(url);
            listRequest.UsePassive = true;
            listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            listRequest.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass); ;

            List<string> lines = new List<string>();

            using (WebResponse listResponse = listRequest.GetResponse())
            using (Stream listStream = listResponse.GetResponseStream())
            using (StreamReader listReader = new StreamReader(listStream))
            {
                while (!listReader.EndOfStream)
                {
                    lines.Add(listReader.ReadLine());
                }
            }

            foreach (string line in lines)
            {
                string[] tokens =
                    line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                string name = tokens[8];
                string permissions = tokens[0];

                //string localFilePath = Path.Combine(localPath, name);
                string localFilePath = localPath +@"\"+ name;
                string fileUrl = url + "/" + name;

                if (permissions[0] == 'd')
                {
                    Directory.CreateDirectory(localFilePath);
                    DownloadFtpDirectory(fileUrl + "/", localFilePath);
                }
                else
                {
                    try
                    {
                        FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
                        ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                        ftpWebRequest.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);

                        FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();

                        if (Directory.Exists(localPath))
                        {
                        }
                        else
                        {
                            DirectoryInfo di = Directory.CreateDirectory(localPath);
                        }

                        FileStream fileStream = new FileStream(localFilePath, FileMode.Create);
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

        public static bool FtpDirectoryExists(string Uri_0) //проверить существует ли папка на сервере 
        {
            List<string> path = new List<string>(Uri_0.Split('\\'));
            var directory = path[path.Count-1];
            Uri_0 = Uri_0.Remove(Uri_0.Length - directory.Length, directory.Length);

            string requestUriString = "ftp://" + StartData.currentServer + "/" + Uri_0;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUriString);
            request.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.UseBinary = true;
            request.KeepAlive = false;

            try
            {
                string[] list = null;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    list = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                }
                foreach (string line in list)
                {
                    string data = line;
                    data = data.Remove(0, 49);

                    if (line[0] == 'd' && data == directory)
                        return true;
                }
            } catch { return false; }

                return false;
        }

        public static bool CheckConnected()
        {
            return false;
        }
    }
}
