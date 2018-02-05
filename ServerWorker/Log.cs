using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerWorker
{
    public static class Log
    {

        public static Action<string> UpdateAction = delegate { };
        

        public static void Send(string message)
        {
            try
            {
                UpdateAction.Invoke(message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //if (textBox!=null)
            //{
            //    textBox.Text = message + "\n" + textBox.Text;
            //}
        }
    }
}
