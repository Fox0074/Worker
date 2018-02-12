using System;
using System.IO;
using System.Net;

namespace ClientWorker
{
	// Token: 0x02000003 RID: 3
	public class FtpClient
	{
		// Token: 0x06000007 RID: 7 RVA: 0x00002394 File Offset: 0x00000594
		public void Init()
		{
			this.buffLength = 2048;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000023A4 File Offset: 0x000005A4
		public void FTPUploadFile(string fileName)
		{
			Log.Send("FTPUploadFile(string)");
			FileInfo fileInfo = new FileInfo(fileName);
			string uriString = "ftp://" + StartData.currentUser + "/" + fileInfo.Name;
			this.reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(uriString));
			this.reqFTP.Method = "STOR";
			this.reqFTP.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);
			this.reqFTP.KeepAlive = false;
			this.reqFTP.UseBinary = true;
			this.reqFTP.ContentLength = fileInfo.Length;
			byte[] buffer = new byte[this.buffLength];
			FileStream fileStream = fileInfo.OpenRead();
			try
			{
				Stream requestStream = this.reqFTP.GetRequestStream();
				for (int i = fileStream.Read(buffer, 0, this.buffLength); i != 0; i = fileStream.Read(buffer, 0, this.buffLength))
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

		// Token: 0x06000009 RID: 9 RVA: 0x000024E8 File Offset: 0x000006E8
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
				byte[] array = new byte[this.buffLength];
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

		// Token: 0x0600000A RID: 10 RVA: 0x000025E4 File Offset: 0x000007E4
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
				byte[] array = new byte[this.buffLength];
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

		// Token: 0x0600000B RID: 11 RVA: 0x000026E8 File Offset: 0x000008E8
		public bool CheckConnected()
		{
			return false;
		}

		// Token: 0x04000007 RID: 7
		private FtpWebRequest reqFTP;

		// Token: 0x04000008 RID: 8
		private int buffLength = 2048;
	}
}
