using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientWorker
{
    public class Resetter
    {
        public  bool exitFlag = false;
        public  Timer timer;

        public void Start()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Tick += new EventHandler(Check);
            timer.Interval = 2000;
            timer.Start();

            while (exitFlag == false)
            {
                // Processes all the events in the queue.
                Application.DoEvents();
            }
        }

        private void Check(Object myObject, EventArgs myEventArgs)
        {
            timer.Stop();

            if (System.Diagnostics.Process.GetProcessesByName(Application.ProductName).Length > 1)
            {
                return;
            }
            else
            {

            }

            timer.Enabled = true;
        }
    }
}
