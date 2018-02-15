using System;
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

		public static void FTPUploadFile(string fileName)
		{
			Log.Send("FTPUploadFile(string)");
			FileInfo fileInfo = new FileInfo(fileName);
			string uriString = "ftp://" + StartData.currentUser + "/" + fileInfo.Name;
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

		public static void FTPDownloadFile(string fileName)
		{
			Log.Send("FTPDownloadFile(string)");
			try
			{
				string requestUriString = "ftp://" + StartData.currentUser + "/" + fileName;
				FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
				ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpWebRequest.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);
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

		public static void FTPDownloadFile(string fileName, string parth)
		{
			Log.Send("FTPDownloadFile(string, string");
			try
			{
				string requestUriString = "ftp://" + StartData.currentUser + "/" + fileName;
				FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
                ftpWebRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                ftpWebRequest.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);
				FtpWebResponse ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
				Stream responseStream = ftpWebResponse.GetResponseStream();
				FileStream fileStream = new FileStream(parth + fileName, FileMode.Create);
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

		public static bool CheckConnected()
		{
			return false;
		}
	}
}
