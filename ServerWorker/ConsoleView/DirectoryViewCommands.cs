using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using ServerWorker.Server;
using Interfaces;

namespace ServerWorker.ConsoleView
{
    public class DirectoryViewCommands : IConsoleCommands
    {
        public Action<IConsoleCommands> PushCommander {get; set;} = delegate { };
        public Action PopCommander {get; set;} = delegate { };

        private User _user;
        private string _currDirectory = "";
        List<string> _tree = new List<string>();
        private List<String> _excludedMethods = new List<string>()
        {
            "Equals",
            "GetHashCode",
            "GetType",
            "ToString"
        };

        public DirectoryViewCommands(User user)
        {
            _user = user;
            List<string> drivers = user.UsersCom.GetDrives();

            foreach(var driver in drivers)
            Console.WriteLine(driver);
        }

        public void RunFile(string fileName)
        {
            Console.WriteLine();
            Console.Write("runas? y:n ");
            var runas = Console.ReadLine().ToLower() == "y" ? "runas" : "";
            Console.Write("args: ");
            var args = Console.ReadLine();
            Console.Write("hidden? y:n ");
            var style = Console.ReadLine().ToLower() == "y" ? System.Diagnostics.ProcessWindowStyle.Hidden : System.Diagnostics.ProcessWindowStyle.Normal;
            Console.Write("use SellExecute? y:n ");
            var shellExecute = Console.ReadLine().ToLower() == "y" ? true : false;
            _user.UsersCom.RunProgram(String.Join("", _tree.ToArray()) + fileName, runas, args, style, shellExecute);
        }

        public void Upload(string fileName)
        {
            _user.UsersCom.UploadDirectory(String.Join("", _tree.ToArray()) + fileName, "Upload");
        }

        public void DeleteFile(string fileName)
        {
            _user.UsersCom.DeleteFile(String.Join("", _tree.ToArray()) + fileName);
        }

        public void ForwardFolder(string selected)
        {
            try
            {
                var path = String.Join("", _tree.ToArray());
                _currDirectory = path + selected + @"\";
                Console.WriteLine("CurrentDirectiry: " + _currDirectory + "\n");
                _tree.Add(selected + @"\");
                IDirectoryInfo directoryInfo = _user.UsersCom.GetDirectoryFiles(_currDirectory, "*");

                foreach (string dir in directoryInfo.Directories)
                    Console.WriteLine(dir);

                foreach (IFileInfo file in directoryInfo.FilesInfo)
                    Console.WriteLine(file.fullName + "  " + GetSize(file.length));           

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                _tree.RemoveAt(_tree.Count-1);
            }
        }
        public void BackFolder()
        {
            try
            {                
                _tree.RemoveAt(_tree.Count - 1);
                if (_tree.Count > 0 )
                {
                    _currDirectory = String.Join("", _tree.ToArray());
                    IDirectoryInfo directoryInfo = _user.UsersCom.GetDirectoryFiles(_currDirectory, "*");

                    foreach (string dir in directoryInfo.Directories)
                        Console.WriteLine(dir);

                    foreach (IFileInfo file in directoryInfo.FilesInfo)
                        Console.WriteLine(file.fullName + "  " + GetSize(file.length));
                }
                else
                {
                    _currDirectory = "";
                    foreach (string driver in _user.UsersCom.GetDrives().ToArray())
                        Console.WriteLine(driver);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        private void Refresh()
        {
            try
            {
                IDirectoryInfo directoryInfo = _user.UsersCom.GetDirectoryFiles(String.Join("", _tree.ToArray()), "*");

                foreach (string dir in directoryInfo.Directories)
                    Console.WriteLine(dir);

                foreach (IFileInfo file in directoryInfo.FilesInfo)
                    Console.WriteLine(file.fullName + "  " + GetSize(file.length));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        private string GetSize(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (byteCount == 0)
            {
                return "0" + suf[0];
            }
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + suf[place];
        }

        public void Help()
        {
            var methods = GetType().GetMethods().ToList();
            methods.Where(x => !_excludedMethods.Contains(x.Name)).ToList().ForEach(x => 
            {
                var parametrs = x.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name).ToList();

                Console.WriteLine("{0}({1})",x.Name, string.Join(",", parametrs));
            });
        }

        public void Back()
        {
            PopCommander();
        }
    }
}