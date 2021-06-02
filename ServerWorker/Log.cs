using System;
using System.Collections.Generic;

namespace ServerWorker
{
    public static class Log
    {

        public static Action<string> UpdateAction = delegate { };
        public static List<string> LogHisory = new List<string>();

        public static void Send(string message)
        {
            try
            {
                LogHisory.Add(message);
                if (LogHisory.Count > 5000)
                    LogHisory.RemoveAt(4999);
                UpdateAction.Invoke(message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
