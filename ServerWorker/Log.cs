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
                //if (Program.form1.listBox2.InvokeRequired) Program.form1.listBox2.BeginInvoke(new Action(() => { Program.form1.listBox2.Items.Add(message); }));
                //else Program.form1.listBox2.Items.Add(message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
