﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

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

        public static void UploadF(string fileName)
        {
            Log.Send("UploadFile");
            FileInfo fileInfo = new FileInfo(fileName);
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            string uriString = "ftp://" + StartData.currentUser + "/" + fileInfo.Name;
            reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(uriString));

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
                string requestUriString = "ftp://" + StartData.currentUser + "/" + fileName;
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

        public static void DownloadF(string fName, string parth)
        {
            Log.Send("DownloadF(string, string");
            string requestUriString = "ftp://" + StartData.currentUser + "/" + fName;
            try
            {
                FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpWebRequest.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);

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

                string localFilePath = Path.Combine(localPath, name);
                string fileUrl = url + "/" + name;

                if (permissions[0] == 'd')
                {
                    Directory.CreateDirectory(localFilePath);
                    DownloadFtpDirectory(fileUrl + "/", credentials, localFilePath);
                }
                else
                {
                    FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
                    downloadRequest.UsePassive = true;
                    downloadRequest.UseBinary = true;
                    downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    downloadRequest.Credentials = credentials;

                    using (Stream ftpStream = downloadRequest.GetResponse().GetResponseStream())
                    using (Stream fileStream = File.Create(localFilePath))
                    {
                        ftpStream.CopyTo(fileStream);
                    }
                }
            }
        }

        public static bool CheckConnected()
        {
            return false;
        }
    }
}
