using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    public static class Log
    {
        public static string logParth = "log.txt";
        private static readonly object syncLock = new object();

        public static void Send(string message)
        {
            try
            {
                lock (syncLock)
                {
                    using (StreamWriter streamWriter = File.AppendText(logParth))
                    {
                        streamWriter.WriteLine(message);
                    }
                }
            }
            catch
            {

            }
        }
    }
}
