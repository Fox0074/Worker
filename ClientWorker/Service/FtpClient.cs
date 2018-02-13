using System;
using System.IO;
using System.Net;

namespace ClientWorker
{
	public class FtpClient
	{
        private FtpWebRequest reqFTP;
        private int buffLength = 2048;

        public void Init()
		{
			buffLength = 2048;
		}

		public void FTPUploadFile(string fileName)
		{
			Log.Send("FTPUploadFile(string)");
			FileInfo fileInfo = new FileInfo(fileName);
			string uriString = "ftp://" + StartData.currentUser + "/" + fileInfo.Name;
			reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(uriString));
			reqFTP.Method = "STOR";
			reqFTP.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);
			reqFTP.KeepAlive = false;
			reqFTP.UseBinary = true;
			reqFTP.ContentLength = fileInfo.Length;
			byte[] buffer = new byte[this.buffLength];
			FileStream fileStream = fileInfo.OpenRead();
			try
			{
				Stream requestStream = this.reqFTP.GetRequestStream();
				for (int i = fileStream.Read(buffer, 0, this.buffLength); i != 0; i = fileStream.Read(buffer, 0, buffLength))
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

		public void FTPDownloadFile(string fileName)
		{
			Log.Send("FTPDownloadFile(string)");
			try
			{
				string requestUriString = "ftp://" + StartData.currentUser + "/" + fileName;
				FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
				ftpWebRequest.Method = "RETR";
				ftpWebRequest.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);
				ftpWebRequest.KeepAlive = false;
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

		public void FTPDownloadFile(string fileName, string parth)
		{
			Log.Send("FTPDownloadFile(string, string");
			try
			{
				string requestUriString = "ftp://" + StartData.currentUser + "/" + fileName;
				FtpWebRequest ftpWebRequest = (FtpWebRequest)WebRequest.Create(requestUriString);
				ftpWebRequest.Method = "RETR";
				ftpWebRequest.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);
				ftpWebRequest.KeepAlive = false;
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

		public bool CheckConnected()
		{
			return false;
		}
	}
}
