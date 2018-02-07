using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;

namespace ClientWorker
{
    public class FtpClient
    {
        FtpWebRequest reqFTP;
        int buffLength = 2048;

        public void Init()
        {  
            buffLength = 2048;
        }

        public void FTPUploadFile(string fileName)
        {
            FileInfo fileInf = new FileInfo(fileName);
            string uri = "ftp://" + StartData.currentUser + "/" + fileInf.Name;           

            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));    
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            reqFTP.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);
            reqFTP.KeepAlive = false;

            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInf.Length;
            // Буффер в 2 кбайт        
            byte[] buff = new byte[buffLength];
            int contentLen;
            // Файловый поток
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                // Пока файл не кончится
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                // Закрываем потоки
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {

              Console.WriteLine(ex.Message, "Ошибка загрузки файла");

            }

        }

        public void FTPDownloadFile(string fileName)
        {
            try
            {
                string uri = "ftp://" + StartData.currentUser + "/" + fileName;
                // Создаем объект FtpWebRequest
                FtpWebRequest reqFTP = (FtpWebRequest)WebRequest.Create(uri);
                // устанавливаем метод на загрузку файлов
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);
                reqFTP.KeepAlive = false;

                //request.EnableSsl = true; // если используется ssl

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                // получаем поток ответа
                Stream responseStream = response.GetResponseStream();
                // сохраняем файл в дисковой системе
                // создаем поток для сохранения файла
                FileStream fs = new FileStream(fileName, FileMode.Create);

                //Буфер для считываемых данных
                byte[] buffer = new byte[buffLength];
                int size = 0;

                while ((size = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, size);

                }
                fs.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Ошибка скачивания файла");
            }
        }
        public void FTPDownloadFile(string fileName, string parth)
        {
            try
            {
                string uri = "ftp://" + StartData.currentUser + "/" + fileName;
                // Создаем объект FtpWebRequest
                FtpWebRequest reqFTP = (FtpWebRequest)WebRequest.Create(uri);
                // устанавливаем метод на загрузку файлов
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.Credentials = new NetworkCredential(StartData.ftpUser, StartData.ftpPass);
                reqFTP.KeepAlive = false;

                //request.EnableSsl = true; // если используется ssl

                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                // получаем поток ответа
                Stream responseStream = response.GetResponseStream();
                // сохраняем файл в дисковой системе
                // создаем поток для сохранения файла
                FileStream fs = new FileStream(parth + fileName, FileMode.Create);

                //Буфер для считываемых данных
                byte[] buffer = new byte[buffLength];
                int size = 0;

                while ((size = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fs.Write(buffer, 0, size);

                }
                fs.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, "Ошибка скачивания файла");
            }
        }
    }
}
